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
            this.btnLoadJob    = new System.Windows.Forms.Button();
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
            SetLabel(lblJobName, "Job Name:", labelFont, 8, 10, 96);
            this.txtJobName.Font     = inputFont;
            this.txtJobName.Location = new System.Drawing.Point(112, 8);
            this.txtJobName.Size     = new System.Drawing.Size(334, 24);

            // Crop
            SetLabel(lblCropLabel, "Crop:", labelFont, 8, 42, 96);
            SetCombo(cboCrop, inputFont, 112, 40, 334);

            // Header
            SetLabel(lblHeaderLabel, "Header:", labelFont, 8, 74, 96);
            SetCombo(cboHeader, inputFont, 112, 72, 334);

            // Profile
            SetLabel(lblProfileLabel, "Profile:", labelFont, 8, 106, 96);
            SetCombo(cboProfile, inputFont, 112, 104, 334);

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

            // Load selected job button
            this.btnLoadJob.Text      = "Load Selected";
            this.btnLoadJob.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnLoadJob.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadJob.Size      = new System.Drawing.Size(180, 34);
            this.btnLoadJob.Location  = new System.Drawing.Point(8, 378);
            this.btnLoadJob.Click    += new System.EventHandler(this.btnLoadJob_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblJobName, txtJobName, lblCropLabel, cboCrop,
                lblHeaderLabel, cboHeader, lblProfileLabel, cboProfile,
                btnStart, btnJobsClose, lblRecent, lvJobs, btnLoadJob });

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

        private void SetLabel(System.Windows.Forms.Label lbl, string text,
            System.Drawing.Font font, int x, int y, int w)
        {
            lbl.Text      = text;
            lbl.Font      = font;
            lbl.Location  = new System.Drawing.Point(x, y + 3);
            lbl.Size      = new System.Drawing.Size(w, 20);
            lbl.AutoSize  = false;
        }

        private void SetCombo(System.Windows.Forms.ComboBox cb,
            System.Drawing.Font font, int x, int y, int w)
        {
            cb.Font          = font;
            cb.Location      = new System.Drawing.Point(x, y);
            cb.Size          = new System.Drawing.Size(w, 24);
            cb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
        private System.Windows.Forms.Button   btnLoadJob;
        private System.Windows.Forms.Label    lblRecent;
        private System.Windows.Forms.ListView lvJobs;
    }
}
