using System;
using System.Collections.Generic;
using YieldFlo.Database;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Accumulates yield data points during an active job.
    /// Positions are buffered for ProcessingDelaySec (the grain transport time
    /// from header to elevator sensor) and paired with the sensor flow measured
    /// when they drain, so grain is mapped where it was actually cut and the
    /// tail still in the machine after sections go off is not lost.
    /// Writes one record per second to the database, carrying the mean yield of
    /// that second. Pass boundaries are exact: the first point after sections
    /// come on and the last point before they go off are always written, and a
    /// zero-yield marker row at the off position breaks the map ribbon so
    /// ground crossed with sections off is never painted.
    /// </summary>
    public class clsDataCollector
    {
        // One entry per GPS tick while sections are on, drained after ProcessingDelaySec
        private struct PendingPoint
        {
            public DateTime Time;
            public double Lat, Lon;
            public float Altitude, Speed, Heading;
            public double AcresInc;
            public bool PassStart;  // first point after sections came on — force-written so the pass begins exactly here
            public bool PassEnd;    // break marker queued when sections went off — becomes a zero-yield row at the off position
        }

        private readonly Queue<PendingPoint> _pipeline = new Queue<PendingPoint>();

        // Yield samples accumulated since the last DB write. Each written row
        // carries the MEAN of the ~10 per-tick yields of its second, not one
        // instantaneous 200 ms sensor sample — a transient flow dip at the
        // write instant used to store YieldRate 0 and punch a false gap in the
        // map ribbon (the swath drawer correctly refuses to bridge zero-yield
        // rows). A row is zero only if the whole interval had no flow.
        private double _yieldSum;
        private int    _yieldSamples;

        // Last drained point not yet written (the 1 Hz write gate skips most
        // points). Flushed when its pass ends so the ribbon reaches the exact
        // section-off position instead of stopping at the last whole second.
        private PendingPoint _lastDrained;
        private bool _lastDrainedUnwritten;

        public bool IsRecording { get; private set; }
        public bool IsAutoPaused { get; private set; }  // true only when paused by AOG condition (not manually)
        public int ActiveJobId { get; private set; } = -1;
        public string ActiveJobName { get; private set; } = "";

        public double TotalAcres { get; private set; }
        public double TotalBushels { get; private set; }
        public double AverageYield => TotalAcres > 0.01 ? TotalBushels / TotalAcres : 0;
        public double AverageMoisture { get; private set; }

        // Positions still waiting for their grain to reach the sensor. Exposed for
        // the diagnostic log: it hits 0 exactly when the app stops attributing
        // grain to a finished pass, so comparing it against the sensor trace shows
        // whether the machine was still delivering after recording stopped.
        public int PipelineCount => _pipeline.Count;

        private DateTime _lastWriteTime = DateTime.MinValue;
        private double _lastLat = 0, _lastLon = 0;
        private DateTime _lastFixTime = DateTime.MinValue;

        // Max plausible combine ground speed, used to bound a single GPS-tick's
        // distance step — guards TotalAcres/TotalBushels against a corrupt fix
        // (e.g. a momentary (0,0) glitch) producing a bogus multi-km jump.
        private const double MaxPlausibleSpeedMps = 15.0; // ~34 mph, generous ceiling
        private const double MinFixIntervalSec = 0.05;

        private double _moistureSum = 0;
        private int _moistureCount = 0;

        // --- Tail drain -----------------------------------------------------
        // A pass stops being integrated when its last position drains, one
        // ProcessingDelaySec after sections go off. The machine is not empty at
        // that moment: grain already inside keeps arriving as the shoe and
        // returns clear, and that mass used to be dropped entirely, so every
        // pass under-read by its own tail. Measured on a real machine 2026-08-03:
        // the elevator took ~100 s to fall back to baseline against a 10 s delay.
        //
        // Mass only — no new ground is being cut, so there is no area to divide
        // by and no bu/ac to compute. The grain lands in the job total and the
        // cal run; the map cells are untouched.
        private bool _tailActive;
        private DateTime _tailStart;
        private DateTime _tailLastTick;
        private DateTime _tailEndsAt;      // set when the next pass's grain is due to arrive
        private DateTime _tailEmptySince;  // first sub-threshold reading of the current run
        private double _tailBushels;

        public bool IsDrainingTail => _tailActive;
        public double LastTailBushels { get; private set; }
        public string LastTailEndReason { get; private set; } = "";

        // Fault backstop only. It bounds the case where the baseline has drifted
        // above the true no-flow reading (dust on the sensor, a paddle sitting in
        // the beam, a dead module holding its last value), where the flow reading
        // never returns to zero and a parked combine would otherwise accumulate
        // phantom grain into the job total indefinitely.
        //
        // Flat seconds, deliberately NOT a multiple of ProcessingDelaySec. Transit
        // and clean-out are not the same process — the delay is how long grain takes
        // to travel, the tail is the returns loop and sieve residue recirculating —
        // so no factor of one expresses the other. 180 clears the measured 100 with
        // room for a tougher, wetter crop.
        //
        // Note this is rarely the terminator that fires. Clean-out outlasts a
        // headland turn, so in continuous harvesting the machine never actually
        // empties and "next pass" ends almost every drain; "empty" only wins when
        // the combine stops, at the end of a field or a run.
        private const double TailTimeoutSec = 180.0;

        // Flow has to stay down this long to end a drain, so a single dropped or
        // glitched reading mid-tail cannot truncate it early.
        private const double TailEmptyConfirmSec = 0.5;

        // Bounds one tick's integration so a stalled UI thread or a suspend/resume
        // cannot turn one long gap into a large bogus mass.
        private const double MaxTailTickSec = 1.0;

        public void StartJob(int jobId, string jobName = "")
        {
            // Close any currently active job before starting a new one
            if (ActiveJobId > 0 && Core.Database != null)
            {
                Core.Database.Jobs.UpdateTotals(ActiveJobId, TotalAcres, TotalBushels);
                Core.Database.Jobs.Close(ActiveJobId);
            }

            ActiveJobId = jobId;
            ActiveJobName = jobName;
            TotalAcres = 0;
            TotalBushels = 0;
            AverageMoisture = 0;
            _moistureSum = 0;
            _moistureCount = 0;
            _lastLat = 0;
            _lastLon = 0;
            _lastFixTime = DateTime.MinValue;
            _lastWriteTime = DateTime.MinValue;
            ResetPipeline();
            IsRecording  = false;
            IsAutoPaused = true;   // starts recording when sections come on
        }

        public void RenameActiveJob(string name) => ActiveJobName = name;

        public void StopJob()
        {
            IsRecording = false;
            ResetPipeline();   // grain still in transit is abandoned on an explicit stop
            if (ActiveJobId > 0 && Core.Database != null)
            {
                Core.Database.Jobs.UpdateTotals(ActiveJobId, TotalAcres, TotalBushels);
                Core.Database.Jobs.Close(ActiveJobId);
            }
            ActiveJobId = -1;
            ActiveJobName = "";
        }

        // Manual pause discards in-transit positions; their flow can't be
        // matched after an arbitrary pause.
        public void PauseJob() { IsRecording = false; IsAutoPaused = false; _pipeline.Clear(); _tailActive = false; }

        // Abandon everything in transit: buffered positions, the yield average
        // in progress, and any unwritten drained point that belonged to them.
        private void ResetPipeline()
        {
            _pipeline.Clear();
            _yieldSum = 0;
            _yieldSamples = 0;
            _lastDrainedUnwritten = false;
            _tailActive = false;   // grain still emptying out is abandoned too
        }

        private void AutoPause() { IsRecording = false; IsAutoPaused = true; }

        private void AutoResume() { IsRecording = true; IsAutoPaused = false; }

        /// <summary>
        /// Saves accumulated totals to the DB but leaves the job status as Active
        /// so it can be auto-resumed on the next app start.
        /// </summary>
        public void SuspendJob()
        {
            IsRecording = false;
            ResetPipeline();
            if (ActiveJobId > 0 && Core.Database != null)
                Core.Database.Jobs.UpdateTotals(ActiveJobId, TotalAcres, TotalBushels);
            ActiveJobId   = -1;
            ActiveJobName = "";
        }

        public void ResumeJob() { IsRecording = false; IsAutoPaused = true; }  // re-arms auto-resume; recording starts when sections come on

        /// <summary>
        /// Loads a previously created job, restoring its accumulated totals.
        /// Used when resuming a job from a prior session.
        /// </summary>
        public void LoadJob(int jobId, string jobName, double existingAcres, double existingBushels)
        {
            // Close any different active job before loading the new one
            if (ActiveJobId > 0 && ActiveJobId != jobId && Core.Database != null)
            {
                Core.Database.Jobs.UpdateTotals(ActiveJobId, TotalAcres, TotalBushels);
                Core.Database.Jobs.Close(ActiveJobId);
            }

            ActiveJobId = jobId;
            ActiveJobName = jobName;
            TotalAcres = existingAcres;
            TotalBushels = existingBushels;
            AverageMoisture = 0;
            _moistureSum = 0;
            _moistureCount = 0;
            _lastLat = 0;
            _lastLon = 0;
            _lastFixTime = DateTime.MinValue;
            _lastWriteTime = DateTime.MinValue;
            ResetPipeline();
            IsRecording  = false;
            IsAutoPaused = true;   // auto-resumes once AOG connects and harvesting starts
        }

        /// <summary>
        /// Adopts a job total that was rewritten underneath us — a map recalculation
        /// rescales the stored total, and if that job is the one currently recording,
        /// this accumulator would put the pre-recalculation figure straight back on
        /// the next save. Ignored for any other job, which owns its own stored total.
        /// </summary>
        public void SyncTotalBushels(int jobId, double bushels)
        {
            if (jobId > 0 && jobId == ActiveJobId) TotalBushels = bushels;
        }

        /// <summary>
        /// Called every GPS update (~10 Hz). Writes to DB once per second.
        /// </summary>
        public void OnGpsUpdate(double rawMoisture)
        {
            if (ActiveJobId < 0) return;

            // Allow auto-resume check even when paused — but skip entirely if manually paused.
            if (!IsRecording && !IsAutoPaused) return;

            var gps = Core.GPS;
            if (!gps.IsConnected) return;

            var yield = Core.Yield;

            // Sections turn off over already-harvested ground even when moving.
            bool harvestActive = gps.SectionsActive;

            // The GPS fix is the antenna; the crop is cut at the header, which sits
            // HeaderFwdOffsetM ahead of it. Record the HEADER position so pass
            // boundaries land where the header crossed them — AOG paints its
            // coverage at the tool the same way.
            double hdgRad = gps.Heading * Math.PI / 180.0;
            double lat = gps.Latitude
                       + yield.HeaderFwdOffsetM * Math.Cos(hdgRad) / 111320.0;
            double lon = gps.Longitude
                       + yield.HeaderFwdOffsetM * Math.Sin(hdgRad)
                         / Math.Max(1.0, 111320.0 * Math.Cos(gps.Latitude * Math.PI / 180.0));

            if (harvestActive)
            {
                // A new pass has started while the previous one is still draining.
                // Its grain cannot reach the sensor for another ProcessingDelaySec,
                // so everything arriving before then still belongs to the old pass —
                // the tail runs on until exactly the moment the new crop is due,
                // which is also when the new pass's own positions start draining.
                // Ending it at sections-on instead would throw away most of the tail
                // on precisely the quick headland turns that lose the most today.
                if (_tailActive && _tailEndsAt == DateTime.MaxValue)
                    _tailEndsAt = DateTime.UtcNow.AddSeconds(yield.ProcessingDelaySec);

                // Crop is entering the machine — buffer this position. Its grain
                // reaches the sensor ProcessingDelaySec from now.
                bool passStart = _lastLat == 0 && _lastLon == 0;
                double acresInc = 0;
                DateTime now = DateTime.UtcNow;
                if (!passStart)
                {
                    double distM = HaversineMetres(_lastLat, _lastLon, lat, lon);
                    double dtSec = Math.Max((now - _lastFixTime).TotalSeconds, MinFixIntervalSec);
                    if (distM <= MaxPlausibleSpeedMps * dtSec)
                        acresInc = clsYieldCalculator.MetresToAcres(distM, yield.HeaderWidthM);
                    // else: implausible GPS jump (e.g. a momentary 0,0 glitch fix) —
                    // skip this tick's acreage/yield contribution instead of adding a
                    // bogus multi-km increment to the job total. The point is still
                    // enqueued below; the map's own AddSwath/MaxBridgeMeters guard
                    // already refuses to bridge a gap this large, so the ribbon is
                    // unaffected.
                }
                _lastLat = lat;
                _lastLon = lon;
                _lastFixTime = now;

                _pipeline.Enqueue(new PendingPoint
                {
                    Time = now,
                    Lat = lat,
                    Lon = lon,
                    Altitude = gps.Altitude,
                    Speed = gps.Speed,
                    Heading = gps.Heading,
                    AcresInc = acresInc,
                    PassStart = passStart
                });
            }
            else
            {
                // Header up / sections off — no new crop, but keep draining the
                // pipeline: delay-time's worth of grain is still in the machine.
                if (_lastLat != 0 || _lastLon != 0)
                {
                    // Sections just went off: send a pass-end marker down the
                    // pipeline at the last harvested position. When it drains it
                    // ends the map ribbon exactly there, so a brief section-off
                    // can never be painted across.
                    _pipeline.Enqueue(new PendingPoint
                    {
                        Time = DateTime.UtcNow,
                        Lat = _lastLat,
                        Lon = _lastLon,
                        Altitude = gps.Altitude,
                        Speed = gps.Speed,
                        Heading = gps.Heading,
                        AcresInc = 0,
                        PassEnd = true
                    });
                }
                _lastLat = 0;
                _lastLon = 0;
                _lastFixTime = DateTime.MinValue;
            }

            // Drain positions older than the transport delay — their grain is at
            // the sensor now, so pair them with the current flow reading.
            double moisture = rawMoisture > 0 ? rawMoisture + Core.ActiveMoistureOffset : 0;
            DateTime cutoff = DateTime.UtcNow.AddSeconds(-yield.ProcessingDelaySec);

            while (_pipeline.Count > 0 && _pipeline.Peek().Time <= cutoff)
            {
                PendingPoint pt = _pipeline.Dequeue();

                if (pt.PassEnd)
                {
                    // The pass ending at this position has fully drained. Flush
                    // the final real point so the ribbon reaches the section-off
                    // location, then write a zero-yield marker row there — the
                    // map's both-ends-flowing guard turns it into a guaranteed
                    // ribbon break however short the section-off was.
                    if (_lastDrainedUnwritten)
                        WritePoint(_lastDrained, _yieldSum / _yieldSamples, moisture);
                    WritePoint(pt, 0, moisture);
                    _yieldSum = 0;
                    _yieldSamples = 0;
                    _lastDrainedUnwritten = false;
                    _lastWriteTime = DateTime.UtcNow;

                    // Everything positional for this pass is now written, but the
                    // machine is still delivering its grain. Keep counting.
                    BeginTailDrain();
                    continue;
                }

                // Real crop is arriving again — the previous pass's tail is over
                // whether or not the timer said so.
                if (_tailActive) EndTailDrain("next pass");

                yield.Calculate(pt.Speed);

                TotalAcres += pt.AcresInc;
                double bushelsInc = yield.InstantYield * pt.AcresInc;
                TotalBushels += bushelsInc;
                yield.AccumulateCalRun(bushelsInc);

                _yieldSum += yield.InstantYield;
                _yieldSamples++;
                _lastDrained = pt;
                _lastDrainedUnwritten = true;

                // Moisture is measured at the sensor, so the current reading
                // belongs to this drained position's grain.
                if (moisture > 0)
                {
                    _moistureSum += moisture;
                    _moistureCount++;
                    AverageMoisture = _moistureSum / _moistureCount;
                }

                // Write to DB once per second; a pass-start point is written
                // immediately so the ribbon begins exactly where sections came on.
                if (pt.PassStart || (DateTime.UtcNow - _lastWriteTime).TotalSeconds >= 1.0)
                {
                    _lastWriteTime = DateTime.UtcNow;
                    WritePoint(pt, _yieldSum / _yieldSamples, moisture);
                    _yieldSum = 0;
                    _yieldSamples = 0;
                    _lastDrainedUnwritten = false;
                }
            }

            if (_tailActive)
                AccumulateTail(yield);

            // Keep the live display honest while idle
            if (!harvestActive && _pipeline.Count == 0)
                yield.Calculate(gps.Speed);

            // Recording while crop is entering the machine or grain is still in transit
            bool shouldRecord = harvestActive || _pipeline.Count > 0 || _tailActive;

            if (shouldRecord && IsAutoPaused)
            {
                AutoResume();
                Core.RaiseJobStateChanged();
            }
            else if (!shouldRecord && !IsAutoPaused)
            {
                AutoPause();
                Core.RaiseJobStateChanged();
            }
        }

        private void BeginTailDrain()
        {
            _tailActive     = true;
            _tailStart      = DateTime.UtcNow;
            _tailLastTick   = _tailStart;
            _tailEndsAt     = DateTime.MaxValue;
            _tailEmptySince = DateTime.MaxValue;
            _tailBushels    = 0;
        }

        /// <summary>
        /// Integrates the grain still leaving the machine after a pass has ended
        /// into the job total. Runs until the elevator is empty, until the next
        /// pass's grain is due, or until the fault timeout — whichever comes first.
        /// </summary>
        private void AccumulateTail(clsYieldCalculator yield)
        {
            DateTime now = DateTime.UtcNow;

            double dt = Math.Min((now - _tailLastTick).TotalSeconds, MaxTailTickSec);
            _tailLastTick = now;

            if (dt > 0)
            {
                // Mass only — CurrentBushelsPerSec is read straight off the sensor
                // and stays valid at a standstill, unlike Calculate().
                double bushelsInc = yield.CurrentBushelsPerSec() * dt;
                _tailBushels += bushelsInc;
                TotalBushels += bushelsInc;
                // Real grain into the tank, so a cal run has to see it too — a run
                // that missed one tail per pass would weigh short against the ticket
                // and bias the computed YieldFactor.
                yield.AccumulateCalRun(bushelsInc);
            }

            if (yield.ActiveFlowRatio <= clsYieldCalculator.FlowStopRatio)
            {
                if (_tailEmptySince == DateTime.MaxValue) _tailEmptySince = now;
                if ((now - _tailEmptySince).TotalSeconds >= TailEmptyConfirmSec)
                {
                    EndTailDrain("empty");
                    return;
                }
            }
            else
            {
                _tailEmptySince = DateTime.MaxValue;   // flow came back — not empty yet
            }

            if (now >= _tailEndsAt)
            {
                EndTailDrain("next pass");
                return;
            }

            if ((now - _tailStart).TotalSeconds >= TailTimeoutSec)
                EndTailDrain("timeout");
        }

        private void EndTailDrain(string reason)
        {
            _tailActive       = false;
            _tailEndsAt       = DateTime.MaxValue;
            _tailEmptySince   = DateTime.MaxValue;
            LastTailBushels   = _tailBushels;
            LastTailEndReason = reason;

            // A timeout is never normal: it means flow never came back to baseline,
            // which is the sensor baseline drifting rather than the machine being
            // slow. Worth a line in the log — the symptom otherwise is a job total
            // that quietly disagrees with the weigh ticket.
            if (reason == "timeout")
                Props.WriteErrorLog("DataCollector/TailDrain still above baseline after "
                                    + TailTimeoutSec.ToString("0") + " s — check SensorBaseline (drained "
                                    + _tailBushels.ToString("0.0") + " bu)");
        }

        private void WritePoint(PendingPoint pt, double yieldRate, double moisture)
        {
            var point = new YieldDataPoint
            {
                JobId = ActiveJobId,
                Timestamp = pt.Time,
                Latitude = pt.Lat,
                Longitude = pt.Lon,
                Elevation = pt.Altitude,
                Speed = pt.Speed,
                Heading = pt.Heading,
                YieldRate = yieldRate,
                Moisture = moisture,
                AcresAccumulated = TotalAcres,
                Sensor1Raw = Core.LastSensor1,
                Sensor2Raw = Core.LastNoiseCount,
                ModuleRpm = Core.LastModuleRpm,
                PaddleHz = Core.LastPaddleHz,
                MinCycleMs = Core.LastMinCycleMs,

                // Both channels are logged raw at every point: the duty ratio in
                // Sensor1Raw and the paddle reading here. Which one produced this
                // point's YieldRate is recorded in the flags, so a map can be
                // re-derived on either channel later without guessing.
                FlowRate = Core.PaddleChannelLive ? Core.LastFlowRate : -1,
                PaddlesPerS = Core.PaddleChannelLive ? Core.LastPaddlesPerS : -1,
                FlowFlags = FlowFlagsNow()
            };

            Core.LastDataWriteOk = Core.Database?.YieldData.Insert(point) ?? true;
        }

        private static int FlowFlagsNow()
        {
            int f = 0;
            var y = Core.Yield;
            if (y != null && y.UsingPaddleChannel) f |= YieldDataPoint.FlagUsedPaddleChannel;
            if (Core.PaddleChannelLive)
            {
                if (Core.LastFlowSaturated)   f |= YieldDataPoint.FlagSaturated;
                if (Core.LastFlowUnaccounted) f |= YieldDataPoint.FlagUnaccounted;
            }
            if (y != null && y.ChannelsDisagree) f |= YieldDataPoint.FlagChannelsDisagree;
            return f;
        }

        private static double HaversineMetres(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6_371_000; // earth radius metres
            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                     + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                     * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRad(double deg) => deg * Math.PI / 180.0;
    }
}
