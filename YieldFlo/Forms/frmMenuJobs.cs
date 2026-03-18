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

        // Parallel list to lvJobs — stores per-job IDs and totals for Load action
        private readonly List<(int jobId, string jobName, int profileId, int cropId, int headerId, double acres, double volume)> _jobData
            = new List<(int, string, int, int, int, double, double)>();

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
            btnStart.BackColor = Color.FromArgb(0, 110, 0);
        }

        private void LoadCombos()
        {
            cboCrop.Items.Clear();    _cropIds.Clear();
            cboHeader.Items.Clear();  _headerIds.Clear();
            cboProfile.Items.Clear(); _profileIds.Clear();

            foreach (var c in Core.Database.Crops.GetAll())
            {
                cboCrop.Items.Add($"{c.name}  ({c.testWeight:F0} lb/bu)");
                _cropIds.Add(c.id);
            }
            int ci = _cropIds.IndexOf(Core.ActiveCropId);
            cboCrop.SelectedIndex = ci >= 0 ? ci : (cboCrop.Items.Count > 0 ? 0 : -1);

            foreach (var h in Core.Database.Headers.GetAll())
            {
                cboHeader.Items.Add($"{h.name}  ({h.widthM:F2} m)");
                _headerIds.Add(h.id);
            }
            int hi = _headerIds.IndexOf(Core.ActiveHeaderId);
            cboHeader.SelectedIndex = hi >= 0 ? hi : (cboHeader.Items.Count > 0 ? 0 : -1);

            foreach (var p in Core.Database.Profiles.GetAll())
            {
                cboProfile.Items.Add(p.name);
                _profileIds.Add(p.id);
            }
            int pi = _profileIds.IndexOf(Core.ActiveProfileId);
            cboProfile.SelectedIndex = pi >= 0 ? pi : (cboProfile.Items.Count > 0 ? 0 : -1);

            txtJobName.Text = "Job " + DateTime.Now.ToString("yyyyMMdd-HHmm");
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cboCrop.SelectedIndex < 0 || _cropIds.Count == 0)
            { Props.ShowMessage("Select a crop first.", "", 3000, true); return; }
            if (cboHeader.SelectedIndex < 0 || _headerIds.Count == 0)
            { Props.ShowMessage("Select a header first.", "", 3000, true); return; }

            int cropId    = _cropIds[cboCrop.SelectedIndex];
            int headerId  = _headerIds[cboHeader.SelectedIndex];
            int profileId = (_profileIds.Count > 0 && cboProfile.SelectedIndex >= 0)
                            ? _profileIds[cboProfile.SelectedIndex] : 1;

            string name = txtJobName.Text.Trim();
            if (string.IsNullOrEmpty(name))
                name = "Job " + DateTime.Now.ToString("yyyyMMdd-HHmm");

            Core.LoadJobConfig(profileId, cropId, headerId);
            int jobId = Core.Database.Jobs.Create(name, profileId, cropId, headerId);
            Core.Collector.StartJob(jobId, name);
            Core.RaiseJobStateChanged();
            this.Close();
        }

        private void btnLoadJob_Click(object sender, EventArgs e)
        {
            if (lvJobs.SelectedIndices.Count == 0)
            { Props.ShowMessage("Select a job from the list first.", "", 3000, true); return; }

            int idx = lvJobs.SelectedIndices[0];
            if (idx < 0 || idx >= _jobData.Count) return;

            var job = _jobData[idx];
            int profileId = job.profileId > 0 ? job.profileId : Core.ActiveProfileId;
            int cropId    = job.cropId    > 0 ? job.cropId    : Core.ActiveCropId;
            int headerId  = job.headerId  > 0 ? job.headerId  : Core.ActiveHeaderId;

            Core.LoadJobConfig(profileId, cropId, headerId);
            Core.Database.Jobs.Reopen(job.jobId);
            Core.Collector.LoadJob(job.jobId, job.jobName, job.acres, job.volume);
            Core.RaiseJobStateChanged();
            this.Close();
        }

        private void btnDeleteJob_Click(object sender, EventArgs e)
        {
            if (lvJobs.SelectedIndices.Count == 0)
            { Props.ShowMessage("Select a job from the list first.", "", 3000, true); return; }

            int idx = lvJobs.SelectedIndices[0];
            if (idx < 0 || idx >= _jobData.Count) return;

            var job = _jobData[idx];
            var answer = MessageBox.Show($"Delete job \"{lvJobs.Items[idx].Text}\"?",
                "Delete Job", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (answer != DialogResult.Yes) return;

            Core.Database.Jobs.Delete(job.jobId);
            LoadRecentJobs();
        }

        private void btnTitleClose_Click(object sender, EventArgs e) => this.Close();
        private void btnJobsClose_Click(object sender, EventArgs e)  => this.Close();
    }
}
