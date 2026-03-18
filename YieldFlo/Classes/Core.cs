using System;
using System.Windows.Forms;
using YieldFlo.Communication;
using YieldFlo.Database;
using YieldFlo.Forms;

namespace YieldFlo.Classes
{
    public static class Core
    {
        // Subsystems
        public static UDPComm UDPaog;           // GPS from AOG       recv:17777 send:15555
        public static UDPComm UDPmodule;        // YieldFlo module    recv:30100 send:30200
        public static clsGPS GPS = new clsGPS();
        public static clsYieldCalculator Yield;
        public static clsDataCollector Collector;
        public static DB Database;

        // UI
        public static frmMain MainForm;

        // Shared tools
        public static clsTools Tls = new clsTools();

        // Live sensor state (written by UDPComm, read by UI + DataCollector)
        public static double LastMoisture { get; set; }
        public static double LastSensor1 { get; set; }
        public static double LastSensor2 { get; set; }
        public static bool ModuleConnected { get; set; }
        public static DateTime LastModuleReceive { get; set; }

        // Active session configuration
        public static int ActiveProfileId { get; set; } = -1;
        public static int ActiveCropId    { get; set; } = -1;
        public static int ActiveHeaderId  { get; set; } = -1;

        // Flags
        public static bool IsShuttingDown { get; private set; }
        public static bool IsRestarting { get; private set; }
        public static bool IsUserExitRequested { get; private set; }

        // Events
        public static event EventHandler UpdateDisplay;
        public static event EventHandler GpsUpdated;
        public static event EventHandler JobStateChanged;
        public static event EventHandler ColorChanged;
        public static event EventHandler AppExit;

        private static DateTime cStartTime;
        private static System.Timers.Timer MainTimer;

        public static void Initialize(frmMain frm)
        {
            try
            {
                MainForm = frm;

                if (Tls.PrevInstance()) Application.Exit();

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

        public static void RequestRestart()
        {
            IsRestarting = true;
            Application.Restart();
        }

        public static void RaiseColorChanged() => SafeEvent.Raise(ColorChanged);
        public static void RaiseJobStateChanged() => SafeEvent.Raise(JobStateChanged);
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
                Yield.TestWeightLbsBu  = c.testWeight;
                Props.TestWeightKgPerBu = c.testWeight * 0.453592;
                break;
            }

            foreach (var h in Database.Headers.GetAll())
            {
                if (h.id != headerId) continue;
                Yield.HeaderWidthM = h.widthM;
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

        public static int UseCanComm(bool enable)
        {
            // Placeholder — CAN bridge implementation added in Phase 2
            Props.CanEnabled = enable;
            Props.ShowMessage(enable ? "CAN mode enabled." : "UDP mode enabled.");
            return enable ? 1 : 0;
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
