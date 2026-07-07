using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        // Precomputed legend tick text (set by BuildSwathOverlay, drawn in pnlLegend_Paint)
        private string[] _tickText;

        // GDI brushes/pens owned by the current swath overlay. Polygons share these;
        // they must be disposed on every rebuild — the live timer rebuilds the overlay
        // every 5 s, and leaking ~6000 handles per rebuild exhausts the 10k GDI limit
        // (coverage and legend go blank once drawing starts failing).
        private readonly List<IDisposable> _overlayGdi = new List<IDisposable>();

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
                btnClose.Location       = new Point(wa.Width - 76, 5);
                btnZoomInFull.Location  = new Point(wa.Width - 118, 5);
                btnZoomOutFull.Location = new Point(wa.Width - 158, 5);
                btnPrint.Location       = new Point(wa.Width - 234, 5);

                // gmap is first in the Controls collection (= top of z-order);
                // make sure toolbar and legend paint above it, and repaint the
                // legend now that it is visible again.
                pnlToolbar.BringToFront();
                pnlLegend.BringToFront();
                pnlLegend.Invalidate();
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

        private void ClearSwathOverlay()
        {
            gmap.Overlays.Clear();
            foreach (var d in _overlayGdi) d.Dispose();
            _overlayGdi.Clear();
        }

        // Job whose data is currently drawn — lets a transient failure (locked DB
        // read, combo mid-reload) keep the existing drawing instead of wiping it.
        private int _drawnJobId = -1;

        private void LoadYieldData(bool center = true)
        {
            int idx = cboJob.SelectedIndex;
            if (idx < 0 || idx >= _jobIds.Count) return;   // combo mid-reload — keep current drawing

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

            List<YieldDataPoint> points;
            try   { points = Core.Database.YieldData.GetByJob(jobId); }
            catch { return; }   // transient read failure — keep current drawing

            if (points.Count == 0)
            {
                // A different job with no data must show empty; the SAME job
                // suddenly reading empty is transient — don't wipe the map.
                if (jobId != _drawnJobId) { ClearSwathOverlay(); _drawnJobId = jobId; }
                return;
            }

            // Build the replacement fully, then swap — clearing before building
            // leaves the map erased whenever anything later fails or returns early.
            var oldGdi = new List<IDisposable>(_overlayGdi);
            _overlayGdi.Clear();
            var overlay = BuildSwathOverlay(points);

            gmap.Overlays.Clear();
            gmap.Overlays.Add(overlay);
            _drawnJobId = jobId;

            // Old polygons are no longer referenced by the map — safe to release
            foreach (var d in oldGdi) d.Dispose();

            if (center) CenterOnData(points);
        }

        private GMapOverlay BuildSwathOverlay(List<YieldDataPoint> points)
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
            if (minY > maxY) return overlay;   // no positive yields yet — empty overlay
            double yRange = Math.Max(0.1, maxY - minY);

            var texts = new string[5];
            double[] tStops = { 0.0, 0.25, 0.5, 0.75, 1.0 };
            for (int i = 0; i < 5; i++)
            {
                double disp = Props.DisplayRate(minY + tStops[i] * (maxY - minY));
                string num  = Props.IsMetric ? $"{disp:F1}" : $"{disp:F0}";
                texts[i]    = (i == 0 || i == 4) ? $"{num} {Props.RateUnit}" : num;
            }
            _tickText = texts;
            pnlLegend.Invalidate();

            // Thin points to keep polygon count manageable
            int step = Math.Max(1, points.Count / 3000);

            // Polygons share pooled brushes (yield quantized to 100 colour steps) and
            // one invisible stroke pen instead of owning ~2 GDI handles each.
            var brushCache = new Dictionary<int, SolidBrush>();
            var stroke = new Pen(Color.FromArgb(0, Color.Black));
            _overlayGdi.Add(stroke);

            for (int i = 0; i < points.Count; i += step)
            {
                var pt = points[i];
                if (pt.YieldRate <= 0 || pt.Speed < 0.5) continue;

                double lengthM = Math.Max(0.5, (pt.Speed / 3.6) * step);
                var corners = ComputeSwathCorners(
                    pt.Latitude, pt.Longitude, pt.Heading,
                    _headerWidthM, lengthM);

                double t   = Math.Max(0, Math.Min(1, (pt.YieldRate - minY) / yRange));
                int    key = (int)Math.Round(t * 100);
                if (!brushCache.TryGetValue(key, out SolidBrush fill))
                {
                    fill = new SolidBrush(Color.FromArgb(210, YieldToColor(key / 100.0)));
                    brushCache[key] = fill;
                    _overlayGdi.Add(fill);
                }

                var poly = new GMapPolygon(corners, "s")
                {
                    Fill   = fill,
                    Stroke = stroke
                };
                overlay.Polygons.Add(poly);
            }

            return overlay;
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

        // 5-stop Blue → Cyan → Green → Yellow → Red (matches OEM yield monitor convention)
        private static readonly (double T, int R, int G, int B)[] YieldStops =
        {
            (0.00,   0,   0, 220),  // Blue
            (0.25,   0, 210, 210),  // Cyan
            (0.50,   0, 200,   0),  // Green
            (0.75, 230, 230,   0),  // Yellow
            (1.00, 220,   0,   0),  // Red
        };

        private static Color YieldToColor(double t)
        {
            t = Math.Max(0, Math.Min(1, t));
            for (int i = 1; i < YieldStops.Length; i++)
            {
                if (t <= YieldStops[i].T)
                {
                    double span = YieldStops[i].T - YieldStops[i - 1].T;
                    double u    = (t - YieldStops[i - 1].T) / span;
                    int r = Clamp((int)(YieldStops[i - 1].R + u * (YieldStops[i].R - YieldStops[i - 1].R)));
                    int g = Clamp((int)(YieldStops[i - 1].G + u * (YieldStops[i].G - YieldStops[i - 1].G)));
                    int b = Clamp((int)(YieldStops[i - 1].B + u * (YieldStops[i].B - YieldStops[i - 1].B)));
                    return Color.FromArgb(r, g, b);
                }
            }
            return Color.FromArgb(220, 0, 0);
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

        private void btnMiniClose_Click(object sender, EventArgs e) => this.Close();
        private void btnClose_Click(object sender, EventArgs e)    => this.Close();

        private void btnPrint_Click(object sender, EventArgs e)
        {
            int idx = cboJob.SelectedIndex;
            if (idx < 0 || idx >= _jobIds.Count) return;

            int jobId = _jobIds[idx];
            string jobName = (string)cboJob.Items[idx];
            string date = "";
            foreach (var j in Core.Database.Jobs.GetAll())
            {
                if (j.id == jobId) { date = j.startedAt.Length >= 10 ? j.startedAt.Substring(0, 10) : j.startedAt; break; }
            }

            string safeName = string.Concat((jobName + "_" + date).Split(Path.GetInvalidFileNameChars()));

            string lastFolder = Properties.Settings.Default.LastExportFolder;
            if (string.IsNullOrEmpty(lastFolder) || !Directory.Exists(lastFolder))
                lastFolder = Props.ExportFolder;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title            = Lang.lgPrint;
                sfd.Filter           = "PNG image (*.png)|*.png";
                sfd.InitialDirectory = lastFolder;
                sfd.FileName         = safeName + ".png";

                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                Properties.Settings.Default.LastExportFolder = Path.GetDirectoryName(sfd.FileName);
                Properties.Settings.Default.Save();

                // Capture gmap + legend into a single bitmap
                int totalH = gmap.Height + (pnlLegend.Visible ? pnlLegend.Height : 0);
                using (var bmp = new Bitmap(gmap.Width, totalH))
                {
                    gmap.DrawToBitmap(bmp, new Rectangle(0, 0, gmap.Width, gmap.Height));
                    if (pnlLegend.Visible)
                    {
                        using (var legendBmp = new Bitmap(pnlLegend.Width, pnlLegend.Height))
                        {
                            pnlLegend.DrawToBitmap(legendBmp, new Rectangle(0, 0, legendBmp.Width, legendBmp.Height));
                            using (var g = Graphics.FromImage(bmp))
                                g.DrawImage(legendBmp, 0, gmap.Height);
                        }
                    }
                    bmp.Save(sfd.FileName, ImageFormat.Png);
                }

                Props.ShowMessage(string.Format(Lang.lgExported, sfd.FileName));
            }
        }

        // ── Legend paint ──────────────────────────────────────────────────────

        private void pnlLegend_Paint(object sender, PaintEventArgs e)
        {
            var g    = e.Graphics;
            int w    = (sender as Panel).Width;
            int h    = (sender as Panel).Height;

            // Gradient bar (top 16 px)
            using (var pen = new Pen(Color.Black))
            {
                for (int x = 0; x < w; x++)
                {
                    double t = (double)x / Math.Max(1, w - 1);
                    pen.Color = YieldToColor(t);
                    g.DrawLine(pen, x, 0, x, 16);
                }
            }

            var texts = _tickText;
            if (texts == null) return;

            double[] tStops = { 0.0, 0.25, 0.5, 0.75, 1.0 };
            using var tickPen = new Pen(Color.White);
            using var font    = new Font("Microsoft Sans Serif", 9f);
            using var brush   = new SolidBrush(Color.White);

            for (int i = 0; i < 5; i++)
            {
                int   tickX = (int)(tStops[i] * (w - 1));
                g.DrawLine(tickPen, tickX, 15, tickX, 19);

                SizeF sz     = g.MeasureString(texts[i], font);
                float labelX = i == 0 ? tickX
                             : i == 4 ? tickX - sz.Width
                             :           tickX - sz.Width / 2f;
                float labelY = h - sz.Height - 1;
                g.DrawString(texts[i], font, brush, labelX, labelY);
            }
        }

        // ── Close ─────────────────────────────────────────────────────────────

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_isMini) FormPositions.Save(this);
            ClearSwathOverlay();
            base.OnFormClosed(e);
        }
    }
}
