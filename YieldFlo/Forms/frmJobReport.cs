using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    public partial class frmJobReport : Form
    {
        private bool _dragging;
        private Point _dragStart;

        // Parallel list to lbJobs — stores (id, name, acres, volume)
        private readonly List<(int id, string name, double acres, double volume)> _jobs
            = new List<(int, string, double, double)>();

        private int _selectedJobId   = -1;
        private string _selectedName = "";

        public frmJobReport()
        {
            InitializeComponent();
        }

        private void frmJobReport_Load(object sender, EventArgs e)
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
            LoadJobs();
            Core.JobStateChanged += Core_JobStateChanged;
            this.FormClosed      += (s2, ev2) => Core.JobStateChanged -= Core_JobStateChanged;
        }

        private void Core_JobStateChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) { Invoke(new Action(() => Core_JobStateChanged(sender, e))); return; }
            LoadJobs();
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var ctrl = Color.FromArgb(60, 60, 60);

            pnlTitle.BackColor      = back;
            pnlContent.BackColor    = back;
            lblTitle.ForeColor      = Color.FromArgb(180, 200, 220);
            btnTitleClose.BackColor = Color.FromArgb(80, 30, 30);
            btnTitleClose.ForeColor = Color.White;

            lbJobs.BackColor = ctrl;
            lbJobs.ForeColor = fore;

            foreach (Control c in pnlContent.Controls)
            {
                if (c is Label lbl) lbl.ForeColor = fore;
                if (c is Button btn && btn != btnExportCsv)
                {
                    btn.BackColor = ctrl;
                    btn.ForeColor = Color.White;
                }
            }

            btnExportCsv.BackColor = Color.FromArgb(0, 110, 0);
            btnExportCsv.ForeColor = Color.White;
        }

        private void LoadJobs()
        {
            lbJobs.Items.Clear();
            _jobs.Clear();

            if (Core.Database == null) return;

            int count = 0;
            foreach (var j in Core.Database.Jobs.GetAll())
            {
                if (++count > 20) break;
                string date = j.startedAt.Length >= 10 ? j.startedAt.Substring(0, 10) : j.startedAt;
                lbJobs.Items.Add($"{j.name}  |  {date}  |  {j.status}");
                _jobs.Add((j.id, j.name, j.acres, j.volume));
            }

            int activeId  = Core.Collector?.ActiveJobId ?? -1;
            int activeIdx = activeId > 0 ? _jobs.FindIndex(j => j.id == activeId) : -1;
            lbJobs.SelectedIndex = activeIdx >= 0 ? activeIdx : (_jobs.Count > 0 ? 0 : -1);
        }

        private void lbJobs_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lbJobs.SelectedIndex;
            if (idx < 0 || idx >= _jobs.Count)
            {
                ClearSummary();
                return;
            }

            var job = _jobs[idx];
            _selectedJobId = job.id;
            _selectedName  = job.name;

            lblReportJobName.Text = job.name;

            double displayArea = Props.DisplayArea(job.acres);
            lblReportArea.Text = $"Area: {displayArea:F2} {Props.AreaUnit}";

            double displayMass = Props.DisplayMass(job.volume);
            lblReportTotal.Text = $"Total: {displayMass:F1} {Props.MassUnit}";

            double avgYield = job.acres > 0.001 ? job.volume / job.acres : 0;
            double displayRate = Props.DisplayRate(avgYield);
            lblReportAvgYield.Text = $"Avg Yield: {displayRate:F1} {Props.RateUnit}";

            // Get point count and average moisture from database
            int pointCount   = 0;
            double avgMoist  = 0;
            try
            {
                var points = Core.Database.YieldData.GetByJob(job.id);
                pointCount = points.Count;
                if (pointCount > 0)
                {
                    double moistSum = 0;
                    foreach (var p in points) moistSum += p.Moisture;
                    avgMoist = moistSum / pointCount;
                }
            }
            catch { }

            lblReportAvgMoist.Text = $"Avg Moisture: {avgMoist:F1} %";
            lblReportPoints.Text   = $"Data Points: {pointCount}";
        }

        private void ClearSummary()
        {
            _selectedJobId = -1;
            _selectedName  = "";
            lblReportJobName.Text  = "--";
            lblReportArea.Text     = "Area: --";
            lblReportTotal.Text    = "Total: --";
            lblReportAvgYield.Text = "Avg Yield: --";
            lblReportAvgMoist.Text = "Avg Moisture: --";
            lblReportPoints.Text   = "Data Points: --";
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (_selectedJobId < 0)
            {
                Props.ShowMessage("Select a job first.", "", 3000, true);
                return;
            }

            string path = CsvExporter.ExportJob(_selectedJobId, _selectedName);
            if (path != null)
                Props.ShowMessage($"Exported: {path}");
            else
                Props.ShowMessage("Export failed — no data or error.", "", 3000, true);
        }

        private void btnTitleClose_Click(object sender, EventArgs e)    => this.Close();
        private void btnReportClose_Click(object sender, EventArgs e)   => this.Close();
    }
}
