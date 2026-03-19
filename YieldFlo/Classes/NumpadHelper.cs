using System;
using System.Windows.Forms;
using YieldFlo.Forms;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Wires a finger-friendly numpad to NumericUpDown controls.
    /// Call Wire() for each control in the form's Shown event.
    /// Call SetFocus() at the end to move focus away from NUDs.
    /// </summary>
    public static class NumpadHelper
    {
        private static bool _open = false;

        public static void Wire(Form owner, NumericUpDown num, double min, double max,
                                int decimals, string title = "")
        {
            void ShowPad(object s, EventArgs e)
            {
                if (_open) return;
                _open = true;
                try
                {
                    using (var pad = new frmNumpad(min, max, (double)num.Value, decimals, title))
                    {
                        if (pad.ShowDialog(owner) == DialogResult.OK)
                        {
                            decimal clamped = (decimal)Math.Min(max, Math.Max(min, pad.ReturnValue));
                            num.Value = Math.Min(num.Maximum, Math.Max(num.Minimum, clamped));
                        }
                    }
                }
                finally { _open = false; }
            }

            num.Enter += ShowPad;
            num.Click  += ShowPad;
        }
    }
}
