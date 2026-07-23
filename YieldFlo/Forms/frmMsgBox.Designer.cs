namespace YieldFlo.Forms
{
    partial class frmMsgBox
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle  = new System.Windows.Forms.Panel();
            this.lblTitle  = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblMsg    = new System.Windows.Forms.Label();
            this.btnYes    = new System.Windows.Forms.Button();
            this.btnNo     = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height    = 44;
            this.pnlTitle.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);

            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Text      = "";

            this.pnlTitle.Controls.Add(this.lblTitle);

            // ── Content panel ─────────────────────────────────────────────────
            this.pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);

            // ── Message ───────────────────────────────────────────────────────
            this.lblMsg.Font      = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblMsg.ForeColor = System.Drawing.Color.White;
            this.lblMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMsg.Location  = new System.Drawing.Point(0, 14);
            this.lblMsg.Size      = new System.Drawing.Size(420, 90);
            this.lblMsg.Text      = "";

            // ── Buttons ───────────────────────────────────────────────────────
            this.btnYes.Text      = "Yes";
            this.btnYes.Font      = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnYes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYes.BackColor = System.Drawing.Color.FromArgb(0, 100, 0);
            this.btnYes.ForeColor = System.Drawing.Color.White;
            this.btnYes.Size      = new System.Drawing.Size(140, 48);
            this.btnYes.Location  = new System.Drawing.Point(60, 120);
            this.btnYes.TabStop   = false;
            this.btnYes.Click    += new System.EventHandler(this.btnYes_Click);

            this.btnNo.Text      = "No";
            this.btnNo.Font      = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNo.BackColor = System.Drawing.Color.FromArgb(80, 30, 30);
            this.btnNo.ForeColor = System.Drawing.Color.White;
            this.btnNo.Size      = new System.Drawing.Size(140, 48);
            this.btnNo.Location  = new System.Drawing.Point(220, 120);
            this.btnNo.TabStop   = false;
            this.btnNo.Click    += new System.EventHandler(this.btnNo_Click);

            this.pnlContent.Controls.Add(this.lblMsg);
            this.pnlContent.Controls.Add(this.btnYes);
            this.pnlContent.Controls.Add(this.btnNo);

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(424, 232);
            this.MinimumSize     = new System.Drawing.Size(424, 232);
            this.MaximumSize     = new System.Drawing.Size(424, 232);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ShowInTaskbar   = false;
            this.BackColor       = System.Drawing.Color.White;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.Name            = "frmMsgBox";

            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel  pnlTitle;
        private System.Windows.Forms.Label  lblTitle;
        private System.Windows.Forms.Panel  pnlContent;
        private System.Windows.Forms.Label  lblMsg;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
    }
}
