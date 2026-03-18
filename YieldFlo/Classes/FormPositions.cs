using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Persists and restores per-form screen positions across sessions.
    /// Stored as a simple text file in the user data folder.
    /// </summary>
    public static class FormPositions
    {
        private static readonly Dictionary<string, Point> _positions = new Dictionary<string, Point>();
        private static string _filePath;
        private static bool _loaded;

        private static void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;
            _filePath = Path.Combine(Props.DataFolder, "FormPositions.txt");
            try
            {
                if (!File.Exists(_filePath)) return;
                foreach (var line in File.ReadAllLines(_filePath))
                {
                    int eq = line.IndexOf('=');
                    if (eq < 1) continue;
                    string name = line.Substring(0, eq);
                    string[] xy = line.Substring(eq + 1).Split(',');
                    if (xy.Length == 2 && int.TryParse(xy[0], out int x) && int.TryParse(xy[1], out int y))
                        _positions[name] = new Point(x, y);
                }
            }
            catch { }
        }

        /// <summary>
        /// Call at the end of a form's Load handler.
        /// Sets StartPosition=Manual and restores location if a saved position is on a connected screen.
        /// </summary>
        public static void Restore(Form frm)
        {
            EnsureLoaded();
            if (!_positions.TryGetValue(frm.Name, out Point pt)) return;
            foreach (Screen s in Screen.AllScreens)
            {
                if (s.WorkingArea.Contains(pt))
                {
                    frm.StartPosition = FormStartPosition.Manual;
                    frm.Location = pt;
                    return;
                }
            }
        }

        /// <summary>
        /// Call from a form's FormClosed handler (or close buttons) to persist position.
        /// </summary>
        public static void Save(Form frm)
        {
            EnsureLoaded();
            _positions[frm.Name] = frm.Location;
            try
            {
                var lines = new List<string>();
                foreach (var kv in _positions)
                    lines.Add($"{kv.Key}={kv.Value.X},{kv.Value.Y}");
                File.WriteAllLines(_filePath, lines);
            }
            catch { }
        }
    }
}
