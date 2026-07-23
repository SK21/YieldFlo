using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMenuProfiles : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private List<(int id, string name, string combineId, double tempOffset, double tempScale, double moistScale)> _profiles;
        private int _editingId = -1;

        public frmMenuProfiles()
        {
            InitializeComponent();
        }

        private void frmMenuProfiles_Load(object sender, EventArgs e)
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
            LoadList();
            int activeIdx = _profiles?.FindIndex(p => p.id == Core.ActiveProfileId) ?? -1;
            if (activeIdx >= 0)
                lbProfiles.SelectedIndex = activeIdx;
            else if (lbProfiles.Items.Count > 0)
                lbProfiles.SelectedIndex = 0;
            else
                ClearEdit();
            this.Shown += frmMenuProfiles_Shown;
        }

        private void frmMenuProfiles_Shown(object sender, EventArgs e)
        {
            KeyboardHelper.Wire(this, txtProfileName, "Profile Name");
            KeyboardHelper.Wire(this, txtCombineId,   "Combine ID");
            btnSave.Focus();
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var ctrl = Color.FromArgb(60, 60, 60);
            pnlTitle.BackColor   = back;
            pnlContent.BackColor = back;
            lblTitle.ForeColor   = Color.FromArgb(180, 200, 220);
            lbProfiles.BackColor = ctrl;
            lbProfiles.ForeColor = fore;
            pnlEdit.BackColor    = back;
            foreach (Control c in pnlEdit.Controls)
            {
                c.ForeColor = fore;
                if (c is TextBox tb) { tb.BackColor = ctrl; tb.ForeColor = fore; }
            }
            btnNew.BackColor    = Color.FromArgb(60, 60, 60); btnNew.ForeColor    = Color.White;
            btnSave.BackColor   = Color.FromArgb(0, 90, 0);   btnSave.ForeColor   = Color.White;
            btnDelete.BackColor = Color.FromArgb(100, 0, 0);  btnDelete.ForeColor = Color.White;
            btnProfilesClose.BackColor = Color.FromArgb(60, 60, 60); btnProfilesClose.ForeColor = Color.White;
        }

        private void LoadList()
        {
            _profiles = Core.Database.Profiles.GetAll();
            lbProfiles.Items.Clear();
            foreach (var p in _profiles)
                lbProfiles.Items.Add($"{p.name}  –  {p.combineId}");
        }

        private void ClearEdit()
        {
            _editingId = -1;
            txtProfileName.Text  = "";
            txtCombineId.Text    = "";
            lbProfiles.ClearSelected();
        }

        private void lbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lbProfiles.SelectedIndex;
            if (idx < 0 || _profiles == null || idx >= _profiles.Count) return;
            var p = _profiles[idx];
            _editingId          = p.id;
            txtProfileName.Text = p.name;
            txtCombineId.Text   = p.combineId;
        }

        private void btnNew_Click(object sender, EventArgs e) => ClearEdit();

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtProfileName.Text.Trim();
            if (string.IsNullOrEmpty(name)) { Props.ShowMessage(Lang.lgEnterProfileName, "", 2000, true); return; }
            string cid = txtCombineId.Text.Trim();

            if (_editingId < 0)
                Core.Database.Profiles.Create(name, cid);
            else
                Core.Database.Profiles.Update(_editingId, name, cid);

            Core.RaiseProfileListChanged();
            LoadList();
            ClearEdit();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editingId < 0) return;
            if (_profiles.Count <= 1) { Props.ShowMessage(Lang.lgMustHaveOneProfile, "", 2000, true); return; }
            using var dlg = new frmMsgBox(Lang.lgDeleteProfilePrompt);
            dlg.ShowDialog(this);
            if (!dlg.Result) return;
            Core.Database.Profiles.Delete(_editingId);
            Core.RaiseProfileListChanged();
            LoadList();
            ClearEdit();
        }

        private void btnProfilesClose_Click(object sender, EventArgs e) => this.Close();
    }
}
