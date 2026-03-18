namespace YieldFlo.Forms
{
    partial class frmMenu
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle   = new System.Windows.Forms.Panel();
            this.lblTitle   = new System.Windows.Forms.Label();
            this.btnTitleClose = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.btnJobs      = new System.Windows.Forms.Button();
            this.btnCrops     = new System.Windows.Forms.Button();
            this.btnHeaders   = new System.Windows.Forms.Button();
            this.btnProfiles  = new System.Windows.Forms.Button();
            this.btnCalibrate = new System.Windows.Forms.Button();
            this.btnSettings  = new System.Windows.Forms.Button();
            this.btnClose     = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = "Menu";
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

            SetMenuButton(btnJobs,      "Jobs",      8,   8);
            SetMenuButton(btnCrops,     "Crops",     238, 8);
            SetMenuButton(btnHeaders,   "Headers",   8,   68);
            SetMenuButton(btnProfiles,  "Profiles",  238, 68);
            SetMenuButton(btnCalibrate, "Calibrate", 8,   128);
            SetMenuButton(btnSettings,  "Settings",  238, 128);

            btnJobs.Click      += new System.EventHandler(this.btnJobs_Click);
            btnCrops.Click     += new System.EventHandler(this.btnCrops_Click);
            btnHeaders.Click   += new System.EventHandler(this.btnHeaders_Click);
            btnProfiles.Click  += new System.EventHandler(this.btnProfiles_Click);
            btnCalibrate.Click += new System.EventHandler(this.btnCalibrate_Click);
            btnSettings.Click  += new System.EventHandler(this.btnSettings_Click);

            this.btnClose.Text      = "Close";
            this.btnClose.Font      = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Size      = new System.Drawing.Size(120, 40);
            this.btnClose.Location  = new System.Drawing.Point(168, 196);
            this.btnClose.Click    += new System.EventHandler(this.btnClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                btnJobs, btnCrops, btnHeaders, btnProfiles, btnCalibrate, btnSettings, btnClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize          = new System.Drawing.Size(456, 284);
            this.MinimumSize         = new System.Drawing.Size(456, 284);
            this.MaximumSize         = new System.Drawing.Size(456, 284);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.None;
            this.Padding             = new System.Windows.Forms.Padding(2);
            this.BackColor           = System.Drawing.Color.White;
            this.TopMost             = true;
            this.ShowInTaskbar       = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name                = "frmMenu";
            this.Text                = "Menu";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenu_Load);

            this.ResumeLayout(false);
        }

        private void SetMenuButton(System.Windows.Forms.Button btn, string text, int x, int y)
        {
            btn.Text      = text;
            btn.Font      = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.Size      = new System.Drawing.Size(210, 52);
            btn.Location  = new System.Drawing.Point(x, y);
        }

        private System.Windows.Forms.Panel  pnlTitle;
        private System.Windows.Forms.Label  lblTitle;
        private System.Windows.Forms.Button btnTitleClose;
        private System.Windows.Forms.Panel  pnlContent;
        private System.Windows.Forms.Button btnJobs;
        private System.Windows.Forms.Button btnCrops;
        private System.Windows.Forms.Button btnHeaders;
        private System.Windows.Forms.Button btnProfiles;
        private System.Windows.Forms.Button btnCalibrate;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnClose;
    }
}
