using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.btnMenu = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnMini = new System.Windows.Forms.Button();
            this.pnlGauges = new System.Windows.Forms.Panel();
            this.pnlYield = new System.Windows.Forms.Panel();
            this.lblYieldUnit = new System.Windows.Forms.Label();
            this.lblYield = new System.Windows.Forms.Label();
            this.lblYieldTitle = new System.Windows.Forms.Label();
            this.pnlMoisture = new System.Windows.Forms.Panel();
            this.lblMoistureUnit = new System.Windows.Forms.Label();
            this.lblMoisture = new System.Windows.Forms.Label();
            this.lblMoistureTitle = new System.Windows.Forms.Label();
            this.pnlTotals = new System.Windows.Forms.Panel();
            this.lblTotRate = new System.Windows.Forms.Label();
            this.lblWorkRate = new System.Windows.Forms.Label();
            this.lblTotTotal = new System.Windows.Forms.Label();
            this.lblTotArea = new System.Windows.Forms.Label();
            this.pnlSensors = new System.Windows.Forms.Panel();
            this.lblSensorHeader = new System.Windows.Forms.Label();
            this.lblSensor1Title = new System.Windows.Forms.Label();
            this.pnlSensor1 = new System.Windows.Forms.Panel();
            this.pnlSensor1Fill = new System.Windows.Forms.Panel();
            this.lblSensor1Value = new System.Windows.Forms.Label();
            this.lblSensor2Title = new System.Windows.Forms.Label();
            this.pnlSensor2 = new System.Windows.Forms.Panel();
            this.pnlSensor2Fill = new System.Windows.Forms.Panel();
            this.lblSensor2Value = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatusGPS = new System.Windows.Forms.Label();
            this.lblStatusModule = new System.Windows.Forms.Label();
            this.lblStatusJob = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblStatusMsg = new System.Windows.Forms.Label();
            this.pnlToolbar.SuspendLayout();
            this.pnlGauges.SuspendLayout();
            this.pnlYield.SuspendLayout();
            this.pnlMoisture.SuspendLayout();
            this.pnlTotals.SuspendLayout();
            this.pnlSensors.SuspendLayout();
            this.pnlSensor1.SuspendLayout();
            this.pnlSensor2.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlToolbar
            // 
            this.pnlToolbar.Controls.Add(this.btnMenu);
            this.pnlToolbar.Controls.Add(this.btnStart);
            this.pnlToolbar.Controls.Add(this.btnPause);
            this.pnlToolbar.Controls.Add(this.btnStop);
            this.pnlToolbar.Controls.Add(this.btnExit);
            this.pnlToolbar.Controls.Add(this.btnMini);
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Location = new System.Drawing.Point(2, 2);
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(398, 68);
            this.pnlToolbar.TabIndex = 3;
            // 
            // btnMenu
            // 
            this.btnMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnMenu.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenu.ForeColor = System.Drawing.Color.White;
            this.btnMenu.Image = global::YieldFlo.Properties.Resources.MenuLight;
            this.btnMenu.Location = new System.Drawing.Point(136, 4);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(60, 60);
            this.btnMenu.TabIndex = 0;
            this.btnMenu.UseVisualStyleBackColor = false;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(110)))), ((int)(((byte)(0)))));
            this.btnStart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Image = global::YieldFlo.Properties.Resources.PlayLight;
            this.btnStart.Location = new System.Drawing.Point(202, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(60, 60);
            this.btnStart.TabIndex = 1;
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(88)))), ((int)(((byte)(0)))));
            this.btnPause.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.ForeColor = System.Drawing.Color.White;
            this.btnPause.Image = global::YieldFlo.Properties.Resources.PauseLight;
            this.btnPause.Location = new System.Drawing.Point(268, 4);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(60, 60);
            this.btnPause.TabIndex = 2;
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStop.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Image = global::YieldFlo.Properties.Resources.StopLight;
            this.btnStop.Location = new System.Drawing.Point(334, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(60, 60);
            this.btnStop.TabIndex = 3;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Image = global::YieldFlo.Properties.Resources.SwitchOffLight;
            this.btnExit.Location = new System.Drawing.Point(4, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(60, 60);
            this.btnExit.TabIndex = 4;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnMini
            // 
            this.btnMini.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.btnMini.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnMini.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMini.ForeColor = System.Drawing.Color.White;
            this.btnMini.Image = global::YieldFlo.Properties.Resources.MinimizeLight;
            this.btnMini.Location = new System.Drawing.Point(70, 4);
            this.btnMini.Name = "btnMini";
            this.btnMini.Size = new System.Drawing.Size(60, 60);
            this.btnMini.TabIndex = 6;
            this.btnMini.UseVisualStyleBackColor = false;
            this.btnMini.Click += new System.EventHandler(this.btnMini_Click);
            // 
            // pnlGauges
            // 
            this.pnlGauges.Controls.Add(this.pnlYield);
            this.pnlGauges.Controls.Add(this.pnlMoisture);
            this.pnlGauges.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGauges.Location = new System.Drawing.Point(2, 70);
            this.pnlGauges.Name = "pnlGauges";
            this.pnlGauges.Size = new System.Drawing.Size(398, 132);
            this.pnlGauges.TabIndex = 2;
            // 
            // pnlYield
            // 
            this.pnlYield.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlYield.Controls.Add(this.lblYieldUnit);
            this.pnlYield.Controls.Add(this.lblYield);
            this.pnlYield.Controls.Add(this.lblYieldTitle);
            this.pnlYield.Location = new System.Drawing.Point(2, 2);
            this.pnlYield.Name = "pnlYield";
            this.pnlYield.Size = new System.Drawing.Size(216, 128);
            this.pnlYield.TabIndex = 0;
            // 
            // lblYieldUnit
            // 
            this.lblYieldUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblYieldUnit.ForeColor = System.Drawing.Color.White;
            this.lblYieldUnit.Location = new System.Drawing.Point(0, 100);
            this.lblYieldUnit.Name = "lblYieldUnit";
            this.lblYieldUnit.Size = new System.Drawing.Size(214, 28);
            this.lblYieldUnit.TabIndex = 0;
            this.lblYieldUnit.Text = "bu/ac";
            this.lblYieldUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblYield
            // 
            this.lblYield.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold);
            this.lblYield.Location = new System.Drawing.Point(0, 28);
            this.lblYield.Name = "lblYield";
            this.lblYield.Size = new System.Drawing.Size(214, 72);
            this.lblYield.TabIndex = 1;
            this.lblYield.Text = "--.-";
            this.lblYield.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblYieldTitle
            // 
            this.lblYieldTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblYieldTitle.Location = new System.Drawing.Point(0, 0);
            this.lblYieldTitle.Name = "lblYieldTitle";
            this.lblYieldTitle.Size = new System.Drawing.Size(214, 28);
            this.lblYieldTitle.TabIndex = 2;
            this.lblYieldTitle.Text = "YIELD";
            this.lblYieldTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlMoisture
            // 
            this.pnlMoisture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMoisture.Controls.Add(this.lblMoistureUnit);
            this.pnlMoisture.Controls.Add(this.lblMoisture);
            this.pnlMoisture.Controls.Add(this.lblMoistureTitle);
            this.pnlMoisture.Location = new System.Drawing.Point(220, 2);
            this.pnlMoisture.Name = "pnlMoisture";
            this.pnlMoisture.Size = new System.Drawing.Size(176, 128);
            this.pnlMoisture.TabIndex = 1;
            // 
            // lblMoistureUnit
            // 
            this.lblMoistureUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblMoistureUnit.ForeColor = System.Drawing.Color.White;
            this.lblMoistureUnit.Location = new System.Drawing.Point(0, 100);
            this.lblMoistureUnit.Name = "lblMoistureUnit";
            this.lblMoistureUnit.Size = new System.Drawing.Size(174, 28);
            this.lblMoistureUnit.TabIndex = 0;
            this.lblMoistureUnit.Text = "%";
            this.lblMoistureUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMoisture
            // 
            this.lblMoisture.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold);
            this.lblMoisture.Location = new System.Drawing.Point(0, 28);
            this.lblMoisture.Name = "lblMoisture";
            this.lblMoisture.Size = new System.Drawing.Size(174, 72);
            this.lblMoisture.TabIndex = 1;
            this.lblMoisture.Text = "--.-";
            this.lblMoisture.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMoistureTitle
            // 
            this.lblMoistureTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblMoistureTitle.Location = new System.Drawing.Point(0, 0);
            this.lblMoistureTitle.Name = "lblMoistureTitle";
            this.lblMoistureTitle.Size = new System.Drawing.Size(174, 28);
            this.lblMoistureTitle.TabIndex = 2;
            this.lblMoistureTitle.Text = "MOISTURE";
            this.lblMoistureTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlTotals
            // 
            this.pnlTotals.Controls.Add(this.lblTotRate);
            this.pnlTotals.Controls.Add(this.lblWorkRate);
            this.pnlTotals.Controls.Add(this.lblTotTotal);
            this.pnlTotals.Controls.Add(this.lblTotArea);
            this.pnlTotals.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTotals.Location = new System.Drawing.Point(2, 202);
            this.pnlTotals.Name = "pnlTotals";
            this.pnlTotals.Size = new System.Drawing.Size(398, 64);
            this.pnlTotals.TabIndex = 1;
            // 
            // lblTotRate
            // 
            this.lblTotRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblTotRate.ForeColor = System.Drawing.Color.Silver;
            this.lblTotRate.Location = new System.Drawing.Point(0, 0);
            this.lblTotRate.Name = "lblTotRate";
            this.lblTotRate.Size = new System.Drawing.Size(199, 32);
            this.lblTotRate.TabIndex = 0;
            this.lblTotRate.Text = "0.0 bu/ac";
            this.lblTotRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWorkRate
            // 
            this.lblWorkRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblWorkRate.ForeColor = System.Drawing.Color.Silver;
            this.lblWorkRate.Location = new System.Drawing.Point(0, 32);
            this.lblWorkRate.Name = "lblWorkRate";
            this.lblWorkRate.Size = new System.Drawing.Size(199, 32);
            this.lblWorkRate.TabIndex = 1;
            this.lblWorkRate.Text = "0.0 bu/hr";
            this.lblWorkRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotTotal
            // 
            this.lblTotTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblTotTotal.ForeColor = System.Drawing.Color.Silver;
            this.lblTotTotal.Location = new System.Drawing.Point(199, 0);
            this.lblTotTotal.Name = "lblTotTotal";
            this.lblTotTotal.Size = new System.Drawing.Size(199, 32);
            this.lblTotTotal.TabIndex = 2;
            this.lblTotTotal.Text = "0 bu";
            this.lblTotTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotArea
            // 
            this.lblTotArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblTotArea.ForeColor = System.Drawing.Color.Silver;
            this.lblTotArea.Location = new System.Drawing.Point(199, 32);
            this.lblTotArea.Name = "lblTotArea";
            this.lblTotArea.Size = new System.Drawing.Size(199, 32);
            this.lblTotArea.TabIndex = 3;
            this.lblTotArea.Text = "0.0 ac";
            this.lblTotArea.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlSensors
            // 
            this.pnlSensors.Controls.Add(this.lblSensorHeader);
            this.pnlSensors.Controls.Add(this.lblSensor1Title);
            this.pnlSensors.Controls.Add(this.pnlSensor1);
            this.pnlSensors.Controls.Add(this.lblSensor1Value);
            this.pnlSensors.Controls.Add(this.lblSensor2Title);
            this.pnlSensors.Controls.Add(this.pnlSensor2);
            this.pnlSensors.Controls.Add(this.lblSensor2Value);
            this.pnlSensors.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSensors.Location = new System.Drawing.Point(2, 266);
            this.pnlSensors.Name = "pnlSensors";
            this.pnlSensors.Size = new System.Drawing.Size(398, 66);
            this.pnlSensors.TabIndex = 0;
            // 
            // lblSensorHeader
            // 
            this.lblSensorHeader.AutoSize = true;
            this.lblSensorHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblSensorHeader.ForeColor = System.Drawing.Color.Silver;
            this.lblSensorHeader.Location = new System.Drawing.Point(4, 2);
            this.lblSensorHeader.Name = "lblSensorHeader";
            this.lblSensorHeader.Size = new System.Drawing.Size(76, 18);
            this.lblSensorHeader.TabIndex = 0;
            this.lblSensorHeader.Text = "Sensors:";
            // 
            // lblSensor1Title
            // 
            this.lblSensor1Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblSensor1Title.ForeColor = System.Drawing.Color.Silver;
            this.lblSensor1Title.Location = new System.Drawing.Point(4, 22);
            this.lblSensor1Title.Name = "lblSensor1Title";
            this.lblSensor1Title.Size = new System.Drawing.Size(78, 20);
            this.lblSensor1Title.TabIndex = 1;
            this.lblSensor1Title.Text = "Elev Flow";
            // 
            // pnlSensor1
            // 
            this.pnlSensor1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.pnlSensor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSensor1.Controls.Add(this.pnlSensor1Fill);
            this.pnlSensor1.Location = new System.Drawing.Point(86, 22);
            this.pnlSensor1.Name = "pnlSensor1";
            this.pnlSensor1.Size = new System.Drawing.Size(234, 20);
            this.pnlSensor1.TabIndex = 2;
            // 
            // pnlSensor1Fill
            // 
            this.pnlSensor1Fill.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(200)))), ((int)(((byte)(50)))));
            this.pnlSensor1Fill.Location = new System.Drawing.Point(0, 0);
            this.pnlSensor1Fill.Name = "pnlSensor1Fill";
            this.pnlSensor1Fill.Size = new System.Drawing.Size(0, 20);
            this.pnlSensor1Fill.TabIndex = 0;
            // 
            // lblSensor1Value
            // 
            this.lblSensor1Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblSensor1Value.ForeColor = System.Drawing.Color.White;
            this.lblSensor1Value.Location = new System.Drawing.Point(324, 22);
            this.lblSensor1Value.Name = "lblSensor1Value";
            this.lblSensor1Value.Size = new System.Drawing.Size(70, 20);
            this.lblSensor1Value.TabIndex = 3;
            this.lblSensor1Value.Text = "0%";
            // 
            // lblSensor2Title
            // 
            this.lblSensor2Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblSensor2Title.ForeColor = System.Drawing.Color.Silver;
            this.lblSensor2Title.Location = new System.Drawing.Point(4, 46);
            this.lblSensor2Title.Name = "lblSensor2Title";
            this.lblSensor2Title.Size = new System.Drawing.Size(78, 20);
            this.lblSensor2Title.TabIndex = 4;
            this.lblSensor2Title.Text = "Moisture";
            // 
            // pnlSensor2
            // 
            this.pnlSensor2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.pnlSensor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSensor2.Controls.Add(this.pnlSensor2Fill);
            this.pnlSensor2.Location = new System.Drawing.Point(86, 46);
            this.pnlSensor2.Name = "pnlSensor2";
            this.pnlSensor2.Size = new System.Drawing.Size(234, 20);
            this.pnlSensor2.TabIndex = 5;
            // 
            // pnlSensor2Fill
            // 
            this.pnlSensor2Fill.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(200)))), ((int)(((byte)(50)))));
            this.pnlSensor2Fill.Location = new System.Drawing.Point(0, 0);
            this.pnlSensor2Fill.Name = "pnlSensor2Fill";
            this.pnlSensor2Fill.Size = new System.Drawing.Size(0, 20);
            this.pnlSensor2Fill.TabIndex = 0;
            // 
            // lblSensor2Value
            // 
            this.lblSensor2Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblSensor2Value.ForeColor = System.Drawing.Color.White;
            this.lblSensor2Value.Location = new System.Drawing.Point(324, 46);
            this.lblSensor2Value.Name = "lblSensor2Value";
            this.lblSensor2Value.Size = new System.Drawing.Size(70, 20);
            this.lblSensor2Value.TabIndex = 6;
            this.lblSensor2Value.Text = "0%";
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.pnlStatus.Controls.Add(this.lblStatusGPS);
            this.pnlStatus.Controls.Add(this.lblStatusModule);
            this.pnlStatus.Controls.Add(this.lblStatusJob);
            this.pnlStatus.Controls.Add(this.lblVersion);
            this.pnlStatus.Controls.Add(this.lblStatusMsg);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.Location = new System.Drawing.Point(2, 332);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(398, 32);
            this.pnlStatus.TabIndex = 4;
            // 
            // lblStatusGPS
            // 
            this.lblStatusGPS.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusGPS.ForeColor = System.Drawing.Color.Silver;
            this.lblStatusGPS.Location = new System.Drawing.Point(4, 4);
            this.lblStatusGPS.Name = "lblStatusGPS";
            this.lblStatusGPS.Size = new System.Drawing.Size(48, 24);
            this.lblStatusGPS.TabIndex = 0;
            this.lblStatusGPS.Text = "GPS";
            this.lblStatusGPS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatusModule
            // 
            this.lblStatusModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusModule.ForeColor = System.Drawing.Color.Silver;
            this.lblStatusModule.Location = new System.Drawing.Point(55, 4);
            this.lblStatusModule.Name = "lblStatusModule";
            this.lblStatusModule.Size = new System.Drawing.Size(74, 24);
            this.lblStatusModule.TabIndex = 1;
            this.lblStatusModule.Text = "Module";
            this.lblStatusModule.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatusJob
            // 
            this.lblStatusJob.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusJob.ForeColor = System.Drawing.Color.Silver;
            this.lblStatusJob.Location = new System.Drawing.Point(132, 4);
            this.lblStatusJob.Name = "lblStatusJob";
            this.lblStatusJob.Size = new System.Drawing.Size(188, 24);
            this.lblStatusJob.TabIndex = 3;
            this.lblStatusJob.Text = "No Active Job";
            this.lblStatusJob.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.lblVersion.Location = new System.Drawing.Point(327, 4);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(67, 24);
            this.lblVersion.TabIndex = 5;
            this.lblVersion.Text = "v8.10.10";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatusMsg
            // 
            this.lblStatusMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusMsg.ForeColor = System.Drawing.Color.Silver;
            this.lblStatusMsg.Location = new System.Drawing.Point(0, 0);
            this.lblStatusMsg.Name = "lblStatusMsg";
            this.lblStatusMsg.Size = new System.Drawing.Size(398, 32);
            this.lblStatusMsg.TabIndex = 4;
            this.lblStatusMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatusMsg.Visible = false;
            // 
            // frmMain
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(402, 366);
            this.Controls.Add(this.pnlSensors);
            this.Controls.Add(this.pnlTotals);
            this.Controls.Add(this.pnlGauges);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Text = "YieldFlo";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.pnlToolbar.ResumeLayout(false);
            this.pnlGauges.ResumeLayout(false);
            this.pnlYield.ResumeLayout(false);
            this.pnlMoisture.ResumeLayout(false);
            this.pnlTotals.ResumeLayout(false);
            this.pnlSensors.ResumeLayout(false);
            this.pnlSensors.PerformLayout();
            this.pnlSensor1.ResumeLayout(false);
            this.pnlSensor2.ResumeLayout(false);
            this.pnlStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        // ── Controls ──────────────────────────────────────────────────────────
        private System.Windows.Forms.Panel  pnlToolbar;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnMini;

        private System.Windows.Forms.Panel pnlGauges;
        private System.Windows.Forms.Panel pnlYield;
        private System.Windows.Forms.Label lblYieldTitle;
        private System.Windows.Forms.Label lblYield;
        private System.Windows.Forms.Label lblYieldUnit;

        private System.Windows.Forms.Panel pnlMoisture;
        private System.Windows.Forms.Label lblMoistureTitle;
        private System.Windows.Forms.Label lblMoisture;
        private System.Windows.Forms.Label lblMoistureUnit;

        private System.Windows.Forms.Panel pnlTotals;
        private System.Windows.Forms.Label lblTotArea;
        private System.Windows.Forms.Label lblTotTotal;
        private System.Windows.Forms.Label lblTotRate;
        private System.Windows.Forms.Label lblWorkRate;

        private System.Windows.Forms.Panel pnlSensors;
        private System.Windows.Forms.Label lblSensorHeader;
        private System.Windows.Forms.Label lblSensor1Title;
        private System.Windows.Forms.Panel pnlSensor1;
        private System.Windows.Forms.Panel pnlSensor1Fill;
        private System.Windows.Forms.Label lblSensor1Value;
        private System.Windows.Forms.Label lblSensor2Title;
        private System.Windows.Forms.Panel pnlSensor2;
        private System.Windows.Forms.Panel pnlSensor2Fill;
        private System.Windows.Forms.Label lblSensor2Value;

        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatusGPS;
        private System.Windows.Forms.Label lblStatusModule;
        private System.Windows.Forms.Label lblStatusJob;
        private System.Windows.Forms.Label lblStatusMsg;
        private System.Windows.Forms.Label lblVersion;
    }
}
