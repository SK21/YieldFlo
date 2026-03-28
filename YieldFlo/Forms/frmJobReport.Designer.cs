namespace YieldFlo.Forms
{
    partial class frmJobReport
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle          = new System.Windows.Forms.Panel();
            this.lblTitle          = new System.Windows.Forms.Label();
            this.btnTitleClose     = new System.Windows.Forms.Button();
            this.pnlContent        = new System.Windows.Forms.Panel();
            this.lbJobs            = new System.Windows.Forms.ListBox();
            this.lblReportJobName  = new System.Windows.Forms.Label();
            this.lblReportField    = new System.Windows.Forms.Label();
            this.lblReportArea     = new System.Windows.Forms.Label();
            this.lblReportTotal    = new System.Windows.Forms.Label();
            this.lblReportAvgYield = new System.Windows.Forms.Label();
            this.lblReportAvgMoist = new System.Windows.Forms.Label();
            this.lblReportPoints   = new System.Windows.Forms.Label();
            this.btnExportCsv      = new System.Windows.Forms.Button();
            this.btnReportClose    = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = "Job Report";
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

            // Jobs list
            this.lbJobs.Location              = new System.Drawing.Point(4, 8);
            this.lbJobs.Size                  = new System.Drawing.Size(448, 120);
            this.lbJobs.Font                  = vf;
            this.lbJobs.ScrollAlwaysVisible   = false;
            this.lbJobs.SelectedIndexChanged += new System.EventHandler(this.lbJobs_SelectedIndexChanged);

            // Summary labels
            this.lblReportJobName.Font      = lf;
            this.lblReportJobName.AutoSize  = false;
            this.lblReportJobName.Location  = new System.Drawing.Point(8, 140);
            this.lblReportJobName.Size      = new System.Drawing.Size(440, 20);
            this.lblReportJobName.Text      = "--";

            this.lblReportField.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F); this.lblReportField.AutoSize = false; this.lblReportField.Location = new System.Drawing.Point(8, 162); this.lblReportField.Size = new System.Drawing.Size(440, 18); this.lblReportField.Text = "Field: --";
            this.lblReportArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F); this.lblReportArea.AutoSize = false; this.lblReportArea.Location = new System.Drawing.Point(8, 184); this.lblReportArea.Size = new System.Drawing.Size(440, 18); this.lblReportArea.Text = "Area: --";
            this.lblReportTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F); this.lblReportTotal.AutoSize = false; this.lblReportTotal.Location = new System.Drawing.Point(8, 206); this.lblReportTotal.Size = new System.Drawing.Size(440, 18); this.lblReportTotal.Text = "Total: --";
            this.lblReportAvgYield.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F); this.lblReportAvgYield.AutoSize = false; this.lblReportAvgYield.Location = new System.Drawing.Point(8, 228); this.lblReportAvgYield.Size = new System.Drawing.Size(440, 18); this.lblReportAvgYield.Text = "Avg Yield: --";
            this.lblReportAvgMoist.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F); this.lblReportAvgMoist.AutoSize = false; this.lblReportAvgMoist.Location = new System.Drawing.Point(8, 250); this.lblReportAvgMoist.Size = new System.Drawing.Size(440, 18); this.lblReportAvgMoist.Text = "Avg Moisture: --";
            this.lblReportPoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F); this.lblReportPoints.AutoSize = false; this.lblReportPoints.Location = new System.Drawing.Point(8, 272); this.lblReportPoints.Size = new System.Drawing.Size(440, 18); this.lblReportPoints.Text = "Data Points: --";

            // Export CSV button
            this.btnExportCsv.Text      = "Export CSV";
            this.btnExportCsv.Font      = lf;
            this.btnExportCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportCsv.FlatAppearance.BorderSize = 0;
            this.btnExportCsv.Size      = new System.Drawing.Size(180, 36);
            this.btnExportCsv.Location  = new System.Drawing.Point(8, 298);
            this.btnExportCsv.Click    += new System.EventHandler(this.btnExportCsv_Click);

            // Close button
            this.btnReportClose.Text      = "Close";
            this.btnReportClose.Font      = lf;
            this.btnReportClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReportClose.FlatAppearance.BorderSize = 0;
            this.btnReportClose.Size      = new System.Drawing.Size(100, 36);
            this.btnReportClose.Location  = new System.Drawing.Point(352, 298);
            this.btnReportClose.Click    += new System.EventHandler(this.btnReportClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lbJobs,
                lblReportJobName, lblReportField, lblReportArea, lblReportTotal,
                lblReportAvgYield, lblReportAvgMoist, lblReportPoints,
                btnExportCsv, btnReportClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 400);
            this.MinimumSize     = new System.Drawing.Size(456, 400);
            this.MaximumSize     = new System.Drawing.Size(456, 400);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name            = "frmJobReport";
            this.Text            = "Job Report";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmJobReport_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel   pnlTitle;
        private System.Windows.Forms.Label   lblTitle;
        private System.Windows.Forms.Button  btnTitleClose;
        private System.Windows.Forms.Panel   pnlContent;
        private System.Windows.Forms.ListBox lbJobs;
        private System.Windows.Forms.Label   lblReportJobName;
        private System.Windows.Forms.Label   lblReportField;
        private System.Windows.Forms.Label   lblReportArea;
        private System.Windows.Forms.Label   lblReportTotal;
        private System.Windows.Forms.Label   lblReportAvgYield;
        private System.Windows.Forms.Label   lblReportAvgMoist;
        private System.Windows.Forms.Label   lblReportPoints;
        private System.Windows.Forms.Button  btnExportCsv;
        private System.Windows.Forms.Button  btnReportClose;
    }
}
