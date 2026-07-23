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
            this.components = new System.ComponentModel.Container();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblCropLbl = new System.Windows.Forms.Label();
            this.cboCrop = new System.Windows.Forms.ComboBox();
            this.lblProfileLbl = new System.Windows.Forms.Label();
            this.cboProfile = new System.Windows.Forms.ComboBox();
            this.pnlSep1 = new System.Windows.Forms.Panel();
            this.lblMoistSection = new System.Windows.Forms.Label();
            this.lblMoistApp = new System.Windows.Forms.Label();
            this.lblMoistLive = new System.Windows.Forms.Label();
            this.lblMoistMeter = new System.Windows.Forms.Label();
            this.numCalMeter = new System.Windows.Forms.NumericUpDown();
            this.lblCalMeterUnit = new System.Windows.Forms.Label();
            this.btnApplyMoist = new System.Windows.Forms.Button();
            this.lblMoistOffset = new System.Windows.Forms.Label();
            this.numMoistOffset = new System.Windows.Forms.NumericUpDown();
            this.lblMoistOffUnit = new System.Windows.Forms.Label();
            this.lblMoistScale = new System.Windows.Forms.Label();
            this.numMoistScale = new System.Windows.Forms.NumericUpDown();
            this.lblMoistScaleUnit = new System.Windows.Forms.Label();
            this.pnlSep2 = new System.Windows.Forms.Panel();
            this.lblTempSection = new System.Windows.Forms.Label();
            this.lblTempApp = new System.Windows.Forms.Label();
            this.lblTempLive = new System.Windows.Forms.Label();
            this.lblTempThermo = new System.Windows.Forms.Label();
            this.numCalThermo = new System.Windows.Forms.NumericUpDown();
            this.lblCalThermoUnit = new System.Windows.Forms.Label();
            this.btnApplyTemp = new System.Windows.Forms.Button();
            this.lblTempOffset = new System.Windows.Forms.Label();
            this.numTempOffset = new System.Windows.Forms.NumericUpDown();
            this.lblTempOffUnit = new System.Windows.Forms.Label();
            this.lblTempScale = new System.Windows.Forms.Label();
            this.numTempScale = new System.Windows.Forms.NumericUpDown();
            this.lblTempScaleUnit = new System.Windows.Forms.Label();
            this.pnlSep3 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSCClose = new System.Windows.Forms.Button();
            this.tmrLive = new System.Windows.Forms.Timer(this.components);
            this.pnlTitle.SuspendLayout();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCalMeter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMoistOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMoistScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCalThermo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTempOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTempScale)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTitle
            // 
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(2, 2);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(580, 48);
            this.pnlTitle.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(580, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Moisture Cal";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlContent
            //
            this.pnlContent.Controls.Add(this.lblCropLbl);
            this.pnlContent.Controls.Add(this.cboCrop);
            this.pnlContent.Controls.Add(this.lblProfileLbl);
            this.pnlContent.Controls.Add(this.cboProfile);
            this.pnlContent.Controls.Add(this.pnlSep1);
            this.pnlContent.Controls.Add(this.lblMoistSection);
            this.pnlContent.Controls.Add(this.lblMoistApp);
            this.pnlContent.Controls.Add(this.lblMoistLive);
            this.pnlContent.Controls.Add(this.lblMoistMeter);
            this.pnlContent.Controls.Add(this.numCalMeter);
            this.pnlContent.Controls.Add(this.lblCalMeterUnit);
            this.pnlContent.Controls.Add(this.btnApplyMoist);
            this.pnlContent.Controls.Add(this.lblMoistOffset);
            this.pnlContent.Controls.Add(this.numMoistOffset);
            this.pnlContent.Controls.Add(this.lblMoistOffUnit);
            this.pnlContent.Controls.Add(this.lblMoistScale);
            this.pnlContent.Controls.Add(this.numMoistScale);
            this.pnlContent.Controls.Add(this.lblMoistScaleUnit);
            this.pnlContent.Controls.Add(this.pnlSep2);
            this.pnlContent.Controls.Add(this.lblTempSection);
            this.pnlContent.Controls.Add(this.lblTempApp);
            this.pnlContent.Controls.Add(this.lblTempLive);
            this.pnlContent.Controls.Add(this.lblTempThermo);
            this.pnlContent.Controls.Add(this.numCalThermo);
            this.pnlContent.Controls.Add(this.lblCalThermoUnit);
            this.pnlContent.Controls.Add(this.btnApplyTemp);
            this.pnlContent.Controls.Add(this.lblTempOffset);
            this.pnlContent.Controls.Add(this.numTempOffset);
            this.pnlContent.Controls.Add(this.lblTempOffUnit);
            this.pnlContent.Controls.Add(this.lblTempScale);
            this.pnlContent.Controls.Add(this.numTempScale);
            this.pnlContent.Controls.Add(this.lblTempScaleUnit);
            this.pnlContent.Controls.Add(this.pnlSep3);
            this.pnlContent.Controls.Add(this.btnSave);
            this.pnlContent.Controls.Add(this.btnSCClose);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(2, 50);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(580, 506);
            this.pnlContent.TabIndex = 0;
            // 
            // lblCropLbl
            // 
            this.lblCropLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblCropLbl.Location = new System.Drawing.Point(8, 10);
            this.lblCropLbl.Name = "lblCropLbl";
            this.lblCropLbl.Size = new System.Drawing.Size(130, 32);
            this.lblCropLbl.TabIndex = 0;
            this.lblCropLbl.Text = "Crop:";
            // 
            // cboCrop
            // 
            this.cboCrop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCrop.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.cboCrop.Location = new System.Drawing.Point(146, 10);
            this.cboCrop.Name = "cboCrop";
            this.cboCrop.Size = new System.Drawing.Size(220, 32);
            this.cboCrop.TabIndex = 1;
            this.cboCrop.SelectedIndexChanged += new System.EventHandler(this.cboCrop_SelectedIndexChanged);
            // 
            // lblProfileLbl
            // 
            this.lblProfileLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblProfileLbl.Location = new System.Drawing.Point(8, 50);
            this.lblProfileLbl.Name = "lblProfileLbl";
            this.lblProfileLbl.Size = new System.Drawing.Size(130, 32);
            this.lblProfileLbl.TabIndex = 2;
            this.lblProfileLbl.Text = "Profile:";
            // 
            // cboProfile
            // 
            this.cboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.cboProfile.Location = new System.Drawing.Point(146, 50);
            this.cboProfile.Name = "cboProfile";
            this.cboProfile.Size = new System.Drawing.Size(220, 32);
            this.cboProfile.TabIndex = 3;
            this.cboProfile.SelectedIndexChanged += new System.EventHandler(this.cboProfile_SelectedIndexChanged);
            // 
            // pnlSep1
            // 
            this.pnlSep1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.pnlSep1.Location = new System.Drawing.Point(4, 92);
            this.pnlSep1.Name = "pnlSep1";
            this.pnlSep1.Size = new System.Drawing.Size(572, 1);
            this.pnlSep1.TabIndex = 4;
            // 
            // lblMoistSection
            // 
            this.lblMoistSection.AutoSize = true;
            this.lblMoistSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblMoistSection.Location = new System.Drawing.Point(8, 102);
            this.lblMoistSection.Name = "lblMoistSection";
            this.lblMoistSection.Size = new System.Drawing.Size(90, 24);
            this.lblMoistSection.TabIndex = 5;
            this.lblMoistSection.Text = "Moisture";
            // 
            // lblMoistApp
            // 
            this.lblMoistApp.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblMoistApp.Location = new System.Drawing.Point(8, 138);
            this.lblMoistApp.Name = "lblMoistApp";
            this.lblMoistApp.Size = new System.Drawing.Size(110, 32);
            this.lblMoistApp.TabIndex = 6;
            this.lblMoistApp.Text = "App reads:";
            // 
            // lblMoistLive
            // 
            this.lblMoistLive.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblMoistLive.Location = new System.Drawing.Point(126, 138);
            this.lblMoistLive.Name = "lblMoistLive";
            this.lblMoistLive.Size = new System.Drawing.Size(70, 32);
            this.lblMoistLive.TabIndex = 7;
            this.lblMoistLive.Text = "—";
            this.lblMoistLive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMoistMeter
            // 
            this.lblMoistMeter.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblMoistMeter.Location = new System.Drawing.Point(204, 138);
            this.lblMoistMeter.Name = "lblMoistMeter";
            this.lblMoistMeter.Size = new System.Drawing.Size(70, 32);
            this.lblMoistMeter.TabIndex = 8;
            this.lblMoistMeter.Text = "Meter:";
            // 
            // numCalMeter
            // 
            this.numCalMeter.DecimalPlaces = 1;
            this.numCalMeter.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.numCalMeter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numCalMeter.Location = new System.Drawing.Point(282, 138);
            this.numCalMeter.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.numCalMeter.Name = "numCalMeter";
            this.numCalMeter.Size = new System.Drawing.Size(90, 29);
            this.numCalMeter.TabIndex = 9;
            // 
            // lblCalMeterUnit
            // 
            this.lblCalMeterUnit.AutoSize = true;
            this.lblCalMeterUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblCalMeterUnit.Location = new System.Drawing.Point(380, 144);
            this.lblCalMeterUnit.Name = "lblCalMeterUnit";
            this.lblCalMeterUnit.Size = new System.Drawing.Size(21, 18);
            this.lblCalMeterUnit.TabIndex = 10;
            this.lblCalMeterUnit.Text = "%";
            // 
            // btnApplyMoist
            // 
            this.btnApplyMoist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyMoist.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnApplyMoist.Location = new System.Drawing.Point(430, 134);
            this.btnApplyMoist.Name = "btnApplyMoist";
            this.btnApplyMoist.Size = new System.Drawing.Size(140, 40);
            this.btnApplyMoist.TabIndex = 11;
            this.btnApplyMoist.Text = global::YieldFlo.Language.Lang.lgApplyCal;
            this.btnApplyMoist.Click += new System.EventHandler(this.btnApplyMoist_Click);
            // 
            // lblMoistOffset
            // 
            this.lblMoistOffset.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblMoistOffset.Location = new System.Drawing.Point(8, 184);
            this.lblMoistOffset.Name = "lblMoistOffset";
            this.lblMoistOffset.Size = new System.Drawing.Size(130, 32);
            this.lblMoistOffset.TabIndex = 12;
            this.lblMoistOffset.Text = "Offset:";
            // 
            // numMoistOffset
            // 
            this.numMoistOffset.DecimalPlaces = 1;
            this.numMoistOffset.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.numMoistOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numMoistOffset.Location = new System.Drawing.Point(146, 184);
            this.numMoistOffset.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.numMoistOffset.Minimum = new decimal(new int[] {
            25,
            0,
            0,
            -2147483648});
            this.numMoistOffset.Name = "numMoistOffset";
            this.numMoistOffset.Size = new System.Drawing.Size(110, 29);
            this.numMoistOffset.TabIndex = 13;
            // 
            // lblMoistOffUnit
            // 
            this.lblMoistOffUnit.AutoSize = true;
            this.lblMoistOffUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblMoistOffUnit.Location = new System.Drawing.Point(264, 190);
            this.lblMoistOffUnit.Name = "lblMoistOffUnit";
            this.lblMoistOffUnit.Size = new System.Drawing.Size(125, 18);
            this.lblMoistOffUnit.TabIndex = 14;
            this.lblMoistOffUnit.Text = "% (saved to crop)";
            // 
            // lblMoistScale
            // 
            this.lblMoistScale.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblMoistScale.Location = new System.Drawing.Point(8, 224);
            this.lblMoistScale.Name = "lblMoistScale";
            this.lblMoistScale.Size = new System.Drawing.Size(130, 32);
            this.lblMoistScale.TabIndex = 15;
            this.lblMoistScale.Text = "Scale:";
            // 
            // numMoistScale
            // 
            this.numMoistScale.DecimalPlaces = 4;
            this.numMoistScale.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.numMoistScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numMoistScale.Location = new System.Drawing.Point(146, 224);
            this.numMoistScale.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numMoistScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numMoistScale.Name = "numMoistScale";
            this.numMoistScale.Size = new System.Drawing.Size(130, 29);
            this.numMoistScale.TabIndex = 16;
            this.numMoistScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // lblMoistScaleUnit
            // 
            this.lblMoistScaleUnit.AutoSize = true;
            this.lblMoistScaleUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblMoistScaleUnit.Location = new System.Drawing.Point(284, 230);
            this.lblMoistScaleUnit.Name = "lblMoistScaleUnit";
            this.lblMoistScaleUnit.Size = new System.Drawing.Size(176, 18);
            this.lblMoistScaleUnit.TabIndex = 17;
            this.lblMoistScaleUnit.Text = "%/count (saved to profile)";
            // 
            // pnlSep2
            // 
            this.pnlSep2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.pnlSep2.Location = new System.Drawing.Point(4, 266);
            this.pnlSep2.Name = "pnlSep2";
            this.pnlSep2.Size = new System.Drawing.Size(572, 1);
            this.pnlSep2.TabIndex = 18;
            // 
            // lblTempSection
            // 
            this.lblTempSection.AutoSize = true;
            this.lblTempSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblTempSection.Location = new System.Drawing.Point(8, 276);
            this.lblTempSection.Name = "lblTempSection";
            this.lblTempSection.Size = new System.Drawing.Size(130, 24);
            this.lblTempSection.TabIndex = 19;
            this.lblTempSection.Text = "Temperature";
            // 
            // lblTempApp
            // 
            this.lblTempApp.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblTempApp.Location = new System.Drawing.Point(8, 312);
            this.lblTempApp.Name = "lblTempApp";
            this.lblTempApp.Size = new System.Drawing.Size(110, 32);
            this.lblTempApp.TabIndex = 20;
            this.lblTempApp.Text = "App reads:";
            // 
            // lblTempLive
            // 
            this.lblTempLive.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblTempLive.Location = new System.Drawing.Point(126, 312);
            this.lblTempLive.Name = "lblTempLive";
            this.lblTempLive.Size = new System.Drawing.Size(70, 32);
            this.lblTempLive.TabIndex = 21;
            this.lblTempLive.Text = "—";
            this.lblTempLive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTempThermo
            // 
            this.lblTempThermo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblTempThermo.Location = new System.Drawing.Point(204, 312);
            this.lblTempThermo.Name = "lblTempThermo";
            this.lblTempThermo.Size = new System.Drawing.Size(70, 32);
            this.lblTempThermo.TabIndex = 22;
            this.lblTempThermo.Text = "Meter:";
            // 
            // numCalThermo
            // 
            this.numCalThermo.DecimalPlaces = 1;
            this.numCalThermo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.numCalThermo.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numCalThermo.Location = new System.Drawing.Point(282, 312);
            this.numCalThermo.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numCalThermo.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.numCalThermo.Name = "numCalThermo";
            this.numCalThermo.Size = new System.Drawing.Size(90, 29);
            this.numCalThermo.TabIndex = 23;
            this.numCalThermo.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // lblCalThermoUnit
            // 
            this.lblCalThermoUnit.AutoSize = true;
            this.lblCalThermoUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblCalThermoUnit.Location = new System.Drawing.Point(380, 318);
            this.lblCalThermoUnit.Name = "lblCalThermoUnit";
            this.lblCalThermoUnit.Size = new System.Drawing.Size(25, 18);
            this.lblCalThermoUnit.TabIndex = 24;
            this.lblCalThermoUnit.Text = "°C";
            // 
            // btnApplyTemp
            // 
            this.btnApplyTemp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnApplyTemp.Location = new System.Drawing.Point(430, 308);
            this.btnApplyTemp.Name = "btnApplyTemp";
            this.btnApplyTemp.Size = new System.Drawing.Size(140, 40);
            this.btnApplyTemp.TabIndex = 25;
            this.btnApplyTemp.Text = global::YieldFlo.Language.Lang.lgApplyCal;
            this.btnApplyTemp.Click += new System.EventHandler(this.btnApplyTemp_Click);
            // 
            // lblTempOffset
            // 
            this.lblTempOffset.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblTempOffset.Location = new System.Drawing.Point(8, 358);
            this.lblTempOffset.Name = "lblTempOffset";
            this.lblTempOffset.Size = new System.Drawing.Size(130, 32);
            this.lblTempOffset.TabIndex = 26;
            this.lblTempOffset.Text = "Offset:";
            // 
            // numTempOffset
            // 
            this.numTempOffset.DecimalPlaces = 1;
            this.numTempOffset.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.numTempOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numTempOffset.Location = new System.Drawing.Point(146, 358);
            this.numTempOffset.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numTempOffset.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numTempOffset.Name = "numTempOffset";
            this.numTempOffset.Size = new System.Drawing.Size(110, 29);
            this.numTempOffset.TabIndex = 27;
            // 
            // lblTempOffUnit
            // 
            this.lblTempOffUnit.AutoSize = true;
            this.lblTempOffUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblTempOffUnit.Location = new System.Drawing.Point(264, 364);
            this.lblTempOffUnit.Name = "lblTempOffUnit";
            this.lblTempOffUnit.Size = new System.Drawing.Size(139, 18);
            this.lblTempOffUnit.TabIndex = 28;
            this.lblTempOffUnit.Text = "°C (saved to profile)";
            // 
            // lblTempScale
            // 
            this.lblTempScale.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblTempScale.Location = new System.Drawing.Point(8, 398);
            this.lblTempScale.Name = "lblTempScale";
            this.lblTempScale.Size = new System.Drawing.Size(130, 32);
            this.lblTempScale.TabIndex = 29;
            this.lblTempScale.Text = "Scale:";
            // 
            // numTempScale
            // 
            this.numTempScale.DecimalPlaces = 4;
            this.numTempScale.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.numTempScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numTempScale.Location = new System.Drawing.Point(146, 398);
            this.numTempScale.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numTempScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numTempScale.Name = "numTempScale";
            this.numTempScale.Size = new System.Drawing.Size(130, 29);
            this.numTempScale.TabIndex = 30;
            this.numTempScale.Value = new decimal(new int[] {
            125,
            0,
            0,
            262144});
            // 
            // lblTempScaleUnit
            // 
            this.lblTempScaleUnit.AutoSize = true;
            this.lblTempScaleUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblTempScaleUnit.Location = new System.Drawing.Point(284, 404);
            this.lblTempScaleUnit.Name = "lblTempScaleUnit";
            this.lblTempScaleUnit.Size = new System.Drawing.Size(180, 18);
            this.lblTempScaleUnit.TabIndex = 31;
            this.lblTempScaleUnit.Text = "°C/count (saved to profile)";
            // 
            // pnlSep3
            // 
            this.pnlSep3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.pnlSep3.Location = new System.Drawing.Point(4, 440);
            this.pnlSep3.Name = "pnlSep3";
            this.pnlSep3.Size = new System.Drawing.Size(572, 1);
            this.pnlSep3.TabIndex = 32;
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(8, 450);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(220, 48);
            this.btnSave.TabIndex = 33;
            this.btnSave.Text = global::YieldFlo.Language.Lang.lgSave;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSCClose
            // 
            this.btnSCClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSCClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnSCClose.Location = new System.Drawing.Point(430, 450);
            this.btnSCClose.Name = "btnSCClose";
            this.btnSCClose.Size = new System.Drawing.Size(140, 48);
            this.btnSCClose.TabIndex = 34;
            this.btnSCClose.Text = global::YieldFlo.Language.Lang.lgClose;
            this.btnSCClose.Click += new System.EventHandler(this.btnSCClose_Click);
            // 
            // tmrLive
            // 
            this.tmrLive.Interval = 500;
            this.tmrLive.Tick += new System.EventHandler(this.tmrLive_Tick);
            // 
            // frmMenuSensorCal
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(584, 558);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(584, 558);
            this.MinimumSize = new System.Drawing.Size(584, 558);
            this.Name = "frmMenuSensorCal";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Moisture Cal";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmMenuSensorCal_Load);
            this.pnlTitle.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCalMeter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMoistOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMoistScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCalThermo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTempOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTempScale)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel         pnlTitle;
        private System.Windows.Forms.Label         lblTitle;
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
