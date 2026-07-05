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
    /// Writes one record per second to the database.
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
        }

        private readonly Queue<PendingPoint> _pipeline = new Queue<PendingPoint>();

        public bool IsRecording { get; private set; }
        public bool IsAutoPaused { get; private set; }  // true only when paused by AOG condition (not manually)
        public int ActiveJobId { get; private set; } = -1;
        public string ActiveJobName { get; private set; } = "";

        public double TotalAcres { get; private set; }
        public double TotalBushels { get; private set; }
        public double AverageYield => TotalAcres > 0.01 ? TotalBushels / TotalAcres : 0;
        public double AverageMoisture { get; private set; }

        private DateTime _lastWriteTime = DateTime.MinValue;
        private double _lastLat = 0, _lastLon = 0;
        private double _moistureSum = 0;
        private int _moistureCount = 0;

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
            _lastWriteTime = DateTime.MinValue;
            _pipeline.Clear();
            IsRecording  = false;
            IsAutoPaused = true;   // starts recording when sections come on
        }

        public void RenameActiveJob(string name) => ActiveJobName = name;

        public void StopJob()
        {
            IsRecording = false;
            _pipeline.Clear();   // grain still in transit is abandoned on an explicit stop
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
        public void PauseJob() { IsRecording = false; IsAutoPaused = false; _pipeline.Clear(); }

        private void AutoPause() { IsRecording = false; IsAutoPaused = true; }

        private void AutoResume() { IsRecording = true; IsAutoPaused = false; }

        /// <summary>
        /// Saves accumulated totals to the DB but leaves the job status as Active
        /// so it can be auto-resumed on the next app start.
        /// </summary>
        public void SuspendJob()
        {
            IsRecording = false;
            _pipeline.Clear();
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
            _lastWriteTime = DateTime.MinValue;
            _pipeline.Clear();
            IsRecording  = false;
            IsAutoPaused = true;   // auto-resumes once AOG connects and harvesting starts
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

            if (harvestActive)
            {
                // Crop is entering the machine — buffer this position. Its grain
                // reaches the sensor ProcessingDelaySec from now.
                double acresInc = 0;
                if (_lastLat != 0 && _lastLon != 0)
                {
                    double distM = HaversineMetres(_lastLat, _lastLon, gps.Latitude, gps.Longitude);
                    acresInc = clsYieldCalculator.MetresToAcres(distM, yield.HeaderWidthM);
                }
                _lastLat = gps.Latitude;
                _lastLon = gps.Longitude;

                _pipeline.Enqueue(new PendingPoint
                {
                    Time = DateTime.UtcNow,
                    Lat = gps.Latitude,
                    Lon = gps.Longitude,
                    Altitude = gps.Altitude,
                    Speed = gps.Speed,
                    Heading = gps.Heading,
                    AcresInc = acresInc
                });
            }
            else
            {
                // Header up / sections off — no new crop, but keep draining the
                // pipeline: delay-time's worth of grain is still in the machine.
                _lastLat = 0;
                _lastLon = 0;
            }

            // Drain positions older than the transport delay — their grain is at
            // the sensor now, so pair them with the current flow reading.
            double moisture = rawMoisture > 0 ? rawMoisture + Core.ActiveMoistureOffset : 0;
            DateTime cutoff = DateTime.UtcNow.AddSeconds(-yield.ProcessingDelaySec);

            while (_pipeline.Count > 0 && _pipeline.Peek().Time <= cutoff)
            {
                PendingPoint pt = _pipeline.Dequeue();

                yield.Calculate(pt.Speed);

                TotalAcres += pt.AcresInc;
                double bushelsInc = yield.InstantYield * pt.AcresInc;
                TotalBushels += bushelsInc;
                yield.AccumulateCalRun(bushelsInc);

                // Moisture is measured at the sensor, so the current reading
                // belongs to this drained position's grain.
                if (moisture > 0)
                {
                    _moistureSum += moisture;
                    _moistureCount++;
                    AverageMoisture = _moistureSum / _moistureCount;
                }

                // Write to DB once per second
                if ((DateTime.UtcNow - _lastWriteTime).TotalSeconds >= 1.0)
                {
                    _lastWriteTime = DateTime.UtcNow;

                    var point = new YieldDataPoint
                    {
                        JobId = ActiveJobId,
                        Timestamp = pt.Time,
                        Latitude = pt.Lat,
                        Longitude = pt.Lon,
                        Elevation = pt.Altitude,
                        Speed = pt.Speed,
                        Heading = pt.Heading,
                        YieldRate = yield.InstantYield,
                        Moisture = moisture,
                        AcresAccumulated = TotalAcres,
                        Sensor1Raw = Core.LastSensor1,
                        Sensor2Raw = Core.LastNoiseCount
                    };

                    Core.Database?.YieldData.Insert(point);
                }
            }

            // Keep the live display honest while idle
            if (!harvestActive && _pipeline.Count == 0)
                yield.Calculate(gps.Speed);

            // Recording while crop is entering the machine or grain is still in transit
            bool shouldRecord = harvestActive || _pipeline.Count > 0;

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
