using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmMini
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlContent    = new System.Windows.Forms.Panel();
            this.lblYieldTitle = new System.Windows.Forms.Label();
            this.lblYield      = new System.Windows.Forms.Label();
            this.lblUnit       = new System.Windows.Forms.Label();
            this.btnRestore    = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Content panel (fills form minus 2px padding = the border) ────
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;

            // Top bar: YIELD | unit | restore button
            this.lblYieldTitle.Text      = Lang.lgYield;
            this.lblYieldTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblYieldTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblYieldTitle.Location  = new System.Drawing.Point(5, 5);
            this.lblYieldTitle.AutoSize  = true;

            this.lblUnit.Text      = "bu/ac";
            this.lblUnit.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblUnit.Location  = new System.Drawing.Point(48, 5);
            this.lblUnit.Size      = new System.Drawing.Size(88, 16);
            this.lblUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblUnit.AutoSize  = false;

            this.btnRestore.Text      = "▣";
            this.btnRestore.Font      = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestore.FlatAppearance.BorderSize = 0;
            this.btnRestore.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.btnRestore.Size      = new System.Drawing.Size(24, 22);
            this.btnRestore.Location  = new System.Drawing.Point(146, 2);
            this.btnRestore.Click    += new System.EventHandler(this.btnRestore_Click);

            // Large yield value — fills below top bar
            this.lblYield.Text      = "--.-";
            this.lblYield.Font      = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold);
            this.lblYield.Location  = new System.Drawing.Point(0, 24);
            this.lblYield.Size      = new System.Drawing.Size(174, 50);
            this.lblYield.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblYield.AutoSize  = false;

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblYieldTitle, btnRestore, lblYield, lblUnit });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(174, 78);
            this.MinimumSize     = new System.Drawing.Size(174, 78);
            this.MaximumSize     = new System.Drawing.Size(174, 78);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = true;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.Manual;
            this.Name            = "frmMini";
            this.Text            = "YieldFlo";
            this.Controls.Add(this.pnlContent);
            this.Load           += new System.EventHandler(this.frmMini_Load);
            this.FormClosing    += new System.Windows.Forms.FormClosingEventHandler(this.frmMini_FormClosing);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel  pnlContent;
        private System.Windows.Forms.Label  lblYieldTitle;
        private System.Windows.Forms.Label  lblYield;
        private System.Windows.Forms.Label  lblUnit;
        private System.Windows.Forms.Button btnRestore;
    }
}
