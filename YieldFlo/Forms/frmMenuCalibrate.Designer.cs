namespace YieldFlo.Forms
{
    partial class frmMenuCalibrate
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle         = new System.Windows.Forms.Panel();
            this.lblTitle         = new System.Windows.Forms.Label();
            this.btnTitleClose    = new System.Windows.Forms.Button();
            this.pnlContent       = new System.Windows.Forms.Panel();
            this.lblDelay         = new System.Windows.Forms.Label();
            this.numDelay         = new System.Windows.Forms.NumericUpDown();
            this.lblDelayUnit     = new System.Windows.Forms.Label();
            this.lblBaseline      = new System.Windows.Forms.Label();
            this.numBaseline      = new System.Windows.Forms.NumericUpDown();
            this.lblBaselineNote  = new System.Windows.Forms.Label();
            this.btnSetBaseline   = new System.Windows.Forms.Button();
            this.lblFactor        = new System.Windows.Forms.Label();
            this.numFactor        = new System.Windows.Forms.NumericUpDown();
            this.lblFactorNote    = new System.Windows.Forms.Label();
            this.btnSaveCal       = new System.Windows.Forms.Button();
            this.btnCalClose      = new System.Windows.Forms.Button();
            this.lblHint          = new System.Windows.Forms.Label();
            this.lblCalSep        = new System.Windows.Forms.Label();
            this.btnStartCal      = new System.Windows.Forms.Button();
            this.btnStopCal       = new System.Windows.Forms.Button();
            this.lblCalMeasured   = new System.Windows.Forms.Label();
            this.lblActualWeight  = new System.Windows.Forms.Label();
            this.numActualWeight  = new System.Windows.Forms.NumericUpDown();
            this.lblActualUnit    = new System.Windows.Forms.Label();
            this.btnApplyFactor   = new System.Windows.Forms.Button();
            this.lblCalResult     = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = "Calibrate";
            this.lblTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.AutoSize  = false;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.btnTitleClose.Text      = "×";
            this.btnTitleClose.Font      = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.btnTitleClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTitleClose.FlatAppearance.BorderSize = 0;
            this.btnTitleClose.Size     = new System.Drawing.Size(36, 30);
            this.btnTitleClose.Location = new System.Drawing.Point(418, 5);
            this.btnTitleClose.Click   += new System.EventHandler(this.btnTitleClose_Click);

            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Controls.Add(this.btnTitleClose);

            // ── Content ───────────────────────────────────────────────────────
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;

            var lf = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            var vf = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            var sf = new System.Drawing.Font("Microsoft Sans Serif", 9F);

            // Processing Delay
            this.lblDelay.Text = "Processing Delay:"; this.lblDelay.Font = lf;
            this.lblDelay.Location = new System.Drawing.Point(8, 19); this.lblDelay.AutoSize = false; this.lblDelay.Width = 160;
            this.numDelay.Font = vf; this.numDelay.Location = new System.Drawing.Point(174, 16);
            this.numDelay.Width = 70; this.numDelay.Minimum = 0; this.numDelay.Maximum = 60; this.numDelay.Value = 10;
            this.lblDelayUnit.Text = "seconds"; this.lblDelayUnit.Font = vf;
            this.lblDelayUnit.Location = new System.Drawing.Point(252, 19); this.lblDelayUnit.AutoSize = true;

            // Sensor Baseline
            this.lblBaseline.Text = "Sensor Baseline:"; this.lblBaseline.Font = lf;
            this.lblBaseline.Location = new System.Drawing.Point(8, 59); this.lblBaseline.AutoSize = false; this.lblBaseline.Width = 160;
            this.numBaseline.Font = vf; this.numBaseline.Location = new System.Drawing.Point(174, 56);
            this.numBaseline.Width = 80; this.numBaseline.Minimum = 0; this.numBaseline.Maximum = (decimal)0.99;
            this.numBaseline.DecimalPlaces = 3; this.numBaseline.Increment = (decimal)0.001; this.numBaseline.Value = 0;
            this.lblBaselineNote.Text = "obstruction ratio with no grain (0–1)";
            this.lblBaselineNote.Font = sf; this.lblBaselineNote.ForeColor = System.Drawing.Color.Silver;
            this.lblBaselineNote.Location = new System.Drawing.Point(8, 80); this.lblBaselineNote.AutoSize = true;

            this.btnSetBaseline.Text      = "Set Baseline";
            this.btnSetBaseline.Font      = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.btnSetBaseline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetBaseline.Size      = new System.Drawing.Size(100, 26);
            this.btnSetBaseline.Location  = new System.Drawing.Point(262, 54);
            this.btnSetBaseline.Click    += new System.EventHandler(this.btnSetBaseline_Click);

            // Yield Factor
            this.lblFactor.Text = "Yield Factor:"; this.lblFactor.Font = lf;
            this.lblFactor.Location = new System.Drawing.Point(8, 104); this.lblFactor.AutoSize = false; this.lblFactor.Width = 160;
            this.numFactor.Font = vf; this.numFactor.Location = new System.Drawing.Point(174, 101);
            this.numFactor.Width = 80; this.numFactor.Minimum = (decimal)0.01; this.numFactor.Maximum = 100;
            this.numFactor.DecimalPlaces = 2; this.numFactor.Increment = (decimal)0.01; this.numFactor.Value = 1;
            this.lblFactorNote.Text = "calibration multiplier — increase to raise yield readings";
            this.lblFactorNote.Font = sf; this.lblFactorNote.ForeColor = System.Drawing.Color.Silver;
            this.lblFactorNote.Location = new System.Drawing.Point(8, 125); this.lblFactorNote.AutoSize = true;

            // Save & Apply / Close
            this.btnSaveCal.Text      = "Save & Apply";
            this.btnSaveCal.Font      = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCal.Size      = new System.Drawing.Size(160, 36);
            this.btnSaveCal.Location  = new System.Drawing.Point(8, 152);
            this.btnSaveCal.Click    += new System.EventHandler(this.btnSaveCal_Click);

            this.btnCalClose.Text      = "Close";
            this.btnCalClose.Font      = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnCalClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalClose.Size      = new System.Drawing.Size(100, 36);
            this.btnCalClose.Location  = new System.Drawing.Point(346, 152);
            this.btnCalClose.Click    += new System.EventHandler(this.btnCalClose_Click);

            this.lblHint.Text = "Tip: run empty elevator for 10s to find baseline sensor reading.";
            this.lblHint.Font = sf; this.lblHint.ForeColor = System.Drawing.Color.Silver;
            this.lblHint.Location = new System.Drawing.Point(8, 196); this.lblHint.AutoSize = true;

            // ── Calibration Run section ───────────────────────────────────────
            this.lblCalSep.Text = "─── Calibration Run ───────────────────────────";
            this.lblCalSep.Font = sf; this.lblCalSep.ForeColor = System.Drawing.Color.FromArgb(120, 140, 160);
            this.lblCalSep.Location = new System.Drawing.Point(8, 222); this.lblCalSep.AutoSize = true;

            this.btnStartCal.Text      = "Start Run";
            this.btnStartCal.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnStartCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartCal.Size      = new System.Drawing.Size(100, 32);
            this.btnStartCal.Location  = new System.Drawing.Point(8, 244);
            this.btnStartCal.Click    += new System.EventHandler(this.btnStartCal_Click);

            this.btnStopCal.Text      = "Stop Run";
            this.btnStopCal.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnStopCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopCal.Size      = new System.Drawing.Size(100, 32);
            this.btnStopCal.Location  = new System.Drawing.Point(116, 244);
            this.btnStopCal.Enabled   = false;
            this.btnStopCal.Click    += new System.EventHandler(this.btnStopCal_Click);

            this.lblCalMeasured.Text     = "Measured: —";
            this.lblCalMeasured.Font     = vf;
            this.lblCalMeasured.Location = new System.Drawing.Point(228, 252); this.lblCalMeasured.AutoSize = true;

            this.lblActualWeight.Text = "Actual weight:"; this.lblActualWeight.Font = lf;
            this.lblActualWeight.Location = new System.Drawing.Point(8, 287); this.lblActualWeight.AutoSize = false; this.lblActualWeight.Width = 160;

            this.numActualWeight.Font = vf; this.numActualWeight.Location = new System.Drawing.Point(174, 284);
            this.numActualWeight.Width = 100; this.numActualWeight.Minimum = 0; this.numActualWeight.Maximum = 999999;
            this.numActualWeight.DecimalPlaces = 1; this.numActualWeight.Increment = 1; this.numActualWeight.Value = 0;

            this.lblActualUnit.Text = "lbs"; this.lblActualUnit.Font = vf;
            this.lblActualUnit.Location = new System.Drawing.Point(282, 287); this.lblActualUnit.AutoSize = true;

            this.btnApplyFactor.Text      = "Apply Corrected Factor";
            this.btnApplyFactor.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnApplyFactor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyFactor.Size      = new System.Drawing.Size(200, 32);
            this.btnApplyFactor.Location  = new System.Drawing.Point(8, 322);
            this.btnApplyFactor.Enabled   = false;
            this.btnApplyFactor.Click    += new System.EventHandler(this.btnApplyFactor_Click);

            this.lblCalResult.Text     = "";
            this.lblCalResult.Font     = vf;
            this.lblCalResult.ForeColor = System.Drawing.Color.FromArgb(100, 200, 100);
            this.lblCalResult.Location = new System.Drawing.Point(220, 330); this.lblCalResult.AutoSize = true;

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblDelay, numDelay, lblDelayUnit,
                lblBaseline, numBaseline, lblBaselineNote, btnSetBaseline,
                lblFactor, numFactor, lblFactorNote,
                btnSaveCal, btnCalClose, lblHint,
                lblCalSep,
                btnStartCal, btnStopCal, lblCalMeasured,
                lblActualWeight, numActualWeight, lblActualUnit,
                btnApplyFactor, lblCalResult });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 410);
            this.MinimumSize     = new System.Drawing.Size(456, 410);
            this.MaximumSize     = new System.Drawing.Size(456, 410);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name            = "frmMenuCalibrate";
            this.Text            = "Calibrate";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuCalibrate_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel         pnlTitle;
        private System.Windows.Forms.Label         lblTitle;
        private System.Windows.Forms.Button        btnTitleClose;
        private System.Windows.Forms.Panel         pnlContent;
        private System.Windows.Forms.Label         lblDelay;
        private System.Windows.Forms.NumericUpDown numDelay;
        private System.Windows.Forms.Label         lblDelayUnit;
        private System.Windows.Forms.Label         lblBaseline;
        private System.Windows.Forms.NumericUpDown numBaseline;
        private System.Windows.Forms.Label         lblBaselineNote;
        private System.Windows.Forms.Button        btnSetBaseline;
        private System.Windows.Forms.Label         lblFactor;
        private System.Windows.Forms.NumericUpDown numFactor;
        private System.Windows.Forms.Label         lblFactorNote;
        private System.Windows.Forms.Button        btnSaveCal;
        private System.Windows.Forms.Button        btnCalClose;
        private System.Windows.Forms.Label         lblHint;
        private System.Windows.Forms.Label         lblCalSep;
        private System.Windows.Forms.Button        btnStartCal;
        private System.Windows.Forms.Button        btnStopCal;
        private System.Windows.Forms.Label         lblCalMeasured;
        private System.Windows.Forms.Label         lblActualWeight;
        private System.Windows.Forms.NumericUpDown numActualWeight;
        private System.Windows.Forms.Label         lblActualUnit;
        private System.Windows.Forms.Button        btnApplyFactor;
        private System.Windows.Forms.Label         lblCalResult;
    }
}
