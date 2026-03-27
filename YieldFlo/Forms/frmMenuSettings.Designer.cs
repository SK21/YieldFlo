namespace YieldFlo.Forms
{
    partial class frmMenuSettings
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMenuSettings));
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnTitleClose = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblUnits = new System.Windows.Forms.Label();
            this.btnImperial = new System.Windows.Forms.Button();
            this.btnMetric = new System.Windows.Forms.Button();
            this.lblTheme = new System.Windows.Forms.Label();
            this.btnDark = new System.Windows.Forms.Button();
            this.btnLight = new System.Windows.Forms.Button();
            this.lblNetwork = new System.Windows.Forms.Label();
            this.btnEthernet = new System.Windows.Forms.Button();
            this.btnCAN = new System.Windows.Forms.Button();
            this.lblWifiInfo = new System.Windows.Forms.Label();
            this.lblCanDriver = new System.Windows.Forms.Label();
            this.cbCanDriver = new System.Windows.Forms.ComboBox();
            this.lblCanPort = new System.Windows.Forms.Label();
            this.cbCanPort = new System.Windows.Forms.ComboBox();
            this.btnRescanPorts = new System.Windows.Forms.Button();
            this.lblResumeJob = new System.Windows.Forms.Label();
            this.btnResumeOn = new System.Windows.Forms.Button();
            this.btnResumeOff = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnSettingsClose = new System.Windows.Forms.Button();
            this.pnlTitle.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTitle
            // 
            this.pnlTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Controls.Add(this.btnTitleClose);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(2, 2);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(452, 40);
            this.pnlTitle.TabIndex = 0;
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
            this.lblTitle.Text = "Settings";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnTitleClose
            // 
            this.btnTitleClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnTitleClose.FlatAppearance.BorderSize = 0;
            this.btnTitleClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTitleClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.btnTitleClose.ForeColor = System.Drawing.Color.White;
            this.btnTitleClose.Location = new System.Drawing.Point(418, 5);
            this.btnTitleClose.Name = "btnTitleClose";
            this.btnTitleClose.Size = new System.Drawing.Size(36, 30);
            this.btnTitleClose.TabIndex = 1;
            this.btnTitleClose.Text = "×";
            this.btnTitleClose.UseVisualStyleBackColor = false;
            this.btnTitleClose.Click += new System.EventHandler(this.btnTitleClose_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlContent.Controls.Add(this.lblWifiInfo);
            this.pnlContent.Controls.Add(this.lblUnits);
            this.pnlContent.Controls.Add(this.btnImperial);
            this.pnlContent.Controls.Add(this.btnMetric);
            this.pnlContent.Controls.Add(this.lblTheme);
            this.pnlContent.Controls.Add(this.btnDark);
            this.pnlContent.Controls.Add(this.btnLight);
            this.pnlContent.Controls.Add(this.lblNetwork);
            this.pnlContent.Controls.Add(this.btnEthernet);
            this.pnlContent.Controls.Add(this.btnCAN);
            this.pnlContent.Controls.Add(this.lblCanDriver);
            this.pnlContent.Controls.Add(this.cbCanDriver);
            this.pnlContent.Controls.Add(this.lblCanPort);
            this.pnlContent.Controls.Add(this.cbCanPort);
            this.pnlContent.Controls.Add(this.btnRescanPorts);
            this.pnlContent.Controls.Add(this.lblResumeJob);
            this.pnlContent.Controls.Add(this.btnResumeOn);
            this.pnlContent.Controls.Add(this.btnResumeOff);
            this.pnlContent.Controls.Add(this.btnSaveSettings);
            this.pnlContent.Controls.Add(this.btnSettingsClose);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(2, 42);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(452, 436);
            this.pnlContent.TabIndex = 1;
            // 
            // lblUnits
            // 
            this.lblUnits.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblUnits.ForeColor = System.Drawing.Color.Silver;
            this.lblUnits.Location = new System.Drawing.Point(8, 8);
            this.lblUnits.Name = "lblUnits";
            this.lblUnits.Size = new System.Drawing.Size(200, 16);
            this.lblUnits.TabIndex = 0;
            this.lblUnits.Text = "Units";
            // 
            // btnImperial
            // 
            this.btnImperial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnImperial.FlatAppearance.BorderSize = 0;
            this.btnImperial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImperial.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnImperial.ForeColor = System.Drawing.Color.White;
            this.btnImperial.Location = new System.Drawing.Point(8, 26);
            this.btnImperial.Name = "btnImperial";
            this.btnImperial.Size = new System.Drawing.Size(210, 44);
            this.btnImperial.TabIndex = 1;
            this.btnImperial.Text = "Imperial";
            this.btnImperial.UseVisualStyleBackColor = false;
            this.btnImperial.Click += new System.EventHandler(this.btnImperial_Click);
            // 
            // btnMetric
            // 
            this.btnMetric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnMetric.FlatAppearance.BorderSize = 0;
            this.btnMetric.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMetric.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnMetric.ForeColor = System.Drawing.Color.White;
            this.btnMetric.Location = new System.Drawing.Point(226, 26);
            this.btnMetric.Name = "btnMetric";
            this.btnMetric.Size = new System.Drawing.Size(210, 44);
            this.btnMetric.TabIndex = 2;
            this.btnMetric.Text = "Metric";
            this.btnMetric.UseVisualStyleBackColor = false;
            this.btnMetric.Click += new System.EventHandler(this.btnMetric_Click);
            // 
            // lblTheme
            // 
            this.lblTheme.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTheme.ForeColor = System.Drawing.Color.Silver;
            this.lblTheme.Location = new System.Drawing.Point(8, 80);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(200, 16);
            this.lblTheme.TabIndex = 3;
            this.lblTheme.Text = "Theme";
            // 
            // btnDark
            // 
            this.btnDark.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnDark.FlatAppearance.BorderSize = 0;
            this.btnDark.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDark.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnDark.ForeColor = System.Drawing.Color.White;
            this.btnDark.Location = new System.Drawing.Point(8, 98);
            this.btnDark.Name = "btnDark";
            this.btnDark.Size = new System.Drawing.Size(210, 44);
            this.btnDark.TabIndex = 4;
            this.btnDark.Text = "Dark";
            this.btnDark.UseVisualStyleBackColor = false;
            this.btnDark.Click += new System.EventHandler(this.btnDark_Click);
            // 
            // btnLight
            // 
            this.btnLight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnLight.FlatAppearance.BorderSize = 0;
            this.btnLight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnLight.ForeColor = System.Drawing.Color.White;
            this.btnLight.Location = new System.Drawing.Point(226, 98);
            this.btnLight.Name = "btnLight";
            this.btnLight.Size = new System.Drawing.Size(210, 44);
            this.btnLight.TabIndex = 5;
            this.btnLight.Text = "Light";
            this.btnLight.UseVisualStyleBackColor = false;
            this.btnLight.Click += new System.EventHandler(this.btnLight_Click);
            // 
            // lblNetwork
            // 
            this.lblNetwork.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblNetwork.ForeColor = System.Drawing.Color.Silver;
            this.lblNetwork.Location = new System.Drawing.Point(8, 152);
            this.lblNetwork.Name = "lblNetwork";
            this.lblNetwork.Size = new System.Drawing.Size(300, 16);
            this.lblNetwork.TabIndex = 6;
            this.lblNetwork.Text = "Module Communication";
            // 
            // btnEthernet
            // 
            this.btnEthernet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnEthernet.FlatAppearance.BorderSize = 0;
            this.btnEthernet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEthernet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnEthernet.ForeColor = System.Drawing.Color.White;
            this.btnEthernet.Location = new System.Drawing.Point(8, 170);
            this.btnEthernet.Name = "btnEthernet";
            this.btnEthernet.Size = new System.Drawing.Size(210, 44);
            this.btnEthernet.TabIndex = 7;
            this.btnEthernet.Text = "WiFi";
            this.btnEthernet.UseVisualStyleBackColor = false;
            this.btnEthernet.Click += new System.EventHandler(this.btnEthernet_Click);
            // 
            // btnCAN
            // 
            this.btnCAN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnCAN.FlatAppearance.BorderSize = 0;
            this.btnCAN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCAN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnCAN.ForeColor = System.Drawing.Color.White;
            this.btnCAN.Location = new System.Drawing.Point(226, 170);
            this.btnCAN.Name = "btnCAN";
            this.btnCAN.Size = new System.Drawing.Size(210, 44);
            this.btnCAN.TabIndex = 8;
            this.btnCAN.Text = "CAN";
            this.btnCAN.UseVisualStyleBackColor = false;
            this.btnCAN.Click += new System.EventHandler(this.btnCAN_Click);
            // 
            // lblWifiInfo
            // 
            this.lblWifiInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblWifiInfo.ForeColor = System.Drawing.Color.Silver;
            this.lblWifiInfo.Location = new System.Drawing.Point(8, 224);
            this.lblWifiInfo.Name = "lblWifiInfo";
            this.lblWifiInfo.Size = new System.Drawing.Size(434, 42);
            this.lblWifiInfo.TabIndex = 9;
            this.lblWifiInfo.Text = "Connect PC to module WiFi access point or other WiFi network.";
            this.lblWifiInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCanDriver
            // 
            this.lblCanDriver.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblCanDriver.ForeColor = System.Drawing.Color.Silver;
            this.lblCanDriver.Location = new System.Drawing.Point(8, 224);
            this.lblCanDriver.Name = "lblCanDriver";
            this.lblCanDriver.Size = new System.Drawing.Size(200, 16);
            this.lblCanDriver.TabIndex = 10;
            this.lblCanDriver.Text = "CAN Driver";
            // 
            // cbCanDriver
            // 
            this.cbCanDriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCanDriver.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cbCanDriver.Items.AddRange(new object[] {
            "SLCAN",
            "InnoMaker",
            "PCAN"});
            this.cbCanDriver.Location = new System.Drawing.Point(8, 242);
            this.cbCanDriver.Name = "cbCanDriver";
            this.cbCanDriver.Size = new System.Drawing.Size(200, 23);
            this.cbCanDriver.TabIndex = 11;
            // 
            // lblCanPort
            // 
            this.lblCanPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblCanPort.ForeColor = System.Drawing.Color.Silver;
            this.lblCanPort.Location = new System.Drawing.Point(228, 224);
            this.lblCanPort.Name = "lblCanPort";
            this.lblCanPort.Size = new System.Drawing.Size(180, 16);
            this.lblCanPort.TabIndex = 12;
            this.lblCanPort.Text = "COM Port";
            // 
            // cbCanPort
            // 
            this.cbCanPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCanPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cbCanPort.Location = new System.Drawing.Point(228, 242);
            this.cbCanPort.Name = "cbCanPort";
            this.cbCanPort.Size = new System.Drawing.Size(174, 23);
            this.cbCanPort.TabIndex = 13;
            // 
            // btnRescanPorts
            // 
            this.btnRescanPorts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnRescanPorts.FlatAppearance.BorderSize = 0;
            this.btnRescanPorts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRescanPorts.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRescanPorts.ForeColor = System.Drawing.Color.White;
            this.btnRescanPorts.Image = ((System.Drawing.Image)(resources.GetObject("btnRescanPorts.Image")));
            this.btnRescanPorts.Location = new System.Drawing.Point(406, 240);
            this.btnRescanPorts.Name = "btnRescanPorts";
            this.btnRescanPorts.Size = new System.Drawing.Size(36, 26);
            this.btnRescanPorts.TabIndex = 14;
            this.btnRescanPorts.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRescanPorts.UseVisualStyleBackColor = false;
            this.btnRescanPorts.Click += new System.EventHandler(this.btnRescanPorts_Click);
            // 
            // lblResumeJob
            // 
            this.lblResumeJob.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblResumeJob.ForeColor = System.Drawing.Color.Silver;
            this.lblResumeJob.Location = new System.Drawing.Point(8, 284);
            this.lblResumeJob.Name = "lblResumeJob";
            this.lblResumeJob.Size = new System.Drawing.Size(300, 16);
            this.lblResumeJob.TabIndex = 15;
            this.lblResumeJob.Text = "Resume Job on Start";
            // 
            // btnResumeOn
            // 
            this.btnResumeOn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnResumeOn.FlatAppearance.BorderSize = 0;
            this.btnResumeOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResumeOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnResumeOn.ForeColor = System.Drawing.Color.White;
            this.btnResumeOn.Location = new System.Drawing.Point(8, 302);
            this.btnResumeOn.Name = "btnResumeOn";
            this.btnResumeOn.Size = new System.Drawing.Size(210, 44);
            this.btnResumeOn.TabIndex = 16;
            this.btnResumeOn.Text = "On";
            this.btnResumeOn.UseVisualStyleBackColor = false;
            this.btnResumeOn.Click += new System.EventHandler(this.btnResumeOn_Click);
            // 
            // btnResumeOff
            // 
            this.btnResumeOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnResumeOff.FlatAppearance.BorderSize = 0;
            this.btnResumeOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResumeOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnResumeOff.ForeColor = System.Drawing.Color.White;
            this.btnResumeOff.Location = new System.Drawing.Point(226, 302);
            this.btnResumeOff.Name = "btnResumeOff";
            this.btnResumeOff.Size = new System.Drawing.Size(210, 44);
            this.btnResumeOff.TabIndex = 17;
            this.btnResumeOff.Text = "Off";
            this.btnResumeOff.UseVisualStyleBackColor = false;
            this.btnResumeOff.Click += new System.EventHandler(this.btnResumeOff_Click);
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(110)))), ((int)(((byte)(0)))));
            this.btnSaveSettings.FlatAppearance.BorderSize = 0;
            this.btnSaveSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveSettings.ForeColor = System.Drawing.Color.White;
            this.btnSaveSettings.Location = new System.Drawing.Point(8, 362);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(160, 36);
            this.btnSaveSettings.TabIndex = 18;
            this.btnSaveSettings.Text = "Save && Apply";
            this.btnSaveSettings.UseVisualStyleBackColor = false;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // btnSettingsClose
            // 
            this.btnSettingsClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnSettingsClose.FlatAppearance.BorderSize = 0;
            this.btnSettingsClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettingsClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnSettingsClose.ForeColor = System.Drawing.Color.White;
            this.btnSettingsClose.Location = new System.Drawing.Point(336, 362);
            this.btnSettingsClose.Name = "btnSettingsClose";
            this.btnSettingsClose.Size = new System.Drawing.Size(112, 36);
            this.btnSettingsClose.TabIndex = 19;
            this.btnSettingsClose.Text = "Close";
            this.btnSettingsClose.UseVisualStyleBackColor = false;
            this.btnSettingsClose.Click += new System.EventHandler(this.btnSettingsClose_Click);
            // 
            // frmMenuSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(456, 480);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(456, 480);
            this.MinimumSize = new System.Drawing.Size(456, 480);
            this.Name = "frmMenuSettings";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmMenuSettings_Load);
            this.pnlTitle.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel    pnlTitle;
        private System.Windows.Forms.Label    lblTitle;
        private System.Windows.Forms.Button   btnTitleClose;
        private System.Windows.Forms.Panel    pnlContent;
        private System.Windows.Forms.Label    lblUnits;
        private System.Windows.Forms.Button   btnImperial;
        private System.Windows.Forms.Button   btnMetric;
        private System.Windows.Forms.Label    lblTheme;
        private System.Windows.Forms.Button   btnDark;
        private System.Windows.Forms.Button   btnLight;
        private System.Windows.Forms.Label    lblNetwork;
        private System.Windows.Forms.Button   btnEthernet;
        private System.Windows.Forms.Button   btnCAN;
        private System.Windows.Forms.Label    lblWifiInfo;
        private System.Windows.Forms.Label    lblCanDriver;
        private System.Windows.Forms.ComboBox cbCanDriver;
        private System.Windows.Forms.Label    lblCanPort;
        private System.Windows.Forms.ComboBox cbCanPort;
        private System.Windows.Forms.Button   btnRescanPorts;
        private System.Windows.Forms.Label    lblResumeJob;
        private System.Windows.Forms.Button   btnResumeOn;
        private System.Windows.Forms.Button   btnResumeOff;
        private System.Windows.Forms.Button   btnSaveSettings;
        private System.Windows.Forms.Button   btnSettingsClose;
    }
}
