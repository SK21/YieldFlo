using System;
using System.Windows.Forms;
using YieldFlo.Communication;
using YieldFlo.Communication.Can;
using YieldFlo.Database;
using YieldFlo.Forms;

namespace YieldFlo.Classes
{
    public static class Core
    {
        // Subsystems
        public static UDPComm UDPaog;                           // GPS from AOG       recv:17777 send:15555
        public static UDPComm UDPmodule;                        // YieldFlo module    recv:30100 (WiFi mode)
        public static CanModuleComm CanModule = new CanModuleComm(); // YieldFlo module    CAN mode
        public static clsGPS GPS = new clsGPS();
        public static clsYieldCalculator Yield;
        public static clsDataCollector Collector;
        public static DB Database;

        // UI
        public static frmMain MainForm;

        // Shared tools
        public static clsTools Tls = new clsTools();

        // Live sensor state (written by UDPComm/CanModuleComm, read by UI + DataCollector)
        public static double LastMoisture     { get; set; }
        public static double LastTemperature  { get; set; }
        public static double LastSensor1      { get; set; }
        public static int    LastNoiseCount   { get; set; }
        public static bool   ModuleConnected  { get; set; }
        public static DateTime LastModuleReceive { get; set; }

        // Active session configuration
        public static int    ActiveProfileId      { get; set; } = -1;
        public static int    ActiveCropId         { get; set; } = -1;
        public static int    ActiveHeaderId       { get; set; } = -1;
        public static double ActiveMoistureOffset { get; set; } = 0;
        public static double ActiveMoistScale     { get; set; } = 0.001;
        public static double ActiveTempOffset     { get; set; } = 0;
        public static double ActiveTempScale      { get; set; } = 0.0125;

        // Flags
        public static bool IsShuttingDown { get; private set; }
        public static bool IsRestarting { get; private set; }
        public static bool IsUserExitRequested { get; private set; }
        public static bool IsRestartedInstance { get; set; }

        // Events
        public static event EventHandler UpdateDisplay;
        public static event EventHandler GpsUpdated;
        public static event EventHandler JobStateChanged;
        public static event EventHandler FieldListChanged;
        public static event EventHandler CropListChanged;
        public static event EventHandler HeaderListChanged;
        public static event EventHandler ProfileListChanged;
        public static event EventHandler ColorChanged;
        public static event EventHandler AppExit;

        private static DateTime cStartTime;
        private static System.Timers.Timer MainTimer;

        public static void Initialize(frmMain frm)
        {
            try
            {
                MainForm = frm;

                if (!IsRestartedInstance && Tls.PrevInstance()) Application.Exit();

                Props.CheckFolders();

                // Database
                string dbPath = System.IO.Path.Combine(Props.DataFolder, "YieldFlo.db");
                Database = new DB(dbPath);
                Database.Initialize();

                // Yield engine
                Yield = new clsYieldCalculator();
                Yield.ProcessingDelaySec = Properties.Settings.Default.ProcessingDelaySec;
                Collector = new clsDataCollector();

                SeedDefaultData();
                TryResumeLastJob();

                // Load persisted comm settings
                Props.CanEnabled = Properties.Settings.Default.ModuleCommType == "CAN";
                if (Enum.TryParse(Properties.Settings.Default.CanDriver, out CanDriver cd))
                    Props.CurrentCanDriver = cd;
                Props.CanPort = Properties.Settings.Default.CanPort;

                // UDP
                UDPaog = new UDPComm(MainForm, 17777, 15555, 1461, "UDPaog", "127.255.255.255"); // send-from 1461 (RC uses 1460)
                UDPmodule = new UDPComm(MainForm, 30100, 30200, 1500, "UDPmodule");

                UDPmodule.Start();
                if (!UDPmodule.IsRunning)
                    Props.ShowMessage("Module UDP failed to start.", "", 3000, true);

                UDPaog.Start();
                if (!UDPaog.IsRunning)
                    Props.ShowMessage("AOG UDP failed to start.", "", 3000, true);

                // CAN (only if configured)
                if (Props.CanEnabled)
                    UseCanComm(true);

                // 1-second status timer
                MainTimer = new System.Timers.Timer(1000);
                MainTimer.Elapsed += MainTimer_Elapsed;
                MainTimer.AutoReset = true;
                MainTimer.Enabled = true;

                Props.WriteActivityLog("Started", true);
                cStartTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start: " + ex.Message, "Fatal Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        public static bool AppShutDown(FormClosingEventArgs e)
        {
            bool allow = IsUserExitRequested || IsRestarting
                || e.CloseReason == CloseReason.WindowsShutDown
                || e.CloseReason == CloseReason.TaskManagerClosing;

            if (allow)
            {
                IsShuttingDown = true;
                SafeTry(() => MainTimer.Enabled = false);
                SafeTry(() => UDPaog?.Stop());
                SafeTry(() => UDPmodule?.Stop());
                SafeTry(() => CanModule?.Stop());
                if (Properties.Settings.Default.ResumeJobOnStart && Collector?.ActiveJobId > 0)
                    SafeTry(() => Collector?.SuspendJob());
                else
                    SafeTry(() => Collector?.StopJob());
                SafeTry(() => Database?.Close());
                SafeTry(() => SafeEvent.Raise(AppExit));
                SafeTry(() => LogRunTime());
            }
            return allow;
        }

        public static void RequestUserExit()
        {
            if (ModuleConnected)
            {
                var answer = MessageBox.Show("Confirm Exit?", "Exit",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes) return;
            }
            IsUserExitRequested = true;
            Application.Exit();
        }

        public static void RequestRestart(string language = null)
        {
            IsRestarting = true;
            // Pass language + restarted flag as args so:
            //   1. Language is received directly (bypasses any settings version-path mismatch)
            //   2. PrevInstance() check is skipped (old process may still be in cleanup when new one starts)
            // Start AFTER ApplicationExit so UDP ports are released before new instance binds them.
            string args = "--restarted" + (language != null ? $" --lang={language}" : "");
            Application.ApplicationExit += (s, ev) =>
                System.Diagnostics.Process.Start(Application.ExecutablePath, args);
            Application.Exit();
        }

        public static void RaiseColorChanged() => SafeEvent.Raise(ColorChanged);
        public static void RaiseJobStateChanged()   => SafeEvent.Raise(JobStateChanged);
        public static void RaiseFieldListChanged()   => SafeEvent.Raise(FieldListChanged);
        public static void RaiseCropListChanged()    => SafeEvent.Raise(CropListChanged);
        public static void RaiseHeaderListChanged()  => SafeEvent.Raise(HeaderListChanged);
        public static void RaiseProfileListChanged() => SafeEvent.Raise(ProfileListChanged);
        public static void RaiseGpsUpdated() => SafeEvent.Raise(GpsUpdated);

        public static void LoadJobConfig(int profileId, int cropId, int headerId)
        {
            ActiveProfileId = profileId;
            ActiveCropId    = cropId;
            ActiveHeaderId  = headerId;

            if (Database == null || Yield == null) return;

            foreach (var c in Database.Crops.GetAll())
            {
                if (c.id != cropId) continue;
                Yield.TestWeightLbsBu = c.testWeight;   // Props.TestWeightKgPerBu derives from this
                ActiveMoistureOffset  = c.moistureOffset;
                break;
            }

            foreach (var h in Database.Headers.GetAll())
            {
                if (h.id != headerId) continue;
                Yield.HeaderWidthM     = h.widthM;
                Yield.HeaderFwdOffsetM = h.fwdOffsetM;
                break;
            }

            foreach (var p in Database.Profiles.GetAll())
            {
                if (p.id != profileId) continue;
                ActiveTempOffset  = p.tempOffset;
                ActiveTempScale   = p.tempScale  > 0 ? p.tempScale  : 0.0125;
                ActiveMoistScale  = p.moistScale > 0 ? p.moistScale : 0.001;
                break;
            }

            if (profileId > 0 && cropId > 0)
            {
                var cal = Database.Calibrations.GetLatest(profileId, cropId);
                Yield.SensorBaseline     = cal.baseline;
                Yield.YieldFactor        = cal.yieldFactor;
                Yield.ProcessingDelaySec = cal.delaySec > 0
                    ? cal.delaySec
                    : Properties.Settings.Default.ProcessingDelaySec;
            }
        }

        private static void SeedDefaultData()
        {
            try
            {
                if (Database.Profiles.GetAll().Count == 0)
                    Database.Profiles.Create("Default", "Combine 1");

                if (Database.Crops.GetAll().Count == 0)
                {
                    Database.Crops.Create("Wheat",  "Cereal",  60.0, 14.0, 14.0);
                    Database.Crops.Create("Canola", "OilSeed", 50.0, 10.0, 10.0);
                    Database.Crops.Create("Corn",   "Corn",    56.0, 15.5, 15.5);
                    Database.Crops.Create("Barley", "Cereal",  48.0, 14.0, 14.0);
                }

                if (Database.Headers.GetAll().Count == 0)
                    Database.Headers.Create("30ft Draper", "Draper", 9.144);

                // Set active to first available so yield calc has reasonable defaults
                var profiles = Database.Profiles.GetAll();
                var crops    = Database.Crops.GetAll();
                var headers  = Database.Headers.GetAll();

                if (profiles.Count > 0) ActiveProfileId = profiles[0].id;
                if (crops.Count    > 0) ActiveCropId    = crops[0].id;
                if (headers.Count  > 0) ActiveHeaderId  = headers[0].id;

                if (ActiveCropId > 0 && ActiveHeaderId > 0)
                    LoadJobConfig(ActiveProfileId, ActiveCropId, ActiveHeaderId);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("Core/SeedDefaultData: " + ex.Message);
            }
        }

        private static void TryResumeLastJob()
        {
            try
            {
                bool resumed = false;
                foreach (var j in Database.Jobs.GetAll())
                {
                    if (j.status != "Active") continue;

                    if (!resumed && Properties.Settings.Default.ResumeJobOnStart)
                    {
                        int profileId = j.profileId > 0 ? j.profileId : ActiveProfileId;
                        int cropId    = j.cropId    > 0 ? j.cropId    : ActiveCropId;
                        int headerId  = j.headerId  > 0 ? j.headerId  : ActiveHeaderId;
                        LoadJobConfig(profileId, cropId, headerId);
                        Collector.LoadJob(j.id, j.name, j.acres, j.volume);
                        RaiseJobStateChanged();
                        resumed = true;
                    }
                    else
                    {
                        // Close any extra stale active jobs
                        Database.Jobs.Close(j.id);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("Core/TryResumeLastJob: " + ex.Message);
            }
        }

        public static int UseCanComm(bool enable)
        {
            if (enable)
            {
                bool ok = CanModule.Start(Props.CurrentCanDriver, Props.CanPort);
                Props.CanEnabled = ok;
                if (!ok)
                    Props.ShowMessage("CAN failed to start. Check driver and COM port.", "", 5000, true);
                return ok ? 1 : 2;
            }
            else
            {
                CanModule?.Stop();
                Props.CanEnabled = false;
                return 0;
            }
        }

        private static void MainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SafeEvent.Raise(UpdateDisplay);
        }

        private static void LogRunTime()
        {
            Props.WriteActivityLog("Stopped");
            string mes = "Run time (hours): " +
                ((DateTime.Now - cStartTime).TotalSeconds / 3600.0).ToString("N1");
            Props.WriteActivityLog(mes);
        }

        private static void SafeTry(Action action)
        {
            try { action(); }
            catch (Exception ex)
            {
                try { Props.WriteActivityLog("Shutdown error: " + ex.Message); }
                catch { }
            }
        }
    }

    // ── SafeEvent ─────────────────────────────────────────────────────────────
    public static class SafeEvent
    {
        public static void Raise(EventHandler evt, EventArgs args = null, object sender = null)
        {
            if (evt == null) return;
            if (args == null) args = EventArgs.Empty;
            if (sender == null) sender = typeof(Core);

            foreach (EventHandler handler in evt.GetInvocationList())
                InvokeHandler(handler, sender, args);
        }

        private static void InvokeHandler(EventHandler handler, object sender, EventArgs args)
        {
            if (handler.Target is System.Windows.Forms.Control ctrl)
            {
                if (ctrl.IsDisposed || ctrl.Disposing || !ctrl.IsHandleCreated) return;
                if (ctrl.InvokeRequired)
                    ctrl.BeginInvoke(new Action(() => handler(sender, args)));
                else
                    handler(sender, args);
            }
            else
            {
                handler(sender, args);
            }
        }
    }

    // ── FormManager ───────────────────────────────────────────────────────────
    public static class FormManager
    {
        private static readonly System.Collections.Generic.Dictionary<string, Form> forms
            = new System.Collections.Generic.Dictionary<string, Form>();

        public static void ShowForm(Form frm)
        {
            string key = frm.GetType().FullName;
            var main = Core.MainForm;
            if (main == null || main.IsDisposed || !main.IsHandleCreated) return;

            if (main.InvokeRequired)
            {
                main.BeginInvoke((Action)(() => ShowForm(frm)));
                return;
            }

            if (forms.TryGetValue(key, out Form existing) && !existing.IsDisposed)
            {
                existing.BringToFront();
                return;
            }

            forms[key] = frm;
            frm.FormClosed += (s, e) => forms.Remove(key);
            frm.Show();
        }
    }
}
