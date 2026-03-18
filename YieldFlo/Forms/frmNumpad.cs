using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    /// <summary>
    /// Finger-friendly numeric keypad for tablet input.
    /// Usage: using (var f = new frmNumpad(min, max, current)) {
    ///            if (f.ShowDialog() == DialogResult.OK) value = f.ReturnValue; }
    /// </summary>
    public partial class frmNumpad : Form
    {
        private readonly double _min;
        private readonly double _max;
        private readonly int    _decimals;
        private bool  _overwrite = true;
        private bool  _dragging;
        private Point _dragStart;

        public double ReturnValue { get; private set; }

        public frmNumpad(double min, double max, double current, int decimals = 3, string title = "")
        {
            _min      = min;
            _max      = max;
            _decimals = decimals;
            InitializeComponent();
            lblTitle.Text    = title;
            lblMinMax.Text   = $"Min: {min}   Max: {max}";
            tboxDisplay.Text = current.ToString(CultureInfo.InvariantCulture);

            FormPositions.Restore(this);
            this.FormClosed += (s, e) => FormPositions.Save(this);

            // Drag on the display box and the panel background
            foreach (var ctl in new System.Windows.Forms.Control[] { tboxDisplay, pnlContent })
            {
                ctl.MouseDown += (s, e) => { if (((MouseEventArgs)e).Button == MouseButtons.Left) { _dragging = true;  _dragStart = ((MouseEventArgs)e).Location; } };
                ctl.MouseMove += (s, e) => { if (_dragging) { Left += ((MouseEventArgs)e).X - _dragStart.X; Top += ((MouseEventArgs)e).Y - _dragStart.Y; } };
                ctl.MouseUp   += (s, e) => _dragging = false;
            }
        }

        private void Append(string digit)
        {
            if (_overwrite) { tboxDisplay.Text = ""; _overwrite = false; }
            if (tboxDisplay.Text == "0") tboxDisplay.Text = digit;
            else tboxDisplay.Text += digit;
        }

        private void AppendDot()
        {
            if (_overwrite) { tboxDisplay.Text = "0"; _overwrite = false; }
            if (tboxDisplay.Text.Contains(".")) return;
            if (tboxDisplay.Text == "" || tboxDisplay.Text == "-")
                tboxDisplay.Text += "0";
            tboxDisplay.Text += ".";
        }

        private void Backspace()
        {
            _overwrite = false;
            if (tboxDisplay.Text.Length > 0)
                tboxDisplay.Text = tboxDisplay.Text.Substring(0, tboxDisplay.Text.Length - 1);
        }

        private void ToggleNeg()
        {
            if (_min >= 0) return;
            _overwrite = false;
            if (tboxDisplay.Text.StartsWith("-"))
                tboxDisplay.Text = tboxDisplay.Text.Substring(1);
            else
                tboxDisplay.Text = "-" + tboxDisplay.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(tboxDisplay.Text, NumberStyles.Any,
                                 CultureInfo.InvariantCulture, out double val))
            {
                tboxDisplay.BackColor = System.Drawing.Color.FromArgb(80, 20, 20);
                return;
            }

            if (val < _min || val > _max)
            {
                tboxDisplay.BackColor = System.Drawing.Color.FromArgb(80, 20, 20);
                lblMinMax.ForeColor   = System.Drawing.Color.OrangeRed;
                return;
            }

            ReturnValue  = Math.Round(val, _decimals);
            DialogResult = DialogResult.OK;
        }
    }
}
