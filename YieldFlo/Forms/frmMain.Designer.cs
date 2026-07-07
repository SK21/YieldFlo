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
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlGauges = new System.Windows.Forms.Panel();
            this.pnlYield = new System.Windows.Forms.Panel();
            this.lblYieldUnit = new System.Windows.Forms.Label();
            this.lblYield = new System.Windows.Forms.Label();
            this.lblYieldTitle = new System.Windows.Forms.Label();
            this.pnlMoisture = new System.Windows.Forms.Panel();
            this.lblMoistureUnit = new System.Windows.Forms.Label();
            this.lblMoisture = new System.Windows.Forms.Label();
            this.lblMoistureTitle = new System.Windows.Forms.Label();
            this.pnlSpeedPanel = new System.Windows.Forms.Panel();
            this.lblSpeedUnit = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblSpeedTitle = new System.Windows.Forms.Label();
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
            this.lblStatusComm = new System.Windows.Forms.Label();
            this.lblStatusJob = new System.Windows.Forms.Label();
            this.lblStatusMsg = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.pnlToolbar.SuspendLayout();
            this.pnlGauges.SuspendLayout();
            this.pnlYield.SuspendLayout();
            this.pnlMoisture.SuspendLayout();
            this.pnlSpeedPanel.SuspendLayout();
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
            this.pnlToolbar.Controls.Add(this.lblTitle);
            this.pnlToolbar.Controls.Add(this.btnMini);
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Location = new System.Drawing.Point(2, 2);
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(496, 44);
            this.pnlToolbar.TabIndex = 3;
            // 
            // btnMenu
            // 
            this.btnMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnMenu.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnMenu.ForeColor = System.Drawing.Color.White;
            this.btnMenu.Location = new System.Drawing.Point(4, 5);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(60, 34);
            this.btnMenu.TabIndex = 0;
            this.btnMenu.Text = global::YieldFlo.Language.Lang.lgMenu;
            this.btnMenu.UseVisualStyleBackColor = false;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(110)))), ((int)(((byte)(0)))));
            this.btnStart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Segoe UI Symbol", 14F, System.Drawing.FontStyle.Regular);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(67, 5);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(68, 34);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "▶";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(88)))), ((int)(((byte)(0)))));
            this.btnPause.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.Font = new System.Drawing.Font("Segoe UI Symbol", 14F, System.Drawing.FontStyle.Regular);
            this.btnPause.ForeColor = System.Drawing.Color.White;
            this.btnPause.Location = new System.Drawing.Point(138, 5);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(68, 34);
            this.btnPause.TabIndex = 2;
            this.btnPause.Text = "❚❚";
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStop.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Segoe UI Symbol", 14F, System.Drawing.FontStyle.Regular);
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(209, 5);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(68, 34);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "■";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(280, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(52, 34);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = global::YieldFlo.Language.Lang.lgExit;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            //
            // lblTitle
            //
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblTitle.Location = new System.Drawing.Point(335, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(127, 44);
            this.lblTitle.TabIndex = 5;
            this.lblTitle.Text = "YieldFlo";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnMini
            //
            this.btnMini.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.btnMini.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnMini.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMini.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnMini.ForeColor = System.Drawing.Color.White;
            this.btnMini.Location = new System.Drawing.Point(464, 5);
            this.btnMini.Name = "btnMini";
            this.btnMini.Size = new System.Drawing.Size(30, 34);
            this.btnMini.TabIndex = 6;
            this.btnMini.Text = "─";
            this.btnMini.UseVisualStyleBackColor = false;
            this.btnMini.Click += new System.EventHandler(this.btnMini_Click);
            // 
            // pnlGauges
            // 
            this.pnlGauges.Controls.Add(this.pnlYield);
            this.pnlGauges.Controls.Add(this.pnlMoisture);
            this.pnlGauges.Controls.Add(this.pnlSpeedPanel);
            this.pnlGauges.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGauges.Location = new System.Drawing.Point(2, 46);
            this.pnlGauges.Name = "pnlGauges";
            this.pnlGauges.Size = new System.Drawing.Size(496, 148);
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
            this.pnlYield.Size = new System.Drawing.Size(188, 144);
            this.pnlYield.TabIndex = 0;
            // 
            // lblYieldUnit
            // 
            this.lblYieldUnit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblYieldUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblYieldUnit.ForeColor = System.Drawing.Color.White;
            this.lblYieldUnit.Location = new System.Drawing.Point(0, 120);
            this.lblYieldUnit.Name = "lblYieldUnit";
            this.lblYieldUnit.Size = new System.Drawing.Size(186, 22);
            this.lblYieldUnit.TabIndex = 0;
            this.lblYieldUnit.Text = "bu/ac";
            this.lblYieldUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblYield
            // 
            this.lblYield.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblYield.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold);
            this.lblYield.Location = new System.Drawing.Point(0, 18);
            this.lblYield.Name = "lblYield";
            this.lblYield.Size = new System.Drawing.Size(186, 124);
            this.lblYield.TabIndex = 1;
            this.lblYield.Text = "--.-";
            this.lblYield.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblYieldTitle
            // 
            this.lblYieldTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblYieldTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblYieldTitle.Location = new System.Drawing.Point(0, 0);
            this.lblYieldTitle.Name = "lblYieldTitle";
            this.lblYieldTitle.Size = new System.Drawing.Size(186, 18);
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
            this.pnlMoisture.Location = new System.Drawing.Point(192, 2);
            this.pnlMoisture.Name = "pnlMoisture";
            this.pnlMoisture.Size = new System.Drawing.Size(154, 144);
            this.pnlMoisture.TabIndex = 1;
            // 
            // lblMoistureUnit
            // 
            this.lblMoistureUnit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblMoistureUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblMoistureUnit.ForeColor = System.Drawing.Color.White;
            this.lblMoistureUnit.Location = new System.Drawing.Point(0, 120);
            this.lblMoistureUnit.Name = "lblMoistureUnit";
            this.lblMoistureUnit.Size = new System.Drawing.Size(152, 22);
            this.lblMoistureUnit.TabIndex = 0;
            this.lblMoistureUnit.Text = "%";
            this.lblMoistureUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMoisture
            // 
            this.lblMoisture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMoisture.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold);
            this.lblMoisture.Location = new System.Drawing.Point(0, 18);
            this.lblMoisture.Name = "lblMoisture";
            this.lblMoisture.Size = new System.Drawing.Size(152, 124);
            this.lblMoisture.TabIndex = 1;
            this.lblMoisture.Text = "--.-";
            this.lblMoisture.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMoistureTitle
            // 
            this.lblMoistureTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMoistureTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblMoistureTitle.Location = new System.Drawing.Point(0, 0);
            this.lblMoistureTitle.Name = "lblMoistureTitle";
            this.lblMoistureTitle.Size = new System.Drawing.Size(152, 18);
            this.lblMoistureTitle.TabIndex = 2;
            this.lblMoistureTitle.Text = "MOISTURE";
            this.lblMoistureTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlSpeedPanel
            // 
            this.pnlSpeedPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSpeedPanel.Controls.Add(this.lblSpeedUnit);
            this.pnlSpeedPanel.Controls.Add(this.lblSpeed);
            this.pnlSpeedPanel.Controls.Add(this.lblSpeedTitle);
            this.pnlSpeedPanel.Location = new System.Drawing.Point(348, 2);
            this.pnlSpeedPanel.Name = "pnlSpeedPanel";
            this.pnlSpeedPanel.Size = new System.Drawing.Size(148, 144);
            this.pnlSpeedPanel.TabIndex = 2;
            // 
            // lblSpeedUnit
            // 
            this.lblSpeedUnit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblSpeedUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblSpeedUnit.ForeColor = System.Drawing.Color.White;
            this.lblSpeedUnit.Location = new System.Drawing.Point(0, 120);
            this.lblSpeedUnit.Name = "lblSpeedUnit";
            this.lblSpeedUnit.Size = new System.Drawing.Size(146, 22);
            this.lblSpeedUnit.TabIndex = 0;
            this.lblSpeedUnit.Text = "km/h";
            this.lblSpeedUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSpeed
            // 
            this.lblSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold);
            this.lblSpeed.Location = new System.Drawing.Point(0, 18);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(146, 124);
            this.lblSpeed.TabIndex = 1;
            this.lblSpeed.Text = "--.-";
            this.lblSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSpeedTitle
            // 
            this.lblSpeedTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSpeedTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblSpeedTitle.Location = new System.Drawing.Point(0, 0);
            this.lblSpeedTitle.Name = "lblSpeedTitle";
            this.lblSpeedTitle.Size = new System.Drawing.Size(146, 18);
            this.lblSpeedTitle.TabIndex = 2;
            this.lblSpeedTitle.Text = "SPEED";
            this.lblSpeedTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlTotals
            // 
            this.pnlTotals.Controls.Add(this.lblTotRate);
            this.pnlTotals.Controls.Add(this.lblWorkRate);
            this.pnlTotals.Controls.Add(this.lblTotTotal);
            this.pnlTotals.Controls.Add(this.lblTotArea);
            this.pnlTotals.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTotals.Location = new System.Drawing.Point(2, 194);
            this.pnlTotals.Name = "pnlTotals";
            this.pnlTotals.Size = new System.Drawing.Size(496, 26);
            this.pnlTotals.TabIndex = 1;
            // 
            // lblTotRate
            // 
            this.lblTotRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblTotRate.ForeColor = System.Drawing.Color.Silver;
            this.lblTotRate.Location = new System.Drawing.Point(0, 0);
            this.lblTotRate.Name = "lblTotRate";
            this.lblTotRate.Size = new System.Drawing.Size(124, 26);
            this.lblTotRate.TabIndex = 0;
            this.lblTotRate.Text = "0.0 bu/ac";
            this.lblTotRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblWorkRate
            //
            this.lblWorkRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblWorkRate.ForeColor = System.Drawing.Color.Silver;
            this.lblWorkRate.Location = new System.Drawing.Point(124, 0);
            this.lblWorkRate.Name = "lblWorkRate";
            this.lblWorkRate.Size = new System.Drawing.Size(124, 26);
            this.lblWorkRate.TabIndex = 1;
            this.lblWorkRate.Text = "0.0 bu/hr";
            this.lblWorkRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTotTotal
            //
            this.lblTotTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblTotTotal.ForeColor = System.Drawing.Color.Silver;
            this.lblTotTotal.Location = new System.Drawing.Point(248, 0);
            this.lblTotTotal.Name = "lblTotTotal";
            this.lblTotTotal.Size = new System.Drawing.Size(124, 26);
            this.lblTotTotal.TabIndex = 2;
            this.lblTotTotal.Text = "0 bu";
            this.lblTotTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTotArea
            //
            this.lblTotArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblTotArea.ForeColor = System.Drawing.Color.Silver;
            this.lblTotArea.Location = new System.Drawing.Point(372, 0);
            this.lblTotArea.Name = "lblTotArea";
            this.lblTotArea.Size = new System.Drawing.Size(124, 26);
            this.lblTotArea.TabIndex = 3;
            this.lblTotArea.Text = "0.00 ac";
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
            this.pnlSensors.Location = new System.Drawing.Point(2, 220);
            this.pnlSensors.Name = "pnlSensors";
            this.pnlSensors.Size = new System.Drawing.Size(496, 60);
            this.pnlSensors.TabIndex = 0;
            // 
            // lblSensorHeader
            // 
            this.lblSensorHeader.AutoSize = true;
            this.lblSensorHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblSensorHeader.ForeColor = System.Drawing.Color.Silver;
            this.lblSensorHeader.Location = new System.Drawing.Point(4, 2);
            this.lblSensorHeader.Name = "lblSensorHeader";
            this.lblSensorHeader.Size = new System.Drawing.Size(63, 15);
            this.lblSensorHeader.TabIndex = 0;
            this.lblSensorHeader.Text = "Sensors:";
            // 
            // lblSensor1Title
            // 
            this.lblSensor1Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblSensor1Title.ForeColor = System.Drawing.Color.Silver;
            this.lblSensor1Title.Location = new System.Drawing.Point(4, 16);
            this.lblSensor1Title.Name = "lblSensor1Title";
            this.lblSensor1Title.Size = new System.Drawing.Size(70, 18);
            this.lblSensor1Title.TabIndex = 1;
            this.lblSensor1Title.Text = "Elev Flow";
            // 
            // pnlSensor1
            // 
            this.pnlSensor1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.pnlSensor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSensor1.Controls.Add(this.pnlSensor1Fill);
            this.pnlSensor1.Location = new System.Drawing.Point(78, 16);
            this.pnlSensor1.Name = "pnlSensor1";
            this.pnlSensor1.Size = new System.Drawing.Size(350, 18);
            this.pnlSensor1.TabIndex = 2;
            // 
            // pnlSensor1Fill
            // 
            this.pnlSensor1Fill.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(200)))), ((int)(((byte)(50)))));
            this.pnlSensor1Fill.Location = new System.Drawing.Point(0, 0);
            this.pnlSensor1Fill.Name = "pnlSensor1Fill";
            this.pnlSensor1Fill.Size = new System.Drawing.Size(0, 18);
            this.pnlSensor1Fill.TabIndex = 0;
            // 
            // lblSensor1Value
            // 
            this.lblSensor1Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblSensor1Value.ForeColor = System.Drawing.Color.White;
            this.lblSensor1Value.Location = new System.Drawing.Point(431, 16);
            this.lblSensor1Value.Name = "lblSensor1Value";
            this.lblSensor1Value.Size = new System.Drawing.Size(62, 18);
            this.lblSensor1Value.TabIndex = 3;
            this.lblSensor1Value.Text = "0%";
            // 
            // lblSensor2Title
            // 
            this.lblSensor2Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblSensor2Title.ForeColor = System.Drawing.Color.Silver;
            this.lblSensor2Title.Location = new System.Drawing.Point(4, 38);
            this.lblSensor2Title.Name = "lblSensor2Title";
            this.lblSensor2Title.Size = new System.Drawing.Size(70, 18);
            this.lblSensor2Title.TabIndex = 4;
            this.lblSensor2Title.Text = "Moisture";
            // 
            // pnlSensor2
            // 
            this.pnlSensor2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.pnlSensor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSensor2.Controls.Add(this.pnlSensor2Fill);
            this.pnlSensor2.Location = new System.Drawing.Point(78, 38);
            this.pnlSensor2.Name = "pnlSensor2";
            this.pnlSensor2.Size = new System.Drawing.Size(350, 18);
            this.pnlSensor2.TabIndex = 5;
            // 
            // pnlSensor2Fill
            // 
            this.pnlSensor2Fill.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(200)))), ((int)(((byte)(50)))));
            this.pnlSensor2Fill.Location = new System.Drawing.Point(0, 0);
            this.pnlSensor2Fill.Name = "pnlSensor2Fill";
            this.pnlSensor2Fill.Size = new System.Drawing.Size(0, 18);
            this.pnlSensor2Fill.TabIndex = 0;
            // 
            // lblSensor2Value
            // 
            this.lblSensor2Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblSensor2Value.ForeColor = System.Drawing.Color.White;
            this.lblSensor2Value.Location = new System.Drawing.Point(431, 38);
            this.lblSensor2Value.Name = "lblSensor2Value";
            this.lblSensor2Value.Size = new System.Drawing.Size(62, 18);
            this.lblSensor2Value.TabIndex = 6;
            this.lblSensor2Value.Text = "0%";
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.pnlStatus.Controls.Add(this.lblStatusGPS);
            this.pnlStatus.Controls.Add(this.lblStatusModule);
            this.pnlStatus.Controls.Add(this.lblStatusComm);
            this.pnlStatus.Controls.Add(this.lblStatusJob);
            this.pnlStatus.Controls.Add(this.lblVersion);
            this.pnlStatus.Controls.Add(this.lblStatusMsg);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.Location = new System.Drawing.Point(2, 281);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(496, 26);
            this.pnlStatus.TabIndex = 4;
            // 
            // lblStatusGPS
            // 
            this.lblStatusGPS.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusGPS.ForeColor = System.Drawing.Color.Silver;
            this.lblStatusGPS.Location = new System.Drawing.Point(4, 4);
            this.lblStatusGPS.Name = "lblStatusGPS";
            this.lblStatusGPS.Size = new System.Drawing.Size(36, 23);
            this.lblStatusGPS.TabIndex = 0;
            this.lblStatusGPS.Text = "GPS";
            // 
            // lblStatusModule
            // 
            this.lblStatusModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusModule.ForeColor = System.Drawing.Color.Silver;
            this.lblStatusModule.Location = new System.Drawing.Point(44, 4);
            this.lblStatusModule.Name = "lblStatusModule";
            this.lblStatusModule.Size = new System.Drawing.Size(60, 23);
            this.lblStatusModule.TabIndex = 1;
            this.lblStatusModule.Text = "Module";
            // 
            // lblStatusComm
            // 
            this.lblStatusComm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusComm.ForeColor = System.Drawing.Color.Silver;
            this.lblStatusComm.Location = new System.Drawing.Point(110, 4);
            this.lblStatusComm.Name = "lblStatusComm";
            this.lblStatusComm.Size = new System.Drawing.Size(36, 23);
            this.lblStatusComm.TabIndex = 2;
            this.lblStatusComm.Text = "UDP";
            // 
            // lblStatusJob
            // 
            this.lblStatusJob.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusJob.ForeColor = System.Drawing.Color.Silver;
            this.lblStatusJob.Location = new System.Drawing.Point(152, 4);
            this.lblStatusJob.Name = "lblStatusJob";
            this.lblStatusJob.Size = new System.Drawing.Size(138, 23);
            this.lblStatusJob.TabIndex = 3;
            this.lblStatusJob.Text = "No Active Job";
            // 
            // lblStatusMsg
            // 
            this.lblStatusMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusMsg.ForeColor = System.Drawing.Color.Silver;
            this.lblStatusMsg.Location = new System.Drawing.Point(0, 0);
            this.lblStatusMsg.Name = "lblStatusMsg";
            this.lblStatusMsg.Size = new System.Drawing.Size(496, 26);
            this.lblStatusMsg.TabIndex = 4;
            this.lblStatusMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatusMsg.Visible = false;
            // 
            // lblVersion
            // 
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.lblVersion.Location = new System.Drawing.Point(434, 4);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(59, 15);
            this.lblVersion.TabIndex = 5;
            this.lblVersion.Text = "v8.10.10";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmMain
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 309);
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
            this.pnlSpeedPanel.ResumeLayout(false);
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
        private System.Windows.Forms.Label  lblTitle;

        private System.Windows.Forms.Panel pnlGauges;
        private System.Windows.Forms.Panel pnlYield;
        private System.Windows.Forms.Label lblYieldTitle;
        private System.Windows.Forms.Label lblYield;
        private System.Windows.Forms.Label lblYieldUnit;

        private System.Windows.Forms.Panel pnlMoisture;
        private System.Windows.Forms.Label lblMoistureTitle;
        private System.Windows.Forms.Label lblMoisture;
        private System.Windows.Forms.Label lblMoistureUnit;

        private System.Windows.Forms.Panel pnlSpeedPanel;
        private System.Windows.Forms.Label lblSpeedTitle;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblSpeedUnit;

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
        private System.Windows.Forms.Label lblStatusComm;
        private System.Windows.Forms.Label lblStatusJob;
        private System.Windows.Forms.Label lblStatusMsg;
        private System.Windows.Forms.Label lblVersion;
    }
}
