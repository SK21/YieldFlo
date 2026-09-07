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

        // The baseline is what an EMPTY elevator reads — the paddles alone
        // occluding the beam. A quarter of the beam blocked with no grain in the
        // machine is not a baseline, it is a dirty sensor or grain left in the
        // elevator, and it silently rescales every reading taken against it.
        // Field case: three captures five minutes apart read 0.92, 0.344 and
        // 0.086, and nothing questioned the first two.
        //
        // A warning, not a limit. There is no ratio that is provably wrong — a
        // genuinely fouled elevator does read high, and the operator may know
        // it — so this asks rather than blocks.
        private const double BaselineWarnRatio = 0.25;

        // Noise readout: rolling average of sampled packet counts. A steady
        // glitch rate quantizes to 4-or-5 per 200 ms packet, so the raw value
        // flutters (20/25); averaging ~5 s of samples steadies it.
        private readonly Queue<int> _noiseSamples = new Queue<int>();
        private const int NoiseSampleCount = 10;       // 10 × 500 ms ticks = 5 s

        // Paddle-rate readout: the module reports whole paddles/s once a second,
        // so a true 7.4 Hz reads 7-or-8 — the same rolling average recovers the
        // fraction. Compare against the FarmTrx sensor cal result (X% @ Y Hz).
        private readonly Queue<int> _hzSamples = new Queue<int>();

        // True while the staged Yield Factor is the one Calculate computed from the
        // standing run. Save clears the run only in that case: the same
        // button also saves a baseline-only change or a hand-typed factor, and
        // neither of those has consumed the run.
        //
        // Set AFTER Calculate assigns numFactor.Value, because that assignment
        // itself raises ValueChanged — which is what clears the flag when the
        // operator overtypes the figure by hand.
        private bool _factorFromCalRun;

        // Same idea for the Whole Field tab, kept separate because the two have
        // different consequences at Save: a cal run is spent and gets
        // discarded, a field fit instead offers to rewrite the job's recorded
        // points so the map and total match the weight that was entered.
        private bool _factorFromFieldCal;

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

            // The numpad writes straight to Value, so a hand-typed baseline gets
            // the same cue as a sampled one — otherwise typing 0.5 looks normal
            // right up until Save.
            numBaseline.ValueChanged += (s2, ev2) => UpdateBaselineWarning();

            // Any later edit to the factor — numpad, spinner, keyboard — means the
            // staged value is no longer the one Calculate derived, so Save
            // must not treat the run as spent.
            numFactor.ValueChanged += (s2, ev2) =>
            {
                _factorFromCalRun   = false;
                _factorFromFieldCal = false;
            };

            // Live implied yield as the weight is typed — see UpdateFieldImplied.
            numFieldWeight.ValueChanged += (s2, ev2) => UpdateFieldImplied();
            tabCal.SelectedIndexChanged += (s2, ev2) => UpdateFieldCalLabels();
            UpdateFieldCalLabels();

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
            // Recursive since the calibration controls moved inside tab pages: a
            // flat walk of pnlContent.Controls now reaches the TabControl and stops,
            // leaving everything on both tabs at default system colours on an
            // otherwise dark form.
            ThemeChildren(pnlContent, fore, ctrl);

            // A TabPage ignores BackColor while UseVisualStyleBackColor is on, and
            // the designer sets it on both pages.
            foreach (TabPage page in tabCal.TabPages)
            {
                page.UseVisualStyleBackColor = false;
                page.BackColor = back;
            }

            // The strip itself is painted by DarkTabControl; give it the same
            // palette the rest of the form is using rather than its defaults.
            tabCal.HeaderBack      = back;
            tabCal.PageBack        = back;
            tabCal.TabBack         = ctrl;
            tabCal.TabSelected     = Color.FromArgb(0, 70, 120);
            tabCal.TabFore         = Color.Silver;
            tabCal.TabForeSelected = Color.White;
            // Same grey as the section rules (panel1/panel2/pnlSep1), lifted so it
            // still reads as an outline rather than disappearing into the panel.
            tabCal.BorderColour    = Color.FromArgb(150, 150, 150);
            tabCal.Invalidate();

            btnSetBaseline.BackColor = Color.FromArgb(0, 70, 110);
            btnSetBaseline.ForeColor = Color.White;
            btnSaveCal.BackColor    = Color.FromArgb(0, 90, 0);
            btnStartCal.BackColor   = Color.FromArgb(0, 90, 0);
            btnStopCal.BackColor    = Color.FromArgb(100, 0, 0);
            btnApplyFactor.BackColor = Color.FromArgb(0, 70, 120); btnApplyFactor.ForeColor = Color.White;
            btnApplyField.BackColor  = Color.FromArgb(0, 70, 120); btnApplyField.ForeColor  = Color.White;
        }

        private static void ThemeChildren(Control parent, Color fore, Color ctrl)
        {
            foreach (Control c in parent.Controls)
            {
                c.ForeColor = fore;
                if (c is NumericUpDown nd) { nd.BackColor = ctrl; nd.ForeColor = fore; }
                if (c is Button btn)       { btn.BackColor = ctrl; btn.ForeColor = Color.White; }
                if (c.HasChildren) ThemeChildren(c, fore, ctrl);
            }
        }

        private void ApplyUnits()
        {
            bool bu = Props.EntryInBushels;

            btnActualUnit.Text = Props.EntryMassUnit;
            btnFieldUnit.Text  = Props.EntryMassUnit;

            // Metric entry is kg and there is nothing to choose, so the toggle is
            // dead there rather than hidden — a control that vanishes between unit
            // settings is harder to find again than one that is simply inactive.
            btnActualUnit.Enabled = !Props.IsMetric;
            btnFieldUnit.Enabled  = !Props.IsMetric;

            // Max sensible load: ~500,000 kg, ~1,100,000 lbs, or the same in bushels.
            numActualWeight.Maximum       = Props.IsMetric ? 500000 : (bu ? 20000 : 1100000);
            // A bushel is ~60 lb, so a bushel figure needs a decimal place to carry
            // the resolution a whole pound gives.
            numActualWeight.DecimalPlaces = bu ? 1 : 0;
            numActualWeight.Increment     = 1;

            // A whole field is many loads, not one, so the ceiling is an order of
            // magnitude above the cal-run box's single cart.
            numFieldWeight.Maximum       = Props.IsMetric ? 5000000 : (bu ? 200000 : 11000000);
            numFieldWeight.DecimalPlaces = bu ? 1 : 0;
            numFieldWeight.Increment     = 1;
        }

        /// <summary>
        /// Flips imperial weight entry between bushels and pounds. Both boxes follow
        /// the one setting and it is persisted, because the choice tracks where the
        /// weigh tickets come from rather than which screen is open.
        /// </summary>
        private void btnUnitToggle_Click(object sender, EventArgs e)
        {
            if (Props.IsMetric) return;

            // Held as bushels across the switch so the same physical amount comes
            // back in the new unit. Reinterpreting the digits instead would turn
            // 21,500 lbs into 21,500 bu without a word.
            double actualBu = DisplayMassToInternalBushels((double)numActualWeight.Value);
            double fieldBu  = DisplayMassToInternalBushels((double)numFieldWeight.Value);

            Properties.Settings.Default.ImperialMassUnit = Props.EntryInBushels ? "lbs" : "bu";
            Properties.Settings.Default.Save();

            // Before the values go back in: ApplyUnits moves Maximum, and assigning
            // above the old ceiling would clamp.
            ApplyUnits();

            numActualWeight.Value = ClampToBox(numActualWeight, InternalBushelsToDisplayMass(actualBu));
            numFieldWeight.Value  = ClampToBox(numFieldWeight,  InternalBushelsToDisplayMass(fieldBu));

            UpdateFieldImplied();
        }

        private static decimal ClampToBox(NumericUpDown box, double value)
        {
            decimal d = (decimal)value;
            return System.Math.Min(box.Maximum, System.Math.Max(box.Minimum, d));
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
            UpdateBaselineWarning();
        }

        // Marks an implausible baseline where the operator is already looking,
        // so the prompt at Save is a confirmation rather than a surprise.
        private void UpdateBaselineWarning()
        {
            numBaseline.ForeColor = (double)numBaseline.Value > BaselineWarnRatio
                ? Color.Orange
                : Properties.Settings.Default.MainForeColour;
        }

        // When this machine's sensor was last zeroed. Profile-scoped, not crop-scoped,
        // and stamped only when the baseline value actually moves — a factor-only save
        // leaves it alone, so the date answers "how stale is my zero point" rather than
        // "when did I last press Save". The baseline is the one calibration value that
        // drifts on its own, as dust and grain build up on the plate.
        private void UpdateSavedLabel()
        {
            string when = "--";   // no profile active, or the baseline has never been set
            if (Core.ActiveProfileId > 0)
            {
                var dt = Core.Database.Profiles.GetBaselineSetDate(Core.ActiveProfileId);
                if (dt.HasValue) when = dt.Value.ToString("g");
            }
            lblCalSaved.Text = Lang.lgBaselineSaved + " " + when;
        }

        private void btnSaveCal_Click(object sender, EventArgs e)
        {
            // Checked here rather than at capture because this is the only point
            // both paths pass through: nothing reaches Core.Yield or the database
            // until Save, and a value typed by hand never goes near the
            // sampling timer.
            if ((double)numBaseline.Value > BaselineWarnRatio)
            {
                using var dlg = new frmMsgBox(
                    string.Format(Lang.lgBaselineHighPrompt, (double)numBaseline.Value),
                    Lang.lgBaselineHigh);
                dlg.ShowDialog(this);
                if (!dlg.Result) return;
            }

            Core.Yield.ProcessingDelaySec = (int)numDelay.Value;
            Core.Yield.SensorBaseline     = (double)numBaseline.Value;
            Core.Yield.YieldFactor        = (double)numFactor.Value;
            Core.Yield.ResetSmoothing();

            Properties.Settings.Default.ProcessingDelaySec = (int)numDelay.Value;
            Properties.Settings.Default.Save();

            // Baseline is stored on the profile — it belongs to the sensor, not the
            // crop. It is still written into the calibrations row as well: that row
            // records which baseline was in force for this calibration, which is what
            // frmYieldMap's recalculate needs to rescale points already on the map.
            if (Core.ActiveProfileId > 0)
                Core.Database.Profiles.UpdateSensorBaseline(
                    Core.ActiveProfileId, (double)numBaseline.Value);

            if (Core.ActiveProfileId > 0 && Core.ActiveCropId > 0)
            {
                Core.Database.Calibrations.Save(
                    Core.ActiveProfileId, Core.ActiveCropId,
                    (double)numBaseline.Value,
                    (double)numFactor.Value,
                    (int)numDelay.Value);
            }
            UpdateSavedLabel();

            // The run has now been spent: its weight produced this factor and the
            // factor is saved. Discarding it here stops a standing total — which
            // survives restarts now — from being applied a second time against the
            // factor it already corrected. Only when the saved factor is the one
            // Calculate derived; a baseline-only save or a hand-typed factor leaves
            // the run alone.
            if (_factorFromCalRun)
            {
                Core.Yield.ClearCalRun();
                _factorFromCalRun = false;
                numActualWeight.Value = 0;
                UpdateCalMeasuredLabel();
                UpdateCalRunButtons();
            }

            // A field fit is not spent the way a cal run is — the job keeps
            // accumulating and can be fitted again later against a larger ticket —
            // so the entry stays put. What it offers instead is bringing the job's
            // already-recorded points up to the factor just saved.
            if (_factorFromFieldCal)
            {
                _factorFromFieldCal = false;
                Props.ShowMessage(Lang.lgCalSaved);
                OfferJobRecalculate();
                return;
            }

            Props.ShowMessage(Lang.lgCalSaved);
        }

        // ── Calibration run ──────────────────────────────────────────────────

        private void btnStartCal_Click(object sender, EventArgs e)
        {
            // StartCalRun zeroes the accumulator. A finished run whose weight has not
            // been entered yet is a real measurement the operator may still be driving
            // to a scale to collect, and it can now be days old — so it is not thrown
            // away silently. Only asked when there is something to lose.
            if (!(Core.Yield?.IsCalRunActive ?? false) && (Core.Yield?.CalRunBushels ?? 0) > 0)
            {
                using var dlg = new frmMsgBox(
                    string.Format(Lang.lgCalRunDiscardPrompt, CalRunMeasuredText(), CalRunWhenText()),
                    Lang.lgCalRunDiscard);
                dlg.ShowDialog(this);
                if (!dlg.Result) return;
            }

            Core.Yield.StartCalRun();
            numActualWeight.Value = 0;   // stale weight from a previous run must not be reused
            _factorFromCalRun     = false;   // any staged factor belongs to the run just replaced
            _calTimer.Start();
            UpdateCalMeasuredLabel();
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
                + $"entered={enteredDisplay:F1} {Props.EntryMassUnit}, "
                + $"actualBushels={actualBushels:F4}, "
                + $"calRunBushels={calRunBushels:F4}, "
                + $"oldFactor={oldFactor:F4}, "
                + $"TWkgDisplay={twKgDisplay:F4}, TWlbsYield={twLbsYield:F4}, "
                + $"TWlbsYield*0.453592={twLbsYield * 0.453592:F4}, "
                + $"isMetric={Props.IsMetric}, running={Core.Yield?.IsCalRunActive}, "
                + $"profileId={Core.ActiveProfileId}, cropId={Core.ActiveCropId}, "
                + $"runProfileId={Core.Yield?.CalRunProfileId}, runCropId={Core.Yield?.CalRunCropId}, "
                + $"interrupted={Core.Yield?.CalRunInterrupted}";

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

            // A run can now be applied long after it was taken, so the crop and
            // profile may have moved on in between. ComputeNewFactor scales the
            // CURRENT YieldFactor and the weight was converted with the CURRENT test
            // weight, and Save writes to the CURRENT profile+crop — so on a
            // mismatch every one of the three is the wrong reference. Blocked rather
            // than warned: unlike a high baseline there is no reading of this where
            // the result is correct.
            int runProfile = Core.Yield.CalRunProfileId;
            int runCrop    = Core.Yield.CalRunCropId;
            if (runProfile > 0 && runCrop > 0 &&
                (runProfile != Core.ActiveProfileId || runCrop != Core.ActiveCropId))
            {
                Props.WriteErrorLog(diag + " -> ABORT (profile/crop changed since the run)");
                Props.ShowMessage(Lang.lgCalRunWrongCrop, "", 4000, true);
                return;
            }

            // Interrupted runs are short by whatever was harvested after the last
            // autosave, which inflates the factor. Warned, not blocked — the operator
            // may know the outage happened with the header out of the crop.
            if (Core.Yield.CalRunInterrupted)
            {
                using var dlg = new frmMsgBox(Lang.lgCalRunInterruptedPrompt, Lang.lgCalRunInterruptedTitle);
                dlg.ShowDialog(this);
                if (!dlg.Result)
                {
                    Props.WriteErrorLog(diag + " -> ABORT (operator declined interrupted run)");
                    return;
                }
            }

            double newFactor = Core.Yield.ComputeNewFactor(actualBushels);
            Props.WriteErrorLog(diag + $" -> newFactor={newFactor:F4} "
                + $"(ratio={actualBushels / calRunBushels:F4})");

            // Clamp to valid range and stage in the field — nothing takes
            // effect (Core.Yield, the database) until Save is pressed.
            decimal clamped = (decimal)System.Math.Min((double)numFactor.Maximum,
                               System.Math.Max((double)numFactor.Minimum, newFactor));
            numFactor.Value = clamped;

            // After the assignment, so the ValueChanged it raises does not undo it.
            // The run is not discarded here — it is discarded at Save, once
            // the factor it produced has actually reached Core.Yield and the
            // database. Clearing at this point would throw the measurement away for
            // an operator who looked at the new figure and closed the form.
            _factorFromCalRun = true;

            Props.ShowMessage(Lang.lgPendingSave);
        }

        private void CalTimer_Tick(object sender, EventArgs e)
        {
            UpdateCalMeasuredLabel();
        }

        private void NoiseTimer_Tick(object sender, EventArgs e)
        {
            // The job total climbs while the combine works, so the Whole Field tab
            // has to track it. It rides this timer rather than _calTimer because
            // _calTimer only runs during a calibration run, and the field figures
            // must update whether or not one is going. Reads in-memory collector
            // properties only — no database work at this rate.
            if (tabCal.SelectedTab == tabFieldCal) UpdateFieldCalLabels();

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

            // Second figure "G:n/s" is the module's period-gate reject rate — leading
            // edges too early to be a paddle, i.e. grain bridging the inter-paddle gap
            // caught before it could split a cycle. Unlike Noise this is not an
            // electrical fault: a low steady rate rising with flow is the gate doing
            // its job. Hidden entirely when the module firmware predates the field.
            lblNoise.Text = Lang.lgNoise + " " + perSec + "/s"
                + (Core.LastGateRejects >= 0 ? "  G:" + Core.LastGateRejects + "/s" : "");
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

        /// <summary>The measured total on its own, in the display unit.</summary>
        private string CalRunMeasuredText()
        {
            double bushels = Core.Yield?.CalRunBushels ?? 0;
            if (Props.IsMetric)
                return string.Format(Lang.lgMeasuredMetric, Props.DisplayMass(bushels));

            double lbs = bushels * (Core.Yield?.TestWeightLbsBu ?? 60.0);
            return string.Format(Lang.lgMeasuredImperial, bushels, lbs);
        }

        /// <summary>
        /// Local date and time the standing total belongs to — start time while a run
        /// is going, otherwise the time it was stopped. Blank when there is no run.
        /// </summary>
        private string CalRunWhenText()
        {
            var y = Core.Yield;
            if (y == null) return "";

            DateTime? utc = (y.IsCalRunActive ? y.CalRunStartedUtc : y.CalRunStoppedUtc)
                            ?? y.CalRunStartedUtc;
            return utc.HasValue ? utc.Value.ToLocalTime().ToString("g") : "";
        }

        private void UpdateCalMeasuredLabel()
        {
            var y = Core.Yield;

            // No run at all — after a clear, or before the first one. Back to the
            // placeholder rather than a measured "0.00 bu", which reads like a run
            // that recorded nothing.
            if (y == null || (!y.IsCalRunActive && y.CalRunBushels <= 0 && !y.CalRunStartedUtc.HasValue))
            {
                lblCalMeasured.Text      = Lang.lgMeasuredBlank;
                lblCalMeasured.ForeColor = Properties.Settings.Default.MainForeColour;
                return;
            }

            string text = CalRunMeasuredText();
            string when = CalRunWhenText();

            // The run survives a restart now, so a total on screen can be from days
            // ago. Stamping it is what lets the operator tell a run they are still
            // driving to the scale from one they already forgot about.
            if (when.Length > 0)
            {
                text += "  " + string.Format(
                    (y?.IsCalRunActive ?? false) ? Lang.lgCalRunSince : Lang.lgCalRunAt, when);
            }

            if (y?.CalRunInterrupted ?? false)
            {
                // Grain harvested between the last autosave and the shutdown is
                // missing from this total but will be on the ticket, so the factor
                // it fits would read high.
                text += "  " + Lang.lgCalRunInterrupted;
                lblCalMeasured.ForeColor = Color.Orange;
            }
            else
            {
                lblCalMeasured.ForeColor = Properties.Settings.Default.MainForeColour;
            }

            lblCalMeasured.Text = text;
        }

        // ── Whole Field calibration ──────────────────────────────────────────
        //
        // Same arithmetic as a calibration run, sourced from the active job's
        // running total instead of a dedicated pass. It is not an end-of-field
        // operation: the job total accumulates continuously, so this can be fitted
        // whenever grain has crossed a scale — after the first few loads, part way
        // through, or when the field is finished. Each application supersedes the
        // last, because Recalculate rewrites every recorded point to the factor
        // being saved, leaving the job wholly at that factor rather than part at
        // one and part at another.
        //
        // The weight entered is therefore CUMULATIVE — everything hauled off this
        // job so far, not the latest load. "Total harvested" on the label is doing
        // real work; an incremental figure would fit a factor several times too
        // small and the operator would have no way to tell from the result.

        /// <summary>Live totals for the active job, or nulls when there is no job.</summary>
        private (int jobId, string name, double acres, double bushels) ActiveJobTotals()
        {
            var c = Core.Collector;
            if (c == null || c.ActiveJobId <= 0) return (-1, "", 0, 0);
            return (c.ActiveJobId, c.ActiveJobName, c.TotalAcres, c.TotalBushels);
        }

        // Resolving the crop costs two table reads, and UpdateFieldCalLabels runs
        // twice a second off the noise timer. Cached against both the job and the
        // active crop, so the reads happen only when one of them actually changes.
        private int _cropCacheJobId = -1;
        private int _cropCacheActiveCropId = -2;
        private string _cropCacheName = "";
        private bool _cropCacheMismatch;

        /// <summary>
        /// The crop the active job was started under — deliberately the job's crop
        /// and not the active one, because the recorded total belongs to it and it
        /// is what Save writes the factor against. Reports a mismatch so a
        /// mid-job crop change is visible here rather than only as a refusal after
        /// the operator has already typed a weight in.
        /// </summary>
        private (string name, bool mismatch) ActiveJobCrop(int jobId)
        {
            if (jobId == _cropCacheJobId && Core.ActiveCropId == _cropCacheActiveCropId)
                return (_cropCacheName, _cropCacheMismatch);

            _cropCacheJobId        = jobId;
            _cropCacheActiveCropId = Core.ActiveCropId;
            _cropCacheName         = "";
            _cropCacheMismatch     = false;

            try
            {
                int jobCropId = -1;
                foreach (var j in Core.Database.Jobs.GetAll())
                    if (j.id == jobId) { jobCropId = j.cropId; break; }

                foreach (var c in Core.Database.Crops.GetAll())
                    if (c.id == jobCropId) { _cropCacheName = c.name; break; }

                _cropCacheMismatch = (jobCropId > 0 && jobCropId != Core.ActiveCropId);
            }
            catch { }

            return (_cropCacheName, _cropCacheMismatch);
        }

        private void UpdateFieldCalLabels()
        {
            var job = ActiveJobTotals();

            if (job.jobId <= 0)
            {
                lblFieldJob.Text        = Lang.lgFieldNoJob;
                lblFieldMeasured.Text   = Lang.lgMeasuredBlank;
                lblFieldImplied.Text    = "";
                numFieldWeight.Enabled  = false;
                btnApplyField.Enabled   = false;
                return;
            }

            numFieldWeight.Enabled = true;
            btnApplyField.Enabled  = true;

            var crop = ActiveJobCrop(job.jobId);
            string cropName = string.IsNullOrEmpty(crop.name) ? "--" : crop.name;
            lblFieldJob.Text = string.Format(Lang.lgFieldJob, job.name, cropName);

            // Flagged here, not just refused at Apply: the operator can see the
            // problem before weighing anything.
            if (crop.mismatch)
            {
                lblFieldJob.Text += "  — " + Lang.lgFieldCropMismatch;
                lblFieldJob.ForeColor = Color.Orange;
            }
            else
            {
                lblFieldJob.ForeColor = Properties.Settings.Default.MainForeColour;
            }

            if (job.bushels <= 0)
            {
                lblFieldMeasured.Text = Lang.lgFieldNoData;
                lblFieldImplied.Text  = "";
                return;
            }

            double avg = job.acres > 0.001 ? job.bushels / job.acres : 0;
            lblFieldMeasured.Text = Props.IsMetric
                ? string.Format(Lang.lgFieldRecordedMetric,
                      Props.DisplayMass(job.bushels), Props.DisplayArea(job.acres), Props.DisplayRate(avg))
                : string.Format(Lang.lgFieldRecordedImperial,
                      job.bushels, Props.DisplayArea(job.acres), Props.DisplayRate(avg));

            UpdateFieldImplied();
        }

        /// <summary>
        /// The yield the entered weight implies over the acres the job actually
        /// recorded. This is the check that matters: the failure that will bite is
        /// unrecorded ground — an auto-pause or sensor-fault gap means acres crossed
        /// the scale that the app never counted, so its total reads short and the
        /// fitted factor comes out high. A percentage change hides that; a yield in
        /// bu/ac does not, because the operator knows what the field can do.
        /// </summary>
        private void UpdateFieldImplied()
        {
            var job = ActiveJobTotals();
            double entered = (double)numFieldWeight.Value;

            if (job.jobId <= 0 || job.acres <= 0.001 || entered <= 0)
            {
                lblFieldImplied.Text = "";
                return;
            }

            double impliedBuAc = DisplayMassToInternalBushels(entered) / job.acres;
            lblFieldImplied.Text = string.Format(Lang.lgFieldImplied,
                Props.DisplayRate(impliedBuAc), Props.RateUnit);

            // Orange once the entered weight implies a yield more than half again
            // the recorded one — the shape a job with missing acres makes.
            double recordedBuAc = job.bushels / job.acres;
            lblFieldImplied.ForeColor = (recordedBuAc > 0 && impliedBuAc > recordedBuAc * 1.5)
                ? Color.Orange
                : Color.Silver;
        }

        private void btnApplyField_Click(object sender, EventArgs e)
        {
            var job = ActiveJobTotals();
            double enteredDisplay = (double)numFieldWeight.Value;
            double actualBushels  = DisplayMassToInternalBushels(enteredDisplay);
            double oldFactor      = Core.Yield?.YieldFactor ?? 0;

            string diag = "ApplyFieldCal: "
                + $"entered={enteredDisplay:F1} {Props.EntryMassUnit}, "
                + $"actualBushels={actualBushels:F4}, "
                + $"jobId={job.jobId}, jobAcres={job.acres:F3}, jobBushels={job.bushels:F4}, "
                + $"oldFactor={oldFactor:F4}, "
                + $"TWlbsYield={Core.Yield?.TestWeightLbsBu ?? 0:F4}, isMetric={Props.IsMetric}, "
                + $"profileId={Core.ActiveProfileId}, cropId={Core.ActiveCropId}";

            if (job.jobId <= 0)
            {
                Props.WriteErrorLog(diag + " -> ABORT (no active job)");
                Props.ShowMessage(Lang.lgFieldNoJob, "", 2500, true);
                return;
            }
            if (actualBushels <= 0)
            {
                Props.WriteErrorLog(diag + " -> ABORT (entered weight <= 0)");
                Props.ShowMessage(Lang.lgEnterWeighedAmt, "", 2000, true);
                return;
            }
            if (job.bushels <= 0)
            {
                Props.WriteErrorLog(diag + " -> ABORT (job has no recorded total)");
                Props.ShowMessage(Lang.lgFieldNoData, "", 2500, true);
                return;
            }

            // The job carries the crop and profile it was started under. Changing
            // either mid-job leaves the recorded total fitted against one reference
            // and the save heading for another — the same mismatch the cal-run path
            // blocks, reached by a different route.
            foreach (var j in Core.Database.Jobs.GetAll())
            {
                if (j.id != job.jobId) continue;
                if (j.profileId != Core.ActiveProfileId || j.cropId != Core.ActiveCropId)
                {
                    Props.WriteErrorLog(diag + $" -> ABORT (job profile/crop {j.profileId}/{j.cropId} "
                        + "differs from active)");
                    Props.ShowMessage(Lang.lgFieldWrongCrop, "", 4000, true);
                    return;
                }
                break;
            }

            double newFactor = oldFactor * (actualBushels / job.bushels);
            Props.WriteErrorLog(diag + $" -> newFactor={newFactor:F4} "
                + $"(ratio={actualBushels / job.bushels:F4})");

            decimal clamped = (decimal)System.Math.Min((double)numFactor.Maximum,
                               System.Math.Max((double)numFactor.Minimum, newFactor));
            numFactor.Value = clamped;

            // After the assignment, for the same reason as the cal-run path.
            _factorFromFieldCal = true;

            Props.ShowMessage(Lang.lgPendingSave);
        }

        /// <summary>
        /// Rewrites the job's recorded points to the saved factor so its map and
        /// total agree with the weight that produced it. Offered rather than done:
        /// it rewrites every row of the job, and the map screen's own Recalculate
        /// prompts for the same reason.
        /// </summary>
        private void OfferJobRecalculate()
        {
            var job = ActiveJobTotals();
            if (job.jobId <= 0) return;

            using (var dlg = new frmMsgBox(Lang.lgFieldRecalcPrompt, Lang.lgFieldRecalcTitle))
            {
                dlg.ShowDialog(this);
                if (!dlg.Result) return;
            }

            // The collector writes the job row once a minute, so its stored
            // total_volume can be up to a minute stale. RecalculateJob scales that
            // stored figure, so flush the live totals first or the rescale lands
            // on a number the operator never saw.
            Core.Database.Jobs.UpdateTotals(job.jobId, job.acres, job.bushels);

            double headerWidthM = Core.Yield.HeaderWidthM;
            var (rows, newTotal) = Core.Database.YieldData.RecalculateJob(
                job.jobId, Core.Yield.SensorBaseline, Core.Yield.YieldFactor,
                headerWidthM, Core.Yield.TestWeightLbsBu);

            if (rows > 0) Core.Collector?.SyncTotalBushels(job.jobId, newTotal);

            Props.WriteErrorLog($"ApplyFieldCal recalc: jobId={job.jobId} rows={rows} "
                + $"newTotal={newTotal:F4} factor={Core.Yield.YieldFactor:F4}");

            UpdateFieldCalLabels();
            Props.ShowMessage(string.Format(Lang.lgFieldRecalcDone, rows));
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
            // Already the internal unit — no conversion, and none of its rounding.
            if (Props.EntryInBushels) return value;
            return twLbs > 0 ? value / twLbs : 0;
        }

        /// <summary>Inverse of the above, for restating a typed amount in a new unit.</summary>
        private double InternalBushelsToDisplayMass(double bushels)
        {
            double twLbs = Core.Yield?.TestWeightLbsBu ?? 60.0;
            if (Props.IsMetric) return bushels * twLbs * 0.453592;
            if (Props.EntryInBushels) return bushels;
            return bushels * twLbs;
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
            UpdateBaselineWarning();

            // Staged only — nothing reaches Core.Yield or the database until
            // Save is pressed. A Calibration Run started before that
            // still measures against the previous baseline.
            Props.ShowMessage(Lang.lgPendingSave);
        }

        private void frmMenuCalibrate_Shown(object sender, EventArgs e)
        {
            NumpadHelper.Wire(this, numDelay,    0,      60,     0, "Processing Delay (s)");
            NumpadHelper.Wire(this, numBaseline, 0,      0.99,   3, "Sensor Baseline");
            NumpadHelper.Wire(this, numFactor,   0.01,   100,    2, "Yield Factor");
            // Both weight boxes resolve their bounds at each press: the unit toggle
            // changes Maximum and DecimalPlaces while the form is open, so anything
            // captured here would go stale the first time it is used.
            NumpadHelper.WireClickOnly(this, numActualWeight, () => (
                (double)numActualWeight.Minimum, (double)numActualWeight.Maximum,
                numActualWeight.DecimalPlaces, "Actual Weight (" + Props.EntryMassUnit + ")"));

            // Click-only, like numActualWeight: this box is enabled and disabled as
            // the active job comes and goes, so focus can land on it programmatically
            // and Enter-wiring would pop the pad unasked.
            NumpadHelper.WireClickOnly(this, numFieldWeight, () => (
                (double)numFieldWeight.Minimum, (double)numFieldWeight.Maximum,
                numFieldWeight.DecimalPlaces, "Total harvested (" + Props.EntryMassUnit + ")"));

            btnSaveCal.Focus();
        }

        private void btnCalClose_Click(object sender, EventArgs e)    => this.Close();

        private void lblCalSaved_Click(object sender, EventArgs e)
        {

        }
    }
}
