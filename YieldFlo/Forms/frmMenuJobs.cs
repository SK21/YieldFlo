using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMenuJobs : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private readonly List<int> _cropIds    = new List<int>();
        private readonly List<int> _headerIds  = new List<int>();
        private readonly List<int> _profileIds = new List<int>();
        private readonly List<int> _fieldIds   = new List<int>();

        private readonly List<(int jobId, string jobName, string status, string startedAt, int profileId, int cropId, int headerId, int fieldId, double acres, double volume, string fieldName, string notes)> _jobData
            = new List<(int, string, string, string, int, int, int, int, double, double, string, string)>();

        private int  _sortCol = 2;   // Date by default
        private bool _sortAsc = false; // newest first
        private bool _creatingNew = false;  // true between New (prepare) and Save (commit)

        private string[] _colNames;


        public frmMenuJobs()
        {
            InitializeComponent();
        }

        private void frmMenuJobs_Load(object sender, EventArgs e)
        {
            _colNames = new string[] { Lang.lgColJobName, Lang.lgColStatus, Lang.lgColDate, Props.AreaUnit, Lang.lgColField };
            ApplyTheme();
            FormPositions.Restore(this);
            this.FormClosed += (s2, ev2) => FormPositions.Save(this);
            foreach (Control c in new Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) { _dragging = true; _dragStart = ev.Location; } };
                c.MouseMove += (s, ev) => { if (_dragging) { Left += ev.X - _dragStart.X; Top += ev.Y - _dragStart.Y; } };
                c.MouseUp   += (s, ev) => _dragging = false;
            }

            LoadCombos();
            LoadRecentJobs();
            SelectInitialJob();
            this.Shown += frmMenuJobs_Shown;
            Core.FieldListChanged   += Core_FieldListChanged;
            Core.CropListChanged    += Core_CropListChanged;
            Core.HeaderListChanged  += Core_HeaderListChanged;
            Core.ProfileListChanged += Core_ProfileListChanged;
            this.FormClosed += (s, ev) =>
            {
                Core.FieldListChanged   -= Core_FieldListChanged;
                Core.CropListChanged    -= Core_CropListChanged;
                Core.HeaderListChanged  -= Core_HeaderListChanged;
                Core.ProfileListChanged -= Core_ProfileListChanged;
            };
        }

        private void Core_FieldListChanged(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            this.BeginInvoke((Action)RefreshFieldCombo);
        }

        private void Core_CropListChanged(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            this.BeginInvoke((Action)RefreshCropCombo);
        }

        private void Core_HeaderListChanged(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            this.BeginInvoke((Action)RefreshHeaderCombo);
        }

        private void Core_ProfileListChanged(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            this.BeginInvoke((Action)RefreshProfileCombo);
        }

        private void RefreshFieldCombo()
        {
            int cur = cboField.SelectedIndex >= 0 ? _fieldIds[cboField.SelectedIndex] : -1;
            cboField.Items.Clear(); _fieldIds.Clear();
            cboField.Items.Add("(none)"); _fieldIds.Add(-1);
            foreach (var f in Core.Database.Fields.GetAll())
            { cboField.Items.Add(f.name); _fieldIds.Add(f.id); }
            int idx = _fieldIds.IndexOf(cur);
            cboField.SelectedIndex = idx >= 0 ? idx : 0;
        }

        private void RefreshCropCombo()
        {
            int cur = cboCrop.SelectedIndex >= 0 ? _cropIds[cboCrop.SelectedIndex] : -1;
            cboCrop.Items.Clear(); _cropIds.Clear();
            foreach (var c in Core.Database.Crops.GetAll())
            { cboCrop.Items.Add($"{c.name}  ({Props.DisplayTestWeight(c.testWeight):F0} {Props.TestWeightUnit})"); _cropIds.Add(c.id); }
            int idx = _cropIds.IndexOf(cur);
            cboCrop.SelectedIndex = idx >= 0 ? idx : (cboCrop.Items.Count > 0 ? 0 : -1);
        }

        private void RefreshHeaderCombo()
        {
            int cur = cboHeader.SelectedIndex >= 0 ? _headerIds[cboHeader.SelectedIndex] : -1;
            cboHeader.Items.Clear(); _headerIds.Clear();
            foreach (var h in Core.Database.Headers.GetAll())
            { double w = Props.IsMetric ? h.widthM : h.widthM * 3.28084; string wu = Props.IsMetric ? "m" : "ft"; string wf = Props.IsMetric ? "F2" : "F1"; cboHeader.Items.Add($"{h.name}  ({w.ToString(wf)} {wu})"); _headerIds.Add(h.id); }
            int idx = _headerIds.IndexOf(cur);
            cboHeader.SelectedIndex = idx >= 0 ? idx : (cboHeader.Items.Count > 0 ? 0 : -1);
        }

        private void RefreshProfileCombo()
        {
            int cur = cboProfile.SelectedIndex >= 0 ? _profileIds[cboProfile.SelectedIndex] : -1;
            cboProfile.Items.Clear(); _profileIds.Clear();
            foreach (var p in Core.Database.Profiles.GetAll())
            { cboProfile.Items.Add(p.name); _profileIds.Add(p.id); }
            int idx = _profileIds.IndexOf(cur);
            cboProfile.SelectedIndex = idx >= 0 ? idx : (cboProfile.Items.Count > 0 ? 0 : -1);
        }

        private void frmMenuJobs_Shown(object sender, EventArgs e)
        {
            KeyboardHelper.Wire(this, txtJobName, "Job Name");
            KeyboardHelper.Wire(this, txtNotes, "Notes", append: true);
            btnJobsClose.Focus();
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var ctrl = Color.FromArgb(60, 60, 60);

            pnlTitle.BackColor   = back;
            pnlContent.BackColor = back;
            lblTitle.ForeColor   = Color.FromArgb(180, 200, 220);
            btnTitleClose.BackColor = Color.FromArgb(80, 30, 30);
            btnTitleClose.ForeColor = Color.White;

            foreach (Control c in pnlContent.Controls)
            {
                c.ForeColor = fore;
                if (c is Button btn)   { btn.BackColor  = ctrl; btn.ForeColor  = Color.White; }
                if (c is TextBox tb)   { tb.BackColor   = ctrl; tb.ForeColor   = Color.White; }
                if (c is ComboBox cb)  { cb.BackColor = ctrl; cb.ForeColor = Color.White; }
                if (c is ListView lv)  { lv.BackColor   = ctrl; lv.ForeColor   = Color.White; }
                if (c is TextBox tb2 && tb2 != txtJobName) { tb2.BackColor = ctrl; tb2.ForeColor = Color.White; }
            }
            btnSave.BackColor   = Color.FromArgb(0, 90, 0);
            btnDelete.BackColor = Color.FromArgb(100, 0, 0);
        }

        private void LoadCombos()
        {
            cboCrop.Items.Clear();    _cropIds.Clear();
            cboHeader.Items.Clear();  _headerIds.Clear();
            cboProfile.Items.Clear(); _profileIds.Clear();
            cboField.Items.Clear();   _fieldIds.Clear();

            foreach (var c in Core.Database.Crops.GetAll())
            { cboCrop.Items.Add($"{c.name}  ({Props.DisplayTestWeight(c.testWeight):F0} {Props.TestWeightUnit})"); _cropIds.Add(c.id); }

            foreach (var h in Core.Database.Headers.GetAll())
            { double w = Props.IsMetric ? h.widthM : h.widthM * 3.28084; string wu = Props.IsMetric ? "m" : "ft"; string wf = Props.IsMetric ? "F2" : "F1"; cboHeader.Items.Add($"{h.name}  ({w.ToString(wf)} {wu})"); _headerIds.Add(h.id); }

            foreach (var p in Core.Database.Profiles.GetAll())
            { cboProfile.Items.Add(p.name); _profileIds.Add(p.id); }

            cboField.Items.Add("(none)"); _fieldIds.Add(-1);
            foreach (var f in Core.Database.Fields.GetAll())
            { cboField.Items.Add(f.name); _fieldIds.Add(f.id); }

            SetDefaultSelections();
        }

        private void SetDefaultSelections()
        {
            int ci = _cropIds.IndexOf(Core.ActiveCropId);
            cboCrop.SelectedIndex = ci >= 0 ? ci : (cboCrop.Items.Count > 0 ? 0 : -1);
            int hi = _headerIds.IndexOf(Core.ActiveHeaderId);
            cboHeader.SelectedIndex = hi >= 0 ? hi : (cboHeader.Items.Count > 0 ? 0 : -1);
            int pi = _profileIds.IndexOf(Core.ActiveProfileId);
            cboProfile.SelectedIndex = pi >= 0 ? pi : (cboProfile.Items.Count > 0 ? 0 : -1);
            cboField.SelectedIndex = 0;  // default "(none)"
            txtJobName.Text = "Job " + DateTime.Now.ToString("yyyyMMdd-HHmm");
        }

        private void LoadRecentJobs()
        {
            _jobData.Clear();
            foreach (var j in Core.Database.Jobs.GetAll())
            {
                int fi = _fieldIds.IndexOf(j.fieldId);
                string fname = fi > 0 ? (string)cboField.Items[fi] : "";
                string date  = j.startedAt.Length >= 10 ? j.startedAt.Substring(0, 10) : j.startedAt;
                _jobData.Add((j.id, j.name, j.status, date, j.profileId, j.cropId, j.headerId, j.fieldId, j.acres, j.volume, fname, j.notes));
            }
            SortJobs();
        }

        private void RefreshListFromData()
        {
            int selJobId = lvJobs.SelectedIndices.Count > 0 ? _jobData[lvJobs.SelectedIndices[0]].jobId : -1;
            lvJobs.Items.Clear();
            for (int i = 0; i < _jobData.Count; i++)
            {
                var j = _jobData[i];
                var item = lvJobs.Items.Add(j.jobName);
                item.SubItems.Add(j.status);
                item.SubItems.Add(j.startedAt);
                item.SubItems.Add(Props.DisplayArea(j.acres).ToString("F2") + " " + Props.AreaUnit);
                item.SubItems.Add(j.fieldName);
            }
            int newSel = _jobData.FindIndex(j => j.jobId == selJobId);
            if (newSel >= 0) { lvJobs.Items[newSel].Selected = true; lvJobs.Items[newSel].EnsureVisible(); }
        }

        private void SortJobs()
        {
            _jobData.Sort((a, b) =>
            {
                int cmp = _sortCol switch
                {
                    0 => string.Compare(a.jobName,   b.jobName,   StringComparison.OrdinalIgnoreCase),
                    1 => string.Compare(a.status,    b.status,    StringComparison.OrdinalIgnoreCase),
                    2 => string.Compare(a.startedAt, b.startedAt, StringComparison.Ordinal),
                    3 => a.acres.CompareTo(b.acres),
                    4 => string.Compare(a.fieldName, b.fieldName, StringComparison.OrdinalIgnoreCase),
                    _ => 0
                };
                return _sortAsc ? cmp : -cmp;
            });
            for (int i = 0; i < lvJobs.Columns.Count; i++)
                lvJobs.Columns[i].Text = i == _sortCol ? _colNames[i] + (_sortAsc ? " ▲" : " ▼") : _colNames[i];
            RefreshListFromData();
        }

        private void SelectInitialJob()
        {
            if (_jobData.Count == 0) return;

            // Prefer the currently active job; fall back to the most recent
            int activeId = Core.Collector.ActiveJobId;
            int idx = activeId > 0 ? _jobData.FindIndex(j => j.jobId == activeId) : -1;
            if (idx < 0) idx = 0;

            lvJobs.Items[idx].Selected = true;
            lvJobs.Items[idx].EnsureVisible();
        }

        private void lvJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool selected = lvJobs.SelectedIndices.Count > 0;
            btnLoad.Enabled   = selected;
            btnDelete.Enabled = selected;

            if (!selected)
            {
                btnSave.Enabled = false;
                txtNotes.Text   = "";
                return;
            }

            int idx = lvJobs.SelectedIndices[0];
            if (idx < 0 || idx >= _jobData.Count) return;

            var job = _jobData[idx];

            txtJobName.Text = job.jobName;
            txtNotes.Text   = job.notes;

            int ci = _cropIds.IndexOf(job.cropId);
            if (ci >= 0) cboCrop.SelectedIndex = ci;

            int hi = _headerIds.IndexOf(job.headerId);
            if (hi >= 0) cboHeader.SelectedIndex = hi;

            int pi = _profileIds.IndexOf(job.profileId);
            if (pi >= 0) cboProfile.SelectedIndex = pi;

            int fi = _fieldIds.IndexOf(job.fieldId);
            cboField.SelectedIndex = fi >= 0 ? fi : 0;

            _creatingNew = false;   // selecting an existing job leaves draft mode
            btnSave.Enabled = true;
        }

        // ── New ───────────────────────────────────────────────────────────────
        private void btnNew_Click(object sender, EventArgs e)
        {
            // Prepare an editable draft only — nothing is written to the DB and no
            // job is started until Save is pressed (see CreateAndStartNewJob).
            _creatingNew = true;
            lvJobs.SelectedItems.Clear();   // leave "edit existing job" mode
            SetDefaultSelections();         // default name + current crop/header/profile, field (none)
            txtNotes.Text = "";
            btnSave.Enabled   = true;
            btnLoad.Enabled   = false;
            btnDelete.Enabled = false;
            txtJobName.Focus();
        }

        // Commit a prepared draft: create the job, then start it (which closes and
        // saves any job currently active). Called from Save when in draft mode.
        private void CreateAndStartNewJob(string name)
        {
            if (cboCrop.SelectedIndex < 0 || _cropIds.Count == 0)
            { Props.ShowMessage(Lang.lgSelectCropFirst, "", 3000, true); return; }
            if (cboHeader.SelectedIndex < 0 || _headerIds.Count == 0)
            { Props.ShowMessage(Lang.lgSelectHeaderFirst, "", 3000, true); return; }

            // Guard against accidentally abandoning a job in progress. Prompt
            // whenever a job is active, including while auto-paused (e.g. a headland
            // turn), since IsRecording briefly drops to false there.
            if (Core.Collector.ActiveJobId > 0)
            {
                using var dlg = new frmMsgBox(string.Format(Lang.lgSwitchJobPrompt,
                    Core.Collector.ActiveJobName, name));
                dlg.ShowDialog(this);
                if (!dlg.Result) return;
            }

            int cropId    = _cropIds[cboCrop.SelectedIndex];
            int headerId  = _headerIds[cboHeader.SelectedIndex];
            int profileId = cboProfile.SelectedIndex >= 0 ? _profileIds[cboProfile.SelectedIndex] : 1;
            int fieldId   = cboField.SelectedIndex   >= 0 ? _fieldIds[cboField.SelectedIndex]     : -1;

            int jobId = Core.Database.Jobs.Create(name, profileId, cropId, headerId, fieldId);
            string notes = txtNotes.Text.Trim();
            if (notes.Length > 0)
                Core.Database.Jobs.Update(jobId, name, cropId, headerId, profileId, fieldId, notes);

            // Start it: apply config, mark Active, then switch the collector —
            // which saves and closes the previously active job. Totals start at zero.
            Core.LoadJobConfig(profileId, cropId, headerId);
            Core.Database.Jobs.Reopen(jobId);
            Core.Collector.LoadJob(jobId, name, 0, 0);
            Core.RaiseJobStateChanged();

            _creatingNew = false;
            LoadRecentJobs();

            int idx = _jobData.FindIndex(j => j.jobId == jobId);
            if (idx >= 0)
            {
                lvJobs.Items[idx].Selected = true;
                lvJobs.Items[idx].EnsureVisible();
            }
        }

        // ── Load (resume selected job and start recording) ────────────────────
        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (lvJobs.SelectedIndices.Count == 0)
            { Props.ShowMessage(Lang.lgSelectJobFirst, "", 3000, true); return; }

            int idx = lvJobs.SelectedIndices[0];
            if (idx < 0 || idx >= _jobData.Count) return;

            StartSelectedJob(_jobData[idx]);
        }

        // ── Save (create new or update existing) ─────────────────────────────
        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtJobName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            { Props.ShowMessage(Lang.lgEnterJobName, "", 2000, true); return; }

            // Draft from New: create and start the job.
            if (_creatingNew)
            {
                CreateAndStartNewJob(name);
                return;
            }

            // Otherwise update the selected existing job.
            if (lvJobs.SelectedIndices.Count == 0) return;

            int idx = lvJobs.SelectedIndices[0];
            if (idx < 0 || idx >= _jobData.Count) return;

            var job2      = _jobData[idx];
            int cropId    = cboCrop.SelectedIndex    >= 0 ? _cropIds[cboCrop.SelectedIndex]       : job2.cropId;
            int headerId  = cboHeader.SelectedIndex  >= 0 ? _headerIds[cboHeader.SelectedIndex]   : job2.headerId;
            int profileId = cboProfile.SelectedIndex >= 0 ? _profileIds[cboProfile.SelectedIndex] : job2.profileId;
            int fieldId   = cboField.SelectedIndex   >= 0 ? _fieldIds[cboField.SelectedIndex]     : job2.fieldId;

            int savedJobId = _jobData[idx].jobId;
            Core.Database.Jobs.Update(savedJobId, name, cropId, headerId, profileId, fieldId, txtNotes.Text.Trim());

            if (savedJobId == Core.Collector.ActiveJobId)
                Core.Collector.RenameActiveJob(name);

            LoadRecentJobs();

            int newIdx = _jobData.FindIndex(j => j.jobId == savedJobId);
            if (newIdx >= 0)
            {
                lvJobs.Items[newIdx].Selected = true;
                lvJobs.Items[newIdx].EnsureVisible();
            }
        }

        // ── Delete ────────────────────────────────────────────────────────────
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvJobs.SelectedIndices.Count == 0)
            { Props.ShowMessage(Lang.lgSelectJobFirst, "", 3000, true); return; }

            if (_jobData.Count <= 1) { Props.ShowMessage(Lang.lgMustHaveOneJob, "", 2000, true); return; }

            int idx = lvJobs.SelectedIndices[0];
            if (idx < 0 || idx >= _jobData.Count) return;

            var job = _jobData[idx];
            using var dlg = new frmMsgBox(string.Format(Lang.lgDeleteJobPrompt, job.jobName));
            dlg.ShowDialog(this);
            if (!dlg.Result) return;

            Core.Database.Jobs.Delete(job.jobId);
            LoadRecentJobs();
            SetDefaultSelections();
            btnSave.Enabled = btnLoad.Enabled = btnDelete.Enabled = false;
        }

        private void StartSelectedJob((int jobId, string jobName, string status, string startedAt, int profileId, int cropId, int headerId, int fieldId, double acres, double volume, string fieldName, string notes) job)
        {
            int activeId = Core.Collector.ActiveJobId;
            if (activeId > 0 && activeId != job.jobId)
            {
                string activeName = Core.Collector.ActiveJobName;
                using var dlg = new frmMsgBox(string.Format(Lang.lgSwitchJobPrompt, activeName, job.jobName));
                dlg.ShowDialog(this);
                if (!dlg.Result) return;
            }

            int profileId = job.profileId > 0 ? job.profileId : Core.ActiveProfileId;
            int cropId    = job.cropId    > 0 ? job.cropId    : Core.ActiveCropId;
            int headerId  = job.headerId  > 0 ? job.headerId  : Core.ActiveHeaderId;

            Core.LoadJobConfig(profileId, cropId, headerId);
            Core.Database.Jobs.Reopen(job.jobId);
            Core.Collector.LoadJob(job.jobId, job.jobName, job.acres, job.volume);
            Core.RaiseJobStateChanged();
            LoadRecentJobs();
            SelectInitialJob();
        }

        private void lvJobs_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            if (_sortCol == e.Column) _sortAsc = !_sortAsc;
            else { _sortCol = e.Column; _sortAsc = true; }
            SortJobs();
        }

        private void lvJobs_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e) => e.DrawDefault = true;

        private void lvJobs_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            bool selected = e.Item.Selected;
            Color back = selected ? SystemColors.Highlight : lvJobs.BackColor;
            Color fore = selected ? SystemColors.HighlightText : lvJobs.ForeColor;
            using (var brush = new SolidBrush(back))
                e.Graphics.FillRectangle(brush, e.Bounds);
            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, lvJobs.Font, e.Bounds, fore,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void btnTitleClose_Click(object sender, EventArgs e) => this.Close();
        private void btnJobsClose_Click(object sender, EventArgs e)  => this.Close();
    }
}
