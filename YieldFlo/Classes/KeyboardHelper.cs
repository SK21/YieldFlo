using System;
using System.Windows.Forms;
using YieldFlo.Forms;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Wires the tablet keyboard to TextBox controls.
    /// Call Wire() for each TextBox in the form's Shown event.
    /// </summary>
    public static class KeyboardHelper
    {
        private static bool _open = false;

        public static void Wire(Form owner, TextBox txt, string title = "", bool append = false)
        {
            void ShowKeyboard(object s, EventArgs e)
            {
                if (_open) return;
                _open = true;
                try
                {
                    using (var kb = new frmKeyboard(txt.Text, title, overwrite: !append))
                    {
                        if (kb.ShowDialog(owner) == DialogResult.OK)
                            txt.Text = kb.ReturnValue;
                    }
                }
                finally { _open = false; }
            }

            txt.Enter += ShowKeyboard;
            txt.Click  += ShowKeyboard;
        }
    }
}
