namespace YieldFlo.Forms
{
    partial class frmMenuSettings
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
            this.lblUnits         = new System.Windows.Forms.Label();
            this.btnImperial      = new System.Windows.Forms.Button();
            this.btnMetric        = new System.Windows.Forms.Button();
            this.lblTheme         = new System.Windows.Forms.Label();
            this.btnDark          = new System.Windows.Forms.Button();
            this.btnLight         = new System.Windows.Forms.Button();
            this.lblNetwork       = new System.Windows.Forms.Label();
            this.btnEthernet      = new System.Windows.Forms.Button();
            this.btnCAN           = new System.Windows.Forms.Button();
            this.lblSubnet        = new System.Windows.Forms.Label();
            this.cbSubnet         = new System.Windows.Forms.ComboBox();
            this.lblCanDriver     = new System.Windows.Forms.Label();
            this.cbCanDriver      = new System.Windows.Forms.ComboBox();
            this.lblCanPort       = new System.Windows.Forms.Label();
            this.cbCanPort        = new System.Windows.Forms.ComboBox();
            this.btnSaveSettings  = new System.Windows.Forms.Button();
            this.btnSettingsClose = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ─────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = "Settings";
            this.lblTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
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

            // Units
            this.lblUnits.Text      = "Units";
            this.lblUnits.Font      = lf;
            this.lblUnits.ForeColor = System.Drawing.Color.Silver;
            this.lblUnits.AutoSize  = false;
            this.lblUnits.Location  = new System.Drawing.Point(8, 8);
            this.lblUnits.Size      = new System.Drawing.Size(200, 16);

            SetToggleButton(this.btnImperial, "Imperial", 8, 26);
            this.btnImperial.Click += new System.EventHandler(this.btnImperial_Click);

            SetToggleButton(this.btnMetric, "Metric", 226, 26);
            this.btnMetric.Click += new System.EventHandler(this.btnMetric_Click);

            // Theme
            this.lblTheme.Text      = "Theme";
            this.lblTheme.Font      = lf;
            this.lblTheme.ForeColor = System.Drawing.Color.Silver;
            this.lblTheme.AutoSize  = false;
            this.lblTheme.Location  = new System.Drawing.Point(8, 80);
            this.lblTheme.Size      = new System.Drawing.Size(200, 16);

            SetToggleButton(this.btnDark, "Dark", 8, 98);
            this.btnDark.Click += new System.EventHandler(this.btnDark_Click);

            SetToggleButton(this.btnLight, "Light", 226, 98);
            this.btnLight.Click += new System.EventHandler(this.btnLight_Click);

            // Module Communication
            this.lblNetwork.Text      = "Module Communication";
            this.lblNetwork.Font      = lf;
            this.lblNetwork.ForeColor = System.Drawing.Color.Silver;
            this.lblNetwork.AutoSize  = false;
            this.lblNetwork.Location  = new System.Drawing.Point(8, 152);
            this.lblNetwork.Size      = new System.Drawing.Size(300, 16);

            SetToggleButton(this.btnEthernet, "Ethernet", 8, 170);
            this.btnEthernet.Click += new System.EventHandler(this.btnEthernet_Click);

            SetToggleButton(this.btnCAN, "CAN", 226, 170);
            this.btnCAN.Click += new System.EventHandler(this.btnCAN_Click);

            // Ethernet: subnet selector
            this.lblSubnet.Text      = "Local Subnet";
            this.lblSubnet.Font      = lf;
            this.lblSubnet.ForeColor = System.Drawing.Color.Silver;
            this.lblSubnet.AutoSize  = false;
            this.lblSubnet.Location  = new System.Drawing.Point(8, 224);
            this.lblSubnet.Size      = new System.Drawing.Size(200, 16);

            this.cbSubnet.Font          = vf;
            this.cbSubnet.Location      = new System.Drawing.Point(8, 242);
            this.cbSubnet.Size          = new System.Drawing.Size(430, 26);
            this.cbSubnet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // CAN: driver
            this.lblCanDriver.Text      = "CAN Driver";
            this.lblCanDriver.Font      = lf;
            this.lblCanDriver.ForeColor = System.Drawing.Color.Silver;
            this.lblCanDriver.AutoSize  = false;
            this.lblCanDriver.Location  = new System.Drawing.Point(8, 224);
            this.lblCanDriver.Size      = new System.Drawing.Size(200, 16);

            this.cbCanDriver.Font          = vf;
            this.cbCanDriver.Location      = new System.Drawing.Point(8, 242);
            this.cbCanDriver.Size          = new System.Drawing.Size(200, 26);
            this.cbCanDriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCanDriver.Items.AddRange(new object[] { "SLCAN", "InnoMaker", "PCAN" });

            // CAN: port
            this.lblCanPort.Text      = "COM Port";
            this.lblCanPort.Font      = lf;
            this.lblCanPort.ForeColor = System.Drawing.Color.Silver;
            this.lblCanPort.AutoSize  = false;
            this.lblCanPort.Location  = new System.Drawing.Point(228, 224);
            this.lblCanPort.Size      = new System.Drawing.Size(180, 16);

            this.cbCanPort.Font          = vf;
            this.cbCanPort.Location      = new System.Drawing.Point(228, 242);
            this.cbCanPort.Size          = new System.Drawing.Size(210, 26);
            this.cbCanPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // Save & Apply button
            this.btnSaveSettings.Text      = "Save && Apply";
            this.btnSaveSettings.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveSettings.FlatAppearance.BorderSize = 0;
            this.btnSaveSettings.Size      = new System.Drawing.Size(160, 36);
            this.btnSaveSettings.Location  = new System.Drawing.Point(8, 284);
            this.btnSaveSettings.Click    += new System.EventHandler(this.btnSaveSettings_Click);

            // Close button
            this.btnSettingsClose.Text      = "Close";
            this.btnSettingsClose.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnSettingsClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettingsClose.FlatAppearance.BorderSize = 0;
            this.btnSettingsClose.Size      = new System.Drawing.Size(112, 36);
            this.btnSettingsClose.Location  = new System.Drawing.Point(336, 284);
            this.btnSettingsClose.Click    += new System.EventHandler(this.btnSettingsClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblUnits, btnImperial, btnMetric,
                lblTheme, btnDark, btnLight,
                lblNetwork, btnEthernet, btnCAN,
                lblSubnet, cbSubnet,
                lblCanDriver, cbCanDriver,
                lblCanPort, cbCanPort,
                btnSaveSettings, btnSettingsClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 364);
            this.MinimumSize     = new System.Drawing.Size(456, 364);
            this.MaximumSize     = new System.Drawing.Size(456, 364);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name            = "frmMenuSettings";
            this.Text            = "Settings";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuSettings_Load);

            this.ResumeLayout(false);
        }

        private void SetToggleButton(System.Windows.Forms.Button btn, string text, int x, int y)
        {
            btn.Text      = text;
            btn.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Size      = new System.Drawing.Size(210, 44);
            btn.Location  = new System.Drawing.Point(x, y);
            btn.ForeColor = System.Drawing.Color.White;
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
        private System.Windows.Forms.Label    lblSubnet;
        private System.Windows.Forms.ComboBox cbSubnet;
        private System.Windows.Forms.Label    lblCanDriver;
        private System.Windows.Forms.ComboBox cbCanDriver;
        private System.Windows.Forms.Label    lblCanPort;
        private System.Windows.Forms.ComboBox cbCanPort;
        private System.Windows.Forms.Button   btnSaveSettings;
        private System.Windows.Forms.Button   btnSettingsClose;
    }
}
