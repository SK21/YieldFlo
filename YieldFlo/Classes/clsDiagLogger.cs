using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Records one CSV row per module data packet (5 Hz) for offline diagnosis,
    /// modelled on RateController's PID logger.
    ///
    /// Why this exists alongside yield_data: the database stores one row per
    /// second and only while a job is recording, so it aliases the paddle stream
    /// (~7 Hz) and captures nothing at all during bench work or a fault that
    /// happens between jobs. This logs every packet the module sends, whether or
    /// not a job is running.
    ///
    /// Deliberately always-on rather than a toggle. RateController gates its log
    /// behind a checkbox because it streams at PID cadence; at 5 Hz this is about
    /// 1.5 MB an hour, cheap enough to leave running — and a field fault is
    /// exactly the thing nobody remembers to arm a recorder for beforehand. Old
    /// files are pruned so it cannot grow without bound.
    ///
    /// Example:
    ///   PCTime,Sensor1,Noise,Rpm,Moisture,PaddleHz,MinCycleMs,GateRejects,MedianCycleMs,SpeedKmh,YieldRate,JobId,Sections,PipelineCount,Tail,S1Valid,SensorFault,NewFrac
    ///   14:32:07.812,0.412,0,412,14.2,7,142,0,141,5.4,48.2,17,1,53,0,1,0,1
    ///   14:32:08.013,0.538,0,411,14.2,7,9,2,140,5.4,63.1,17,1,54,0,1,0,0.62
    ///
    ///   PCTime      - PC clock when the packet was parsed (HH:mm:ss.fff)
    ///   Sensor1     - duty-channel obstruction ratio, raw, NOT baseline-corrected
    ///   Noise       - glitch edges the module rejected in that 200 ms window
    ///   Rpm         - elevator RPM (fixed 200 when no RPM sensor is fitted)
    ///   Moisture    - scaled moisture reading
    ///   PaddleHz    - paddles/s from the 1 Hz packet; -1 = not reported
    ///   MinCycleMs  - shortest completed paddle cycle in the 1 Hz window; -1 = not reported.
    ///                 Collapsing toward single digits is the signature of a spurious edge
    ///                 being committed as if it were a full paddle cycle.
    ///   GateRejects - edges the period gate rejected; -1 = not reported
    ///   MedianCycleMs - the gate's period estimate, ms; its threshold is 75% of this.
    ///                 0 = estimator unarmed, gate passing everything. -1 = not reported.
    ///   SpeedKmh    - ground speed at that moment
    ///   YieldRate   - instantaneous yield the app computed from this packet
    ///   JobId       - active job, or -1 when none is recording
    ///   Sections    - AOG section state: 1 = crop entering the machine, 0 = header out.
    ///                 Its falling edge is the anchor every clean-out measurement is
    ///                 timed from.
    ///   PipelineCount - positions buffered waiting for their grain to reach the sensor.
    ///                 Returns to 0 one ProcessingDelaySec after Sections drops.
    ///   Tail        - 1 while the collector is still counting grain leaving the machine
    ///                 after a pass ended. Starts as PipelineCount reaches 0 and ends
    ///                 when Sensor1 settles to baseline; ending while Sensor1 is still
    ///                 high means a terminator other than empty fired — the next pass's
    ///                 grain arriving, or the fault timeout.
    ///   S1Valid     - the module's SensorOK flag for this packet. 0 means Sensor1's 0 was
    ///                 substituted by the parser, not measured. Logged because the database
    ///                 does not store it, which is why a field snapshot full of zeros could
    ///                 not be told apart from a genuinely empty elevator.
    ///   NewFrac     - fraction of the last swath that was ground not already cut this
    ///                 job. 1 = virgin ground, 0.5 = half the header running over what
    ///                 the previous pass took. Below 0.15 the tick contributes mass but
    ///                 no area and no map row. A pass sitting well under 1 is where the
    ///                 old acres figure was inflating and the map was painting a cold
    ///                 streak that the ground never had.
    ///                 NOT comparable row-wise against YieldRate: NewFrac is the position
    ///                 being marked NOW, while YieldRate is grain cut one ProcessingDelaySec
    ///                 earlier, so on any single row the two describe ground ~10 s apart.
    ///                 Driving onto cut ground shows as NewFrac falling to 0 with YieldRate
    ///                 unchanged, and the yield only responds a delay later. Shift NewFrac
    ///                 forward by ProcessingDelaySec before correlating them.
    ///                 Quantised to 1/n, where n is the number of samples across the
    ///                 header (72 for a 30 ft header) — so values come in steps of ~0.014
    ///                 and a run of identical values means a steady overlap, not a stuck
    ///                 reading.
    ///   SensorFault - 1 while the collector is holding recording off because the reading
    ///                 cannot be trusted (S1Valid 0, module silent, or Sensor1 pinned at
    ///                 hard zero with sections on). Its rising edge is where the map ribbon
    ///                 breaks and the pass is abandoned.
    ///
    /// Measuring transport delay and clean-out: find a falling edge on Sections and
    /// read Sensor1 forward from it. Sensor1 holds up while the grain already in the
    /// machine keeps arriving, then decays; where it settles back to its no-flow
    /// baseline is the clean-out time. Compare that against ProcessingDelaySec.
    /// PipelineCount reaching 0 while Sensor1 is still above baseline is the failure
    /// case directly on screen: the machine was still delivering grain after the app
    /// had stopped attributing any of it to ground, so those cells map low and that
    /// grain is missing from the job total. The rising edge shows the same thing from
    /// the other side — the fill ramp, which is not the same length as the clean-out.
    ///
    /// Reading the file: rows arrive at 5 Hz but PaddleHz, MinCycleMs, GateRejects and
    /// MedianCycleMs come from the 1 Hz packet, so each of their values is repeated on
    /// about five consecutive rows. Any per-second aggregate of those four columns has
    /// to de-duplicate first — summing GateRejects across rows overcounts five-fold.
    /// Sensor1, Noise, Rpm, Moisture, Sections, PipelineCount and Tail are genuinely
    /// per-row.
    ///
    /// Sections, PipelineCount, Tail, S1Valid, SensorFault and NewFrac are appended
    /// after JobId rather than grouped with the columns they relate to, so every
    /// existing column keeps its index and scripts written against the older files
    /// still parse these.
    ///
    /// The four gate columns together say why every cycle was accepted or rejected:
    /// GateRejects counts what the gate caught, MinCycleMs what got through,
    /// MedianCycleMs the threshold both were judged against, and PaddleHz whether real
    /// paddles were being lost. Rejects rising while MinCycleMs holds near the paddle
    /// period is the gate working; PaddleHz halving with MinCycleMs at roughly double
    /// the period is the gate rejecting real paddles, which merges two into one cycle.
    /// </summary>
    public class clsDiagLogger
    {
        private readonly object cLock = new object();
        private StreamWriter cWriter;
        private string cFilePath;
        private bool cRunning;

        // Keep roughly a working week of sessions. Small enough that nobody has to
        // manage the folder, long enough that a fault reported days later is still
        // on disk.
        private const int KeepFiles = 20;

        public string FilePath { get { return cFilePath; } }
        public bool IsRunning { get { return cRunning; } }

        public void Start()
        {
            lock (cLock)
            {
                if (cRunning) return;
                try
                {
                    string folder = Path.Combine(Props.DataFolder, "DiagLogs");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    Prune(folder);

                    cFilePath = Path.Combine(folder,
                        "Diag_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                    cWriter = new StreamWriter(cFilePath, false) { AutoFlush = true };
                    cWriter.WriteLine("PCTime,Sensor1,Noise,Rpm,Moisture,PaddleHz," +
                                      "MinCycleMs,GateRejects,MedianCycleMs," +
                                      "SpeedKmh,YieldRate,JobId,Sections,PipelineCount,Tail," +
                                      "S1Valid,SensorFault,NewFrac");
                    cRunning = true;
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("DiagLogger/Start " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Called from the module packet parsers. Reads live Core state rather than
        /// taking arguments, so both the CAN and UDP paths log identical columns
        /// without either having to know what the other carries.
        /// </summary>
        public void Log()
        {
            lock (cLock)
            {
                if (!cRunning || cWriter == null) return;
                try
                {
                    var ci = CultureInfo.InvariantCulture;
                    cWriter.WriteLine(string.Join(",",
                        DateTime.Now.ToString("HH:mm:ss.fff", ci),
                        Core.LastSensor1.ToString("0.####", ci),
                        Core.LastNoiseCount.ToString(ci),
                        Core.LastModuleRpm.ToString(ci),
                        Core.LastMoisture.ToString("0.##", ci),
                        Core.LastPaddleHz.ToString(ci),
                        Core.LastMinCycleMs.ToString(ci),
                        Core.LastGateRejects.ToString(ci),
                        Core.LastMedianCycleMs.ToString(ci),
                        (Core.GPS?.Speed ?? 0).ToString("0.##", ci),
                        (Core.Yield?.InstantYield ?? 0).ToString("0.##", ci),
                        (Core.Collector?.ActiveJobId ?? -1).ToString(ci),
                        // 1/0 rather than True/False — these two are meant to be
                        // plotted against Sensor1 in a spreadsheet.
                        ((Core.GPS?.SectionsActive ?? false) ? 1 : 0).ToString(ci),
                        (Core.Collector?.PipelineCount ?? -1).ToString(ci),
                        ((Core.Collector?.IsDrainingTail ?? false) ? 1 : 0).ToString(ci),
                        (Core.LastSensor1Valid ? 1 : 0).ToString(ci),
                        ((Core.Collector?.SensorFault ?? false) ? 1 : 0).ToString(ci),
                        (Core.Collector?.LastNewFraction ?? 1.0).ToString("0.###", ci)));
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("DiagLogger/Log " + ex.Message);
                    // Stop after a write failure rather than logging an error per
                    // packet — a full disk would otherwise flood Errors.log at 5 Hz.
                    cRunning = false;
                }
            }
        }

        public void Stop()
        {
            lock (cLock)
            {
                if (!cRunning) return;
                try
                {
                    cWriter?.Flush();
                    cWriter?.Dispose();
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("DiagLogger/Stop " + ex.Message);
                }
                cWriter = null;
                cRunning = false;
            }
        }

        private void Prune(string folder)
        {
            try
            {
                var old = new DirectoryInfo(folder).GetFiles("Diag_*.csv")
                    .OrderByDescending(f => f.LastWriteTimeUtc)
                    .Skip(KeepFiles - 1);
                foreach (var f in old) f.Delete();
            }
            catch (Exception ex)
            {
                // Pruning is housekeeping — never let it stop a session recording.
                Props.WriteErrorLog("DiagLogger/Prune " + ex.Message);
            }
        }
    }
}
