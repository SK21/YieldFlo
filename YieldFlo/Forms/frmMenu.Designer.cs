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
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnTitleClose = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.btnJobs = new System.Windows.Forms.Button();
            this.btnCrops = new System.Windows.Forms.Button();
            this.btnHeaders = new System.Windows.Forms.Button();
            this.btnProfiles = new System.Windows.Forms.Button();
            this.btnCalibrate = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnFields = new System.Windows.Forms.Button();
            this.btnReports = new System.Windows.Forms.Button();
            this.btnYieldMap = new System.Windows.Forms.Button();
            this.btnSensorCal = new System.Windows.Forms.Button();
            this.btnLanguage = new System.Windows.Forms.Button();
            this.pnlTitle.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTitle
            // 
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Controls.Add(this.btnHelp);
            this.pnlTitle.Controls.Add(this.btnTitleClose);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(2, 2);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(452, 40);
            this.pnlTitle.TabIndex = 1;
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
            this.lblTitle.Text = "Menu";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnHelp
            // 
            this.btnHelp.FlatAppearance.BorderSize = 0;
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnHelp.Location = new System.Drawing.Point(376, 5);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(36, 30);
            this.btnHelp.TabIndex = 1;
            this.btnHelp.Text = "?";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnTitleClose
            // 
            this.btnTitleClose.FlatAppearance.BorderSize = 0;
            this.btnTitleClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTitleClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.btnTitleClose.Location = new System.Drawing.Point(418, 5);
            this.btnTitleClose.Name = "btnTitleClose";
            this.btnTitleClose.Size = new System.Drawing.Size(36, 30);
            this.btnTitleClose.TabIndex = 2;
            this.btnTitleClose.Text = "×";
            this.btnTitleClose.Click += new System.EventHandler(this.btnTitleClose_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.btnJobs);
            this.pnlContent.Controls.Add(this.btnCrops);
            this.pnlContent.Controls.Add(this.btnHeaders);
            this.pnlContent.Controls.Add(this.btnProfiles);
            this.pnlContent.Controls.Add(this.btnCalibrate);
            this.pnlContent.Controls.Add(this.btnSettings);
            this.pnlContent.Controls.Add(this.btnFields);
            this.pnlContent.Controls.Add(this.btnReports);
            this.pnlContent.Controls.Add(this.btnYieldMap);
            this.pnlContent.Controls.Add(this.btnSensorCal);
            this.pnlContent.Controls.Add(this.btnLanguage);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(2, 42);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(452, 360);
            this.pnlContent.TabIndex = 0;
            // 
            // btnJobs
            // 
            this.btnJobs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnJobs.Location = new System.Drawing.Point(8, 8);
            this.btnJobs.Name = "btnJobs";
            this.btnJobs.Size = new System.Drawing.Size(436, 52);
            this.btnJobs.TabIndex = 0;
            this.btnJobs.Text = global::YieldFlo.Language.Lang.lgTitleJobs;
            this.btnJobs.Click += new System.EventHandler(this.btnJobs_Click);
            // 
            // btnCrops
            // 
            this.btnCrops.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrops.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnCrops.Location = new System.Drawing.Point(8, 188);
            this.btnCrops.Name = "btnCrops";
            this.btnCrops.Size = new System.Drawing.Size(210, 52);
            this.btnCrops.TabIndex = 1;
            this.btnCrops.Text = global::YieldFlo.Language.Lang.lgTitleCrops;
            this.btnCrops.Click += new System.EventHandler(this.btnCrops_Click);
            // 
            // btnHeaders
            // 
            this.btnHeaders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeaders.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnHeaders.Location = new System.Drawing.Point(234, 188);
            this.btnHeaders.Name = "btnHeaders";
            this.btnHeaders.Size = new System.Drawing.Size(210, 52);
            this.btnHeaders.TabIndex = 2;
            this.btnHeaders.Text = global::YieldFlo.Language.Lang.lgTitleHeaders;
            this.btnHeaders.Click += new System.EventHandler(this.btnHeaders_Click);
            // 
            // btnProfiles
            // 
            this.btnProfiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnProfiles.Location = new System.Drawing.Point(234, 248);
            this.btnProfiles.Name = "btnProfiles";
            this.btnProfiles.Size = new System.Drawing.Size(210, 52);
            this.btnProfiles.TabIndex = 3;
            this.btnProfiles.Text = global::YieldFlo.Language.Lang.lgTitleProfiles;
            this.btnProfiles.Click += new System.EventHandler(this.btnProfiles_Click);
            // 
            // btnCalibrate
            // 
            this.btnCalibrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalibrate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnCalibrate.Location = new System.Drawing.Point(8, 128);
            this.btnCalibrate.Name = "btnCalibrate";
            this.btnCalibrate.Size = new System.Drawing.Size(210, 52);
            this.btnCalibrate.TabIndex = 4;
            this.btnCalibrate.Text = global::YieldFlo.Language.Lang.lgTitleYieldCal;
            this.btnCalibrate.Click += new System.EventHandler(this.btnCalibrate_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnSettings.Location = new System.Drawing.Point(8, 308);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(210, 44);
            this.btnSettings.TabIndex = 5;
            this.btnSettings.Text = global::YieldFlo.Language.Lang.lgTitleSettings;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnFields
            // 
            this.btnFields.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFields.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnFields.Location = new System.Drawing.Point(8, 248);
            this.btnFields.Name = "btnFields";
            this.btnFields.Size = new System.Drawing.Size(210, 52);
            this.btnFields.TabIndex = 6;
            this.btnFields.Text = global::YieldFlo.Language.Lang.lgTitleFields;
            this.btnFields.Click += new System.EventHandler(this.btnFields_Click);
            // 
            // btnReports
            // 
            this.btnReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReports.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnReports.Location = new System.Drawing.Point(234, 68);
            this.btnReports.Name = "btnReports";
            this.btnReports.Size = new System.Drawing.Size(210, 52);
            this.btnReports.TabIndex = 7;
            this.btnReports.Text = global::YieldFlo.Language.Lang.lgReports;
            this.btnReports.Click += new System.EventHandler(this.btnReports_Click);
            // 
            // btnYieldMap
            // 
            this.btnYieldMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYieldMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnYieldMap.Location = new System.Drawing.Point(8, 68);
            this.btnYieldMap.Name = "btnYieldMap";
            this.btnYieldMap.Size = new System.Drawing.Size(210, 52);
            this.btnYieldMap.TabIndex = 8;
            this.btnYieldMap.Text = global::YieldFlo.Language.Lang.lgTitleYieldMap;
            this.btnYieldMap.Click += new System.EventHandler(this.btnYieldMap_Click);
            // 
            // btnSensorCal
            // 
            this.btnSensorCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSensorCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnSensorCal.Location = new System.Drawing.Point(234, 128);
            this.btnSensorCal.Name = "btnSensorCal";
            this.btnSensorCal.Size = new System.Drawing.Size(210, 52);
            this.btnSensorCal.TabIndex = 9;
            this.btnSensorCal.Text = global::YieldFlo.Language.Lang.lgTitleMoistureCal;
            this.btnSensorCal.Click += new System.EventHandler(this.btnSensorCal_Click);
            // 
            // btnLanguage
            // 
            this.btnLanguage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnLanguage.Location = new System.Drawing.Point(234, 308);
            this.btnLanguage.Name = "btnLanguage";
            this.btnLanguage.Size = new System.Drawing.Size(210, 44);
            this.btnLanguage.TabIndex = 10;
            this.btnLanguage.Text = global::YieldFlo.Language.Lang.lgLanguage;
            this.btnLanguage.Click += new System.EventHandler(this.btnLanguage_Click);
            // 
            // frmMenu
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(456, 404);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenu";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmMenu_Load);
            this.pnlTitle.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel  pnlTitle;
        private System.Windows.Forms.Label  lblTitle;
        private System.Windows.Forms.Button btnHelp;
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
    }
}
