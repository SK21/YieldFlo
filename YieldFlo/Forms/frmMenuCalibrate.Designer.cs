using YieldFlo.Language;

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
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnTitleClose = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlSep1 = new System.Windows.Forms.Panel();
            this.lblDelay = new System.Windows.Forms.Label();
            this.numDelay = new System.Windows.Forms.NumericUpDown();
            this.lblDelayUnit = new System.Windows.Forms.Label();
            this.lblBaseline = new System.Windows.Forms.Label();
            this.numBaseline = new System.Windows.Forms.NumericUpDown();
            this.lblBaselineNote = new System.Windows.Forms.Label();
            this.btnSetBaseline = new System.Windows.Forms.Button();
            this.lblFactor = new System.Windows.Forms.Label();
            this.numFactor = new System.Windows.Forms.NumericUpDown();
            this.lblFactorNote = new System.Windows.Forms.Label();
            this.btnSaveCal = new System.Windows.Forms.Button();
            this.btnCalClose = new System.Windows.Forms.Button();
            this.lblHint = new System.Windows.Forms.Label();
            this.lblCalSep = new System.Windows.Forms.Label();
            this.btnStartCal = new System.Windows.Forms.Button();
            this.btnStopCal = new System.Windows.Forms.Button();
            this.lblCalMeasured = new System.Windows.Forms.Label();
            this.lblActualWeight = new System.Windows.Forms.Label();
            this.numActualWeight = new System.Windows.Forms.NumericUpDown();
            this.lblActualUnit = new System.Windows.Forms.Label();
            this.btnApplyFactor = new System.Windows.Forms.Button();
            this.lblNoise = new System.Windows.Forms.Label();
            this.lblPaddleHz = new System.Windows.Forms.Label();
            this.lblCalSaved = new System.Windows.Forms.Label();
            this.pnlTitle.SuspendLayout();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBaseline)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numActualWeight)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTitle
            // 
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Controls.Add(this.btnTitleClose);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(2, 2);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(452, 40);
            this.pnlTitle.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(452, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Yield Cal";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnTitleClose
            // 
            this.btnTitleClose.FlatAppearance.BorderSize = 0;
            this.btnTitleClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTitleClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.btnTitleClose.Location = new System.Drawing.Point(418, 5);
            this.btnTitleClose.Name = "btnTitleClose";
            this.btnTitleClose.Size = new System.Drawing.Size(36, 30);
            this.btnTitleClose.TabIndex = 1;
            this.btnTitleClose.Text = "×";
            this.btnTitleClose.Click += new System.EventHandler(this.btnTitleClose_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.panel2);
            this.pnlContent.Controls.Add(this.panel1);
            this.pnlContent.Controls.Add(this.pnlSep1);
            this.pnlContent.Controls.Add(this.lblDelay);
            this.pnlContent.Controls.Add(this.numDelay);
            this.pnlContent.Controls.Add(this.lblDelayUnit);
            this.pnlContent.Controls.Add(this.lblBaseline);
            this.pnlContent.Controls.Add(this.numBaseline);
            this.pnlContent.Controls.Add(this.lblBaselineNote);
            this.pnlContent.Controls.Add(this.btnSetBaseline);
            this.pnlContent.Controls.Add(this.lblFactor);
            this.pnlContent.Controls.Add(this.numFactor);
            this.pnlContent.Controls.Add(this.lblFactorNote);
            this.pnlContent.Controls.Add(this.btnSaveCal);
            this.pnlContent.Controls.Add(this.btnCalClose);
            this.pnlContent.Controls.Add(this.lblHint);
            this.pnlContent.Controls.Add(this.lblCalSep);
            this.pnlContent.Controls.Add(this.btnStartCal);
            this.pnlContent.Controls.Add(this.btnStopCal);
            this.pnlContent.Controls.Add(this.lblCalMeasured);
            this.pnlContent.Controls.Add(this.lblActualWeight);
            this.pnlContent.Controls.Add(this.numActualWeight);
            this.pnlContent.Controls.Add(this.lblActualUnit);
            this.pnlContent.Controls.Add(this.btnApplyFactor);
            this.pnlContent.Controls.Add(this.lblNoise);
            this.pnlContent.Controls.Add(this.lblPaddleHz);
            this.pnlContent.Controls.Add(this.lblCalSaved);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(2, 42);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(452, 519);
            this.pnlContent.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.panel2.Location = new System.Drawing.Point(4, 422);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(444, 1);
            this.panel2.TabIndex = 27;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.panel1.Location = new System.Drawing.Point(4, 213);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(444, 1);
            this.panel1.TabIndex = 26;
            // 
            // pnlSep1
            // 
            this.pnlSep1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.pnlSep1.Location = new System.Drawing.Point(4, 54);
            this.pnlSep1.Name = "pnlSep1";
            this.pnlSep1.Size = new System.Drawing.Size(444, 1);
            this.pnlSep1.TabIndex = 25;
            // 
            // lblDelay
            // 
            this.lblDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblDelay.Location = new System.Drawing.Point(8, 16);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(160, 21);
            this.lblDelay.TabIndex = 0;
            this.lblDelay.Text = "Processing Delay:";
            // 
            // numDelay
            // 
            this.numDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.numDelay.Location = new System.Drawing.Point(174, 16);
            this.numDelay.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numDelay.Name = "numDelay";
            this.numDelay.Size = new System.Drawing.Size(70, 21);
            this.numDelay.TabIndex = 1;
            this.numDelay.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblDelayUnit
            // 
            this.lblDelayUnit.AutoSize = true;
            this.lblDelayUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblDelayUnit.Location = new System.Drawing.Point(252, 19);
            this.lblDelayUnit.Name = "lblDelayUnit";
            this.lblDelayUnit.Size = new System.Drawing.Size(53, 15);
            this.lblDelayUnit.TabIndex = 2;
            this.lblDelayUnit.Text = "seconds";
            // 
            // lblBaseline
            // 
            this.lblBaseline.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblBaseline.Location = new System.Drawing.Point(8, 74);
            this.lblBaseline.Name = "lblBaseline";
            this.lblBaseline.Size = new System.Drawing.Size(160, 21);
            this.lblBaseline.TabIndex = 3;
            this.lblBaseline.Text = "Sensor Baseline:";
            // 
            // numBaseline
            // 
            this.numBaseline.DecimalPlaces = 3;
            this.numBaseline.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.numBaseline.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numBaseline.Location = new System.Drawing.Point(174, 74);
            this.numBaseline.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            131072});
            this.numBaseline.Name = "numBaseline";
            this.numBaseline.Size = new System.Drawing.Size(80, 21);
            this.numBaseline.TabIndex = 4;
            // 
            // lblBaselineNote
            // 
            this.lblBaselineNote.AutoSize = true;
            this.lblBaselineNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblBaselineNote.ForeColor = System.Drawing.Color.Silver;
            this.lblBaselineNote.Location = new System.Drawing.Point(8, 98);
            this.lblBaselineNote.Name = "lblBaselineNote";
            this.lblBaselineNote.Size = new System.Drawing.Size(196, 15);
            this.lblBaselineNote.TabIndex = 5;
            this.lblBaselineNote.Text = "obstruction ratio with no grain (0–1)";
            // 
            // btnSetBaseline
            // 
            this.btnSetBaseline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetBaseline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.btnSetBaseline.Location = new System.Drawing.Point(262, 70);
            this.btnSetBaseline.Name = "btnSetBaseline";
            this.btnSetBaseline.Size = new System.Drawing.Size(100, 28);
            this.btnSetBaseline.TabIndex = 6;
            this.btnSetBaseline.Text = global::YieldFlo.Language.Lang.lgSetBaseline;
            this.btnSetBaseline.Click += new System.EventHandler(this.btnSetBaseline_Click);
            // 
            // lblFactor
            // 
            this.lblFactor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblFactor.Location = new System.Drawing.Point(8, 235);
            this.lblFactor.Name = "lblFactor";
            this.lblFactor.Size = new System.Drawing.Size(160, 21);
            this.lblFactor.TabIndex = 7;
            this.lblFactor.Text = "Yield Factor:";
            // 
            // numFactor
            // 
            this.numFactor.DecimalPlaces = 2;
            this.numFactor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.numFactor.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numFactor.Location = new System.Drawing.Point(174, 235);
            this.numFactor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numFactor.Name = "numFactor";
            this.numFactor.Size = new System.Drawing.Size(80, 21);
            this.numFactor.TabIndex = 8;
            this.numFactor.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblFactorNote
            // 
            this.lblFactorNote.AutoSize = true;
            this.lblFactorNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblFactorNote.ForeColor = System.Drawing.Color.Silver;
            this.lblFactorNote.Location = new System.Drawing.Point(8, 259);
            this.lblFactorNote.Name = "lblFactorNote";
            this.lblFactorNote.Size = new System.Drawing.Size(300, 15);
            this.lblFactorNote.TabIndex = 9;
            this.lblFactorNote.Text = "calibration multiplier — increase to raise yield readings";
            // 
            // btnSaveCal
            // 
            this.btnSaveCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveCal.Location = new System.Drawing.Point(8, 473);
            this.btnSaveCal.Name = "btnSaveCal";
            this.btnSaveCal.Size = new System.Drawing.Size(160, 36);
            this.btnSaveCal.TabIndex = 10;
            this.btnSaveCal.Text = global::YieldFlo.Language.Lang.lgSaveApply;
            this.btnSaveCal.Click += new System.EventHandler(this.btnSaveCal_Click);
            // 
            // btnCalClose
            // 
            this.btnCalClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnCalClose.Location = new System.Drawing.Point(346, 473);
            this.btnCalClose.Name = "btnCalClose";
            this.btnCalClose.Size = new System.Drawing.Size(100, 36);
            this.btnCalClose.TabIndex = 11;
            this.btnCalClose.Text = global::YieldFlo.Language.Lang.lgClose;
            this.btnCalClose.Click += new System.EventHandler(this.btnCalClose_Click);
            // 
            // lblHint
            // 
            this.lblHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblHint.ForeColor = System.Drawing.Color.Silver;
            this.lblHint.Location = new System.Drawing.Point(8, 160);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(354, 32);
            this.lblHint.TabIndex = 12;
            this.lblHint.Text = "Tip: Run the elevator empty for a few seconds, then press Set Baseline (5 s sampl" +
    "e).\r\n";
            // 
            // lblCalSep
            // 
            this.lblCalSep.AutoSize = true;
            this.lblCalSep.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblCalSep.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(140)))), ((int)(((byte)(160)))));
            this.lblCalSep.Location = new System.Drawing.Point(152, 298);
            this.lblCalSep.Name = "lblCalSep";
            this.lblCalSep.Size = new System.Drawing.Size(152, 15);
            this.lblCalSep.TabIndex = 13;
            this.lblCalSep.Text = "─── Calibration Run ───\r\n";
            // 
            // btnStartCal
            // 
            this.btnStartCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnStartCal.Location = new System.Drawing.Point(66, 326);
            this.btnStartCal.Name = "btnStartCal";
            this.btnStartCal.Size = new System.Drawing.Size(100, 32);
            this.btnStartCal.TabIndex = 14;
            this.btnStartCal.Text = global::YieldFlo.Language.Lang.lgStartRun;
            this.btnStartCal.Click += new System.EventHandler(this.btnStartCal_Click);
            // 
            // btnStopCal
            // 
            this.btnStopCal.Enabled = false;
            this.btnStopCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnStopCal.Location = new System.Drawing.Point(174, 326);
            this.btnStopCal.Name = "btnStopCal";
            this.btnStopCal.Size = new System.Drawing.Size(100, 32);
            this.btnStopCal.TabIndex = 15;
            this.btnStopCal.Text = global::YieldFlo.Language.Lang.lgStopRun;
            this.btnStopCal.Click += new System.EventHandler(this.btnStopCal_Click);
            // 
            // lblCalMeasured
            // 
            this.lblCalMeasured.AutoSize = true;
            this.lblCalMeasured.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblCalMeasured.Location = new System.Drawing.Point(286, 334);
            this.lblCalMeasured.Name = "lblCalMeasured";
            this.lblCalMeasured.Size = new System.Drawing.Size(76, 15);
            this.lblCalMeasured.TabIndex = 16;
            this.lblCalMeasured.Text = "Measured: —";
            // 
            // lblActualWeight
            // 
            this.lblActualWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblActualWeight.Location = new System.Drawing.Point(8, 375);
            this.lblActualWeight.Name = "lblActualWeight";
            this.lblActualWeight.Size = new System.Drawing.Size(160, 21);
            this.lblActualWeight.TabIndex = 17;
            this.lblActualWeight.Text = "Actual weight:";
            // 
            // numActualWeight
            // 
            this.numActualWeight.DecimalPlaces = 1;
            this.numActualWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.numActualWeight.Location = new System.Drawing.Point(174, 375);
            this.numActualWeight.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numActualWeight.Name = "numActualWeight";
            this.numActualWeight.Size = new System.Drawing.Size(100, 21);
            this.numActualWeight.TabIndex = 18;
            // 
            // lblActualUnit
            // 
            this.lblActualUnit.AutoSize = true;
            this.lblActualUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblActualUnit.Location = new System.Drawing.Point(282, 378);
            this.lblActualUnit.Name = "lblActualUnit";
            this.lblActualUnit.Size = new System.Drawing.Size(23, 15);
            this.lblActualUnit.TabIndex = 19;
            this.lblActualUnit.Text = "lbs";
            // 
            // btnApplyFactor
            // 
            this.btnApplyFactor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyFactor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnApplyFactor.Location = new System.Drawing.Point(328, 371);
            this.btnApplyFactor.Name = "btnApplyFactor";
            this.btnApplyFactor.Size = new System.Drawing.Size(96, 28);
            this.btnApplyFactor.TabIndex = 20;
            this.btnApplyFactor.Text = global::YieldFlo.Language.Lang.lgApplyCal;
            this.btnApplyFactor.Click += new System.EventHandler(this.btnApplyFactor_Click);
            // 
            // lblNoise
            // 
            this.lblNoise.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblNoise.ForeColor = System.Drawing.Color.Silver;
            this.lblNoise.Location = new System.Drawing.Point(262, 99);
            this.lblNoise.Name = "lblNoise";
            this.lblNoise.Size = new System.Drawing.Size(103, 23);
            this.lblNoise.TabIndex = 22;
            this.lblNoise.Text = "Noise";
            this.lblNoise.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPaddleHz
            // 
            this.lblPaddleHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPaddleHz.ForeColor = System.Drawing.Color.Silver;
            this.lblPaddleHz.Location = new System.Drawing.Point(262, 123);
            this.lblPaddleHz.Name = "lblPaddleHz";
            this.lblPaddleHz.Size = new System.Drawing.Size(184, 23);
            this.lblPaddleHz.TabIndex = 23;
            this.lblPaddleHz.Text = "Paddles";
            this.lblPaddleHz.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCalSaved
            // 
            this.lblCalSaved.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblCalSaved.ForeColor = System.Drawing.Color.Silver;
            this.lblCalSaved.Location = new System.Drawing.Point(9, 439);
            this.lblCalSaved.Name = "lblCalSaved";
            this.lblCalSaved.Size = new System.Drawing.Size(438, 15);
            this.lblCalSaved.TabIndex = 24;
            this.lblCalSaved.Text = "---";
            this.lblCalSaved.Click += new System.EventHandler(this.lblCalSaved_Click);
            // 
            // frmMenuCalibrate
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(456, 563);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuCalibrate";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Yield Cal";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmMenuCalibrate_Load);
            this.pnlTitle.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBaseline)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numActualWeight)).EndInit();
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
        private System.Windows.Forms.Label         lblNoise;
        private System.Windows.Forms.Label         lblPaddleHz;
        private System.Windows.Forms.Label         lblCalSaved;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlSep1;
        private System.Windows.Forms.Panel panel2;
    }
}
