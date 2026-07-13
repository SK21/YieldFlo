using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMenuCalibrate : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private System.Windows.Forms.Timer _calTimer;
        private System.Windows.Forms.Timer _noiseTimer;

        // Baseline sampling (Set Baseline button)
        private System.Windows.Forms.Timer _baselineTimer;
        private readonly List<double> _baselineSamples = new List<double>();
        private const int BaselineSampleMs = 200;      // one module packet period
        private const int BaselineSampleCount = 25;    // ~5 seconds

        // Noise readout: rolling average of sampled packet counts. A steady
        // glitch rate quantizes to 4-or-5 per 200 ms packet, so the raw value
        // flutters (20/25); averaging ~5 s of samples steadies it.
        private readonly Queue<int> _noiseSamples = new Queue<int>();
        private const int NoiseSampleCount = 10;       // 10 × 500 ms ticks = 5 s

        // Paddle-rate readout: the module reports whole paddles/s once a second,
        // so a true 7.4 Hz reads 7-or-8 — the same rolling average recovers the
        // fraction. Compare against the FarmTrx sensor cal result (X% @ Y Hz).
        private readonly Queue<int> _hzSamples = new Queue<int>();

        public frmMenuCalibrate()
        {
            InitializeComponent();
        }

        private void frmMenuCalibrate_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            ApplyUnits();
            FormPositions.Restore(this);
            this.FormClosed += (s2, ev2) => FormPositions.Save(this);
            foreach (Control c in new Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) { _dragging = true; _dragStart = ev.Location; } };
                c.MouseMove += (s, ev) => { if (_dragging) { Left += ev.X - _dragStart.X; Top += ev.Y - _dragStart.Y; } };
                c.MouseUp   += (s, ev) => _dragging = false;
            }
            LoadCurrentValues();
            UpdateSavedLabel();
            this.Shown += frmMenuCalibrate_Shown;

            _calTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _calTimer.Tick += CalTimer_Tick;

            // Live noise readout — glitch edges the module rejected in the last
            // 200 ms packet. Nonzero at idle means electrical noise on the
            // sensor wire; it should read 0 on a healthy installation.
            _noiseTimer = new System.Windows.Forms.Timer { Interval = 500 };
            _noiseTimer.Tick += NoiseTimer_Tick;
            _noiseTimer.Start();

            // Reflect state if a cal run was already active when form opened
            UpdateCalRunButtons();

            // The run keeps accumulating in Core.Yield while the form is closed.
            // On reopen, restore the measured display — and resume live updates if
            // still running — otherwise the label stays blank until the next
            // Start/Stop even though a run is in progress or has data.
            if (Core.Yield?.IsCalRunActive ?? false)
            {
                UpdateCalMeasuredLabel();
                _calTimer.Start();
            }
            else if ((Core.Yield?.CalRunBushels ?? 0) > 0)
            {
                UpdateCalMeasuredLabel();
            }
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
                if (c is NumericUpDown nd) { nd.BackColor = ctrl; nd.ForeColor = fore; }
                if (c is Button btn)       { btn.BackColor = ctrl; btn.ForeColor = Color.White; }
            }
            btnSetBaseline.BackColor = Color.FromArgb(0, 70, 110);
            btnSetBaseline.ForeColor = Color.White;
            btnSaveCal.BackColor    = Color.FromArgb(0, 90, 0);
            btnStartCal.BackColor   = Color.FromArgb(0, 90, 0);
            btnStopCal.BackColor    = Color.FromArgb(100, 0, 0);
            btnApplyFactor.BackColor = Color.FromArgb(0, 70, 120); btnApplyFactor.ForeColor = Color.White;
        }

        private void ApplyUnits()
        {
            lblActualUnit.Text = Props.IsMetric ? "kg" : "lbs";
            // Max sensible load: ~500,000 kg or ~1,100,000 lbs
            numActualWeight.Maximum       = Props.IsMetric ? 500000 : 1100000;
            numActualWeight.DecimalPlaces = 0;
            numActualWeight.Increment     = 1;
        }

        private void LoadCurrentValues()
        {
            var y = Core.Yield;
            if (y == null) return;
            numDelay.Value    = System.Math.Min(numDelay.Maximum,
                                System.Math.Max(numDelay.Minimum, y.ProcessingDelaySec));
            numBaseline.Value = (decimal)System.Math.Min((double)numBaseline.Maximum,
                                System.Math.Max((double)numBaseline.Minimum, y.SensorBaseline));
            numFactor.Value   = (decimal)System.Math.Min((double)numFactor.Maximum,
                                System.Math.Max((double)numFactor.Minimum, y.YieldFactor));
        }

        // FarmTrx-style "last calibration" stamp — when the current profile/crop's
        // calibration record was last written (Save && Apply or Apply Cal).
        private void UpdateSavedLabel()
        {
            string when = "--";   // no profile/crop active, or nothing saved for them yet
            if (Core.ActiveProfileId > 0 && Core.ActiveCropId > 0)
            {
                var dt = Core.Database.Calibrations.GetLatestDate(Core.ActiveProfileId, Core.ActiveCropId);
                if (dt.HasValue) when = dt.Value.ToString("g");
            }
            lblCalSaved.Text = Lang.lgLastSaved + " " + when;
        }

        private void btnSaveCal_Click(object sender, EventArgs e)
        {
            Core.Yield.ProcessingDelaySec = (int)numDelay.Value;
            Core.Yield.SensorBaseline     = (double)numBaseline.Value;
            Core.Yield.YieldFactor        = (double)numFactor.Value;
            Core.Yield.ResetSmoothing();

            Properties.Settings.Default.ProcessingDelaySec = (int)numDelay.Value;
            Properties.Settings.Default.Save();

            if (Core.ActiveProfileId > 0 && Core.ActiveCropId > 0)
            {
                Core.Database.Calibrations.Save(
                    Core.ActiveProfileId, Core.ActiveCropId,
                    (double)numBaseline.Value,
                    (double)numFactor.Value,
                    (int)numDelay.Value);
            }
            UpdateSavedLabel();

            Props.ShowMessage(Lang.lgCalSaved);
        }

        // ── Calibration run ──────────────────────────────────────────────────

        private void btnStartCal_Click(object sender, EventArgs e)
        {
            Core.Yield.StartCalRun();
            numActualWeight.Value = 0;   // stale weight from a previous run must not be reused
            _calTimer.Start();
            UpdateCalRunButtons();
        }

        private void btnStopCal_Click(object sender, EventArgs e)
        {
            Core.Yield.StopCalRun();
            _calTimer.Stop();
            UpdateCalMeasuredLabel();
            UpdateCalRunButtons();
        }

        private void btnApplyFactor_Click(object sender, EventArgs e)
        {
            // Diagnostic snapshot — captured before anything mutates, so a
            // reported "wrong yield factor" can be reconstructed exactly from
            // the log instead of from recollection. Records the raw entry, both
            // test-weight sources (to catch a Props/Yield mismatch), and the
            // computed result, including the two paths where Apply does nothing.
            double enteredDisplay = (double)numActualWeight.Value;
            double actualBushels  = DisplayMassToInternalBushels(enteredDisplay);
            double oldFactor      = Core.Yield?.YieldFactor ?? 0;
            double calRunBushels  = Core.Yield?.CalRunBushels ?? 0;
            double twKgDisplay    = Props.TestWeightKgPerBu;
            double twLbsYield     = Core.Yield?.TestWeightLbsBu ?? 0;

            string diag = "ApplyCal: "
                + $"entered={enteredDisplay:F1} {(Props.IsMetric ? "kg" : "lbs")}, "
                + $"actualBushels={actualBushels:F4}, "
                + $"calRunBushels={calRunBushels:F4}, "
                + $"oldFactor={oldFactor:F4}, "
                + $"TWkgDisplay={twKgDisplay:F4}, TWlbsYield={twLbsYield:F4}, "
                + $"TWlbsYield*0.453592={twLbsYield * 0.453592:F4}, "
                + $"isMetric={Props.IsMetric}, running={Core.Yield?.IsCalRunActive}, "
                + $"profileId={Core.ActiveProfileId}, cropId={Core.ActiveCropId}";

            if (actualBushels <= 0)
            {
                Props.WriteErrorLog(diag + " -> ABORT (entered weight <= 0)");
                Props.ShowMessage(Lang.lgEnterWeighedAmt, "", 2000, true);
                return;
            }
            if (Core.Yield.CalRunBushels <= 0)
            {
                Props.WriteErrorLog(diag + " -> ABORT (no measured data)");
                Props.ShowMessage(Lang.lgNoMeasuredData, "", 2000, true);
                return;
            }

            double newFactor = Core.Yield.ComputeNewFactor(actualBushels);
            Props.WriteErrorLog(diag + $" -> newFactor={newFactor:F4} "
                + $"(ratio={actualBushels / calRunBushels:F4})");

            // Clamp to valid range and stage in the field — nothing takes
            // effect (Core.Yield, the database) until Save & Apply is pressed.
            decimal clamped = (decimal)System.Math.Min((double)numFactor.Maximum,
                               System.Math.Max((double)numFactor.Minimum, newFactor));
            numFactor.Value = clamped;

            Props.ShowMessage(Lang.lgPendingSave);
        }

        private void CalTimer_Tick(object sender, EventArgs e)
        {
            UpdateCalMeasuredLabel();
        }

        private void NoiseTimer_Tick(object sender, EventArgs e)
        {
            if (!Core.ModuleConnected)
            {
                _noiseSamples.Clear();
                lblNoise.Text = Lang.lgNoise + " --";
                lblNoise.ForeColor = Color.Silver;
                _hzSamples.Clear();
                lblPaddleHz.Text = Lang.lgPaddles + " --";
                return;
            }

            _noiseSamples.Enqueue(Core.LastNoiseCount);
            while (_noiseSamples.Count > NoiseSampleCount) _noiseSamples.Dequeue();

            // Packet carries rejects per 200 ms window — ×5 = rejects per second
            double sum = 0;
            foreach (int s in _noiseSamples) sum += s;
            int perSec = (int)Math.Round(sum * 5.0 / _noiseSamples.Count);

            lblNoise.Text = Lang.lgNoise + " " + perSec + "/s";
            lblNoise.ForeColor = perSec > 0 ? Color.Orange : Color.Silver;

            if (Core.LastPaddleHz < 0)
            {
                // module firmware predates the paddle_hz field
                _hzSamples.Clear();
                lblPaddleHz.Text = Lang.lgPaddles + " --";
            }
            else
            {
                _hzSamples.Enqueue(Core.LastPaddleHz);
                while (_hzSamples.Count > NoiseSampleCount) _hzSamples.Dequeue();

                double hzSum = 0;
                foreach (int s in _hzSamples) hzSum += s;
                lblPaddleHz.Text = Lang.lgPaddles + " " + (hzSum / _hzSamples.Count).ToString("0.0") + " Hz";
            }
        }

        private void UpdateCalMeasuredLabel()
        {
            double bushels = Core.Yield?.CalRunBushels ?? 0;
            if (Props.IsMetric)
            {
                double tonnes = Props.DisplayMass(bushels);
                lblCalMeasured.Text = string.Format(Lang.lgMeasuredMetric, tonnes);
            }
            else
            {
                double lbs = bushels * (Core.Yield?.TestWeightLbsBu ?? 60.0);
                lblCalMeasured.Text = string.Format(Lang.lgMeasuredImperial, bushels, lbs);
            }
        }

        private void UpdateCalRunButtons()
        {
            bool running = Core.Yield?.IsCalRunActive ?? false;
            btnStartCal.Enabled     = !running;
            btnStartCal.BackColor   = running ? Color.FromArgb(60, 60, 60) : Color.FromArgb(0, 90, 0);
            btnStopCal.Enabled      = running;
            numActualWeight.Enabled = !running;
        }

        // Weight entry (kg or lbs) → internal bushels
        private double DisplayMassToInternalBushels(double value)
        {
            double twLbs = Core.Yield?.TestWeightLbsBu ?? 60.0;
            if (Props.IsMetric)
            {
                double kgPerBu = twLbs * 0.453592;
                return kgPerBu > 0 ? value / kgPerBu : 0;
            }
            return twLbs > 0 ? value / twLbs : 0;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _calTimer?.Stop();
            _baselineTimer?.Stop();
            _noiseTimer?.Stop();
            base.OnFormClosing(e);
        }

        private void btnSetBaseline_Click(object sender, EventArgs e)
        {
            if (_baselineTimer != null && _baselineTimer.Enabled) return;   // already sampling

            if (!Core.ModuleConnected)
            {
                Props.ShowMessage(Lang.lgBaselineNoModule, "", 2000, true);
                return;
            }

            // Sample the live reading for ~5 s and take the median — a single
            // instantaneous read can catch a spiked packet (e.g. a no-pulse
            // 100% reading) and store a baseline far off the true idle ratio.
            _baselineSamples.Clear();

            // Left enabled during sampling — the re-entrancy check above already
            // blocks a second press, and disabling would render the countdown
            // text in WinForms' dim system disabled color regardless of theme.
            if (_baselineTimer == null)
            {
                _baselineTimer = new System.Windows.Forms.Timer { Interval = BaselineSampleMs };
                _baselineTimer.Tick += BaselineTimer_Tick;
            }
            btnSetBaseline.Text = Lang.lgSetBaseline + " 5";
            _baselineTimer.Start();
        }

        private void BaselineTimer_Tick(object sender, EventArgs e)
        {
            _baselineSamples.Add(Core.LastSensor1);

            int secondsLeft = (BaselineSampleCount - _baselineSamples.Count) * BaselineSampleMs / 1000;
            btnSetBaseline.Text = Lang.lgSetBaseline + " " + (secondsLeft + 1);

            if (_baselineSamples.Count < BaselineSampleCount) return;

            _baselineTimer.Stop();
            btnSetBaseline.Text = Lang.lgSetBaseline;

            _baselineSamples.Sort();
            double median = _baselineSamples[_baselineSamples.Count / 2];
            median = Math.Round(median, 3);

            decimal clamped = (decimal)Math.Min((double)numBaseline.Maximum,
                               Math.Max((double)numBaseline.Minimum, median));
            numBaseline.Value = clamped;

            // Staged only — nothing reaches Core.Yield or the database until
            // Save & Apply is pressed. A Calibration Run started before that
            // still measures against the previous baseline.
            Props.ShowMessage(Lang.lgPendingSave);
        }

        private void frmMenuCalibrate_Shown(object sender, EventArgs e)
        {
            NumpadHelper.Wire(this, numDelay,    0,      60,     0, "Processing Delay (s)");
            NumpadHelper.Wire(this, numBaseline, 0,      0.99,   3, "Sensor Baseline");
            NumpadHelper.Wire(this, numFactor,   0.01,   100,    2, "Yield Factor");
            NumpadHelper.WireClickOnly(this, numActualWeight, 0, 999999, 1, "Actual Weight");
            btnSaveCal.Focus();
        }

        private void btnTitleClose_Click(object sender, EventArgs e)  => this.Close();
        private void btnCalClose_Click(object sender, EventArgs e)    => this.Close();

        private void lblCalSaved_Click(object sender, EventArgs e)
        {

        }
    }
}
