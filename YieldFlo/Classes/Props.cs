using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace YieldFlo.Classes
{
    public enum CanDriver { SLCAN, InnoMaker, PCAN }

    public static class Props
    {
        public static readonly string AppName = "YieldFlo";
        public static readonly string AppVersion = "1.0.0";
        public static readonly string AppDate = "30-Apr-2026";

        private static string cApplicationFolder;
        private static string cDataFolder;
        private static string cLogsFolder;
        private static string cExportFolder;
        private static bool cCanEnabled = false;
        private static CanDriver cCurrentCanDriver = CanDriver.SLCAN;
        private static string cCanPort = "";

        public static bool CanEnabled
        {
            get { return cCanEnabled; }
            set { cCanEnabled = value; }
        }

        public static string Version { get { return AppVersion; } }

        public static CanDriver CurrentCanDriver
        {
            get { return cCurrentCanDriver; }
            set { cCurrentCanDriver = value; }
        }

        public static string CanPort
        {
            get { return cCanPort; }
            set { cCanPort = value; }
        }

        // ── Units ─────────────────────────────────────────────────────────────
        // Internal values are always imperial (acres, bu, bu/ac).
        // TestWeightKgPerBu can be overridden by the active crop profile.
        public static double TestWeightKgPerBu { get; set; } = 25.4;  // default: corn (56 lb/bu)

        public static bool IsMetric => Properties.Settings.Default.Units == "Metric";
        public static string AreaUnit => IsMetric ? "ha" : "ac";
        public static string MassUnit => IsMetric ? "t" : "bu";
        public static string RateUnit => IsMetric ? "t/ha" : "bu/ac";
        public static string SpeedUnit => IsMetric ? "km/h" : "mph";

        public static string TestWeightUnit => IsMetric ? "kg/hL" : "lb/bu";

        // Grain test weight (specific / hectolitre weight) is stored internally
        // as lb/bu (US Winchester bushel = 35.239 L). Metric users work in kg/hL,
        // the standard European/Canadian grain unit: 1 lb/bu = 1.287184 kg/hL.
        private const double KgHlPerLbBu = 1.287184;
        public static double DisplayTestWeight(double lbBu)     => IsMetric ? lbBu * KgHlPerLbBu : lbBu;
        public static double TestWeightToLbBu(double displayTw) => IsMetric ? displayTw / KgHlPerLbBu : displayTw;

        public static double DisplayArea(double acres) => IsMetric ? acres * 0.404686 : acres;
        public static double DisplayMass(double bushels) => IsMetric ? bushels * TestWeightKgPerBu / 1000.0 : bushels;
        public static double DisplayRate(double buPerAc) => IsMetric ? buPerAc * TestWeightKgPerBu / 1000.0 / 0.404686 : buPerAc;
        public static double DisplaySpeed(double kmh) => IsMetric ? kmh : kmh * 0.621371;

        public static string ApplicationFolder { get { return cApplicationFolder; } }
        public static string DataFolder { get { return cDataFolder; } }
        public static string LogsFolder { get { return cLogsFolder; } }
        public static string ExportFolder { get { return cExportFolder; } }

        public static void CheckFolders()
        {
            cApplicationFolder = AppDomain.CurrentDomain.BaseDirectory;
            cDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                AppName);
            cLogsFolder = Path.Combine(cDataFolder, "Logs");
            cExportFolder = Path.Combine(cDataFolder, "Exports");

            Directory.CreateDirectory(cDataFolder);
            Directory.CreateDirectory(cLogsFolder);
            Directory.CreateDirectory(cExportFolder);
        }

        public static void WriteErrorLog(string message)
        {
            try
            {
                string path = Path.Combine(cLogsFolder ?? AppDomain.CurrentDomain.BaseDirectory, "Errors.log");
                File.AppendAllText(path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}{Environment.NewLine}");
            }
            catch { }
        }

        public static void WriteActivityLog(string message, bool newFile = false)
        {
            try
            {
                string path = Path.Combine(cLogsFolder ?? AppDomain.CurrentDomain.BaseDirectory, "Activity.log");
                if (newFile && File.Exists(path))
                    File.Delete(path);
                File.AppendAllText(path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}{Environment.NewLine}");
            }
            catch { }
        }

        public static void WriteLog(string fileName, string content, bool append = true, bool newFile = false)
        {
            try
            {
                string path = Path.Combine(cLogsFolder ?? AppDomain.CurrentDomain.BaseDirectory, fileName);
                if (newFile && File.Exists(path))
                    File.Delete(path);
                if (append)
                    File.AppendAllText(path, content);
                else
                    File.WriteAllText(path, content);
            }
            catch { }
        }

        public static void SaveFormLocation(Form frm)
        {
            try
            {
                Properties.Settings.Default[$"Form_{frm.Name}_X"] = frm.Location.X;
                Properties.Settings.Default[$"Form_{frm.Name}_Y"] = frm.Location.Y;
                Properties.Settings.Default.Save();
            }
            catch { }
        }

        public static void LoadFormLocation(Form frm)
        {
            try
            {
                object x = Properties.Settings.Default[$"Form_{frm.Name}_X"];
                object y = Properties.Settings.Default[$"Form_{frm.Name}_Y"];
                if (x != null && y != null)
                {
                    int px = Convert.ToInt32(x);
                    int py = Convert.ToInt32(y);
                    Rectangle screen = Screen.GetWorkingArea(frm);
                    if (screen.Contains(px, py))
                        frm.Location = new Point(px, py);
                }
            }
            catch { }
        }

        public static void ShowMessage(string message, string title = "Info", int durationMs = 3000, bool isError = false, bool topMost = false)
        {
            try
            {
                if (Core.MainForm == null || Core.MainForm.IsDisposed) return;

                if (isError)
                    System.Media.SystemSounds.Exclamation.Play();

                Core.MainForm.BeginInvoke((Action)(() =>
                {
                    Core.MainForm.ShowStatusMessage(message, isError);
                }));
            }
            catch { }
        }

        public static void ApplyTheme(Control control)
        {
            control.BackColor = Properties.Settings.Default.MainBackColour;
            control.ForeColor = Properties.Settings.Default.MainForeColour;
            foreach (Control child in control.Controls)
                ApplyTheme(child);
        }
    }
}
