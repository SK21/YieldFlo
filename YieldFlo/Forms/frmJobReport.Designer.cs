using YieldFlo.Language;

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
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnTitleClose = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lbJobs = new System.Windows.Forms.ListBox();
            this.lblReportJobName = new System.Windows.Forms.Label();
            this.lblReportField = new System.Windows.Forms.Label();
            this.lblReportArea = new System.Windows.Forms.Label();
            this.lblReportTotal = new System.Windows.Forms.Label();
            this.lblReportAvgYield = new System.Windows.Forms.Label();
            this.lblReportNotesTitle = new System.Windows.Forms.Label();
            this.lblReportNotes = new System.Windows.Forms.Label();
            this.lblReportAvgMoist = new System.Windows.Forms.Label();
            this.lblReportPoints = new System.Windows.Forms.Label();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnReportClose = new System.Windows.Forms.Button();
            this.pnlTitle.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTitle
            //
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Controls.Add(this.btnTitleClose);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(2, 2);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(560, 48);
            this.pnlTitle.TabIndex = 1;
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
            this.lblTitle.Text = "Job Report";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnTitleClose
            //
            this.btnTitleClose.FlatAppearance.BorderSize = 0;
            this.btnTitleClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTitleClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.btnTitleClose.Location = new System.Drawing.Point(512, 4);
            this.btnTitleClose.Name = "btnTitleClose";
            this.btnTitleClose.Size = new System.Drawing.Size(48, 40);
            this.btnTitleClose.TabIndex = 1;
            this.btnTitleClose.Text = "×";
            this.btnTitleClose.Click += new System.EventHandler(this.btnTitleClose_Click);
            //
            // pnlContent
            //
            this.pnlContent.Controls.Add(this.lbJobs);
            this.pnlContent.Controls.Add(this.lblReportJobName);
            this.pnlContent.Controls.Add(this.lblReportField);
            this.pnlContent.Controls.Add(this.lblReportNotesTitle);
            this.pnlContent.Controls.Add(this.lblReportNotes);
            this.pnlContent.Controls.Add(this.lblReportArea);
            this.pnlContent.Controls.Add(this.lblReportTotal);
            this.pnlContent.Controls.Add(this.lblReportAvgYield);
            this.pnlContent.Controls.Add(this.lblReportAvgMoist);
            this.pnlContent.Controls.Add(this.lblReportPoints);
            this.pnlContent.Controls.Add(this.btnExportCsv);
            this.pnlContent.Controls.Add(this.btnPrint);
            this.pnlContent.Controls.Add(this.btnReportClose);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(2, 50);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(560, 512);
            this.pnlContent.TabIndex = 0;
            //
            // lbJobs
            //
            this.lbJobs.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lbJobs.Location = new System.Drawing.Point(4, 4);
            this.lbJobs.Name = "lbJobs";
            this.lbJobs.Size = new System.Drawing.Size(552, 130);
            this.lbJobs.TabIndex = 0;
            this.lbJobs.SelectedIndexChanged += new System.EventHandler(this.lbJobs_SelectedIndexChanged);
            //
            // lblReportJobName
            //
            this.lblReportJobName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblReportJobName.Location = new System.Drawing.Point(8, 142);
            this.lblReportJobName.Name = "lblReportJobName";
            this.lblReportJobName.Size = new System.Drawing.Size(544, 28);
            this.lblReportJobName.TabIndex = 1;
            this.lblReportJobName.Text = "--";
            //
            // lblReportField
            //
            this.lblReportField.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblReportField.Location = new System.Drawing.Point(8, 174);
            this.lblReportField.Name = "lblReportField";
            this.lblReportField.Size = new System.Drawing.Size(544, 28);
            this.lblReportField.TabIndex = 2;
            this.lblReportField.Text = "Field: --";
            //
            // lblReportArea
            //
            this.lblReportArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblReportArea.Location = new System.Drawing.Point(8, 206);
            this.lblReportArea.Name = "lblReportArea";
            this.lblReportArea.Size = new System.Drawing.Size(544, 28);
            this.lblReportArea.TabIndex = 3;
            this.lblReportArea.Text = "Area: --";
            //
            // lblReportTotal
            //
            this.lblReportTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblReportTotal.Location = new System.Drawing.Point(8, 238);
            this.lblReportTotal.Name = "lblReportTotal";
            this.lblReportTotal.Size = new System.Drawing.Size(544, 28);
            this.lblReportTotal.TabIndex = 4;
            this.lblReportTotal.Text = "Total: --";
            //
            // lblReportAvgYield
            //
            this.lblReportAvgYield.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblReportAvgYield.Location = new System.Drawing.Point(8, 270);
            this.lblReportAvgYield.Name = "lblReportAvgYield";
            this.lblReportAvgYield.Size = new System.Drawing.Size(544, 28);
            this.lblReportAvgYield.TabIndex = 5;
            this.lblReportAvgYield.Text = "Avg Yield: --";
            //
            // lblReportAvgMoist
            //
            this.lblReportAvgMoist.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblReportAvgMoist.Location = new System.Drawing.Point(8, 302);
            this.lblReportAvgMoist.Name = "lblReportAvgMoist";
            this.lblReportAvgMoist.Size = new System.Drawing.Size(544, 28);
            this.lblReportAvgMoist.TabIndex = 6;
            this.lblReportAvgMoist.Text = "Avg Moisture: --";
            //
            // lblReportPoints
            //
            this.lblReportPoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblReportPoints.Location = new System.Drawing.Point(8, 334);
            this.lblReportPoints.Name = "lblReportPoints";
            this.lblReportPoints.Size = new System.Drawing.Size(544, 28);
            this.lblReportPoints.TabIndex = 7;
            this.lblReportPoints.Text = "Data Points: --";
            //
            // lblReportNotesTitle
            //
            this.lblReportNotesTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblReportNotesTitle.Location = new System.Drawing.Point(8, 366);
            this.lblReportNotesTitle.Name = "lblReportNotesTitle";
            this.lblReportNotesTitle.Size = new System.Drawing.Size(544, 28);
            this.lblReportNotesTitle.TabIndex = 12;
            this.lblReportNotesTitle.Text = "Notes:";
            //
            // lblReportNotes
            //
            this.lblReportNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblReportNotes.Location = new System.Drawing.Point(8, 398);
            this.lblReportNotes.Name = "lblReportNotes";
            this.lblReportNotes.Size = new System.Drawing.Size(544, 50);
            this.lblReportNotes.TabIndex = 11;
            this.lblReportNotes.Text = "";
            //
            // btnExportCsv
            //
            this.btnExportCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportCsv.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnExportCsv.Location = new System.Drawing.Point(8, 456);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(180, 48);
            this.btnExportCsv.TabIndex = 8;
            this.btnExportCsv.Text = global::YieldFlo.Language.Lang.lgExportCsv;
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            //
            // btnPrint
            //
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnPrint.Location = new System.Drawing.Point(196, 456);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(170, 48);
            this.btnPrint.TabIndex = 10;
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            //
            // btnReportClose
            //
            this.btnReportClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReportClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnReportClose.Location = new System.Drawing.Point(374, 456);
            this.btnReportClose.Name = "btnReportClose";
            this.btnReportClose.Size = new System.Drawing.Size(178, 48);
            this.btnReportClose.TabIndex = 9;
            this.btnReportClose.Text = global::YieldFlo.Language.Lang.lgClose;
            this.btnReportClose.Click += new System.EventHandler(this.btnReportClose_Click);
            //
            // frmJobReport
            //
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(564, 564);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmJobReport";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Job Report";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmJobReport_Load);
            this.pnlTitle.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
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
        private System.Windows.Forms.Label   lblReportNotesTitle;
        private System.Windows.Forms.Label   lblReportNotes;
        private System.Windows.Forms.Label   lblReportAvgMoist;
        private System.Windows.Forms.Label   lblReportPoints;
        private System.Windows.Forms.Button  btnExportCsv;
        private System.Windows.Forms.Button  btnPrint;
        private System.Windows.Forms.Button  btnReportClose;
    }
}
