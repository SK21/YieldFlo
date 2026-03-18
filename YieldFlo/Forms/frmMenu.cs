using System;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    public partial class frmMenu : Form
    {
        private bool _dragging;
        private Point _dragStart;

        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            FormPositions.Restore(this);
            this.FormClosed += (s2, ev2) => FormPositions.Save(this);
            foreach (Control c in new Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) { _dragging = true; _dragStart = ev.Location; } };
                c.MouseMove += (s, ev) => { if (_dragging) { Left += ev.X - _dragStart.X; Top += ev.Y - _dragStart.Y; } };
                c.MouseUp   += (s, ev) => _dragging = false;
            }
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            pnlTitle.BackColor   = back;
            pnlContent.BackColor = back;
            lblTitle.ForeColor   = Color.FromArgb(180, 200, 220);
            btnTitleClose.BackColor = Color.FromArgb(80, 30, 30);
            btnTitleClose.ForeColor = Color.White;
            foreach (Control c in pnlContent.Controls)
            {
                c.ForeColor = fore;
                if (c is Button btn)
                {
                    btn.BackColor = Color.FromArgb(60, 60, 60);
                    btn.ForeColor = Color.White;
                }
            }
        }

        private void btnTitleClose_Click(object sender, EventArgs e) => this.Close();
        private void btnClose_Click(object sender, EventArgs e)      => this.Close();

        private void btnJobs_Click(object sender, EventArgs e)      => FormManager.ShowForm(new frmMenuJobs());
        private void btnCrops_Click(object sender, EventArgs e)     => FormManager.ShowForm(new frmMenuCrops());
        private void btnHeaders_Click(object sender, EventArgs e)   => FormManager.ShowForm(new frmMenuHeaders());
        private void btnProfiles_Click(object sender, EventArgs e)  => FormManager.ShowForm(new frmMenuProfiles());
        private void btnCalibrate_Click(object sender, EventArgs e) => FormManager.ShowForm(new frmMenuCalibrate());
        private void btnSettings_Click(object sender, EventArgs e) => FormManager.ShowForm(new frmMenuSettings());
        private void btnReports_Click(object sender, EventArgs e)   => FormManager.ShowForm(new frmJobReport());
        private void btnYieldMap_Click(object sender, EventArgs e)  => FormManager.ShowForm(new frmYieldMap());
    }
}
