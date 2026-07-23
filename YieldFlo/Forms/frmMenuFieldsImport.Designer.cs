using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmMenuFieldsImport
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle              = new System.Windows.Forms.Panel();
            this.lblTitle              = new System.Windows.Forms.Label();
            this.btnTitleClose         = new System.Windows.Forms.Button();
            this.pnlContent            = new System.Windows.Forms.Panel();
            this.clbFields             = new System.Windows.Forms.CheckedListBox();
            this.btnSelectAll          = new System.Windows.Forms.Button();
            this.btnImport             = new System.Windows.Forms.Button();
            this.btnFieldsImportClose  = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 48;

            this.lblTitle.Text      = Lang.lgTitleImportFields;
            this.lblTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.AutoSize  = false;
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.btnTitleClose.Text      = "×";
            this.btnTitleClose.Font      = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.btnTitleClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTitleClose.FlatAppearance.BorderSize = 0;
            this.btnTitleClose.Size     = new System.Drawing.Size(48, 40);
            this.btnTitleClose.Location = new System.Drawing.Point(508, 4);
            this.btnTitleClose.Click   += new System.EventHandler(this.btnTitleClose_Click);

            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Controls.Add(this.btnTitleClose);

            // ── Content ───────────────────────────────────────────────────────
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;

            var inputFont = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            var btnFont   = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);

            this.clbFields.Location    = new System.Drawing.Point(4, 4);
            this.clbFields.Size        = new System.Drawing.Size(552, 240);
            this.clbFields.Font        = inputFont;
            this.clbFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clbFields.CheckOnClick = true;
            this.clbFields.ItemCheck  += new System.Windows.Forms.ItemCheckEventHandler(this.clbFields_ItemCheck);

            this.btnSelectAll.Text      = Lang.lgSelectAll; this.btnSelectAll.Font = btnFont;
            this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectAll.Size      = new System.Drawing.Size(176, 48);
            this.btnSelectAll.Location  = new System.Drawing.Point(8, 252);
            this.btnSelectAll.Click    += new System.EventHandler(this.btnSelectAll_Click);

            this.btnImport.Text      = Lang.lgImport; this.btnImport.Font = btnFont;
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.Size      = new System.Drawing.Size(176, 48);
            this.btnImport.Location  = new System.Drawing.Point(194, 252);
            this.btnImport.Click    += new System.EventHandler(this.btnImport_Click);

            this.btnFieldsImportClose.Text      = Lang.lgClose; this.btnFieldsImportClose.Font = btnFont;
            this.btnFieldsImportClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFieldsImportClose.Size      = new System.Drawing.Size(176, 48);
            this.btnFieldsImportClose.Location  = new System.Drawing.Point(380, 252);
            this.btnFieldsImportClose.Click    += new System.EventHandler(this.btnFieldsImportClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                clbFields, btnSelectAll, btnImport, btnFieldsImportClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(564, 360);
            this.MinimumSize     = new System.Drawing.Size(564, 360);
            this.MaximumSize     = new System.Drawing.Size(564, 360);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.Name            = "frmMenuFieldsImport";
            this.Text            = "Import Fields";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuFieldsImport_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel           pnlTitle;
        private System.Windows.Forms.Label            lblTitle;
        private System.Windows.Forms.Button           btnTitleClose;
        private System.Windows.Forms.Panel            pnlContent;
        private System.Windows.Forms.CheckedListBox    clbFields;
        private System.Windows.Forms.Button           btnSelectAll;
        private System.Windows.Forms.Button           btnImport;
        private System.Windows.Forms.Button           btnFieldsImportClose;
    }
}
