using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMenuFields : Form
    {
        private List<(int id, string name)> _fields;
        private int   _editingId = -1;
        private bool  _dragging;
        private Point _dragStart;

        public frmMenuFields()
        {
            InitializeComponent();
        }

        private void frmMenuFields_Load(object sender, EventArgs e)
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
            this.Shown += (s, ev) => KeyboardHelper.Wire(this, txtName, "Field Name");
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var ctrl = Color.FromArgb(60, 60, 60);
            pnlTitle.BackColor      = back;
            pnlContent.BackColor    = back;
            lblTitle.ForeColor      = Color.FromArgb(180, 200, 220);
            foreach (Control c in pnlContent.Controls)
            {
                c.ForeColor = fore;
                if (c is Button  btn) { btn.BackColor = ctrl; btn.ForeColor = Color.White; }
                if (c is TextBox tb)  { tb.BackColor  = ctrl; tb.ForeColor  = Color.White; }
                if (c is ListBox lb)  { lb.BackColor  = ctrl; lb.ForeColor  = Color.White; }
            }
            btnSave.BackColor   = Color.FromArgb(0, 90, 0);
            btnDelete.BackColor = Color.FromArgb(100, 0, 0);
        }

        private void LoadList()
        {
            int savedId = _editingId;
            _fields = Core.Database.Fields.GetAll();
            lbFields.Items.Clear();
            foreach (var f in _fields)
                lbFields.Items.Add(f.name);
            ClearEdit();
            int sel = _fields?.FindIndex(f => f.id == savedId) ?? -1;
            if (sel >= 0) lbFields.SelectedIndex = sel;
        }

        private void ClearEdit()
        {
            _editingId  = -1;
            txtName.Text = "";
        }

        private void lbFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lbFields.SelectedIndex;
            if (idx < 0 || idx >= _fields.Count) { ClearEdit(); return; }
            _editingId   = _fields[idx].id;
            txtName.Text = _fields[idx].name;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            { Props.ShowMessage(Lang.lgEnterFieldName, "", 2000, true); return; }

            if (_editingId < 0)
                Core.Database.Fields.Create(name);
            else
                Core.Database.Fields.Update(_editingId, name);

            Core.RaiseFieldListChanged();
            LoadList();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editingId < 0) return;
            using var dlg = new frmMsgBox(Lang.lgDeleteFieldPrompt);
            dlg.ShowDialog(this);
            if (!dlg.Result) return;
            try { Core.Database.Fields.Delete(_editingId); }
            catch (Database.ItemInUseException) { Props.ShowMessage(Lang.lgItemInUseByJob, "", 3000, true); return; }
            Core.RaiseFieldListChanged();
            LoadList();
        }

        private void btnNew_Click(object sender, EventArgs e)         => ClearEdit();
        private void btnFieldsClose_Click(object sender, EventArgs e) => this.Close();

        private void btnImport_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            using var dlg = new frmMenuFieldsImport();
            dlg.ShowDialog(this);
            this.TopMost = true;
            LoadList();
        }
    }
}
