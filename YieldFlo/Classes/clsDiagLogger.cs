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
    ///   PCTime,Sensor1,Noise,Rpm,Moisture,PaddleHz,MinCycleMs,GateRejects,SpeedKmh,YieldRate,JobId
    ///   14:32:07.812,0.412,0,412,14.2,7,142,0,5.4,48.2,17
    ///   14:32:08.013,0.538,0,411,14.2,7,9,2,5.4,63.1,17
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
    ///   SpeedKmh    - ground speed at that moment
    ///   YieldRate   - instantaneous yield the app computed from this packet
    ///   JobId       - active job, or -1 when none is recording
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
                                      "MinCycleMs,GateRejects,SpeedKmh,YieldRate,JobId");
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
                        (Core.GPS?.Speed ?? 0).ToString("0.##", ci),
                        (Core.Yield?.InstantYield ?? 0).ToString("0.##", ci),
                        (Core.Collector?.ActiveJobId ?? -1).ToString(ci)));
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
