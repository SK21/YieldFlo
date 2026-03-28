using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using YieldFlo.Forms;

namespace YieldFlo
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.ThreadException += (s, e) => Log(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log(e.ExceptionObject as Exception);

            // --restarted flag: skip PrevInstance check (old process may still be exiting)
            bool isRestarted = false;
            string langArg   = null;
            foreach (string a in args)
            {
                if (a == "--restarted") isRestarted = true;
                if (a.StartsWith("--lang=")) langArg = a.Substring(7);
            }
            Classes.Core.IsRestartedInstance = isRestarted;

            // Apply language: prefer command-line arg (avoids settings version-path issues),
            // fall back to saved setting, then AOG registry setting, then English.
            try
            {
                string lang = langArg ?? Properties.Settings.Default.CurrentLanguage;
                if (string.IsNullOrWhiteSpace(lang))
                {
                    lang = TryGetAogLanguage() ?? "en";
                    Properties.Settings.Default.CurrentLanguage = lang;
                    Properties.Settings.Default.Save();
                }
                // Keep setting in sync if it was passed via arg
                if (langArg != null && Properties.Settings.Default.CurrentLanguage != langArg)
                {
                    Properties.Settings.Default.CurrentLanguage = langArg;
                    Properties.Settings.Default.Save();
                }
                var culture = new CultureInfo(lang);
                Thread.CurrentThread.CurrentCulture   = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var startup = new frmMain())
            {
                Application.Run(startup);
            }
        }

        // Checks AgOpenGPS registry for a language setting (same approach as RateController).
        // Returns the language code if found and supported, otherwise null.
        private static string TryGetAogLanguage()
        {
            try
            {
                string[] valid = { "en", "de", "hu", "nl", "pl", "ru", "fr", "lt" };
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AgOpenGPS"))
                {
                    string lang = key?.GetValue("Language") as string;
                    if (lang != null && Array.IndexOf(valid, lang) >= 0) return lang;
                }
            }
            catch { }
            return null;
        }

        private static void Log(Exception ex)
        {
            if (ex == null) return;
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "YieldFlo_Crash.log");
                File.AppendAllText(path,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {ex}{Environment.NewLine}{Environment.NewLine}");
            }
            catch { }
        }
    }
}
