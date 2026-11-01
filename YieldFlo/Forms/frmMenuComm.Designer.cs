using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmMenuComm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMenuComm));
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.tabComm = new YieldFlo.Forms.DarkTabControl();
            this.tabWifi = new System.Windows.Forms.TabPage();
            this.ckWifi = new System.Windows.Forms.CheckBox();
            this.lblModuleIP = new System.Windows.Forms.LinkLabel();
            this.lblWifiInfo = new System.Windows.Forms.Label();
            this.btnWifiSetup = new System.Windows.Forms.Button();
            this.lblHotspot = new System.Windows.Forms.Label();
            this.btnHotspotOn = new System.Windows.Forms.Button();
            this.btnHotspotOff = new System.Windows.Forms.Button();
            this.lblHotspotStatus = new System.Windows.Forms.Label();
            this.tabCan = new System.Windows.Forms.TabPage();
            this.ckCan = new System.Windows.Forms.CheckBox();
            this.lblCanModule = new System.Windows.Forms.Label();
            this.lblCanDriver = new System.Windows.Forms.Label();
            this.cbCanDriver = new System.Windows.Forms.ComboBox();
            this.lblCanPort = new System.Windows.Forms.Label();
            this.cbCanPort = new System.Windows.Forms.ComboBox();
            this.btnRescanPorts = new System.Windows.Forms.Button();
            this.lblCanStatus = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tmrStatus = new System.Windows.Forms.Timer(this.components);
            this.pnlTitle.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.tabComm.SuspendLayout();
            this.tabWifi.SuspendLayout();
            this.tabCan.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTitle
            //
            this.pnlTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(2, 2);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(560, 48);
            this.pnlTitle.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(560, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Module Communication";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlContent
            //
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlContent.Controls.Add(this.tabComm);
            this.pnlContent.Controls.Add(this.btnSave);
            this.pnlContent.Controls.Add(this.btnClose);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(2, 50);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(560, 408);
            this.pnlContent.TabIndex = 1;
            //
            // tabComm
            //
            this.tabComm.Controls.Add(this.tabWifi);
            this.tabComm.Controls.Add(this.tabCan);
            this.tabComm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.tabComm.ItemSize = new System.Drawing.Size(272, 26);
            this.tabComm.Location = new System.Drawing.Point(4, 8);
            this.tabComm.Name = "tabComm";
            this.tabComm.SelectedIndex = 0;
            this.tabComm.Size = new System.Drawing.Size(552, 336);
            this.tabComm.TabIndex = 0;
            //
            // tabWifi
            //
            this.tabWifi.Controls.Add(this.ckWifi);
            this.tabWifi.Controls.Add(this.lblModuleIP);
            this.tabWifi.Controls.Add(this.lblWifiInfo);
            this.tabWifi.Controls.Add(this.btnWifiSetup);
            this.tabWifi.Controls.Add(this.lblHotspot);
            this.tabWifi.Controls.Add(this.btnHotspotOn);
            this.tabWifi.Controls.Add(this.btnHotspotOff);
            this.tabWifi.Controls.Add(this.lblHotspotStatus);
            this.tabWifi.Location = new System.Drawing.Point(4, 30);
            this.tabWifi.Name = "tabWifi";
            this.tabWifi.Padding = new System.Windows.Forms.Padding(3);
            this.tabWifi.Size = new System.Drawing.Size(544, 302);
            this.tabWifi.TabIndex = 0;
            this.tabWifi.Text = global::YieldFlo.Language.Lang.lgWiFi;
            this.tabWifi.UseVisualStyleBackColor = true;
            //
            // ckWifi
            //
            this.ckWifi.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.ckWifi.Location = new System.Drawing.Point(8, 8);
            this.ckWifi.Name = "ckWifi";
            this.ckWifi.Size = new System.Drawing.Size(140, 32);
            this.ckWifi.TabIndex = 0;
            this.ckWifi.Text = "Enabled";
            this.ckWifi.UseVisualStyleBackColor = false;
            this.ckWifi.CheckedChanged += new System.EventHandler(this.ckWifi_CheckedChanged);
            //
            // lblModuleIP
            //
            this.lblModuleIP.ActiveLinkColor = System.Drawing.Color.White;
            this.lblModuleIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblModuleIP.ForeColor = System.Drawing.Color.Silver;
            this.lblModuleIP.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.lblModuleIP.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(180)))), ((int)(((byte)(233)))));
            this.lblModuleIP.Location = new System.Drawing.Point(156, 8);
            this.lblModuleIP.Name = "lblModuleIP";
            this.lblModuleIP.Size = new System.Drawing.Size(380, 32);
            this.lblModuleIP.TabIndex = 1;
            this.lblModuleIP.TabStop = false;
            this.lblModuleIP.Text = "Module: --";
            this.lblModuleIP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblModuleIP.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(180)))), ((int)(((byte)(233)))));
            this.lblModuleIP.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblModuleIP_LinkClicked);
            //
            // lblWifiInfo
            //
            this.lblWifiInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.lblWifiInfo.ForeColor = System.Drawing.Color.Silver;
            this.lblWifiInfo.Location = new System.Drawing.Point(8, 44);
            this.lblWifiInfo.Name = "lblWifiInfo";
            this.lblWifiInfo.Size = new System.Drawing.Size(528, 64);
            this.lblWifiInfo.TabIndex = 2;
            this.lblWifiInfo.Text = "Connect the PC to the module\'s access point, a WiFi network, or wired Ethernet (W5" +
    "500). The module\'s own page is at http://192.168.200.1 on its access point.";
            this.lblWifiInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnWifiSetup
            //
            this.btnWifiSetup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWifiSetup.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnWifiSetup.Location = new System.Drawing.Point(8, 114);
            this.btnWifiSetup.Name = "btnWifiSetup";
            this.btnWifiSetup.Size = new System.Drawing.Size(340, 44);
            this.btnWifiSetup.TabIndex = 3;
            this.btnWifiSetup.Text = "Set Up Module WiFi";
            this.btnWifiSetup.UseVisualStyleBackColor = false;
            this.btnWifiSetup.Click += new System.EventHandler(this.btnWifiSetup_Click);
            //
            // lblHotspot
            //
            this.lblHotspot.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblHotspot.ForeColor = System.Drawing.Color.Silver;
            this.lblHotspot.Location = new System.Drawing.Point(8, 170);
            this.lblHotspot.Name = "lblHotspot";
            this.lblHotspot.Size = new System.Drawing.Size(400, 26);
            this.lblHotspot.TabIndex = 4;
            this.lblHotspot.Text = "Tablet Hotspot";
            //
            // btnHotspotOn
            //
            this.btnHotspotOn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnHotspotOn.FlatAppearance.BorderSize = 0;
            this.btnHotspotOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHotspotOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnHotspotOn.ForeColor = System.Drawing.Color.White;
            this.btnHotspotOn.Location = new System.Drawing.Point(8, 200);
            this.btnHotspotOn.Name = "btnHotspotOn";
            this.btnHotspotOn.Size = new System.Drawing.Size(260, 48);
            this.btnHotspotOn.TabIndex = 5;
            this.btnHotspotOn.Text = global::YieldFlo.Language.Lang.lgOn;
            this.btnHotspotOn.UseVisualStyleBackColor = false;
            this.btnHotspotOn.Click += new System.EventHandler(this.btnHotspotOn_Click);
            //
            // btnHotspotOff
            //
            this.btnHotspotOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnHotspotOff.FlatAppearance.BorderSize = 0;
            this.btnHotspotOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHotspotOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnHotspotOff.ForeColor = System.Drawing.Color.White;
            this.btnHotspotOff.Location = new System.Drawing.Point(276, 200);
            this.btnHotspotOff.Name = "btnHotspotOff";
            this.btnHotspotOff.Size = new System.Drawing.Size(260, 48);
            this.btnHotspotOff.TabIndex = 6;
            this.btnHotspotOff.Text = global::YieldFlo.Language.Lang.lgOff;
            this.btnHotspotOff.UseVisualStyleBackColor = false;
            this.btnHotspotOff.Click += new System.EventHandler(this.btnHotspotOff_Click);
            //
            // lblHotspotStatus
            //
            this.lblHotspotStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblHotspotStatus.ForeColor = System.Drawing.Color.Silver;
            this.lblHotspotStatus.Location = new System.Drawing.Point(8, 254);
            this.lblHotspotStatus.Name = "lblHotspotStatus";
            this.lblHotspotStatus.Size = new System.Drawing.Size(528, 40);
            this.lblHotspotStatus.TabIndex = 7;
            this.lblHotspotStatus.Text = "Hotspot: --";
            //
            // tabCan
            //
            this.tabCan.Controls.Add(this.ckCan);
            this.tabCan.Controls.Add(this.lblCanModule);
            this.tabCan.Controls.Add(this.lblCanDriver);
            this.tabCan.Controls.Add(this.cbCanDriver);
            this.tabCan.Controls.Add(this.lblCanPort);
            this.tabCan.Controls.Add(this.cbCanPort);
            this.tabCan.Controls.Add(this.btnRescanPorts);
            this.tabCan.Controls.Add(this.lblCanStatus);
            this.tabCan.Location = new System.Drawing.Point(4, 30);
            this.tabCan.Name = "tabCan";
            this.tabCan.Padding = new System.Windows.Forms.Padding(3);
            this.tabCan.Size = new System.Drawing.Size(544, 302);
            this.tabCan.TabIndex = 1;
            this.tabCan.Text = "CAN";
            this.tabCan.UseVisualStyleBackColor = true;
            //
            // ckCan
            //
            this.ckCan.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.ckCan.Location = new System.Drawing.Point(8, 8);
            this.ckCan.Name = "ckCan";
            this.ckCan.Size = new System.Drawing.Size(140, 32);
            this.ckCan.TabIndex = 0;
            this.ckCan.Text = "Enabled";
            this.ckCan.UseVisualStyleBackColor = false;
            this.ckCan.CheckedChanged += new System.EventHandler(this.ckCan_CheckedChanged);
            //
            // lblCanModule
            //
            this.lblCanModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblCanModule.ForeColor = System.Drawing.Color.Silver;
            this.lblCanModule.Location = new System.Drawing.Point(156, 8);
            this.lblCanModule.Name = "lblCanModule";
            this.lblCanModule.Size = new System.Drawing.Size(380, 32);
            this.lblCanModule.TabIndex = 7;
            this.lblCanModule.Text = "Module: --";
            this.lblCanModule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCanDriver
            //
            this.lblCanDriver.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblCanDriver.ForeColor = System.Drawing.Color.Silver;
            this.lblCanDriver.Location = new System.Drawing.Point(8, 54);
            this.lblCanDriver.Name = "lblCanDriver";
            this.lblCanDriver.Size = new System.Drawing.Size(260, 26);
            this.lblCanDriver.TabIndex = 1;
            this.lblCanDriver.Text = "CAN Driver";
            //
            // cbCanDriver
            //
            this.cbCanDriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCanDriver.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.cbCanDriver.Items.AddRange(new object[] {
            "SLCAN",
            "InnoMaker",
            "PCAN"});
            this.cbCanDriver.Location = new System.Drawing.Point(8, 86);
            this.cbCanDriver.Name = "cbCanDriver";
            this.cbCanDriver.Size = new System.Drawing.Size(260, 32);
            this.cbCanDriver.TabIndex = 2;
            //
            // lblCanPort
            //
            this.lblCanPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblCanPort.ForeColor = System.Drawing.Color.Silver;
            this.lblCanPort.Location = new System.Drawing.Point(276, 54);
            this.lblCanPort.Name = "lblCanPort";
            this.lblCanPort.Size = new System.Drawing.Size(260, 26);
            this.lblCanPort.TabIndex = 3;
            this.lblCanPort.Text = "COM Port";
            //
            // cbCanPort
            //
            this.cbCanPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCanPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.cbCanPort.Location = new System.Drawing.Point(276, 86);
            this.cbCanPort.Name = "cbCanPort";
            this.cbCanPort.Size = new System.Drawing.Size(188, 32);
            this.cbCanPort.TabIndex = 4;
            //
            // btnRescanPorts
            //
            this.btnRescanPorts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnRescanPorts.FlatAppearance.BorderSize = 0;
            this.btnRescanPorts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRescanPorts.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRescanPorts.ForeColor = System.Drawing.Color.White;
            this.btnRescanPorts.Image = ((System.Drawing.Image)(resources.GetObject("btnRescanPorts.Image")));
            this.btnRescanPorts.Location = new System.Drawing.Point(472, 82);
            this.btnRescanPorts.Name = "btnRescanPorts";
            this.btnRescanPorts.Size = new System.Drawing.Size(64, 40);
            this.btnRescanPorts.TabIndex = 5;
            this.btnRescanPorts.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRescanPorts.UseVisualStyleBackColor = false;
            this.btnRescanPorts.Click += new System.EventHandler(this.btnRescanPorts_Click);
            //
            // lblCanStatus
            //
            this.lblCanStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblCanStatus.ForeColor = System.Drawing.Color.Silver;
            this.lblCanStatus.Location = new System.Drawing.Point(8, 132);
            this.lblCanStatus.Name = "lblCanStatus";
            this.lblCanStatus.Size = new System.Drawing.Size(528, 20);
            this.lblCanStatus.TabIndex = 6;
            this.lblCanStatus.Text = "Adapter: --";
            //
            // btnSave
            //
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(110)))), ((int)(((byte)(0)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(8, 354);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(130, 44);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = global::YieldFlo.Language.Lang.lgSave;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnClose
            //
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(422, 354);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(130, 44);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = global::YieldFlo.Language.Lang.lgClose;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // tmrStatus
            //
            this.tmrStatus.Enabled = true;
            this.tmrStatus.Interval = 500;
            this.tmrStatus.Tick += new System.EventHandler(this.tmrStatus_Tick);
            //
            // frmMenuComm
            //
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(564, 460);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuComm";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Module Communication";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmMenuComm_Load);
            this.pnlTitle.ResumeLayout(false);
            this.tabWifi.ResumeLayout(false);
            this.tabCan.ResumeLayout(false);
            this.tabComm.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel    pnlTitle;
        private System.Windows.Forms.Label    lblTitle;
        private System.Windows.Forms.Panel    pnlContent;
        private YieldFlo.Forms.DarkTabControl tabComm;
        private System.Windows.Forms.TabPage  tabWifi;
        private System.Windows.Forms.CheckBox ckWifi;
        private System.Windows.Forms.LinkLabel lblModuleIP;
        private System.Windows.Forms.Label    lblWifiInfo;
        private System.Windows.Forms.Button   btnWifiSetup;
        private System.Windows.Forms.Label    lblHotspot;
        private System.Windows.Forms.Button   btnHotspotOn;
        private System.Windows.Forms.Button   btnHotspotOff;
        private System.Windows.Forms.Label    lblHotspotStatus;
        private System.Windows.Forms.TabPage  tabCan;
        private System.Windows.Forms.CheckBox ckCan;
        private System.Windows.Forms.Label    lblCanModule;
        private System.Windows.Forms.Label    lblCanDriver;
        private System.Windows.Forms.ComboBox cbCanDriver;
        private System.Windows.Forms.Label    lblCanPort;
        private System.Windows.Forms.ComboBox cbCanPort;
        private System.Windows.Forms.Button   btnRescanPorts;
        private System.Windows.Forms.Label    lblCanStatus;
        private System.Windows.Forms.Button   btnSave;
        private System.Windows.Forms.Button   btnClose;
        private System.Windows.Forms.Timer    tmrStatus;
    }
}
