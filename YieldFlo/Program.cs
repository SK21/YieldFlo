using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using YieldFlo.Forms;

namespace YieldFlo
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.ThreadException += (s, e) => Log(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log(e.ExceptionObject as Exception);

            // Default to English; extend locale list here as translations are added
            try
            {
                var culture = new CultureInfo("en");
                Thread.CurrentThread.CurrentCulture = culture;
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
