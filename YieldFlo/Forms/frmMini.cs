using System;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    public partial class frmMini : Form
    {
        private bool _dragging;
        private Point _dragStart;

        public frmMini()
        {
            InitializeComponent();
        }

        private void frmMini_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            RestorePosition();

            // Make entire panel + labels draggable
            pnlContent.MouseDown += Drag_MouseDown;
            pnlContent.MouseMove += Drag_MouseMove;
            pnlContent.MouseUp += Drag_MouseUp;
            lblYield.MouseDown += Drag_MouseDown;
            lblYield.MouseMove += Drag_MouseMove;
            lblYield.MouseUp += Drag_MouseUp;
            lblUnit.MouseDown += Drag_MouseDown;
            lblUnit.MouseMove += Drag_MouseMove;
            lblUnit.MouseUp += Drag_MouseUp;

            Core.UpdateDisplay += Core_UpdateDisplay;
            UpdateYield();
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var dispFore = Properties.Settings.Default.DisplayForeColour;

            pnlContent.BackColor = back;
            btnRestore.BackColor = back;
            btnRestore.ForeColor = Color.FromArgb(180, 200, 220);
            lblYield.BackColor = back;
            lblYield.ForeColor = dispFore;
            lblUnit.BackColor = back;
            lblUnit.ForeColor = dispFore;
        }

        private void RestorePosition()
        {
            int x = Properties.Settings.Default.MiniFormX;
            int y = Properties.Settings.Default.MiniFormY;
            if (x >= 0 && y >= 0)
            {
                var pt = new Point(x, y);
                foreach (Screen s in Screen.AllScreens)
                {
                    if (s.WorkingArea.Contains(pt))
                    {
                        this.Location = pt;
                        return;
                    }
                }
            }
            // Default: same position as main form
            var main = Core.MainForm;
            if (main != null)
                this.Location = main.Location;
        }

        private void SavePosition()
        {
            Properties.Settings.Default.MiniFormX = this.Location.X;
            Properties.Settings.Default.MiniFormY = this.Location.Y;
            Properties.Settings.Default.Save();
        }

        private void Core_UpdateDisplay(object sender, EventArgs e)
        {
            if (this.IsDisposed) return;
            this.BeginInvoke((Action)UpdateYield);
        }

        private void UpdateYield()
        {
            if (Core.IsShuttingDown) return;
            double yield = Props.DisplayRate(Core.Yield?.SmoothedYield ?? 0);
            lblYield.Text = yield.ToString("F1");
            lblUnit.Text = Props.RateUnit;
        }

        private void btnRestore_Click(object sender, EventArgs e) => Restore();

        private void frmMini_FormClosing(object sender, FormClosingEventArgs e)
        {
            SavePosition();
            Core.UpdateDisplay -= Core_UpdateDisplay;
            Core.MainForm?.Show();
        }

        private void Restore()
        {
            SavePosition();
            Core.UpdateDisplay -= Core_UpdateDisplay;
            Core.MainForm?.Show();
            this.Close();
        }

        private void Drag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { _dragging = true; _dragStart = e.Location; }
        }
        private void Drag_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging) { Left += e.X - _dragStart.X; Top += e.Y - _dragStart.Y; }
        }
        private void Drag_MouseUp(object sender, MouseEventArgs e) => _dragging = false;
    }
}
