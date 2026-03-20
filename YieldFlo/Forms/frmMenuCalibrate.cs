using System;
using System.Drawing;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    public partial class frmMenuCalibrate : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private System.Windows.Forms.Timer _calTimer;

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
            this.Shown += frmMenuCalibrate_Shown;

            _calTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _calTimer.Tick += CalTimer_Tick;

            // Reflect state if a cal run was already active when form opened
            UpdateCalRunButtons();
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
            btnApplyFactor.BackColor = Color.FromArgb(0, 60, 120);
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

            Props.ShowMessage("Calibration saved and applied.");
        }

        // ── Calibration run ──────────────────────────────────────────────────

        private void btnStartCal_Click(object sender, EventArgs e)
        {
            Core.Yield.StartCalRun();
            lblCalResult.Text = "";
            btnApplyFactor.Enabled = false;
            _calTimer.Start();
            UpdateCalRunButtons();
            btnStopCal.Focus();
        }

        private void btnStopCal_Click(object sender, EventArgs e)
        {
            Core.Yield.StopCalRun();
            _calTimer.Stop();
            UpdateCalMeasuredLabel();
            btnApplyFactor.Enabled = Core.Yield.CalRunBushels > 0;
            UpdateCalRunButtons();
        }

        private void btnApplyFactor_Click(object sender, EventArgs e)
        {
            double actualBushels = DisplayMassToInternalBushels((double)numActualWeight.Value);
            if (actualBushels <= 0)
            {
                Props.ShowMessage("Enter the actual weighed amount.", "", 2000, true);
                return;
            }
            if (Core.Yield.CalRunBushels <= 0)
            {
                Props.ShowMessage("No measured data — run a calibration pass first.", "", 2000, true);
                return;
            }

            double newFactor = Core.Yield.ComputeNewFactor(actualBushels);
            Core.Yield.ResetSmoothing();

            // Clamp to valid range and update the field
            decimal clamped = (decimal)System.Math.Min((double)numFactor.Maximum,
                               System.Math.Max((double)numFactor.Minimum, newFactor));
            numFactor.Value = clamped;

            lblCalResult.Text = $"New factor: {newFactor:F3}";

            // Auto-persist
            Properties.Settings.Default.ProcessingDelaySec = (int)numDelay.Value;
            Properties.Settings.Default.Save();
            if (Core.ActiveProfileId > 0 && Core.ActiveCropId > 0)
            {
                Core.Database.Calibrations.Save(
                    Core.ActiveProfileId, Core.ActiveCropId,
                    (double)numBaseline.Value,
                    newFactor,
                    (int)numDelay.Value);
            }

            Props.ShowMessage($"Factor updated to {newFactor:F3} and saved.");
        }

        private void CalTimer_Tick(object sender, EventArgs e)
        {
            UpdateCalMeasuredLabel();
        }

        private void UpdateCalMeasuredLabel()
        {
            double bushels = Core.Yield?.CalRunBushels ?? 0;
            double display = Props.DisplayMass(bushels);
            string unit    = Props.MassUnit;
            string fmt     = Props.IsMetric ? "F2" : "F0";
            lblCalMeasured.Text = $"Measured: {display.ToString(fmt)} {unit}";
        }

        private void UpdateCalRunButtons()
        {
            bool running = Core.Yield?.IsCalRunActive ?? false;
            numActualWeight.Enabled = !running;   // disable weight entry before focus moves
            btnStopCal.Enabled      = running;    // enable stop before disabling start
            btnStartCal.Enabled     = !running;   // disabling this triggers focus move
            btnStartCal.BackColor   = running ? Color.FromArgb(60, 60, 60) : Color.FromArgb(0, 90, 0);
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
            base.OnFormClosing(e);
        }

        private void btnSetBaseline_Click(object sender, EventArgs e)
        {
            double live = (Core.LastSensor1 + Core.LastSensor2) / 2.0;
            live = Math.Round(live, 3);

            var answer = MessageBox.Show(
                $"Set baseline to {live:F3}?\n\nMake sure the elevator is running with no grain.",
                "Set Baseline",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            decimal clamped = (decimal)Math.Min((double)numBaseline.Maximum,
                               Math.Max((double)numBaseline.Minimum, live));
            numBaseline.Value = clamped;
        }

        private void frmMenuCalibrate_Shown(object sender, EventArgs e)
        {
            NumpadHelper.Wire(this, numActualWeight, 0, 999999, 1, "Actual Weight");
            btnSaveCal.Focus();
        }

        private void btnTitleClose_Click(object sender, EventArgs e)  => this.Close();
        private void btnCalClose_Click(object sender, EventArgs e)    => this.Close();
    }
}
