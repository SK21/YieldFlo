using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using YieldFlo.Classes;
using YieldFlo.Communication.Map;
using YieldFlo.Database;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmYieldMap : Form
    {
        private bool   _isMini = true;
        private double _savedFullZoom = 17;
        private double _headerWidthM  = 9.144;

        // Mini bar drag
        private bool  _miniDragging;
        private Point _miniDragStart;

        // Click-vs-drag detection on gmap in mini mode
        private Point _gmapClickStart;

        // Job list
        private readonly List<int> _jobIds = new List<int>();

        // Live-update timer
        private System.Windows.Forms.Timer _liveTimer;

        public frmYieldMap()
        {
            InitializeComponent();
        }

        // ── Load ──────────────────────────────────────────────────────────────

        private void frmYieldMap_Load(object sender, EventArgs e)
        {
            InitMap();
            LoadJobCombo();
            SelectCurrentJob();
            SetMiniMode(true);
            LoadYieldData();
            Core.JobStateChanged += Core_JobStateChanged;
            this.FormClosed += (s, ev) => Core.JobStateChanged -= Core_JobStateChanged;

            _liveTimer = new System.Windows.Forms.Timer { Interval = 5000 };
            _liveTimer.Tick += LiveTimer_Tick;
            _liveTimer.Start();
            this.FormClosed += (s, ev) => _liveTimer.Stop();
        }

        private void Core_JobStateChanged(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            this.BeginInvoke((Action)(() =>
            {
                LoadJobCombo();
                SelectCurrentJob();
                LoadYieldData();
            }));
        }

        private void LiveTimer_Tick(object sender, EventArgs e)
        {
            if (Core.Collector == null || !Core.Collector.IsRecording) return;
            int idx = cboJob.SelectedIndex;
            if (idx < 0 || idx >= _jobIds.Count) return;
            if (_jobIds[idx] != Core.Collector.ActiveJobId) return;
            LoadYieldData(center: false);
        }

        // ── GMap init ─────────────────────────────────────────────────────────

        private void InitMap()
        {
            string cacheDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "YieldFlo", "MapCache");
            Directory.CreateDirectory(cacheDir);

            GMaps.Instance.Mode  = AccessMode.ServerAndCache;
            gmap.CacheLocation   = cacheDir;
            gmap.MapProvider     = ArcGIS_World_Imagery_Provider.Instance;
            gmap.MinZoom         = 1;
            gmap.MaxZoom         = 20;
            gmap.Zoom            = 16;
            gmap.ShowCenter      = false;
            gmap.PolygonsEnabled = true;
            gmap.MarkersEnabled  = false;
            gmap.RoutesEnabled   = false;
            gmap.Position        = new PointLatLng(52.0, -106.0);  // default: prairies
        }

        // ── Mini / full mode ──────────────────────────────────────────────────

        private void SetMiniMode(bool mini)
        {
            _isMini            = mini;
            pnlToolbar.Visible = !mini;
            pnlLegend.Visible  = !mini;
            pnlMiniBar.Visible = mini;
            this.TopMost       = true;

            if (mini)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Size            = new Size(300, 300);
                gmap.CanDragMap      = true;
                gmap.DragButton      = MouseButtons.Left;
                gmap.Bounds          = new Rectangle(0, 30, 300, 270);
                pnlMiniBar.Bounds    = new Rectangle(0, 0, 300, 30);
                FormPositions.Restore(this);
            }
            else
            {
                var wa = Screen.GetWorkingArea(this);
                this.FormBorderStyle = FormBorderStyle.None;
                this.Bounds          = wa;
                gmap.CanDragMap      = true;
                gmap.DragButton      = MouseButtons.Left;

                int toolH   = pnlToolbar.Height;
                int legendH = pnlLegend.Height;
                pnlToolbar.Bounds = new Rectangle(0, 0, wa.Width, toolH);
                gmap.Bounds       = new Rectangle(0, toolH, wa.Width, wa.Height - toolH - legendH);
                pnlLegend.Bounds  = new Rectangle(0, wa.Height - legendH, wa.Width, legendH);

                gmap.Zoom = _savedFullZoom;

                // Anchor buttons to right edge of toolbar
                btnClose.Location    = new Point(wa.Width - 76, 5);
                btnRefresh.Location  = new Point(wa.Width - 156, 5);
                btnMinimize.Location = new Point(wa.Width - 226, 5);

                // Legend high label to right edge
                lblLegendHigh.Location = new Point(wa.Width - 84, 20);
            }
        }

        // ── Mini bar drag ─────────────────────────────────────────────────────

        private void pnlMiniBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { _miniDragging = true; _miniDragStart = e.Location; }
        }

        private void pnlMiniBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_miniDragging) { Left += e.X - _miniDragStart.X; Top += e.Y - _miniDragStart.Y; }
        }

        private void pnlMiniBar_MouseUp(object sender, MouseEventArgs e)
        {
            _miniDragging = false;
        }

        // ── GMap mouse — click to expand ──────────────────────────────────────

        private void Gmap_MouseDown(object sender, MouseEventArgs e)
        {
            _gmapClickStart = e.Location;
        }

        private void Gmap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            int dx = Math.Abs(e.Location.X - _gmapClickStart.X);
            int dy = Math.Abs(e.Location.Y - _gmapClickStart.Y);
            if (dx >= 6 || dy >= 6) return;

            if (_isMini)
            {
                FormPositions.Save(this);
                _savedFullZoom = gmap.Zoom;
                SetMiniMode(false);
                LoadYieldData();
            }
            else
            {
                _savedFullZoom = gmap.Zoom;
                SetMiniMode(true);
                LoadYieldData();
            }
        }

        // ── Job combo ────────────────────────────────────────────────────────

        private void LoadJobCombo()
        {
            cboJob.Items.Clear();
            _jobIds.Clear();
            foreach (var j in Core.Database.Jobs.GetAll())
            {
                cboJob.Items.Add(j.name);
                _jobIds.Add(j.id);
            }
        }

        private void SelectCurrentJob()
        {
            int targetId = Core.Collector?.ActiveJobId ?? -1;
            if (targetId <= 0 && _jobIds.Count > 0) targetId = _jobIds[0];

            int idx = _jobIds.IndexOf(targetId);
            cboJob.SelectedIndex = idx >= 0 ? idx : (_jobIds.Count > 0 ? 0 : -1);

            int sel = cboJob.SelectedIndex;
            lblMiniJob.Text = (sel >= 0 && sel < cboJob.Items.Count)
                ? (string)cboJob.Items[sel]
                : Lang.lgNoJobs;
        }

        private void cboJob_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadYieldData();
        }

        // ── Yield overlay ─────────────────────────────────────────────────────

        private void LoadYieldData(bool center = true)
        {
            gmap.Overlays.Clear();

            int idx = cboJob.SelectedIndex;
            if (idx < 0 || idx >= _jobIds.Count) return;

            int jobId = _jobIds[idx];

            // Resolve header width for this job
            _headerWidthM = 9.144;
            foreach (var j in Core.Database.Jobs.GetAll())
            {
                if (j.id != jobId) continue;
                if (j.headerId > 0)
                {
                    foreach (var h in Core.Database.Headers.GetAll())
                        if (h.id == j.headerId) { _headerWidthM = h.widthM; break; }
                }
                break;
            }

            var points = Core.Database.YieldData.GetByJob(jobId);
            if (points.Count == 0) return;

            BuildSwathOverlay(points);
            if (center) CenterOnData(points);
        }

        private void BuildSwathOverlay(List<YieldDataPoint> points)
        {
            var overlay = new GMapOverlay("yield");

            // Compute yield range (skip zeroes)
            double minY = double.MaxValue, maxY = double.MinValue;
            foreach (var pt in points)
            {
                if (pt.YieldRate <= 0) continue;
                if (pt.YieldRate < minY) minY = pt.YieldRate;
                if (pt.YieldRate > maxY) maxY = pt.YieldRate;
            }
            if (minY > maxY) { gmap.Overlays.Add(overlay); return; }
            double yRange = Math.Max(0.1, maxY - minY);

            lblLegendLow.Text  = $"{minY:F0} {Props.RateUnit}";
            lblLegendHigh.Text = $"{maxY:F0} {Props.RateUnit}";

            // Thin points to keep polygon count manageable
            int step = Math.Max(1, points.Count / 3000);

            for (int i = 0; i < points.Count; i += step)
            {
                var pt = points[i];
                if (pt.YieldRate <= 0 || pt.Speed < 0.5) continue;

                double lengthM = Math.Max(0.5, (pt.Speed / 3.6) * step);
                var corners = ComputeSwathCorners(
                    pt.Latitude, pt.Longitude, pt.Heading,
                    _headerWidthM, lengthM);

                double t   = Math.Max(0, Math.Min(1, (pt.YieldRate - minY) / yRange));
                Color  col = YieldToColor(t);

                var poly = new GMapPolygon(corners, "s")
                {
                    Fill   = new SolidBrush(Color.FromArgb(210, col)),
                    Stroke = new Pen(Color.FromArgb(0, col))
                };
                overlay.Polygons.Add(poly);
            }

            gmap.Overlays.Add(overlay);
        }

        private static List<PointLatLng> ComputeSwathCorners(
            double lat, double lon, double headingDeg,
            double widthM, double lengthM)
        {
            double hdg     = headingDeg * Math.PI / 180.0;
            double mPerLat = 111320.0;
            double mPerLon = Math.Max(1.0, 111320.0 * Math.Cos(lat * Math.PI / 180.0));

            // Forward and perpendicular unit vectors in lat/lon space
            double fwdLat  =  Math.Cos(hdg) / mPerLat;
            double fwdLon  =  Math.Sin(hdg)  / mPerLon;
            double perpLat = -Math.Sin(hdg)  / mPerLat;
            double perpLon =  Math.Cos(hdg)  / mPerLon;

            double hL = lengthM / 2.0;
            double hW = widthM  / 2.0;

            return new List<PointLatLng>
            {
                new PointLatLng(lat + hL*fwdLat + hW*perpLat, lon + hL*fwdLon + hW*perpLon),
                new PointLatLng(lat + hL*fwdLat - hW*perpLat, lon + hL*fwdLon - hW*perpLon),
                new PointLatLng(lat - hL*fwdLat - hW*perpLat, lon - hL*fwdLon - hW*perpLon),
                new PointLatLng(lat - hL*fwdLat + hW*perpLat, lon - hL*fwdLon + hW*perpLon),
            };
        }

        private static Color YieldToColor(double t)
        {
            if (t <= 0.5)
            {
                double u = t * 2.0;
                return Color.FromArgb(Clamp((int)(u * 255)), Clamp((int)(180 + u * 40)), 0);
            }
            else
            {
                double u = (t - 0.5) * 2.0;
                return Color.FromArgb(Clamp((int)(255 - u * 35)), Clamp((int)(220 - u * 220)), 0);
            }
        }

        private static int Clamp(int v) => v < 0 ? 0 : v > 255 ? 255 : v;

        private void CenterOnData(List<YieldDataPoint> points)
        {
            double minLat = double.MaxValue, maxLat = double.MinValue;
            double minLon = double.MaxValue, maxLon = double.MinValue;
            foreach (var pt in points)
            {
                if (pt.Latitude  < minLat) minLat = pt.Latitude;
                if (pt.Latitude  > maxLat) maxLat = pt.Latitude;
                if (pt.Longitude < minLon) minLon = pt.Longitude;
                if (pt.Longitude > maxLon) maxLon = pt.Longitude;
            }
            if (minLat > maxLat) return;

            gmap.Position = new PointLatLng((minLat + maxLat) / 2.0, (minLon + maxLon) / 2.0);

            // Zoom to fit bbox — ZoomAndCenterMarkers only works for marker overlays
            double dLon = Math.Max(0.0001, (maxLon - minLon) * 1.4);
            double dLat = Math.Max(0.0001, (maxLat - minLat) * 1.4);
            int w = gmap.Width  > 0 ? gmap.Width  : 300;
            int h = gmap.Height > 0 ? gmap.Height : 270;
            int zLon = (int)Math.Floor(Math.Log(360.0 * w / (256.0 * dLon), 2));
            int zLat = (int)Math.Floor(Math.Log(180.0 * h / (256.0 * dLat), 2));
            gmap.Zoom = Math.Max(gmap.MinZoom, Math.Min(gmap.MaxZoom, Math.Min(zLon, zLat)));
        }

        // ── Toolbar buttons ───────────────────────────────────────────────────

        private void btnZoomIn_Click(object sender, EventArgs e)  => gmap.Zoom = Math.Min(gmap.MaxZoom, gmap.Zoom + 1);
        private void btnZoomOut_Click(object sender, EventArgs e) => gmap.Zoom = Math.Max(gmap.MinZoom, gmap.Zoom - 1);

        private void btnExpand_Click(object sender, EventArgs e)
        {
            FormPositions.Save(this);
            _savedFullZoom = gmap.Zoom;
            SetMiniMode(false);
            LoadYieldData();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            _savedFullZoom = gmap.Zoom;
            SetMiniMode(true);
            LoadYieldData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadJobCombo();
            SelectCurrentJob();
            LoadYieldData();
        }

        private void btnMiniClose_Click(object sender, EventArgs e) => this.Close();
        private void btnClose_Click(object sender, EventArgs e)    => this.Close();

        // ── Legend paint ──────────────────────────────────────────────────────

        private void pnlLegend_Paint(object sender, PaintEventArgs e)
        {
            var g    = e.Graphics;
            var rect = (sender as Panel).ClientRectangle;
            int w    = rect.Width;
            for (int x = 0; x < w; x++)
            {
                double t = (double)x / Math.Max(1, w - 1);
                using var pen = new Pen(YieldToColor(t));
                g.DrawLine(pen, x, 0, x, 16);
            }
        }

        // ── Close ─────────────────────────────────────────────────────────────

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_isMini) FormPositions.Save(this);
            gmap.Overlays.Clear();
            base.OnFormClosed(e);
        }
    }
}
