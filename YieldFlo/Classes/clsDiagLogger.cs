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
    ///   PCTime,Sensor1,Noise,Rpm,Moisture,PaddleHz,MinCycleMs,FlowRate,PaddlesPerS,FlowRejects,SpeedKmh,YieldRate,JobId,Sections,PipelineCount,Tail
    ///   14:32:07.812,0.412,0,412,14.2,7,142,0.398,6.98,0,5.4,48.2,17,1,53,0
    ///   14:32:08.013,0.538,0,411,14.2,7,141,0.402,7.01,0,5.4,63.1,17,1,54,0
    ///
    ///   PCTime      - PC clock when the packet was parsed (HH:mm:ss.fff)
    ///   Sensor1     - duty-channel obstruction ratio, raw, NOT baseline-corrected
    ///   Noise       - glitch edges the module rejected in that 200 ms window
    ///   Rpm         - elevator RPM (fixed 200 when no RPM sensor is fitted)
    ///   Moisture    - scaled moisture reading
    ///   PaddleHz    - paddles/s from the 1 Hz packet; -1 = not reported
    ///   MinCycleMs  - shortest completed paddle cycle in the 1 Hz window; -1 = not reported
    ///   FlowRate    - paddle channel: per-paddle obstruction per second, uncorrected
    ///   PaddlesPerS - paddle rate over the same window
    ///   FlowRejects - cycles the module merged/scaled/reseeded this window
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
    ///
    /// THE TWO MEASUREMENTS THIS FILE EXISTS FOR:
    ///
    /// Clean-out — find a FALLING edge on Sections and read Sensor1 forward. It holds
    /// up while grain already inside the machine keeps arriving, then decays; where it
    /// settles back to its no-flow baseline is the clean-out time. Measured ~100 s on a
    /// real machine on 2026-08-03, against a ProcessingDelaySec of 10.
    ///
    /// Transport delay — find a RISING edge and measure the gap until Sensor1 lifts.
    /// That IS the true delay, and it is the more urgent of the two: ProcessingDelaySec
    /// has never actually been measured on a machine, 10 s is only the default, and a
    /// delay set too short produces blue at the start of every pass that looks identical
    /// to the fill ramp but is free to fix.
    ///
    /// Reading the file: rows arrive at 5 Hz but PaddleHz, MinCycleMs, FlowRate,
    /// PaddlesPerS and FlowRejects come from the 1 Hz packet, so each of their values is
    /// repeated on about five consecutive rows. Any per-second aggregate of those has to
    /// de-duplicate first — summing FlowRejects across rows overcounts five-fold.
    /// Sensor1, Noise, Rpm, Moisture, Sections, PipelineCount and Tail are genuinely
    /// per-row.
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
                                      "MinCycleMs,FlowRate,PaddlesPerS,FlowRejects," +
                                      "SpeedKmh,YieldRate,JobId,Sections,PipelineCount,Tail");
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
                        Core.LastFlowRate.ToString("0.####", ci),
                        Core.LastPaddlesPerS.ToString("0.##", ci),
                        Core.LastFlowRejects.ToString(ci),
                        (Core.GPS?.Speed ?? 0).ToString("0.##", ci),
                        (Core.Yield?.InstantYield ?? 0).ToString("0.##", ci),
                        (Core.Collector?.ActiveJobId ?? -1).ToString(ci),
                        // 1/0 rather than True/False — these are meant to be plotted
                        // against Sensor1 in a spreadsheet.
                        ((Core.GPS?.SectionsActive ?? false) ? 1 : 0).ToString(ci),
                        (Core.Collector?.PipelineCount ?? -1).ToString(ci),
                        ((Core.Collector?.IsDrainingTail ?? false) ? 1 : 0).ToString(ci)));
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
