namespace YieldFlo.Forms
{
    partial class frmWifiSetup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle      = new System.Windows.Forms.Panel();
            this.lblTitle      = new System.Windows.Forms.Label();
            this.pnlContent    = new System.Windows.Forms.Panel();
            this.lblHome       = new System.Windows.Forms.Label();
            this.lblKey        = new System.Windows.Forms.Label();
            this.txtKey        = new System.Windows.Forms.TextBox();
            this.lblModules    = new System.Windows.Forms.Label();
            this.btnScan       = new System.Windows.Forms.Button();
            this.lstModules    = new System.Windows.Forms.ListBox();
            this.lblModuleKey  = new System.Windows.Forms.Label();
            this.txtModuleKey  = new System.Windows.Forms.TextBox();
            this.btnSend       = new System.Windows.Forms.Button();
            this.lblHint       = new System.Windows.Forms.Label();
            this.txtLog        = new System.Windows.Forms.TextBox();
            this.btnClose      = new System.Windows.Forms.Button();

            this.SuspendLayout();

            var labelFont = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            var boldFont  = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            var inputFont = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);

            // ── Title bar ─────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 48;

            this.lblTitle.Text      = "Module WiFi Setup";
            this.lblTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.pnlTitle.Controls.Add(this.lblTitle);

            // ── Content ───────────────────────────────────────────────────────
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;

            this.lblHome.Font      = labelFont;
            this.lblHome.AutoSize  = false;
            this.lblHome.Location  = new System.Drawing.Point(8, 6);
            this.lblHome.Size      = new System.Drawing.Size(536, 42);
            this.lblHome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblHome.Text      = "";

            this.lblKey.Font      = labelFont;
            this.lblKey.AutoSize  = false;
            this.lblKey.Location  = new System.Drawing.Point(8, 54);
            this.lblKey.Size      = new System.Drawing.Size(180, 32);
            this.lblKey.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblKey.Text      = "Network password";

            this.txtKey.Font        = inputFont;
            this.txtKey.Location    = new System.Drawing.Point(192, 54);
            this.txtKey.Size        = new System.Drawing.Size(352, 32);
            this.txtKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblModules.Font      = labelFont;
            this.lblModules.AutoSize  = false;
            this.lblModules.Location  = new System.Drawing.Point(8, 96);
            this.lblModules.Size      = new System.Drawing.Size(280, 32);
            this.lblModules.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblModules.Text      = "Modules in range";

            this.btnScan.Font      = boldFont;
            this.btnScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScan.Location  = new System.Drawing.Point(414, 92);
            this.btnScan.Size      = new System.Drawing.Size(130, 40);
            this.btnScan.Text      = "Scan";

            this.lstModules.Font          = inputFont;
            this.lstModules.Location      = new System.Drawing.Point(8, 134);
            this.lstModules.Size          = new System.Drawing.Size(536, 100);
            this.lstModules.BorderStyle   = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstModules.IntegralHeight = false;

            this.lblModuleKey.Font      = labelFont;
            this.lblModuleKey.AutoSize  = false;
            this.lblModuleKey.Location  = new System.Drawing.Point(8, 242);
            this.lblModuleKey.Size      = new System.Drawing.Size(180, 32);
            this.lblModuleKey.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblModuleKey.Text      = "Hotspot password";

            this.txtModuleKey.Font        = inputFont;
            this.txtModuleKey.Location    = new System.Drawing.Point(192, 242);
            this.txtModuleKey.Size        = new System.Drawing.Size(352, 32);
            this.txtModuleKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.btnSend.Font      = boldFont;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Location  = new System.Drawing.Point(8, 284);
            this.btnSend.Size      = new System.Drawing.Size(260, 48);
            this.btnSend.Text      = "Set Up Module";

            this.lblHint.Font      = labelFont;
            this.lblHint.AutoSize  = false;
            this.lblHint.Location  = new System.Drawing.Point(276, 284);
            this.lblHint.Size      = new System.Drawing.Size(268, 48);
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblHint.Text      = "The PC leaves the network for about a minute. Do this when not harvesting.";

            this.txtLog.Font       = new System.Drawing.Font("Consolas", 10F);
            this.txtLog.Location   = new System.Drawing.Point(8, 340);
            this.txtLog.Size       = new System.Drawing.Size(536, 108);
            this.txtLog.Multiline  = true;
            this.txtLog.ReadOnly   = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLog.TabStop    = false;

            this.btnClose.Font      = boldFont;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location  = new System.Drawing.Point(414, 456);
            this.btnClose.Size      = new System.Drawing.Size(130, 44);
            this.btnClose.Text      = "Close";

            // Explicit, because tab order decides where focus lands when a control is
            // disabled mid-run — and landing on a keyboard-wired box pops the keyboard.
            this.txtKey.TabIndex       = 0;
            this.btnScan.TabIndex      = 1;
            this.lstModules.TabIndex   = 2;
            this.txtModuleKey.TabIndex = 3;
            this.btnSend.TabIndex      = 4;
            this.btnClose.TabIndex     = 5;
            this.txtLog.TabIndex       = 6;

            this.pnlContent.Controls.Add(this.lblHome);
            this.pnlContent.Controls.Add(this.lblKey);
            this.pnlContent.Controls.Add(this.txtKey);
            this.pnlContent.Controls.Add(this.lblModules);
            this.pnlContent.Controls.Add(this.btnScan);
            this.pnlContent.Controls.Add(this.lstModules);
            this.pnlContent.Controls.Add(this.lblModuleKey);
            this.pnlContent.Controls.Add(this.txtModuleKey);
            this.pnlContent.Controls.Add(this.btnSend);
            this.pnlContent.Controls.Add(this.lblHint);
            this.pnlContent.Controls.Add(this.txtLog);
            this.pnlContent.Controls.Add(this.btnClose);

            this.btnScan.Click  += new System.EventHandler(this.btnScan_Click);
            this.btnSend.Click  += new System.EventHandler(this.btnSend_Click);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // ── Form ──────────────────────────────────────────────────────────
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.ClientSize      = new System.Drawing.Size(556, 560);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost         = true;
            this.Name            = "frmWifiSetup";
            this.Text            = "Module WiFi Setup";
            this.Load           += new System.EventHandler(this.frmWifiSetup_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblHome;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label lblModules;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.ListBox lstModules;
        private System.Windows.Forms.Label lblModuleKey;
        private System.Windows.Forms.TextBox txtModuleKey;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnClose;
    }
}
