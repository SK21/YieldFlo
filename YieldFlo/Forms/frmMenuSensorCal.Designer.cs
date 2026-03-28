using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmMenuSensorCal
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle        = new System.Windows.Forms.Panel();
            this.lblTitle        = new System.Windows.Forms.Label();
            this.btnTitleClose   = new System.Windows.Forms.Button();
            this.pnlContent      = new System.Windows.Forms.Panel();
            this.lblCropLbl      = new System.Windows.Forms.Label();
            this.cboCrop         = new System.Windows.Forms.ComboBox();
            this.lblProfileLbl   = new System.Windows.Forms.Label();
            this.cboProfile      = new System.Windows.Forms.ComboBox();
            this.pnlSep1         = new System.Windows.Forms.Panel();
            this.lblMoistSection = new System.Windows.Forms.Label();
            this.lblMoistApp     = new System.Windows.Forms.Label();
            this.lblMoistLive    = new System.Windows.Forms.Label();
            this.lblMoistMeter   = new System.Windows.Forms.Label();
            this.numCalMeter     = new System.Windows.Forms.NumericUpDown();
            this.lblCalMeterUnit = new System.Windows.Forms.Label();
            this.btnApplyMoist   = new System.Windows.Forms.Button();
            this.lblMoistOffset  = new System.Windows.Forms.Label();
            this.numMoistOffset  = new System.Windows.Forms.NumericUpDown();
            this.lblMoistOffUnit = new System.Windows.Forms.Label();
            this.pnlSep2         = new System.Windows.Forms.Panel();
            this.lblTempSection  = new System.Windows.Forms.Label();
            this.lblTempApp      = new System.Windows.Forms.Label();
            this.lblTempLive     = new System.Windows.Forms.Label();
            this.lblTempThermo   = new System.Windows.Forms.Label();
            this.numCalThermo    = new System.Windows.Forms.NumericUpDown();
            this.lblCalThermoUnit = new System.Windows.Forms.Label();
            this.btnApplyTemp    = new System.Windows.Forms.Button();
            this.lblMoistScale    = new System.Windows.Forms.Label();
            this.numMoistScale    = new System.Windows.Forms.NumericUpDown();
            this.lblMoistScaleUnit = new System.Windows.Forms.Label();
            this.lblTempOffset    = new System.Windows.Forms.Label();
            this.numTempOffset    = new System.Windows.Forms.NumericUpDown();
            this.lblTempOffUnit   = new System.Windows.Forms.Label();
            this.lblTempScale     = new System.Windows.Forms.Label();
            this.numTempScale     = new System.Windows.Forms.NumericUpDown();
            this.lblTempScaleUnit = new System.Windows.Forms.Label();
            this.pnlSep3          = new System.Windows.Forms.Panel();
            this.btnSave         = new System.Windows.Forms.Button();
            this.btnSCClose      = new System.Windows.Forms.Button();
            this.tmrLive         = new System.Windows.Forms.Timer();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = Lang.lgTitleMoistureCal;
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

            // Crop selector (y=8)
            this.lblCropLbl.Text = Lang.lgCrop; this.lblCropLbl.Font = lf; this.lblCropLbl.Location = new System.Drawing.Point(8, 11); this.lblCropLbl.AutoSize = false; this.lblCropLbl.Width = 76;
            this.cboCrop.Font = vf; this.cboCrop.Location = new System.Drawing.Point(88, 8); this.cboCrop.Width = 200; this.cboCrop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCrop.SelectedIndexChanged += new System.EventHandler(this.cboCrop_SelectedIndexChanged);
            this.pnlContent.Controls.Add(this.lblCropLbl); this.pnlContent.Controls.Add(this.cboCrop);

            // Profile selector (y=40)
            this.lblProfileLbl.Text = Lang.lgProfile; this.lblProfileLbl.Font = lf; this.lblProfileLbl.Location = new System.Drawing.Point(8, 43); this.lblProfileLbl.AutoSize = false; this.lblProfileLbl.Width = 76;
            this.cboProfile.Font = vf; this.cboProfile.Location = new System.Drawing.Point(88, 40); this.cboProfile.Width = 200; this.cboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfile.SelectedIndexChanged += new System.EventHandler(this.cboProfile_SelectedIndexChanged);
            this.pnlContent.Controls.Add(this.lblProfileLbl); this.pnlContent.Controls.Add(this.cboProfile);

            // Separator 1 (y=72)
            this.pnlSep1.Location = new System.Drawing.Point(4, 72); this.pnlSep1.Size = new System.Drawing.Size(444, 1); this.pnlSep1.BackColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.pnlContent.Controls.Add(this.pnlSep1);

            // Moisture section header (y=78)
            this.lblMoistSection.Text = Lang.lgMoistureSection; this.lblMoistSection.Font = lf; this.lblMoistSection.Location = new System.Drawing.Point(8, 78); this.lblMoistSection.AutoSize = true;
            this.pnlContent.Controls.Add(this.lblMoistSection);

            // Moisture live + meter row (y=100)
            this.lblMoistApp.Text = Lang.lgAppReads; this.lblMoistApp.Font = lf; this.lblMoistApp.Location = new System.Drawing.Point(8, 103); this.lblMoistApp.AutoSize = false; this.lblMoistApp.Width = 80;
            this.lblMoistLive.Text = "—"; this.lblMoistLive.Font = vf; this.lblMoistLive.Location = new System.Drawing.Point(92, 103); this.lblMoistLive.AutoSize = false; this.lblMoistLive.Width = 64;
            this.lblMoistMeter.Text = Lang.lgMeter; this.lblMoistMeter.Font = lf; this.lblMoistMeter.Location = new System.Drawing.Point(164, 103); this.lblMoistMeter.AutoSize = true;
            this.numCalMeter.Font = vf; this.numCalMeter.Location = new System.Drawing.Point(218, 100); this.numCalMeter.Width = 72; this.numCalMeter.Minimum = 0; this.numCalMeter.Maximum = 40; this.numCalMeter.Increment = (decimal)0.1; this.numCalMeter.Value = 0; this.numCalMeter.DecimalPlaces = 1;
            this.lblCalMeterUnit.Text = "%"; this.lblCalMeterUnit.Font = vf; this.lblCalMeterUnit.Location = new System.Drawing.Point(296, 103); this.lblCalMeterUnit.AutoSize = true;
            this.btnApplyMoist.Text = Lang.lgApplyCal; this.btnApplyMoist.Font = lf; this.btnApplyMoist.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnApplyMoist.Location = new System.Drawing.Point(316, 98); this.btnApplyMoist.Size = new System.Drawing.Size(96, 28);
            this.btnApplyMoist.Click += new System.EventHandler(this.btnApplyMoist_Click);
            this.pnlContent.Controls.Add(this.lblMoistApp); this.pnlContent.Controls.Add(this.lblMoistLive);
            this.pnlContent.Controls.Add(this.lblMoistMeter); this.pnlContent.Controls.Add(this.numCalMeter);
            this.pnlContent.Controls.Add(this.lblCalMeterUnit); this.pnlContent.Controls.Add(this.btnApplyMoist);

            // Moisture offset row (y=132)
            this.lblMoistOffset.Text = Lang.lgOffset; this.lblMoistOffset.Font = lf; this.lblMoistOffset.Location = new System.Drawing.Point(8, 135); this.lblMoistOffset.AutoSize = false; this.lblMoistOffset.Width = 80;
            this.numMoistOffset.Font = vf; this.numMoistOffset.Location = new System.Drawing.Point(92, 132); this.numMoistOffset.Width = 80; this.numMoistOffset.Minimum = -25; this.numMoistOffset.Maximum = 25; this.numMoistOffset.Increment = (decimal)0.1; this.numMoistOffset.Value = 0; this.numMoistOffset.DecimalPlaces = 1;
            this.lblMoistOffUnit.Text = Lang.lgPercentSavedToCrop; this.lblMoistOffUnit.Font = vf; this.lblMoistOffUnit.Location = new System.Drawing.Point(180, 135); this.lblMoistOffUnit.AutoSize = true;
            this.pnlContent.Controls.Add(this.lblMoistOffset); this.pnlContent.Controls.Add(this.numMoistOffset); this.pnlContent.Controls.Add(this.lblMoistOffUnit);

            // Moisture scale row (y=164)
            this.lblMoistScale.Text = Lang.lgScale; this.lblMoistScale.Font = lf; this.lblMoistScale.Location = new System.Drawing.Point(8, 167); this.lblMoistScale.AutoSize = false; this.lblMoistScale.Width = 80;
            this.numMoistScale.Font = vf; this.numMoistScale.Location = new System.Drawing.Point(92, 164); this.numMoistScale.Width = 96; this.numMoistScale.Minimum = (decimal)0.0001; this.numMoistScale.Maximum = (decimal)0.01; this.numMoistScale.Increment = (decimal)0.0001; this.numMoistScale.Value = (decimal)0.001; this.numMoistScale.DecimalPlaces = 4;
            this.lblMoistScaleUnit.Text = Lang.lgPercentPerCount; this.lblMoistScaleUnit.Font = vf; this.lblMoistScaleUnit.Location = new System.Drawing.Point(196, 167); this.lblMoistScaleUnit.AutoSize = true;
            this.pnlContent.Controls.Add(this.lblMoistScale); this.pnlContent.Controls.Add(this.numMoistScale); this.pnlContent.Controls.Add(this.lblMoistScaleUnit);

            // Separator 2 (y=204)
            this.pnlSep2.Location = new System.Drawing.Point(4, 204); this.pnlSep2.Size = new System.Drawing.Size(444, 1); this.pnlSep2.BackColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.pnlContent.Controls.Add(this.pnlSep2);

            // Temperature section header (y=210)
            this.lblTempSection.Text = Lang.lgTemperatureSection; this.lblTempSection.Font = lf; this.lblTempSection.Location = new System.Drawing.Point(8, 210); this.lblTempSection.AutoSize = true;
            this.pnlContent.Controls.Add(this.lblTempSection);

            // Temperature live + thermo row (y=232)
            this.lblTempApp.Text = Lang.lgAppReads; this.lblTempApp.Font = lf; this.lblTempApp.Location = new System.Drawing.Point(8, 235); this.lblTempApp.AutoSize = false; this.lblTempApp.Width = 80;
            this.lblTempLive.Text = "—"; this.lblTempLive.Font = vf; this.lblTempLive.Location = new System.Drawing.Point(92, 235); this.lblTempLive.AutoSize = false; this.lblTempLive.Width = 64;
            this.lblTempThermo.Text = Lang.lgMeter; this.lblTempThermo.Font = lf; this.lblTempThermo.Location = new System.Drawing.Point(164, 235); this.lblTempThermo.AutoSize = true;
            this.numCalThermo.Font = vf; this.numCalThermo.Location = new System.Drawing.Point(218, 232); this.numCalThermo.Width = 72; this.numCalThermo.Minimum = -20; this.numCalThermo.Maximum = 60; this.numCalThermo.Increment = (decimal)0.1; this.numCalThermo.Value = 20; this.numCalThermo.DecimalPlaces = 1;
            this.lblCalThermoUnit.Text = "°C"; this.lblCalThermoUnit.Font = vf; this.lblCalThermoUnit.Location = new System.Drawing.Point(296, 235); this.lblCalThermoUnit.AutoSize = true;
            this.btnApplyTemp.Text = Lang.lgApplyCal; this.btnApplyTemp.Font = lf; this.btnApplyTemp.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnApplyTemp.Location = new System.Drawing.Point(316, 230); this.btnApplyTemp.Size = new System.Drawing.Size(96, 28);
            this.btnApplyTemp.Click += new System.EventHandler(this.btnApplyTemp_Click);
            this.pnlContent.Controls.Add(this.lblTempApp); this.pnlContent.Controls.Add(this.lblTempLive);
            this.pnlContent.Controls.Add(this.lblTempThermo); this.pnlContent.Controls.Add(this.numCalThermo);
            this.pnlContent.Controls.Add(this.lblCalThermoUnit); this.pnlContent.Controls.Add(this.btnApplyTemp);

            // Temperature offset row (y=264)
            this.lblTempOffset.Text = Lang.lgOffset; this.lblTempOffset.Font = lf; this.lblTempOffset.Location = new System.Drawing.Point(8, 267); this.lblTempOffset.AutoSize = false; this.lblTempOffset.Width = 80;
            this.numTempOffset.Font = vf; this.numTempOffset.Location = new System.Drawing.Point(92, 264); this.numTempOffset.Width = 80; this.numTempOffset.Minimum = -10; this.numTempOffset.Maximum = 10; this.numTempOffset.Increment = (decimal)0.1; this.numTempOffset.Value = 0; this.numTempOffset.DecimalPlaces = 1;
            this.lblTempOffUnit.Text = Lang.lgDegCSavedToProfile; this.lblTempOffUnit.Font = vf; this.lblTempOffUnit.Location = new System.Drawing.Point(180, 267); this.lblTempOffUnit.AutoSize = true;
            this.pnlContent.Controls.Add(this.lblTempOffset); this.pnlContent.Controls.Add(this.numTempOffset); this.pnlContent.Controls.Add(this.lblTempOffUnit);

            // Temperature scale row (y=300)
            this.lblTempScale.Text = Lang.lgScale; this.lblTempScale.Font = lf; this.lblTempScale.Location = new System.Drawing.Point(8, 303); this.lblTempScale.AutoSize = false; this.lblTempScale.Width = 80;
            this.numTempScale.Font = vf; this.numTempScale.Location = new System.Drawing.Point(92, 300); this.numTempScale.Width = 96; this.numTempScale.Minimum = (decimal)0.001; this.numTempScale.Maximum = (decimal)0.1; this.numTempScale.Increment = (decimal)0.0001; this.numTempScale.Value = (decimal)0.0125; this.numTempScale.DecimalPlaces = 4;
            this.lblTempScaleUnit.Text = Lang.lgDegCPerCount; this.lblTempScaleUnit.Font = vf; this.lblTempScaleUnit.Location = new System.Drawing.Point(196, 303); this.lblTempScaleUnit.AutoSize = true;
            this.pnlContent.Controls.Add(this.lblTempScale); this.pnlContent.Controls.Add(this.numTempScale); this.pnlContent.Controls.Add(this.lblTempScaleUnit);

            // Separator 3 (y=336)
            this.pnlSep3.Location = new System.Drawing.Point(4, 336); this.pnlSep3.Size = new System.Drawing.Size(444, 1); this.pnlSep3.BackColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.pnlContent.Controls.Add(this.pnlSep3);

            // Save / Close buttons (y=344)
            this.btnSave.Text = Lang.lgSave; this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold); this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnSave.Location = new System.Drawing.Point(8, 344); this.btnSave.Size = new System.Drawing.Size(100, 36);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnSCClose.Text = Lang.lgClose; this.btnSCClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold); this.btnSCClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnSCClose.Location = new System.Drawing.Point(348, 344); this.btnSCClose.Size = new System.Drawing.Size(100, 36);
            this.btnSCClose.Click += new System.EventHandler(this.btnSCClose_Click);
            this.pnlContent.Controls.Add(this.btnSave); this.pnlContent.Controls.Add(this.btnSCClose);

            // Timer — refreshes live readings
            this.tmrLive.Interval = 500;
            this.tmrLive.Tick += new System.EventHandler(this.tmrLive_Tick);

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 432);
            this.MinimumSize     = new System.Drawing.Size(456, 432);
            this.MaximumSize     = new System.Drawing.Size(456, 432);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name            = "frmMenuSensorCal";
            this.Text            = "Moisture Cal";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuSensorCal_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel         pnlTitle;
        private System.Windows.Forms.Label         lblTitle;
        private System.Windows.Forms.Button        btnTitleClose;
        private System.Windows.Forms.Panel         pnlContent;
        private System.Windows.Forms.Label         lblCropLbl;
        private System.Windows.Forms.ComboBox      cboCrop;
        private System.Windows.Forms.Label         lblProfileLbl;
        private System.Windows.Forms.ComboBox      cboProfile;
        private System.Windows.Forms.Panel         pnlSep1;
        private System.Windows.Forms.Label         lblMoistSection;
        private System.Windows.Forms.Label         lblMoistApp;
        private System.Windows.Forms.Label         lblMoistLive;
        private System.Windows.Forms.Label         lblMoistMeter;
        private System.Windows.Forms.NumericUpDown numCalMeter;
        private System.Windows.Forms.Label         lblCalMeterUnit;
        private System.Windows.Forms.Button        btnApplyMoist;
        private System.Windows.Forms.Label         lblMoistOffset;
        private System.Windows.Forms.NumericUpDown numMoistOffset;
        private System.Windows.Forms.Label         lblMoistOffUnit;
        private System.Windows.Forms.Label         lblMoistScale;
        private System.Windows.Forms.NumericUpDown numMoistScale;
        private System.Windows.Forms.Label         lblMoistScaleUnit;
        private System.Windows.Forms.Panel         pnlSep2;
        private System.Windows.Forms.Label         lblTempSection;
        private System.Windows.Forms.Label         lblTempApp;
        private System.Windows.Forms.Label         lblTempLive;
        private System.Windows.Forms.Label         lblTempThermo;
        private System.Windows.Forms.NumericUpDown numCalThermo;
        private System.Windows.Forms.Label         lblCalThermoUnit;
        private System.Windows.Forms.Button        btnApplyTemp;
        private System.Windows.Forms.Label         lblTempOffset;
        private System.Windows.Forms.NumericUpDown numTempOffset;
        private System.Windows.Forms.Label         lblTempOffUnit;
        private System.Windows.Forms.Label         lblTempScale;
        private System.Windows.Forms.NumericUpDown numTempScale;
        private System.Windows.Forms.Label         lblTempScaleUnit;
        private System.Windows.Forms.Panel         pnlSep3;
        private System.Windows.Forms.Button        btnSave;
        private System.Windows.Forms.Button        btnSCClose;
        private System.Windows.Forms.Timer         tmrLive;
    }
}
