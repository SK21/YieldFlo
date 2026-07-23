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

        // Precomputed legend tick text (set on a full rebuild, drawn in pnlLegend_Paint)
        private string[] _tickText;

        // GDI brushes/pens owned by the current swath overlay. Polygons share these;
        // they must be disposed on every full rebuild — leaking ~6000 handles per
        // rebuild exhausts the 10k GDI limit (coverage and legend go blank once
        // drawing starts failing).
        private readonly List<IDisposable> _overlayGdi = new List<IDisposable>();

        // Persistent swath overlay + incremental-draw state. The live timer must NOT
        // rebuild everything every 5 s (that erases the painted coverage for a frame
        // and rescales/redraws the legend). Instead we keep one overlay, freeze the
        // colour scale on the first build, and append only swaths for new points.
        private GMapOverlay _yieldOverlay;
        private int    _drawnRawCount;               // raw points already turned into swaths
        private double _scaleMin, _scaleMax, _scaleRange;
        private bool   _scaleSet;
        private Dictionary<int, SolidBrush> _brushCache;  // shared across rebuild + append
        private Pen    _stroke;

        // Ribbon-break guards (match RateController YieldOverlayCreator). A segment is
        // drawn only where grain flows on both ends AND the two points are close in
        // space and time — otherwise the machine travelled that ground without grain
        // (flow stopped, headland turn, GPS jump) and it must stay blank.
        private const double MaxBridgeMeters  = 5.0;
        private const double MaxBridgeSeconds = 3.0;

        // Job list
        private readonly List<int> _jobIds = new List<int>();

        // Live-update timer
        private System.Windows.Forms.Timer _liveTimer;

        // Heading-up rotation state (mini mode only)
        private float _lastBearing;

        // Suppresses cboJob_SelectedIndexChanged while the combo is being
        // rebuilt, so a list refresh can't recentre/rezoom the map.
        private bool _reloadingCombo;

        public frmYieldMap()
        {
            InitializeComponent();
        }

        // ── Load ──────────────────────────────────────────────────────────────

        private void frmYieldMap_Load(object sender, EventArgs e)
        {
            InitMap();
            _reloadingCombo = true;
            LoadJobCombo();
            SelectCurrentJob();
            _reloadingCombo = false;
            SetMiniMode(true);
            LoadYieldData();
            Core.JobStateChanged += Core_JobStateChanged;
            this.FormClosed += (s, ev) => Core.JobStateChanged -= Core_JobStateChanged;

            // Mini map follows the vehicle (like RateController): re-centre on the
            // current GPS fix each update. Unsubscribed on close.
            Core.UpdateDisplay += Core_UpdateDisplay;
            this.FormClosed += (s, ev) => Core.UpdateDisplay -= Core_UpdateDisplay;

            _liveTimer = new System.Windows.Forms.Timer { Interval = 2000 };
            _liveTimer.Tick += LiveTimer_Tick;
            _liveTimer.Start();
            this.FormClosed += (s, ev) => _liveTimer.Stop();
        }

        private void Core_JobStateChanged(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            this.BeginInvoke((Action)(() =>
            {
                // JobStateChanged also fires on every auto-pause/resume (grain
                // flow stopping/starting). Rebuild the list with the selection
                // event suppressed and recentre/rezoom ONLY when the job the
                // map shows actually changed — never over a flow stop, which
                // used to reset the operator's zoom.
                _reloadingCombo = true;
                LoadJobCombo();
                SelectCurrentJob();
                _reloadingCombo = false;
                int idx = cboJob.SelectedIndex;
                bool jobChanged = idx >= 0 && idx < _jobIds.Count && _jobIds[idx] != _drawnJobId;
                LoadYieldData(center: jobChanged);
            }));
        }

        // Keep the mini map centred on the vehicle. Fires on the UI thread with each
        // display update (same event frmMain uses to refresh gauges). Full mode is
        // left free so the operator can pan/zoom the whole field. Only Position is
        // set — the current zoom is preserved.
        private void Core_UpdateDisplay(object sender, EventArgs e)
        {
            if (!_isMini || this.IsDisposed || !this.IsHandleCreated) return;
            var gps = Core.GPS;
            if (gps == null || !gps.IsConnected) return;
            if (gps.Latitude == 0 && gps.Longitude == 0) return;
            gmap.Position = new PointLatLng(gps.Latitude, gps.Longitude);

            // Heading-up (like AOG): rotate the mini map so the direction of
            // travel points up. Rate-limited to >5° changes so tile redraws
            // don't fire on every GPS tick; GPS heading is noise when parked,
            // so below walking speed the last rotation is held.
            if (gps.Speed > 0.5f)
            {
                float hdg = gps.Heading;
                float diff = Math.Abs(((hdg - _lastBearing + 540f) % 360f) - 180f);
                if (diff > 5f)
                {
                    _lastBearing = hdg;
                    gmap.Bearing = hdg;
                }
            }
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
                this.Size            = new Size(300, 314);
                gmap.CanDragMap      = true;
                gmap.DragButton      = MouseButtons.Left;
                gmap.Bounds          = new Rectangle(0, 44, 300, 270);
                pnlMiniBar.Bounds    = new Rectangle(0, 0, 300, 44);
                FormPositions.Restore(this);
            }
            else
            {
                var wa = Screen.GetWorkingArea(this);
                this.FormBorderStyle = FormBorderStyle.None;
                this.Bounds          = wa;
                gmap.CanDragMap      = true;
                gmap.DragButton      = MouseButtons.Left;

                // Full mode reviews the whole field north-up
                gmap.Bearing = 0;
                _lastBearing = 0;

                int toolH   = pnlToolbar.Height;
                int legendH = pnlLegend.Height;
                pnlToolbar.Bounds = new Rectangle(0, 0, wa.Width, toolH);
                gmap.Bounds       = new Rectangle(0, toolH, wa.Width, wa.Height - toolH - legendH);
                pnlLegend.Bounds  = new Rectangle(0, wa.Height - legendH, wa.Width, legendH);

                gmap.Zoom = _savedFullZoom;

                // Anchor buttons to right edge of toolbar
                btnClose.Location       = new Point(wa.Width - 108, 6);
                btnZoomInFull.Location  = new Point(wa.Width - 158, 6);
                btnZoomOutFull.Location = new Point(wa.Width - 208, 6);
                btnPrint.Location       = new Point(wa.Width - 314, 6);
                btnRecalc.Location      = new Point(wa.Width - 530, 6);

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
            if (_reloadingCombo) return;
            LoadYieldData();
        }

        // ── Yield overlay ─────────────────────────────────────────────────────

        // Job whose data is currently drawn — lets a transient failure (locked DB
        // read, combo mid-reload) keep the existing drawing instead of wiping it.
        private int _drawnJobId = -1;

        private void ClearSwathOverlay()
        {
            gmap.Overlays.Clear();
            foreach (var d in _overlayGdi) d.Dispose();
            _overlayGdi.Clear();
            _yieldOverlay  = null;
            _brushCache    = null;
            _stroke        = null;
            _scaleSet      = false;
            _drawnRawCount = 0;
        }

        private void LoadYieldData(bool center = true)
        {
            int idx = cboJob.SelectedIndex;
            if (idx < 0 || idx >= _jobIds.Count) return;   // combo mid-reload — keep current drawing

            int jobId = _jobIds[idx];

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

            // Live update of the same job: if only new points arrived and they all
            // fall within the frozen colour scale, append their swaths to the
            // existing overlay instead of rebuilding. This is what stops the coverage
            // flashing away and the legend being redrawn on every 5 s tick.
            bool sameJob = jobId == _drawnJobId && _yieldOverlay != null && _scaleSet;
            if (sameJob && points.Count >= _drawnRawCount)
            {
                // The scale is percentile-based, so the odd point beyond it is
                // EXPECTED (slow-down spikes clamp to the end colours). Only a
                // run of out-of-scale points — the crop genuinely outgrew the
                // frozen scale — justifies a full rebuild.
                int outOfScale = 0;
                for (int i = _drawnRawCount; i < points.Count; i++)
                {
                    double y = points[i].YieldRate;
                    if (y > 0 && (y < _scaleMin || y > _scaleMax)) outOfScale++;
                }
                if (outOfScale <= 3)
                {
                    // Bridge each new point to its predecessor so coverage stays
                    // continuous (start at 1 so points[i-1] is valid; _drawnRawCount
                    // is the first unbridged point, whose predecessor is already drawn).
                    for (int i = Math.Max(1, _drawnRawCount); i < points.Count; i++)
                        AddSwath(_yieldOverlay, points[i - 1], points[i]);
                    _drawnRawCount = points.Count;
                    // gmap.Refresh() — a plain Invalidate leaves the tile-cached
                    // control unpainted, so appended swaths never show (matches the
                    // RateControl GMap usage).
                    gmap.Refresh();
                    if (center) CenterOnData(points);
                    return;
                }
            }

            // Full (re)build: new job, or new data pushed past the frozen scale so
            // every swath's colour must be recomputed.
            RebuildSwaths(points, jobId, center);
        }

        private void RebuildSwaths(List<YieldDataPoint> points, int jobId, bool center)
        {
            // Header width can change between jobs; resolve it on a full rebuild only.
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

            // Colour scale = 2nd..98th percentile of positive yields, frozen until
            // the next rebuild (RateController's TryComputeScale rule). Plain
            // min..max let a single slow-down spike (yield ∝ 1/speed with grain
            // still flowing) own the whole legend and squash the real crop into
            // one colour band; percentile ends clamp such outliers instead.
            double minY = double.MaxValue, maxY = double.MinValue;
            var vals = new List<double>();
            foreach (var pt in points)
            {
                double y = pt.YieldRate;
                if (y > 0 && !double.IsNaN(y) && !double.IsInfinity(y)) vals.Add(y);
            }
            if (vals.Count > 0)
            {
                vals.Sort();
                if (vals.Count < 10)
                {
                    minY = vals[0];
                    maxY = vals[vals.Count - 1];
                }
                else
                {
                    minY = vals[(int)Math.Floor(0.02 * (vals.Count - 1))];
                    maxY = vals[(int)Math.Ceiling(0.98 * (vals.Count - 1))];
                    if (maxY <= minY) { minY = vals[0]; maxY = vals[vals.Count - 1]; }
                }
            }

            var newOverlay = new GMapOverlay("yield");
            var oldGdi     = new List<IDisposable>(_overlayGdi);
            _overlayGdi.Clear();

            if (minY > maxY)
            {
                // No positive yields yet — show an empty overlay, no scale/legend.
                _scaleSet = false;
                _tickText = null;
                pnlLegend.Invalidate();
            }
            else
            {
                _scaleMin   = minY;
                _scaleMax   = maxY;
                _scaleRange = Math.Max(0.1, maxY - minY);
                _scaleSet   = true;

                UpdateLegendTicks();

                _brushCache = new Dictionary<int, SolidBrush>();
                _stroke     = new Pen(Color.FromArgb(0, Color.Black));
                _overlayGdi.Add(_stroke);

                // Bridge every consecutive pair; AddSwath itself breaks the ribbon
                // across no-yield stretches and space/time gaps (no thinning — that
                // would inflate the gap between drawn points past the break guards).
                for (int i = 1; i < points.Count; i++)
                    AddSwath(newOverlay, points[i - 1], points[i]);
            }

            gmap.Overlays.Clear();
            gmap.Overlays.Add(newOverlay);
            _yieldOverlay  = newOverlay;
            _drawnJobId    = jobId;
            _drawnRawCount = points.Count;

            foreach (var d in oldGdi) d.Dispose();   // old polygons no longer referenced

            gmap.Refresh();
            NudgeRepaint();
            if (center) CenterOnData(points);
        }

        // GMap.NET 1.7.5 quirk: swapping Overlays' *content* (Clear + Add a new
        // GMapOverlay) while Position/Zoom/Size all stay unchanged does not
        // actually repaint — gmap.Refresh() alone leaves the old polygons on
        // screen (confirmed: btnRecalc_Click's rebuild never visibly updates
        // until something resizes gmap, e.g. toggling mini/full mode via
        // SetMiniMode's gmap.Bounds reassignment). A real Bounds change is what
        // forces GMap.NET to redraw, so replicate that here with an invisible
        // 1px nudge instead of relying on the caller to happen to resize/recenter.
        private void NudgeRepaint()
        {
            var b = gmap.Bounds;
            gmap.Bounds = new Rectangle(b.X, b.Y, Math.Max(1, b.Width - 1), b.Height);
            gmap.Bounds = b;
        }

        // Adds one swath as a ribbon quad spanning from point a to point b, coloured
        // by b's yield. Drawing a segment BETWEEN consecutive points (instead of a
        // fixed-length rectangle centred on one point) guarantees continuous coverage
        // regardless of speed or GPS-point spacing — the old approach guessed the
        // length from speed×time and left gaps whenever the guess fell short.
        private void AddSwath(GMapOverlay overlay, YieldDataPoint a, YieldDataPoint b)
        {
            // Both ends must carry grain — a ribbon must never bridge FROM a no-yield
            // point (the ground travelled empty stays blank).
            if (!_scaleSet || a.YieldRate <= 0 || b.YieldRate <= 0 || b.Speed < 0.5) return;

            // Break across a time gap (stop / headland turn / auto-pause resume).
            double dt = (b.Timestamp - a.Timestamp).TotalSeconds;
            if (dt <= 0 || dt > MaxBridgeSeconds) return;

            var corners = ComputeSegmentCorners(
                a.Latitude, a.Longitude, b.Latitude, b.Longitude, _headerWidthM, MaxBridgeMeters);
            if (corners == null) return;   // points coincide or spatial gap too large

            double t   = Math.Max(0, Math.Min(1, (b.YieldRate - _scaleMin) / _scaleRange));
            int    key = (int)Math.Round(t * 100);
            if (!_brushCache.TryGetValue(key, out SolidBrush fill))
            {
                fill = new SolidBrush(Color.FromArgb(210, YieldToColor(key / 100.0)));
                _brushCache[key] = fill;
                _overlayGdi.Add(fill);
            }

            overlay.Polygons.Add(new GMapPolygon(corners, "s") { Fill = fill, Stroke = _stroke });
        }

        // Recomputes the 5 legend tick labels from the frozen scale. Called only on a
        // full rebuild, so the legend no longer flickers/rescales on every live tick.
        private void UpdateLegendTicks()
        {
            var texts = new string[5];
            double[] tStops = { 0.0, 0.25, 0.5, 0.75, 1.0 };
            for (int i = 0; i < 5; i++)
            {
                double disp = Props.DisplayRate(_scaleMin + tStops[i] * (_scaleMax - _scaleMin));
                string num  = Props.IsMetric ? $"{disp:F1}" : $"{disp:F0}";
                texts[i]    = (i == 0 || i == 4) ? $"{num} {Props.RateUnit}" : num;
            }
            _tickText = texts;
            pnlLegend.Invalidate();
        }

        // Builds a header-width ribbon quad from point 1 to point 2. The two long
        // edges are offset perpendicular to the travel direction by half the header
        // width, so consecutive segments share an edge and tile without gaps on
        // straight runs. Returns null if the points coincide (no direction).
        private static List<PointLatLng> ComputeSegmentCorners(
            double lat1, double lon1, double lat2, double lon2, double widthM, double maxLenM)
        {
            double midLat  = (lat1 + lat2) / 2.0;
            double mPerLat = 111320.0;
            double mPerLon = Math.Max(1.0, 111320.0 * Math.Cos(midLat * Math.PI / 180.0));

            double dEast  = (lon2 - lon1) * mPerLon;
            double dNorth = (lat2 - lat1) * mPerLat;
            double len    = Math.Sqrt(dEast * dEast + dNorth * dNorth);
            if (len < 0.05 || len > maxLenM) return null;   // no movement, or gap too large to bridge

            // Unit perpendicular (travel direction rotated 90°), scaled to half width.
            double hW    = widthM / 2.0;
            double pLat  = ( dEast  / len) * hW / mPerLat;
            double pLon  = (-dNorth / len) * hW / mPerLon;

            return new List<PointLatLng>
            {
                new PointLatLng(lat1 + pLat, lon1 + pLon),
                new PointLatLng(lat2 + pLat, lon2 + pLon),
                new PointLatLng(lat2 - pLat, lon2 - pLon),
                new PointLatLng(lat1 - pLat, lon1 - pLon),
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

        // Re-derives every point's yield in the selected job from its stored raw
        // sensor reading using the crop's CURRENT calibration, then repaints. Only
        // the recompute is new — RebuildSwaths already recolors purely from the
        // stored YieldRate, so no drawing logic changes.
        private void btnRecalc_Click(object sender, EventArgs e)
        {
            int idx = cboJob.SelectedIndex;
            if (idx < 0 || idx >= _jobIds.Count) return;
            int jobId = _jobIds[idx];

            var answer = MessageBox.Show(Lang.lgRecalcMapPrompt, Lang.lgRecalcMap,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            int profileId = -1, cropId = -1, headerId = -1;
            foreach (var j in Core.Database.Jobs.GetAll())
            {
                if (j.id != jobId) continue;
                profileId = j.profileId;
                cropId    = j.cropId;
                headerId  = j.headerId;
                break;
            }
            if (profileId <= 0 || cropId <= 0) return;

            double testWeightLbsBu = 60.0;
            foreach (var c in Core.Database.Crops.GetAll())
                if (c.id == cropId) { testWeightLbsBu = c.testWeight; break; }

            double headerWidthM = 9.144;
            foreach (var h in Core.Database.Headers.GetAll())
                if (h.id == headerId) { headerWidthM = h.widthM; break; }

            var cal = Core.Database.Calibrations.GetLatest(profileId, cropId);
            Core.Database.YieldData.RecalculateJob(
                jobId, cal.baseline, cal.yieldFactor, headerWidthM, testWeightLbsBu);

            RebuildSwaths(Core.Database.YieldData.GetByJob(jobId), jobId, center: false);
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
            using var font    = new Font("Microsoft Sans Serif", 11f);
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
