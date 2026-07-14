using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmMenuFields
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle       = new System.Windows.Forms.Panel();
            this.lblTitle       = new System.Windows.Forms.Label();
            this.btnTitleClose  = new System.Windows.Forms.Button();
            this.pnlContent     = new System.Windows.Forms.Panel();
            this.lbFields       = new System.Windows.Forms.ListBox();
            this.lblNameLabel   = new System.Windows.Forms.Label();
            this.txtName        = new System.Windows.Forms.TextBox();
            this.btnSave        = new System.Windows.Forms.Button();
            this.btnDelete      = new System.Windows.Forms.Button();
            this.btnNew         = new System.Windows.Forms.Button();
            this.btnFieldsClose = new System.Windows.Forms.Button();
            this.btnImport      = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = Lang.lgTitleFields;
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

            var labelFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            var inputFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);

            this.lbFields.Location    = new System.Drawing.Point(4, 4);
            this.lbFields.Size        = new System.Drawing.Size(448, 168);
            this.lbFields.Font        = inputFont;
            this.lbFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbFields.SelectedIndexChanged += new System.EventHandler(this.lbFields_SelectedIndexChanged);

            this.lblNameLabel.Text      = Lang.lgName;
            this.lblNameLabel.Font      = labelFont;
            this.lblNameLabel.Location  = new System.Drawing.Point(8, 183);
            this.lblNameLabel.Size      = new System.Drawing.Size(80, 20);
            this.lblNameLabel.AutoSize  = false;

            this.txtName.Font     = inputFont;
            this.txtName.Location = new System.Drawing.Point(92, 180);
            this.txtName.Size     = new System.Drawing.Size(360, 24);

            var btnFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);

            this.btnImport.Text      = Lang.lgImport; this.btnImport.Font      = btnFont;
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.Size      = new System.Drawing.Size(436, 36);
            this.btnImport.Location  = new System.Drawing.Point(8, 214);
            this.btnImport.Click    += new System.EventHandler(this.btnImport_Click);

            this.btnNew.Text      = Lang.lgNew;    this.btnNew.Font      = btnFont;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Size      = new System.Drawing.Size(106, 36);
            this.btnNew.Location  = new System.Drawing.Point(8, 254);
            this.btnNew.Click    += new System.EventHandler(this.btnNew_Click);

            this.btnSave.Text      = Lang.lgSave;   this.btnSave.Font      = btnFont;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Size      = new System.Drawing.Size(106, 36);
            this.btnSave.Location  = new System.Drawing.Point(118, 254);
            this.btnSave.Click    += new System.EventHandler(this.btnSave_Click);

            this.btnDelete.Text      = Lang.lgDelete; this.btnDelete.Font      = btnFont;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Size      = new System.Drawing.Size(106, 36);
            this.btnDelete.Location  = new System.Drawing.Point(228, 254);
            this.btnDelete.Click    += new System.EventHandler(this.btnDelete_Click);

            this.btnFieldsClose.Text      = Lang.lgClose; this.btnFieldsClose.Font      = btnFont;
            this.btnFieldsClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFieldsClose.Size      = new System.Drawing.Size(106, 36);
            this.btnFieldsClose.Location  = new System.Drawing.Point(338, 254);
            this.btnFieldsClose.Click    += new System.EventHandler(this.btnFieldsClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lbFields, lblNameLabel, txtName,
                btnNew, btnSave, btnDelete, btnFieldsClose, btnImport });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 338);
            this.MinimumSize     = new System.Drawing.Size(456, 338);
            this.MaximumSize     = new System.Drawing.Size(456, 338);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name            = "frmMenuFields";
            this.Text            = "Fields";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuFields_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel   pnlTitle;
        private System.Windows.Forms.Label   lblTitle;
        private System.Windows.Forms.Button  btnTitleClose;
        private System.Windows.Forms.Panel   pnlContent;
        private System.Windows.Forms.ListBox lbFields;
        private System.Windows.Forms.Label   lblNameLabel;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button  btnSave;
        private System.Windows.Forms.Button  btnDelete;
        private System.Windows.Forms.Button  btnNew;
        private System.Windows.Forms.Button  btnFieldsClose;
        private System.Windows.Forms.Button  btnImport;
    }
}
