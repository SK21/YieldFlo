using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMain : Form
    {
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
        private static bool IsTabletPC() => GetSystemMetrics(86) != 0;

        // Status message fade timer
        private System.Windows.Forms.Timer _msgTimer;
        private int _msgCountdown;

        // Borderless drag
        private bool _dragging;
        private System.Drawing.Point _dragStart;

        public frmMain()
        {
            InitializeComponent();
        }

        // ── Startup / Shutdown ────────────────────────────────────────────────

        private void frmMain_Load(object sender, EventArgs e)
        {
            ApplyTheme();

            // Enable drag on toolbar and title label (no title bar)
            pnlToolbar.MouseDown += Drag_MouseDown;
            pnlToolbar.MouseMove += Drag_MouseMove;
            pnlToolbar.MouseUp += Drag_MouseUp;
            lblTitle.MouseDown += Drag_MouseDown;
            lblTitle.MouseMove += Drag_MouseMove;
            lblTitle.MouseUp += Drag_MouseUp;

            RestorePosition();
            Core.Initialize(this);
            Core.UpdateDisplay += Core_UpdateDisplay;
            Core.JobStateChanged += Core_JobStateChanged;
            Core.ColorChanged += Core_ColorChanged;

            _msgTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _msgTimer.Tick += MsgTimer_Tick;

            UpdateStatusBar();
            SetJobButtons(Core.Collector?.IsRecording ?? false);

            // Tablet mode: slightly larger controls
            if (IsTabletPC())
                lblYield.Font = new Font(lblYield.Font.FontFamily, 36f, FontStyle.Bold);

            lblVersion.Text = "v" + Props.AppVersion;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Core.AppShutDown(e))
            {
                e.Cancel = true;
                return;
            }
            SavePosition();
        }

        private void SavePosition()
        {
            Properties.Settings.Default.MainFormX = this.Location.X;
            Properties.Settings.Default.MainFormY = this.Location.Y;
            Properties.Settings.Default.Save();
        }

        private void RestorePosition()
        {
            int x = Properties.Settings.Default.MainFormX;
            int y = Properties.Settings.Default.MainFormY;
            if (x < 0 || y < 0) return;

            var pt = new System.Drawing.Point(x, y);
            foreach (Screen s in Screen.AllScreens)
            {
                if (s.WorkingArea.Contains(pt))
                {
                    this.Location = pt;
                    return;
                }
            }
        }

        // ── Theme ─────────────────────────────────────────────────────────────

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var dispBack = Properties.Settings.Default.DisplayBackColour;
            var dispFore = Properties.Settings.Default.DisplayForeColour;

            // this.BackColor stays White — it shows through the 2px Padding as the border
            pnlToolbar.BackColor = back;
            pnlGauges.BackColor = back;
            pnlTotals.BackColor = back;
            pnlSensors.BackColor = back;

            lblYield.BackColor = dispBack;
            lblYield.ForeColor = dispFore;
            lblMoisture.BackColor = dispBack;
            lblMoisture.ForeColor = dispFore;
            lblSpeed.BackColor = dispBack;
            lblSpeed.ForeColor = dispFore;

            lblYieldTitle.ForeColor = fore;
            lblMoistureTitle.ForeColor = fore;
            lblSpeedTitle.ForeColor = fore;
            lblTotArea.ForeColor = fore;
            lblTotTotal.ForeColor = fore;
            lblTotRate.ForeColor = fore;
            lblMoistureUnit.ForeColor = System.Drawing.Color.White;
            lblMoistureUnit.BackColor = dispBack;

            pnlSensor1.BackColor = Color.FromArgb(30, 120, 30);
            pnlSensor2.BackColor = Color.FromArgb(30, 120, 30);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Form BackColor = White shows through the 2px Padding as the border.
            // No drawing needed here — child controls can't paint outside Padding.
        }

        // ── Drag (no title bar) ───────────────────────────────────────────────
        private void Drag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStart = e.Location;
            }
        }

        private void Drag_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                this.Left += e.X - _dragStart.X;
                this.Top += e.Y - _dragStart.Y;
            }
        }

        private void Drag_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        // ── Core events ───────────────────────────────────────────────────────

        private void Core_UpdateDisplay(object sender, EventArgs e)
        {
            UpdateGauges();
            UpdateStatusBar();
            CheckModuleTimeout();
        }

        private void Core_JobStateChanged(object sender, EventArgs e)
        {
            bool active = Core.Collector.IsRecording;
            SetJobButtons(active);
            UpdateStatusBar();
        }

        private void Core_ColorChanged(object sender, EventArgs e)
        {
            ApplyTheme();
            UpdateGauges();
            this.Invalidate();
        }

        // ── Gauge updates ─────────────────────────────────────────────────────

        private void UpdateGauges()
        {
            if (Core.IsShuttingDown) return;

            double yield = Props.DisplayRate(Core.Yield?.SmoothedYield ?? 0);
            double moisture = Core.LastMoisture > 0 ? Core.LastMoisture + Core.ActiveMoistureOffset : 0;
            double speed = Props.DisplaySpeed(Core.GPS.Speed);

            lblYield.Text = yield.ToString("F1");
            lblYieldUnit.Text = Props.RateUnit;
            lblMoisture.Text = moisture > 0 ? moisture.ToString("F1") : "--.-";
            lblSpeed.Text = speed.ToString("F1");
            lblSpeedUnit.Text = Props.SpeedUnit;

            // Bar 1 — Elevator flow (obstruction ratio 0–100%)
            double flow = Math.Min(1.0, Math.Max(0, Core.LastSensor1));
            pnlSensor1Fill.Width = (int)(pnlSensor1.Width * flow);
            lblSensor1Value.Text = (flow * 100).ToString("F0") + "%";
            pnlSensor1Fill.BackColor = flow > 0.05
                ? Color.FromArgb(50, 200, 50)
                : Color.FromArgb(80, 80, 80);

            // Bar 2 — Moisture (0–30% range mapped to full bar width)
            const double MaxMoisture = 30.0;
            double moistRatio = Math.Min(1.0, Math.Max(0, moisture / MaxMoisture));
            pnlSensor2Fill.Width = (int)(pnlSensor2.Width * moistRatio);
            lblSensor2Value.Text = moisture > 0 ? moisture.ToString("F1") + "%" : "--";
            pnlSensor2Fill.BackColor = moisture > 0
                ? Color.FromArgb(50, 150, 220)   // blue for moisture
                : Color.FromArgb(80, 80, 80);

            double area = Props.DisplayArea(Core.Collector.TotalAcres);
            double total = Props.DisplayMass(Core.Collector.TotalBushels);
            double avg = Props.DisplayRate(Core.Collector.AverageYield);

            lblTotArea.Text = $"{area:F2} {Props.AreaUnit}";
            lblTotTotal.Text = $"{total:F0} {Props.MassUnit}";
            lblTotRate.Text = $"{avg:F1} {Props.RateUnit}";
        }

        private void UpdateStatusBar()
        {
            bool gpsOk = Core.GPS.IsConnected;
            bool modOk = Core.ModuleConnected
                && (DateTime.UtcNow - Core.LastModuleReceive).TotalSeconds < 5;

            lblStatusGPS.Text = Lang.lgGPS;
            lblStatusGPS.ForeColor = gpsOk ? Color.LimeGreen : Color.Red;

            lblStatusModule.Text = Lang.lgModule;
            lblStatusModule.ForeColor = modOk ? Color.LimeGreen : Color.Red;

            string commType = Properties.Settings.Default.ModuleCommType;
            lblStatusComm.Text = commType;
            lblStatusComm.ForeColor = Color.Silver;

            if (Core.Collector.ActiveJobId > 0)
            {
                bool recording = Core.Collector.IsRecording;
                string jobName = Core.Collector.ActiveJobName.Length > 0 ? Core.Collector.ActiveJobName : "Active Job";
                lblStatusJob.Text = recording ? jobName + Lang.lgJobStatusOn : jobName + Lang.lgJobStatusOff;
                lblStatusJob.ForeColor = recording ? Color.LimeGreen : Color.Orange;
                //lblStatusJob.Font      = new System.Drawing.Font("Microsoft Sans Serif", recording ? 9F : 7F, System.Drawing.FontStyle.Bold);
            }
            else
            {
                lblStatusJob.Text = Lang.lgNoActiveJob;
                lblStatusJob.ForeColor = Color.Silver;
                //lblStatusJob.Font      = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            }
        }

        private void CheckModuleTimeout()
        {
            if (Core.ModuleConnected &&
                (DateTime.UtcNow - Core.LastModuleReceive).TotalSeconds > 5)
            {
                Core.ModuleConnected = false;
            }
        }

        // ── Toolbar buttons ───────────────────────────────────────────────────

        private void btnMenu_Click(object sender, EventArgs e)
        {
            FormManager.ShowForm(new frmMenu());
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Core.Collector.IsRecording) return;

            if (Core.Collector.ActiveJobId > 0)
            {
                // Resume paused job
                Core.Collector.ResumeJob();
                SetJobButtons(true);
                Core.RaiseJobStateChanged();
            }
            else
            {
                // Open Jobs menu to configure and start a new job
                FormManager.ShowForm(new frmMenuJobs());
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Core.Collector.PauseJob();
            SetJobButtons(false);
            Core.RaiseJobStateChanged();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            var answer = MessageBox.Show(Lang.lgStopJobPrompt, Lang.lgStopJob,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            Core.Collector.StopJob();
            SetJobButtons(false);
            Core.RaiseJobStateChanged();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Core.RequestUserExit();
        }

        private void btnMini_Click(object sender, EventArgs e)
        {
            this.Hide();
            new frmMini().Show();
        }

        // Full-strength colours for each job button when it is available.
        private static readonly Color StartActive = Color.FromArgb(0, 150, 0);    // green
        private static readonly Color PauseActive = Color.FromArgb(200, 150, 0);  // amber
        private static readonly Color StopActive  = Color.FromArgb(170, 0, 0);    // red
        // Shared "unavailable" look — clearly off, not just a dimmed word.
        private static readonly Color BtnOffBack   = Color.FromArgb(40, 40, 40);
        private static readonly Color BtnOffFore   = Color.FromArgb(95, 95, 95);
        private static readonly Color BtnOffBorder = Color.FromArgb(70, 70, 70);

        private void SetJobButtons(bool jobActive)
        {
            // jobActive param is ignored — button states are derived from the job
            // lifecycle, not the moment-to-moment IsRecording flag. IsRecording
            // toggles automatically as AOG sections come on/off (every headland
            // turn), which must NOT restyle the buttons or the operator sees Start
            // "light up" mid-harvest and thinks they need to press it.
            var col = Core.Collector;
            bool hasJob = col != null && col.ActiveJobId > 0;
            // Manual pause zeroes both flags; auto-pause keeps IsAutoPaused set.
            bool manuallyPaused = hasJob && !col.IsRecording && !col.IsAutoPaused;

            // Start (▶) = create a new job (none active) or resume a manually
            // paused one. Icon is constant; availability shows via lit/dark colour.
            btnStart.Enabled = !hasJob || manuallyPaused;
            // Pause = manually pause an armed/recording job (pointless once paused).
            btnPause.Enabled = hasJob && !manuallyPaused;
            // Stop = end the job; available whenever a job exists.
            btnStop.Enabled  = hasJob;

            StyleJobButton(btnStart, StartActive);
            StyleJobButton(btnPause, PauseActive);
            StyleJobButton(btnStop,  StopActive);
        }

        // Make availability obvious at a glance: an enabled button shows its
        // full colour with a bright border; a disabled one goes flat dark grey
        // so it plainly reads as "off" rather than a slightly greyed word.
        private static void StyleJobButton(Button btn, Color activeBack)
        {
            if (btn.Enabled)
            {
                btn.BackColor = activeBack;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = Color.FromArgb(230, 230, 230);
                btn.FlatAppearance.BorderSize  = 2;
            }
            else
            {
                btn.BackColor = BtnOffBack;
                btn.ForeColor = BtnOffFore;
                btn.FlatAppearance.BorderColor = BtnOffBorder;
                btn.FlatAppearance.BorderSize  = 1;
            }
        }

        // ── Status message ────────────────────────────────────────────────────

        public void ShowStatusMessage(string message, bool isError)
        {
            lblStatusMsg.BackColor = pnlStatus.BackColor;
            lblStatusMsg.Text = message;
            lblStatusMsg.ForeColor = isError ? Color.Red : Color.Yellow;
            lblStatusMsg.Visible = true;
            lblStatusMsg.BringToFront();
            _msgCountdown = 10;
            _msgTimer.Start();
        }

        private void MsgTimer_Tick(object sender, EventArgs e)
        {
            _msgCountdown--;
            if (_msgCountdown <= 0)
            {
                lblStatusMsg.Visible = false;
                lblStatusMsg.Text = "";
                _msgTimer.Stop();
            }
        }
    }
}
