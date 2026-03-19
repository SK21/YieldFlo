namespace YieldFlo.Forms
{
    partial class frmMenuJobs
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
            this.btnTitleClose = new System.Windows.Forms.Button();
            this.pnlContent    = new System.Windows.Forms.Panel();
            this.lblJobName    = new System.Windows.Forms.Label();
            this.txtJobName    = new System.Windows.Forms.TextBox();
            this.lblCropLabel  = new System.Windows.Forms.Label();
            this.cboCrop       = new System.Windows.Forms.ComboBox();
            this.lblHeaderLabel  = new System.Windows.Forms.Label();
            this.cboHeader     = new System.Windows.Forms.ComboBox();
            this.lblProfileLabel = new System.Windows.Forms.Label();
            this.cboProfile    = new System.Windows.Forms.ComboBox();
            this.btnStart      = new System.Windows.Forms.Button();
            this.btnJobsClose  = new System.Windows.Forms.Button();
            this.btnNew    = new System.Windows.Forms.Button();
            this.btnLoad   = new System.Windows.Forms.Button();
            this.btnSave   = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblRecent     = new System.Windows.Forms.Label();
            this.lvJobs        = new System.Windows.Forms.ListView();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = "Start Job";
            this.lblTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.AutoSize  = false;
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

            var labelFont  = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            var inputFont  = new System.Drawing.Font("Microsoft Sans Serif", 9F);

            // Job Name
            this.lblJobName.Text     = "Job Name:"; this.lblJobName.Font = labelFont; this.lblJobName.Location = new System.Drawing.Point(8, 13); this.lblJobName.Size = new System.Drawing.Size(96, 20); this.lblJobName.AutoSize = false;
            this.txtJobName.Font     = inputFont;
            this.txtJobName.Location = new System.Drawing.Point(112, 8);
            this.txtJobName.Size     = new System.Drawing.Size(334, 24);

            // Crop
            this.lblCropLabel.Text = "Crop:"; this.lblCropLabel.Font = labelFont; this.lblCropLabel.Location = new System.Drawing.Point(8, 45); this.lblCropLabel.Size = new System.Drawing.Size(96, 20); this.lblCropLabel.AutoSize = false;
            this.cboCrop.Font = inputFont; this.cboCrop.Location = new System.Drawing.Point(112, 40); this.cboCrop.Size = new System.Drawing.Size(334, 24); this.cboCrop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // Header
            this.lblHeaderLabel.Text = "Header:"; this.lblHeaderLabel.Font = labelFont; this.lblHeaderLabel.Location = new System.Drawing.Point(8, 77); this.lblHeaderLabel.Size = new System.Drawing.Size(96, 20); this.lblHeaderLabel.AutoSize = false;
            this.cboHeader.Font = inputFont; this.cboHeader.Location = new System.Drawing.Point(112, 72); this.cboHeader.Size = new System.Drawing.Size(334, 24); this.cboHeader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // Profile
            this.lblProfileLabel.Text = "Profile:"; this.lblProfileLabel.Font = labelFont; this.lblProfileLabel.Location = new System.Drawing.Point(8, 109); this.lblProfileLabel.Size = new System.Drawing.Size(96, 20); this.lblProfileLabel.AutoSize = false;
            this.cboProfile.Font = inputFont; this.cboProfile.Location = new System.Drawing.Point(112, 104); this.cboProfile.Size = new System.Drawing.Size(334, 24); this.cboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // Buttons
            this.btnStart.Text      = "▶  Start Job";
            this.btnStart.Font      = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Size      = new System.Drawing.Size(200, 42);
            this.btnStart.Location  = new System.Drawing.Point(8, 148);
            this.btnStart.Click    += new System.EventHandler(this.btnStart_Click);

            this.btnJobsClose.Text      = "Close";
            this.btnJobsClose.Font      = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnJobsClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobsClose.Size      = new System.Drawing.Size(120, 42);
            this.btnJobsClose.Location  = new System.Drawing.Point(326, 148);
            this.btnJobsClose.Click    += new System.EventHandler(this.btnJobsClose_Click);

            // Recent jobs
            this.lblRecent.Text      = "Recent jobs:";
            this.lblRecent.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblRecent.ForeColor = System.Drawing.Color.Silver;
            this.lblRecent.AutoSize  = true;
            this.lblRecent.Location  = new System.Drawing.Point(8, 202);

            this.lvJobs.SelectedIndexChanged += new System.EventHandler(this.lvJobs_SelectedIndexChanged);
            this.lvJobs.Location      = new System.Drawing.Point(4, 222);
            this.lvJobs.Size          = new System.Drawing.Size(448, 150);
            this.lvJobs.View          = System.Windows.Forms.View.Details;
            this.lvJobs.FullRowSelect = true;
            this.lvJobs.HeaderStyle   = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvJobs.Font          = inputFont;
            this.lvJobs.Columns.Add("Job Name",  160);
            this.lvJobs.Columns.Add("Status",     80);
            this.lvJobs.Columns.Add("Date",        90);
            this.lvJobs.Columns.Add("Acres",       70);

            // ── Bottom row: New / Load / Save / Delete ────────────────────────
            this.btnNew.Text      = "New";
            this.btnNew.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Size      = new System.Drawing.Size(106, 34);
            this.btnNew.Location  = new System.Drawing.Point(4, 378);
            this.btnNew.Click    += new System.EventHandler(this.btnNew_Click);

            this.btnLoad.Text      = "Load";
            this.btnLoad.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Size      = new System.Drawing.Size(106, 34);
            this.btnLoad.Location  = new System.Drawing.Point(114, 378);
            this.btnLoad.Enabled   = false;
            this.btnLoad.Click    += new System.EventHandler(this.btnLoad_Click);

            this.btnSave.Text      = "Save";
            this.btnSave.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Size      = new System.Drawing.Size(106, 34);
            this.btnSave.Location  = new System.Drawing.Point(224, 378);
            this.btnSave.Enabled   = false;
            this.btnSave.Click    += new System.EventHandler(this.btnSave_Click);

            this.btnDelete.Text      = "Delete";
            this.btnDelete.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Size      = new System.Drawing.Size(106, 34);
            this.btnDelete.Location  = new System.Drawing.Point(334, 378);
            this.btnDelete.Enabled   = false;
            this.btnDelete.Click    += new System.EventHandler(this.btnDelete_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblJobName, txtJobName, lblCropLabel, cboCrop,
                lblHeaderLabel, cboHeader, lblProfileLabel, cboProfile,
                btnStart, btnJobsClose, lblRecent, lvJobs,
                btnNew, btnLoad, btnSave, btnDelete });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 458);
            this.MinimumSize     = new System.Drawing.Size(456, 458);
            this.MaximumSize     = new System.Drawing.Size(456, 458);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name            = "frmMenuJobs";
            this.Text            = "Start Job";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuJobs_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel    pnlTitle;
        private System.Windows.Forms.Label    lblTitle;
        private System.Windows.Forms.Button   btnTitleClose;
        private System.Windows.Forms.Panel    pnlContent;
        private System.Windows.Forms.Label    lblJobName;
        private System.Windows.Forms.TextBox  txtJobName;
        private System.Windows.Forms.Label    lblCropLabel;
        private System.Windows.Forms.ComboBox cboCrop;
        private System.Windows.Forms.Label    lblHeaderLabel;
        private System.Windows.Forms.ComboBox cboHeader;
        private System.Windows.Forms.Label    lblProfileLabel;
        private System.Windows.Forms.ComboBox cboProfile;
        private System.Windows.Forms.Button   btnStart;
        private System.Windows.Forms.Button   btnJobsClose;
        private System.Windows.Forms.Button   btnNew;
        private System.Windows.Forms.Button   btnLoad;
        private System.Windows.Forms.Button   btnSave;
        private System.Windows.Forms.Button   btnDelete;
        private System.Windows.Forms.Label    lblRecent;
        private System.Windows.Forms.ListView lvJobs;
    }
}
