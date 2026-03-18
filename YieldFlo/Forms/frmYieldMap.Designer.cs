namespace YieldFlo.Forms
{
    partial class frmYieldMap
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
            this.mapPanel      = new YieldFlo.Forms.YieldMapPanel();
            this.pnlLegend     = new System.Windows.Forms.Panel();
            this.lblLegendLow  = new System.Windows.Forms.Label();
            this.lblLegendHigh = new System.Windows.Forms.Label();
            this.lblMapJob     = new System.Windows.Forms.Label();
            this.btnRefresh    = new System.Windows.Forms.Button();
            this.btnMapClose   = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = "Yield Map";
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
            this.btnTitleClose.Location = new System.Drawing.Point(462, 5);
            this.btnTitleClose.Click   += new System.EventHandler(this.btnTitleClose_Click);

            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Controls.Add(this.btnTitleClose);

            // ── Content ───────────────────────────────────────────────────────
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;

            var lf = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            var vf = new System.Drawing.Font("Microsoft Sans Serif", 9F);

            // Map panel (custom painted)
            this.mapPanel.Location = new System.Drawing.Point(4, 4);
            this.mapPanel.Size     = new System.Drawing.Size(492, 320);

            // Legend panel (color gradient drawn by OnPaint)
            this.pnlLegend.Location = new System.Drawing.Point(4, 328);
            this.pnlLegend.Size     = new System.Drawing.Size(492, 40);
            this.pnlLegend.Paint   += new System.Windows.Forms.PaintEventHandler(this.pnlLegend_Paint);

            // Legend labels
            this.lblLegendLow.Text      = "Low";
            this.lblLegendLow.Font      = vf;
            this.lblLegendLow.ForeColor = System.Drawing.Color.White;
            this.lblLegendLow.AutoSize  = false;
            this.lblLegendLow.Location  = new System.Drawing.Point(4, 328);
            this.lblLegendLow.Size      = new System.Drawing.Size(40, 16);
            this.lblLegendLow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblLegendHigh.Text      = "High";
            this.lblLegendHigh.Font      = vf;
            this.lblLegendHigh.ForeColor = System.Drawing.Color.White;
            this.lblLegendHigh.AutoSize  = false;
            this.lblLegendHigh.Location  = new System.Drawing.Point(452, 328);
            this.lblLegendHigh.Size      = new System.Drawing.Size(44, 16);
            this.lblLegendHigh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // Job name label
            this.lblMapJob.Text      = "";
            this.lblMapJob.Font      = vf;
            this.lblMapJob.AutoSize  = false;
            this.lblMapJob.Location  = new System.Drawing.Point(4, 372);
            this.lblMapJob.Size      = new System.Drawing.Size(306, 20);
            this.lblMapJob.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // Refresh button
            this.btnRefresh.Text      = "Refresh";
            this.btnRefresh.Font      = lf;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.Size      = new System.Drawing.Size(80, 28);
            this.btnRefresh.Location  = new System.Drawing.Point(318, 368);
            this.btnRefresh.Click    += new System.EventHandler(this.btnRefresh_Click);

            // Close button
            this.btnMapClose.Text      = "Close";
            this.btnMapClose.Font      = lf;
            this.btnMapClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMapClose.FlatAppearance.BorderSize = 0;
            this.btnMapClose.Size      = new System.Drawing.Size(80, 28);
            this.btnMapClose.Location  = new System.Drawing.Point(412, 368);
            this.btnMapClose.Click    += new System.EventHandler(this.btnMapClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                mapPanel, pnlLegend, lblLegendLow, lblLegendHigh,
                lblMapJob, btnRefresh, btnMapClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(500, 462);
            this.MinimumSize     = new System.Drawing.Size(500, 462);
            this.MaximumSize     = new System.Drawing.Size(500, 462);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name            = "frmYieldMap";
            this.Text            = "Yield Map";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmYieldMap_Load);

            this.ResumeLayout(false);
        }

        private void pnlLegend_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Draw gradient: green → yellow → red
            var g   = e.Graphics;
            var rect = (sender as System.Windows.Forms.Panel).ClientRectangle;
            int w = rect.Width;

            for (int x = 0; x < w; x++)
            {
                double t = (double)x / (w - 1);
                System.Drawing.Color col;
                if (t <= 0.5)
                {
                    double u = t * 2.0;
                    col = System.Drawing.Color.FromArgb(
                        (int)(u * 255), (int)(180 + u * 40), 0);
                }
                else
                {
                    double u = (t - 0.5) * 2.0;
                    col = System.Drawing.Color.FromArgb(
                        (int)(255 - u * 35), (int)(220 - u * 220), 0);
                }
                using (var pen = new System.Drawing.Pen(col))
                    g.DrawLine(pen, x, 0, x, rect.Height);
            }
        }

        private System.Windows.Forms.Panel   pnlTitle;
        private System.Windows.Forms.Label   lblTitle;
        private System.Windows.Forms.Button  btnTitleClose;
        private System.Windows.Forms.Panel   pnlContent;
        private YieldFlo.Forms.YieldMapPanel mapPanel;
        private System.Windows.Forms.Panel   pnlLegend;
        private System.Windows.Forms.Label   lblLegendLow;
        private System.Windows.Forms.Label   lblLegendHigh;
        private System.Windows.Forms.Label   lblMapJob;
        private System.Windows.Forms.Button  btnRefresh;
        private System.Windows.Forms.Button  btnMapClose;
    }
}
