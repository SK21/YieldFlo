using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    public partial class frmYieldMap : Form
    {
        private bool _dragging;
        private Point _dragStart;

        public frmYieldMap()
        {
            InitializeComponent();
        }

        private void frmYieldMap_Load(object sender, EventArgs e)
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

            LoadMapData();
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

            mapPanel.BackColor = Color.FromArgb(30, 30, 30);

            foreach (Control c in pnlContent.Controls)
            {
                if (c is Label lbl) { lbl.ForeColor = fore; }
                if (c is Button btn)
                {
                    btn.BackColor = ctrl;
                    btn.ForeColor = Color.White;
                }
            }
        }

        private void LoadMapData()
        {
            int jobId = -1;

            // Try active job first, then last job in DB
            if (Core.Collector != null && Core.Collector.ActiveJobId > 0)
                jobId = Core.Collector.ActiveJobId;
            else if (Core.Database != null)
            {
                var jobs = Core.Database.Jobs.GetAll();
                if (jobs.Count > 0) jobId = jobs[0].id;
            }

            if (jobId < 0 || Core.Database == null)
            {
                mapPanel.Points.Clear();
                lblMapJob.Text = "No job data";
                mapPanel.Invalidate();
                return;
            }

            try
            {
                var points = Core.Database.YieldData.GetByJob(jobId);
                mapPanel.Points.Clear();
                mapPanel.ScaleMin = Properties.Settings.Default.YieldScaleMin;
                mapPanel.ScaleMax = Properties.Settings.Default.YieldScaleMax;

                foreach (var p in points)
                    mapPanel.Points.Add((p.Latitude, p.Longitude, p.YieldRate));

                // Find job name
                string jobName = $"Job #{jobId}";
                foreach (var j in Core.Database.Jobs.GetAll())
                {
                    if (j.id == jobId) { jobName = j.name; break; }
                }
                lblMapJob.Text = jobName;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmYieldMap/LoadMapData: " + ex.Message);
                lblMapJob.Text = "Error loading data";
            }

            mapPanel.Invalidate();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadMapData();
        }

        private void btnTitleClose_Click(object sender, EventArgs e)  => this.Close();
        private void btnMapClose_Click(object sender, EventArgs e)    => this.Close();
    }

    // ── YieldMapPanel ─────────────────────────────────────────────────────────
    public class YieldMapPanel : Panel
    {
        public List<(double lat, double lon, double yield)> Points { get; } = new List<(double, double, double)>();
        public double ScaleMin { get; set; } = 0;
        public double ScaleMax { get; set; } = 150;

        public YieldMapPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw  = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            if (Points.Count == 0)
            {
                using (var brush = new SolidBrush(Color.FromArgb(120, 120, 120)))
                using (var font  = new Font("Microsoft Sans Serif", 10F))
                {
                    var sz = g.MeasureString("No data", font);
                    g.DrawString("No data", font, brush,
                        (Width - sz.Width) / 2f, (Height - sz.Height) / 2f);
                }
                return;
            }

            // Compute lat/lon bounds with 5% padding
            double minLat = double.MaxValue, maxLat = double.MinValue;
            double minLon = double.MaxValue, maxLon = double.MinValue;
            foreach (var p in Points)
            {
                if (p.lat < minLat) minLat = p.lat;
                if (p.lat > maxLat) maxLat = p.lat;
                if (p.lon < minLon) minLon = p.lon;
                if (p.lon > maxLon) maxLon = p.lon;
            }

            double latRange = maxLat - minLat;
            double lonRange = maxLon - minLon;

            // Avoid zero range (single point)
            if (latRange < 1e-7) latRange = 1e-7;
            if (lonRange < 1e-7) lonRange = 1e-7;

            double padLat = latRange * 0.05;
            double padLon = lonRange * 0.05;
            minLat -= padLat; maxLat += padLat;
            minLon -= padLon; maxLon += padLon;
            latRange = maxLat - minLat;
            lonRange = maxLon - minLon;

            double scaleRange = ScaleMax - ScaleMin;
            if (scaleRange < 0.001) scaleRange = 0.001;

            foreach (var p in Points)
            {
                // Map to pixel (lat increases upward, so invert)
                float px = (float)((p.lon - minLon) / lonRange * (Width  - 8)) + 4;
                float py = (float)((1.0 - (p.lat - minLat) / latRange) * (Height - 8)) + 4;

                double t = Math.Max(0, Math.Min(1, (p.yield - ScaleMin) / scaleRange));
                Color col = YieldToColor(t);

                using (var brush = new SolidBrush(col))
                    g.FillEllipse(brush, px - 4, py - 4, 8, 8);
            }
        }

        private static Color YieldToColor(double t)
        {
            // t=0 → green, t=0.5 → yellow, t=1 → red
            if (t <= 0.5)
            {
                double u = t * 2.0;
                int r = (int)(0   + u * 255);
                int g2 = (int)(180 + u * (220 - 180));
                int b  = 0;
                return Color.FromArgb(Clamp(r), Clamp(g2), b);
            }
            else
            {
                double u = (t - 0.5) * 2.0;
                int r  = (int)(255 + u * (220 - 255));
                int g2 = (int)(220 - u * 220);
                int b  = 0;
                return Color.FromArgb(Clamp(r), Clamp(g2), b);
            }
        }

        private static int Clamp(int v) => v < 0 ? 0 : v > 255 ? 255 : v;
    }
}
