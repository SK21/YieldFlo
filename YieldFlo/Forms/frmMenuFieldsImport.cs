using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMenuFieldsImport : Form
    {
        private bool  _dragging;
        private Point _dragStart;

        private static readonly string[] SourceSubfolders = { @"AgOpenGPS\Fields", @"TWOL\Fields" };

        private List<string>   _folderNames = new List<string>();
        private HashSet<string> _existingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public frmMenuFieldsImport()
        {
            InitializeComponent();
        }

        private void frmMenuFieldsImport_Load(object sender, EventArgs e)
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
            LoadFolders();
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
                if (c is Button          btn) { btn.BackColor = ctrl; btn.ForeColor = Color.White; }
                if (c is CheckedListBox  clb) { clb.BackColor = ctrl; clb.ForeColor = Color.White; }
            }
            btnImport.BackColor = Color.FromArgb(0, 90, 0);
        }

        private void LoadFolders()
        {
            clbFields.Items.Clear();
            _folderNames.Clear();

            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var found = new List<string>();
            foreach (var sub in SourceSubfolders)
            {
                string dir = Path.Combine(docs, sub);
                if (!Directory.Exists(dir)) continue;
                found.AddRange(Directory.GetDirectories(dir).Select(Path.GetFileName));
            }

            _folderNames = found
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

            _existingNames = new HashSet<string>(
                Core.Database.Fields.GetAll().Select(f => f.name),
                StringComparer.OrdinalIgnoreCase);

            foreach (var name in _folderNames)
            {
                bool exists = _existingNames.Contains(name);
                clbFields.Items.Add(exists ? string.Format(Lang.lgAlreadyAdded, name) : name);
            }

            bool any = _folderNames.Count > 0;
            if (!any) Props.ShowMessage(Lang.lgNoFieldsFound, "", 3000, true);
            btnImport.Enabled    = any;
            btnSelectAll.Enabled = any;
        }

        private void clbFields_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _folderNames.Count) return;
            if (_existingNames.Contains(_folderNames[e.Index]))
                e.NewValue = CheckState.Unchecked;
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _folderNames.Count; i++)
                if (!_existingNames.Contains(_folderNames[i]))
                    clbFields.SetItemChecked(i, true);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i < _folderNames.Count; i++)
            {
                if (!clbFields.GetItemChecked(i)) continue;
                string name = _folderNames[i];
                if (_existingNames.Contains(name)) continue;
                Core.Database.Fields.Create(name);
                count++;
            }

            if (count > 0)
            {
                Core.RaiseFieldListChanged();
                Props.ShowMessage(string.Format(Lang.lgFieldsImported, count), "", 2000, false);
            }
            LoadFolders();
        }

        private void btnFieldsImportClose_Click(object sender, EventArgs e) => this.Close();
    }
}
