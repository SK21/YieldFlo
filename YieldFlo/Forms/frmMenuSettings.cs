using System;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    // General settings. Module communication has its own page, frmMenuComm.
    public partial class frmMenuSettings : Form
    {
        private bool _dragging;
        private Point _dragStart;

        private static readonly Color ActiveColour = Color.FromArgb(0, 80, 160);
        private static readonly Color InactiveColour = Color.FromArgb(60, 60, 60);

        public frmMenuSettings()
        {
            InitializeComponent();
        }

        private void frmMenuSettings_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            FormPositions.Restore(this);
            this.FormClosed += (s2, ev2) => FormPositions.Save(this);
            foreach (Control c in new System.Windows.Forms.Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) { _dragging = true; _dragStart = ev.Location; } };
                c.MouseMove += (s, ev) => { if (_dragging) { Left += ev.X - _dragStart.X; Top += ev.Y - _dragStart.Y; } };
                c.MouseUp += (s, ev) => _dragging = false;
            }
            LoadCurrentSettings();
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;

            pnlTitle.BackColor = back;
            pnlContent.BackColor = back;
            lblTitle.ForeColor = Color.FromArgb(180, 200, 220);

            foreach (Control c in pnlContent.Controls)
            {
                c.ForeColor = fore;
                if (c is Button btn && btn != btnSaveSettings)
                {
                    btn.BackColor = InactiveColour;
                    btn.ForeColor = Color.White;
                }
            }

            btnSaveSettings.BackColor = Color.FromArgb(0, 110, 0);
            btnSaveSettings.ForeColor = Color.White;
            btnSettingsClose.BackColor = InactiveColour;
            btnSettingsClose.ForeColor = Color.White;
        }

        private void LoadCurrentSettings()
        {
            SetToggle(btnImperial, btnMetric, Properties.Settings.Default.Units == "Imperial");
            SetToggle(btnResumeOn, btnResumeOff, Properties.Settings.Default.ResumeJobOnStart);
        }

        private void SetToggle(Button active, Button inactive, bool firstActive)
        {
            active.BackColor = firstActive ? ActiveColour : InactiveColour;
            inactive.BackColor = firstActive ? InactiveColour : ActiveColour;
        }

        private void btnImperial_Click(object sender, EventArgs e) => SetToggle(btnImperial, btnMetric, true);
        private void btnMetric_Click(object sender, EventArgs e) => SetToggle(btnImperial, btnMetric, false);
        private void btnResumeOn_Click(object sender, EventArgs e) => SetToggle(btnResumeOn, btnResumeOff, true);
        private void btnResumeOff_Click(object sender, EventArgs e) => SetToggle(btnResumeOn, btnResumeOff, false);

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Units = btnImperial.BackColor == ActiveColour ? "Imperial" : "Metric";
            Properties.Settings.Default.ResumeJobOnStart = btnResumeOn.BackColor == ActiveColour;

            Properties.Settings.Default.Save();
            Core.RaiseColorChanged();
            Props.ShowMessage(Lang.lgSettingsSaved);
        }

        private void btnSettingsClose_Click(object sender, EventArgs e) => this.Close();
    }
}
