using System;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMenuLanguage : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private string _selectedLang;

        public frmMenuLanguage()
        {
            InitializeComponent();
        }

        private void frmMenuLanguage_Load(object sender, EventArgs e)
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

            _selectedLang = Properties.Settings.Default.CurrentLanguage ?? "en";
            if (string.IsNullOrWhiteSpace(_selectedLang)) _selectedLang = "en";
            HighlightSelected();
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var ctrl = Color.FromArgb(60, 60, 60);

            pnlTitle.BackColor   = back;
            pnlContent.BackColor = back;
            lblTitle.ForeColor   = Color.FromArgb(180, 200, 220);
            lblNote.ForeColor    = fore;

            foreach (Control c in pnlContent.Controls)
            {
                if (c is Button btn && btn != btnSaveRestart)
                {
                    btn.BackColor = ctrl;
                    btn.ForeColor = Color.White;
                }
            }

            btnSaveRestart.BackColor = Color.FromArgb(0, 110, 0);
            btnSaveRestart.ForeColor = Color.White;
        }

        private void HighlightSelected()
        {
            var active   = Color.FromArgb(0, 110, 180);
            var inactive = Color.FromArgb(60, 60, 60);

            foreach (Control c in pnlContent.Controls)
            {
                if (c is Button btn && btn.Tag is string tag &&
                    btn != btnSaveRestart && btn != btnClose)
                {
                    btn.BackColor = tag == _selectedLang ? active : inactive;
                }
            }
        }

        private void btnLang_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                _selectedLang = tag;
                HighlightSelected();
            }
        }

        private void btnSaveRestart_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CurrentLanguage = _selectedLang;
            Properties.Settings.Default.Save();
            Core.RequestRestart(_selectedLang);
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}
