using YieldFlo.Language;

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
            this.pnlTitle    = new System.Windows.Forms.Panel();
            this.lblTitle    = new System.Windows.Forms.Label();
            this.btnTitleClose = new System.Windows.Forms.Button();
            this.pnlContent  = new System.Windows.Forms.Panel();
            this.btnJobs      = new System.Windows.Forms.Button();
            this.btnCrops     = new System.Windows.Forms.Button();
            this.btnHeaders   = new System.Windows.Forms.Button();
            this.btnProfiles  = new System.Windows.Forms.Button();
            this.btnCalibrate = new System.Windows.Forms.Button();
            this.btnSettings  = new System.Windows.Forms.Button();
            this.btnFields    = new System.Windows.Forms.Button();
            this.btnReports   = new System.Windows.Forms.Button();
            this.btnYieldMap  = new System.Windows.Forms.Button();
            this.btnSensorCal = new System.Windows.Forms.Button();
            this.btnLanguage  = new System.Windows.Forms.Button();
            this.btnClose     = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = Lang.lgTitleMenu;
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

            // Row 1 — Jobs (full width, most used)
            this.btnJobs.Text = Lang.lgTitleJobs; this.btnJobs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold); this.btnJobs.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnJobs.Size = new System.Drawing.Size(436, 52); this.btnJobs.Location = new System.Drawing.Point(8, 8);
            // Row 2 — Map / reporting
            this.btnYieldMap.Text = Lang.lgTitleYieldMap; this.btnYieldMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold); this.btnYieldMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnYieldMap.Size = new System.Drawing.Size(210, 52); this.btnYieldMap.Location = new System.Drawing.Point(8, 68);
            this.btnReports.Text = Lang.lgReports; this.btnReports.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold); this.btnReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnReports.Size = new System.Drawing.Size(210, 52); this.btnReports.Location = new System.Drawing.Point(238, 68);
            // Row 3 — Calibration
            this.btnCalibrate.Text = Lang.lgTitleYieldCal; this.btnCalibrate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold); this.btnCalibrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnCalibrate.Size = new System.Drawing.Size(210, 52); this.btnCalibrate.Location = new System.Drawing.Point(8, 128);
            this.btnSensorCal.Text = Lang.lgTitleMoistureCal; this.btnSensorCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold); this.btnSensorCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnSensorCal.Size = new System.Drawing.Size(210, 52); this.btnSensorCal.Location = new System.Drawing.Point(238, 128);
            // Row 4 — Setup
            this.btnCrops.Text = Lang.lgTitleCrops; this.btnCrops.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold); this.btnCrops.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnCrops.Size = new System.Drawing.Size(210, 52); this.btnCrops.Location = new System.Drawing.Point(8, 188);
            this.btnHeaders.Text = Lang.lgTitleHeaders; this.btnHeaders.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold); this.btnHeaders.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnHeaders.Size = new System.Drawing.Size(210, 52); this.btnHeaders.Location = new System.Drawing.Point(238, 188);
            // Row 5 — Setup
            this.btnFields.Text = Lang.lgTitleFields; this.btnFields.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold); this.btnFields.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnFields.Size = new System.Drawing.Size(210, 52); this.btnFields.Location = new System.Drawing.Point(8, 248);
            this.btnProfiles.Text = Lang.lgTitleProfiles; this.btnProfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold); this.btnProfiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnProfiles.Size = new System.Drawing.Size(210, 52); this.btnProfiles.Location = new System.Drawing.Point(238, 248);

            btnJobs.Click      += new System.EventHandler(this.btnJobs_Click);
            btnCrops.Click     += new System.EventHandler(this.btnCrops_Click);
            btnHeaders.Click   += new System.EventHandler(this.btnHeaders_Click);
            btnProfiles.Click  += new System.EventHandler(this.btnProfiles_Click);
            btnCalibrate.Click += new System.EventHandler(this.btnCalibrate_Click);
            btnSettings.Click  += new System.EventHandler(this.btnSettings_Click);
            btnFields.Click    += new System.EventHandler(this.btnFields_Click);
            btnReports.Click   += new System.EventHandler(this.btnReports_Click);
            btnYieldMap.Click  += new System.EventHandler(this.btnYieldMap_Click);
            btnSensorCal.Click += new System.EventHandler(this.btnSensorCal_Click);
            btnLanguage.Click  += new System.EventHandler(this.btnLanguage_Click);

            // Row 6 — Language (full width)
            this.btnLanguage.Text = Lang.lgLanguage; this.btnLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold); this.btnLanguage.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnLanguage.Size = new System.Drawing.Size(436, 44); this.btnLanguage.Location = new System.Drawing.Point(8, 308);

            // Row 7 — Settings | Close
            this.btnSettings.Text = Lang.lgTitleSettings; this.btnSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold); this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnSettings.Size = new System.Drawing.Size(210, 44); this.btnSettings.Location = new System.Drawing.Point(8, 360);

            this.btnClose.Text      = Lang.lgClose;
            this.btnClose.Font      = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Size      = new System.Drawing.Size(210, 44);
            this.btnClose.Location  = new System.Drawing.Point(238, 360);
            this.btnClose.Click    += new System.EventHandler(this.btnClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                btnJobs, btnCrops, btnHeaders, btnProfiles, btnCalibrate, btnSettings,
                btnFields, btnReports, btnYieldMap, btnSensorCal, btnLanguage, btnClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize          = new System.Drawing.Size(456, 460);
            this.MinimumSize         = new System.Drawing.Size(456, 460);
            this.MaximumSize         = new System.Drawing.Size(456, 460);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.None;
            this.Padding             = new System.Windows.Forms.Padding(2);
            this.BackColor           = System.Drawing.Color.White;
            this.TopMost             = true;
            this.ShowInTaskbar       = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font                = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name                = "frmMenu";
            this.Text                = "Menu";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenu_Load);

            this.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnFields;
        private System.Windows.Forms.Button btnReports;
        private System.Windows.Forms.Button btnYieldMap;
        private System.Windows.Forms.Button btnSensorCal;
        private System.Windows.Forms.Button btnLanguage;
        private System.Windows.Forms.Button btnClose;
    }
}
