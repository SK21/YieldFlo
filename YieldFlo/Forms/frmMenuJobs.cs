using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    public partial class frmMenuJobs : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private readonly List<int> _cropIds    = new List<int>();
        private readonly List<int> _headerIds  = new List<int>();
        private readonly List<int> _profileIds = new List<int>();

        private readonly List<(int jobId, string jobName, int profileId, int cropId, int headerId, double acres, double volume)> _jobData
            = new List<(int, string, int, int, int, double, double)>();

        private bool   _suppressSelectionChange = false;

        public frmMenuJobs()
        {
            InitializeComponent();
        }

        private void frmMenuJobs_Load(object sender, EventArgs e)
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

            LoadCombos();
            LoadRecentJobs();
            SelectInitialJob();
            this.Shown += frmMenuJobs_Shown;
        }

        private void frmMenuJobs_Shown(object sender, EventArgs e)
        {
            KeyboardHelper.Wire(this, txtJobName, "Job Name");
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
                if (c is ComboBox cb)  { cb.BackColor   = ctrl; cb.ForeColor   = Color.White; }
                if (c is ListView lv)  { lv.BackColor   = ctrl; lv.ForeColor   = Color.White; }
            }
            btnSave.BackColor   = Color.FromArgb(0, 90, 0);
            btnDelete.BackColor = Color.FromArgb(100, 0, 0);
        }

        private void LoadCombos()
        {
            _suppressSelectionChange = true;

            cboCrop.Items.Clear();    _cropIds.Clear();
            cboHeader.Items.Clear();  _headerIds.Clear();
            cboProfile.Items.Clear(); _profileIds.Clear();

            foreach (var c in Core.Database.Crops.GetAll())
            { cboCrop.Items.Add($"{c.name}  ({c.testWeight:F0} lb/bu)"); _cropIds.Add(c.id); }

            foreach (var h in Core.Database.Headers.GetAll())
            { cboHeader.Items.Add($"{h.name}  ({h.widthM:F2} m)"); _headerIds.Add(h.id); }

            foreach (var p in Core.Database.Profiles.GetAll())
            { cboProfile.Items.Add(p.name); _profileIds.Add(p.id); }

            _suppressSelectionChange = false;

            SetDefaultSelections();
        }

        private void SetDefaultSelections()
        {
            _suppressSelectionChange = true;
            int ci = _cropIds.IndexOf(Core.ActiveCropId);
            cboCrop.SelectedIndex = ci >= 0 ? ci : (cboCrop.Items.Count > 0 ? 0 : -1);
            int hi = _headerIds.IndexOf(Core.ActiveHeaderId);
            cboHeader.SelectedIndex = hi >= 0 ? hi : (cboHeader.Items.Count > 0 ? 0 : -1);
            int pi = _profileIds.IndexOf(Core.ActiveProfileId);
            cboProfile.SelectedIndex = pi >= 0 ? pi : (cboProfile.Items.Count > 0 ? 0 : -1);
            txtJobName.Text = "Job " + DateTime.Now.ToString("yyyyMMdd-HHmm");
            _suppressSelectionChange = false;
        }

        private void LoadRecentJobs()
        {
            lvJobs.Items.Clear();
            _jobData.Clear();
            int count = 0;
            foreach (var j in Core.Database.Jobs.GetAll())
            {
                if (++count > 8) break;
                var item = lvJobs.Items.Add(j.name);
                item.SubItems.Add(j.status);
                item.SubItems.Add(j.startedAt.Length >= 10 ? j.startedAt.Substring(0, 10) : j.startedAt);
                item.SubItems.Add(j.acres.ToString("F2") + " ac");
                _jobData.Add((j.id, j.name, j.profileId, j.cropId, j.headerId, j.acres, j.volume));
            }
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
                return;
            }

            int idx = lvJobs.SelectedIndices[0];
            if (idx < 0 || idx >= _jobData.Count) return;

            var job = _jobData[idx];
            _suppressSelectionChange = true;

            txtJobName.Text = job.jobName;

            int ci = _cropIds.IndexOf(job.cropId);
            if (ci >= 0) cboCrop.SelectedIndex = ci;

            int hi = _headerIds.IndexOf(job.headerId);
            if (hi >= 0) cboHeader.SelectedIndex = hi;

            int pi = _profileIds.IndexOf(job.profileId);
            if (pi >= 0) cboProfile.SelectedIndex = pi;

            _suppressSelectionChange = false;
            btnSave.Enabled = true;
        }

        // ── New ───────────────────────────────────────────────────────────────
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (cboCrop.SelectedIndex < 0 || _cropIds.Count == 0)
            { Props.ShowMessage("Select a crop first.", "", 3000, true); return; }
            if (cboHeader.SelectedIndex < 0 || _headerIds.Count == 0)
            { Props.ShowMessage("Select a header first.", "", 3000, true); return; }

            string name = "Job " + DateTime.Now.ToString("yyyyMMdd-HHmm");
            int cropId    = _cropIds[cboCrop.SelectedIndex];
            int headerId  = _headerIds[cboHeader.SelectedIndex];
            int profileId = cboProfile.SelectedIndex >= 0 ? _profileIds[cboProfile.SelectedIndex] : 1;

            int jobId = Core.Database.Jobs.Create(name, profileId, cropId, headerId);

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
            { Props.ShowMessage("Select a job from the list first.", "", 3000, true); return; }

            int idx = lvJobs.SelectedIndices[0];
            if (idx < 0 || idx >= _jobData.Count) return;

            StartSelectedJob(_jobData[idx]);
        }

        // ── Save (create new or update existing) ─────────────────────────────
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lvJobs.SelectedIndices.Count == 0) return;

            int idx = lvJobs.SelectedIndices[0];
            if (idx < 0 || idx >= _jobData.Count) return;

            string name = txtJobName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            { Props.ShowMessage("Enter a job name.", "", 2000, true); return; }

            var job2      = _jobData[idx];
            int cropId    = cboCrop.SelectedIndex    >= 0 ? _cropIds[cboCrop.SelectedIndex]       : job2.cropId;
            int headerId  = cboHeader.SelectedIndex  >= 0 ? _headerIds[cboHeader.SelectedIndex]   : job2.headerId;
            int profileId = cboProfile.SelectedIndex >= 0 ? _profileIds[cboProfile.SelectedIndex] : job2.profileId;

            int savedJobId = _jobData[idx].jobId;
            Core.Database.Jobs.Update(savedJobId, name, cropId, headerId, profileId);

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
            { Props.ShowMessage("Select a job from the list first.", "", 3000, true); return; }

            int idx = lvJobs.SelectedIndices[0];
            if (idx < 0 || idx >= _jobData.Count) return;

            var job = _jobData[idx];
            using var dlg = new frmMsgBox($"Delete job \"{job.jobName}\"?");
            dlg.ShowDialog(this);
            if (!dlg.Result) return;

            Core.Database.Jobs.Delete(job.jobId);
            LoadRecentJobs();
            SetDefaultSelections();
            btnSave.Enabled = btnLoad.Enabled = btnDelete.Enabled = false;
        }

        private void StartSelectedJob((int jobId, string jobName, int profileId, int cropId, int headerId, double acres, double volume) job)
        {
            int activeId = Core.Collector.ActiveJobId;
            if (activeId > 0 && activeId != job.jobId)
            {
                string activeName = Core.Collector.ActiveJobName;
                using var dlg = new frmMsgBox($"Job \"{activeName}\" is active. Switch to \"{job.jobName}\"?");
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
