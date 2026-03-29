using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;
using YieldFlo.Printing;

namespace YieldFlo.Forms
{
    public partial class frmJobReport : Form
    {
        private bool _dragging;
        private Point _dragStart;

        private readonly List<(int id, string name, string startedAt, double acres, double volume, string fieldName, int cropId, int headerId, string notes)> _jobs
            = new List<(int, string, string, double, double, string, int, int, string)>();

        private int _selectedJobId     = -1;
        private string _selectedName   = "";
        private string _selectedDate   = "";

        public frmJobReport()
        {
            InitializeComponent();
        }

        private void frmJobReport_Load(object sender, EventArgs e)
        {
            lblTitle.Text  = Lang.lgJobReport;
            btnPrint.Text  = Lang.lgPrint;
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
            btnPrint.BackColor = Color.FromArgb(0, 70, 130);
            btnPrint.ForeColor = Color.White;
        }

        private void LoadJobs()
        {
            lbJobs.Items.Clear();
            _jobs.Clear();

            if (Core.Database == null) return;

            var fieldMap = new System.Collections.Generic.Dictionary<int, string>();
            foreach (var f in Core.Database.Fields.GetAll())
                fieldMap[f.id] = f.name;

            foreach (var j in Core.Database.Jobs.GetAll())
            {
                string date  = j.startedAt.Length >= 10 ? j.startedAt.Substring(0, 10) : j.startedAt;
                string fname = j.fieldId > 0 && fieldMap.TryGetValue(j.fieldId, out string fn) ? fn : "";
                string entry = string.IsNullOrEmpty(fname)
                    ? $"{j.name}  |  {date}  |  {j.status}"
                    : $"{j.name}  |  {date}  |  {j.status}  |  {fname}";
                lbJobs.Items.Add(entry);
                _jobs.Add((j.id, j.name, date, j.acres, j.volume, fname, j.cropId, j.headerId, j.notes));
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
            _selectedDate  = job.startedAt;

            lblReportJobName.Text  = job.name;
            lblReportField.Text    = string.IsNullOrEmpty(job.fieldName) ? $"{Lang.lgFieldColon} --" : $"{Lang.lgFieldColon} {job.fieldName}";
            lblReportNotes.Text         = job.notes;
            lblReportNotesTitle.Visible = !string.IsNullOrEmpty(job.notes);
            lblReportNotes.Visible      = !string.IsNullOrEmpty(job.notes);

            double displayArea = Props.DisplayArea(job.acres);
            lblReportArea.Text = $"{Lang.lgAreaColon} {displayArea:F2} {Props.AreaUnit}";

            double displayMass = Props.DisplayMass(job.volume);
            lblReportTotal.Text = $"{Lang.lgTotalColon} {displayMass:F1} {Props.MassUnit}";

            double avgYield = job.acres > 0.001 ? job.volume / job.acres : 0;
            double displayRate = Props.DisplayRate(avgYield);
            lblReportAvgYield.Text = $"{Lang.lgAvgYield} {displayRate:F1} {Props.RateUnit}";

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

            lblReportAvgMoist.Text = $"{Lang.lgAvgMoisture} {avgMoist:F1} %";
            lblReportPoints.Text   = $"{Lang.lgDataPoints} {pointCount}";
        }

        private void ClearSummary()
        {
            _selectedJobId = -1;
            _selectedName  = "";
            _selectedDate  = "";
            lblReportJobName.Text  = "--";
            lblReportField.Text    = $"{Lang.lgFieldColon} --";
            lblReportNotes.Text         = "";
            lblReportNotesTitle.Visible = false;
            lblReportNotes.Visible      = false;
            lblReportArea.Text     = $"{Lang.lgAreaColon} --";
            lblReportTotal.Text    = $"{Lang.lgTotalColon} --";
            lblReportAvgYield.Text = $"{Lang.lgAvgYield} --";
            lblReportAvgMoist.Text = $"{Lang.lgAvgMoisture} --";
            lblReportPoints.Text   = $"{Lang.lgDataPoints} --";
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (_selectedJobId < 0)
            {
                Props.ShowMessage(Lang.lgSelectJobFirst, "", 3000, true);
                return;
            }

            string lastFolder = Properties.Settings.Default.LastExportFolder;
            if (string.IsNullOrEmpty(lastFolder) || !Directory.Exists(lastFolder))
                lastFolder = Props.ExportFolder;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title            = Lang.lgExportCsv;
                sfd.Filter           = "CSV files (*.csv)|*.csv";
                sfd.InitialDirectory = lastFolder;
                string safeName      = string.Concat((_selectedName + "_" + _selectedDate).Split(Path.GetInvalidFileNameChars()));
                sfd.FileName         = safeName + ".csv";

                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                Properties.Settings.Default.LastExportFolder = Path.GetDirectoryName(sfd.FileName);
                Properties.Settings.Default.Save();

                string path = CsvExporter.ExportJob(_selectedJobId, _selectedName, sfd.FileName);
                if (path != null)
                    Props.ShowMessage(string.Format(Lang.lgExported, path));
                else
                    Props.ShowMessage(Lang.lgExportFailed, "", 3000, true);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_selectedJobId < 0)
            {
                Props.ShowMessage(Lang.lgSelectJobFirst, "", 3000, true);
                return;
            }

            int idx = lbJobs.SelectedIndex;
            if (idx < 0 || idx >= _jobs.Count) return;
            var job = _jobs[idx];

            // Resolve crop and header names
            string cropName   = "";
            string headerDesc = "";
            foreach (var c in Core.Database.Crops.GetAll())
                if (c.id == job.cropId) { cropName = c.name; break; }
            foreach (var h in Core.Database.Headers.GetAll())
                if (h.id == job.headerId)
                {
                    double dispW = Props.IsMetric ? h.widthM : h.widthM * 3.28084;
                    string wu = Props.IsMetric ? "m" : "ft";
                    headerDesc = $"{h.name}  ({dispW:F1} {wu})";
                    break;
                }

            // Average moisture and point count
            int    pointCount = 0;
            double avgMoist   = 0;
            try
            {
                var points = Core.Database.YieldData.GetByJob(job.id);
                pointCount = points.Count;
                if (pointCount > 0)
                {
                    double sum = 0;
                    foreach (var p in points) sum += p.Moisture;
                    avgMoist = sum / pointCount;
                }
            }
            catch { }

            // Build RTF content in a hidden RichTextBox
            var rtb = new RichTextBox { Font = new Font("Courier New", 10f), Visible = false };
            rtb.CreateControl();

            void AddLine(string text, bool bold = false)
            {
                Font cur = rtb.SelectionFont ?? rtb.Font;
                rtb.SelectionFont = new Font(cur, bold ? FontStyle.Bold : FontStyle.Regular);
                rtb.AppendText(text + Environment.NewLine);
            }
            void AddRow(string label, string value)
            {
                Font cur = rtb.SelectionFont ?? rtb.Font;
                rtb.SelectionFont = new Font(cur, FontStyle.Regular);
                rtb.AppendText(label.PadRight(16) + value + Environment.NewLine);
            }

            string rule = new string('─', 42);

            AddLine(Lang.lgJobReport, bold: true);
            AddLine(rule);
            AddLine("");
            AddRow("Job:",    job.name);
            AddRow("Date:",   job.startedAt);
            if (!string.IsNullOrEmpty(job.fieldName)) AddRow("Field:",  job.fieldName);
            if (!string.IsNullOrEmpty(cropName))      AddRow("Crop:",   cropName);
            if (!string.IsNullOrEmpty(headerDesc))    AddRow("Header:", headerDesc);
            AddLine("");
            AddLine(rule);

            double displayArea = Props.DisplayArea(job.acres);
            double displayMass = Props.DisplayMass(job.volume);
            double avgYield    = job.acres > 0.001 ? job.volume / job.acres : 0;
            double displayRate = Props.DisplayRate(avgYield);

            AddRow(Lang.lgAreaColon,   $"{displayArea:F2} {Props.AreaUnit}");
            AddRow(Lang.lgTotalColon,  $"{displayMass:F1} {Props.MassUnit}");
            AddRow(Lang.lgAvgYield,    $"{displayRate:F1} {Props.RateUnit}");
            AddRow(Lang.lgAvgMoisture, $"{avgMoist:F1} %");
            AddRow(Lang.lgDataPoints,  $"{pointCount}");
            AddLine(rule);

            if (!string.IsNullOrEmpty(job.notes))
            {
                AddLine("");
                AddLine("Notes:", bold: true);
                AddLine(job.notes);
            }

            string docName = $"{job.name}_{job.startedAt}";
            new RichTextBoxPrinterRTF(rtb).Print(docName);
            rtb.Dispose();
        }

        private void btnTitleClose_Click(object sender, EventArgs e)    => this.Close();
        private void btnReportClose_Click(object sender, EventArgs e)   => this.Close();
    }
}
