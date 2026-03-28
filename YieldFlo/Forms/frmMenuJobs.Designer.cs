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
            this.pnlTitle        = new System.Windows.Forms.Panel();
            this.lblTitle        = new System.Windows.Forms.Label();
            this.btnTitleClose   = new System.Windows.Forms.Button();
            this.pnlContent      = new System.Windows.Forms.Panel();
            this.lvJobs          = new System.Windows.Forms.ListView();
            this.lblJobName      = new System.Windows.Forms.Label();
            this.txtJobName      = new System.Windows.Forms.TextBox();
            this.lblCropLabel    = new System.Windows.Forms.Label();
            this.cboCrop         = new System.Windows.Forms.ComboBox();
            this.lblHeaderLabel  = new System.Windows.Forms.Label();
            this.cboHeader       = new System.Windows.Forms.ComboBox();
            this.lblProfileLabel = new System.Windows.Forms.Label();
            this.cboProfile      = new System.Windows.Forms.ComboBox();
            this.lblFieldLabel   = new System.Windows.Forms.Label();
            this.cboField        = new System.Windows.Forms.ComboBox();
            this.btnNew          = new System.Windows.Forms.Button();
            this.btnLoad         = new System.Windows.Forms.Button();
            this.btnSave         = new System.Windows.Forms.Button();
            this.btnDelete       = new System.Windows.Forms.Button();
            this.btnJobsClose    = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = "Jobs";
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

            var labelFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            var inputFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);

            // ── List (top) ────────────────────────────────────────────────────
            this.lvJobs.Location    = new System.Drawing.Point(4, 4);
            this.lvJobs.Size        = new System.Drawing.Size(448, 150);
            this.lvJobs.View        = System.Windows.Forms.View.Details;
            this.lvJobs.FullRowSelect   = true;
            this.lvJobs.HideSelection   = false;
            this.lvJobs.HeaderStyle     = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvJobs.BorderStyle     = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvJobs.Font        = inputFont;
            this.lvJobs.Columns.Add("Job Name", 130);
            this.lvJobs.Columns.Add("Status",    56);
            this.lvJobs.Columns.Add("Date",      82);
            this.lvJobs.Columns.Add("Acres",     56);
            this.lvJobs.Columns.Add("Field",    100);
            this.lvJobs.OwnerDraw = true;
            this.lvJobs.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lvJobs_DrawColumnHeader);
            this.lvJobs.DrawSubItem      += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lvJobs_DrawSubItem);
            this.lvJobs.SelectedIndexChanged += new System.EventHandler(this.lvJobs_SelectedIndexChanged);
            this.lvJobs.ColumnClick          += new System.Windows.Forms.ColumnClickEventHandler(this.lvJobs_ColumnClick);

            // ── Edit fields ───────────────────────────────────────────────────
            this.lblJobName.Text     = "Job Name:"; this.lblJobName.Font = labelFont;
            this.lblJobName.Location = new System.Drawing.Point(8, 163); this.lblJobName.Size = new System.Drawing.Size(100, 20); this.lblJobName.AutoSize = false;
            this.txtJobName.Font     = inputFont;
            this.txtJobName.Location = new System.Drawing.Point(116, 160); this.txtJobName.Size = new System.Drawing.Size(326, 24);

            this.lblCropLabel.Text     = "Crop:"; this.lblCropLabel.Font = labelFont;
            this.lblCropLabel.Location = new System.Drawing.Point(8, 195); this.lblCropLabel.Size = new System.Drawing.Size(100, 20); this.lblCropLabel.AutoSize = false;
            this.cboCrop.Font          = inputFont;
            this.cboCrop.Location      = new System.Drawing.Point(116, 192); this.cboCrop.Size = new System.Drawing.Size(326, 24); this.cboCrop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.lblHeaderLabel.Text     = "Header:"; this.lblHeaderLabel.Font = labelFont;
            this.lblHeaderLabel.Location = new System.Drawing.Point(8, 227); this.lblHeaderLabel.Size = new System.Drawing.Size(100, 20); this.lblHeaderLabel.AutoSize = false;
            this.cboHeader.Font          = inputFont;
            this.cboHeader.Location      = new System.Drawing.Point(116, 224); this.cboHeader.Size = new System.Drawing.Size(326, 24); this.cboHeader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.lblProfileLabel.Text     = "Profile:"; this.lblProfileLabel.Font = labelFont;
            this.lblProfileLabel.Location = new System.Drawing.Point(8, 259); this.lblProfileLabel.Size = new System.Drawing.Size(100, 20); this.lblProfileLabel.AutoSize = false;
            this.cboProfile.Font          = inputFont;
            this.cboProfile.Location      = new System.Drawing.Point(116, 256); this.cboProfile.Size = new System.Drawing.Size(326, 24); this.cboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.lblFieldLabel.Text     = "Field:"; this.lblFieldLabel.Font = labelFont;
            this.lblFieldLabel.Location = new System.Drawing.Point(8, 291); this.lblFieldLabel.Size = new System.Drawing.Size(100, 20); this.lblFieldLabel.AutoSize = false;
            this.cboField.Font          = inputFont;
            this.cboField.Location      = new System.Drawing.Point(116, 288); this.cboField.Size = new System.Drawing.Size(326, 24); this.cboField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // ── Bottom row: New / Load / Save / Delete / Close ────────────────
            this.btnNew.Text      = "New";      this.btnNew.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Size      = new System.Drawing.Size(86, 36); this.btnNew.Location = new System.Drawing.Point(4, 328);
            this.btnNew.Click    += new System.EventHandler(this.btnNew_Click);

            this.btnLoad.Text      = "Load";    this.btnLoad.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Size      = new System.Drawing.Size(86, 36); this.btnLoad.Location = new System.Drawing.Point(94, 328);
            this.btnLoad.Enabled   = false;
            this.btnLoad.Click    += new System.EventHandler(this.btnLoad_Click);

            this.btnSave.Text      = "Save";    this.btnSave.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Size      = new System.Drawing.Size(86, 36); this.btnSave.Location = new System.Drawing.Point(184, 328);
            this.btnSave.Enabled   = false;
            this.btnSave.Click    += new System.EventHandler(this.btnSave_Click);

            this.btnDelete.Text      = "Delete"; this.btnDelete.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Size      = new System.Drawing.Size(86, 36); this.btnDelete.Location = new System.Drawing.Point(274, 328);
            this.btnDelete.Enabled   = false;
            this.btnDelete.Click    += new System.EventHandler(this.btnDelete_Click);

            this.btnJobsClose.Text      = "Close"; this.btnJobsClose.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnJobsClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJobsClose.Size      = new System.Drawing.Size(84, 36); this.btnJobsClose.Location = new System.Drawing.Point(364, 328);
            this.btnJobsClose.Click    += new System.EventHandler(this.btnJobsClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lvJobs,
                lblJobName, txtJobName, lblCropLabel, cboCrop,
                lblHeaderLabel, cboHeader, lblProfileLabel, cboProfile,
                lblFieldLabel, cboField,
                btnNew, btnLoad, btnSave, btnDelete, btnJobsClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 412);
            this.MinimumSize     = new System.Drawing.Size(456, 412);
            this.MaximumSize     = new System.Drawing.Size(456, 412);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name            = "frmMenuJobs";
            this.Text            = "Jobs";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuJobs_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel    pnlTitle;
        private System.Windows.Forms.Label    lblTitle;
        private System.Windows.Forms.Button   btnTitleClose;
        private System.Windows.Forms.Panel    pnlContent;
        private System.Windows.Forms.ListView lvJobs;
        private System.Windows.Forms.Label    lblJobName;
        private System.Windows.Forms.TextBox  txtJobName;
        private System.Windows.Forms.Label    lblCropLabel;
        private System.Windows.Forms.ComboBox cboCrop;
        private System.Windows.Forms.Label    lblHeaderLabel;
        private System.Windows.Forms.ComboBox cboHeader;
        private System.Windows.Forms.Label    lblProfileLabel;
        private System.Windows.Forms.ComboBox cboProfile;
        private System.Windows.Forms.Label    lblFieldLabel;
        private System.Windows.Forms.ComboBox cboField;
        private System.Windows.Forms.Button   btnNew;
        private System.Windows.Forms.Button   btnLoad;
        private System.Windows.Forms.Button   btnSave;
        private System.Windows.Forms.Button   btnDelete;
        private System.Windows.Forms.Button   btnJobsClose;
    }
}
