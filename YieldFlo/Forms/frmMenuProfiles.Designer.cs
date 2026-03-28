using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmMenuProfiles
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
            this.lbProfiles      = new System.Windows.Forms.ListBox();
            this.pnlEdit         = new System.Windows.Forms.Panel();
            this.lblProfileName  = new System.Windows.Forms.Label();
            this.txtProfileName  = new System.Windows.Forms.TextBox();
            this.lblCombineId    = new System.Windows.Forms.Label();
            this.txtCombineId    = new System.Windows.Forms.TextBox();
            this.btnNew          = new System.Windows.Forms.Button();
            this.btnSave         = new System.Windows.Forms.Button();
            this.btnDelete       = new System.Windows.Forms.Button();
            this.btnProfilesClose = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = Lang.lgTitleProfiles;
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

            this.lbProfiles.Location = new System.Drawing.Point(4, 4);
            this.lbProfiles.Size     = new System.Drawing.Size(448, 130);
            this.lbProfiles.Font     = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lbProfiles.SelectedIndexChanged += new System.EventHandler(this.lbProfiles_SelectedIndexChanged);

            this.pnlEdit.Location = new System.Drawing.Point(0, 138);
            this.pnlEdit.Size     = new System.Drawing.Size(456, 72);

            var lf = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            var vf = new System.Drawing.Font("Microsoft Sans Serif", 9F);

            // Profile Name
            this.lblProfileName.Text     = Lang.lgName; this.lblProfileName.Font = lf;
            this.lblProfileName.Location = new System.Drawing.Point(8, 3); this.lblProfileName.AutoSize = false; this.lblProfileName.Width = 100;
            this.txtProfileName.Font     = vf;
            this.txtProfileName.Location = new System.Drawing.Point(116, 0); this.txtProfileName.Width = 326; this.txtProfileName.Height = 24;

            // Combine ID
            this.lblCombineId.Text     = Lang.lgCombineId; this.lblCombineId.Font = lf;
            this.lblCombineId.Location = new System.Drawing.Point(8, 35); this.lblCombineId.AutoSize = false; this.lblCombineId.Width = 100;
            this.txtCombineId.Font     = vf;
            this.txtCombineId.Location = new System.Drawing.Point(116, 32); this.txtCombineId.Width = 326; this.txtCombineId.Height = 24;

            this.pnlEdit.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblProfileName, txtProfileName, lblCombineId, txtCombineId });

            this.pnlContent.Controls.Add(this.lbProfiles);
            this.pnlContent.Controls.Add(this.pnlEdit);

            // Buttons
            var bf = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);

            this.btnNew.Text      = Lang.lgNew;    this.btnNew.Font    = bf; this.btnNew.FlatStyle    = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location  = new System.Drawing.Point(8, 222); this.btnNew.Size = new System.Drawing.Size(106, 36);
            this.btnNew.Click    += new System.EventHandler(this.btnNew_Click);

            this.btnSave.Text      = Lang.lgSave;   this.btnSave.Font   = bf; this.btnSave.FlatStyle   = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location  = new System.Drawing.Point(118, 222); this.btnSave.Size = new System.Drawing.Size(106, 36);
            this.btnSave.Click    += new System.EventHandler(this.btnSave_Click);

            this.btnDelete.Text      = Lang.lgDelete; this.btnDelete.Font = bf; this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location  = new System.Drawing.Point(228, 222); this.btnDelete.Size = new System.Drawing.Size(106, 36);
            this.btnDelete.Click    += new System.EventHandler(this.btnDelete_Click);

            this.btnProfilesClose.Text      = Lang.lgClose; this.btnProfilesClose.Font = bf; this.btnProfilesClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProfilesClose.Location  = new System.Drawing.Point(338, 222); this.btnProfilesClose.Size = new System.Drawing.Size(106, 36);
            this.btnProfilesClose.Click    += new System.EventHandler(this.btnProfilesClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                btnNew, btnSave, btnDelete, btnProfilesClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 306);
            this.MinimumSize     = new System.Drawing.Size(456, 306);
            this.MaximumSize     = new System.Drawing.Size(456, 306);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name            = "frmMenuProfiles";
            this.Text            = "Profiles";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuProfiles_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel   pnlTitle;
        private System.Windows.Forms.Label   lblTitle;
        private System.Windows.Forms.Button  btnTitleClose;
        private System.Windows.Forms.Panel   pnlContent;
        private System.Windows.Forms.ListBox lbProfiles;
        private System.Windows.Forms.Panel   pnlEdit;
        private System.Windows.Forms.Label   lblProfileName;
        private System.Windows.Forms.TextBox txtProfileName;
        private System.Windows.Forms.Label   lblCombineId;
        private System.Windows.Forms.TextBox txtCombineId;
        private System.Windows.Forms.Button  btnNew;
        private System.Windows.Forms.Button  btnSave;
        private System.Windows.Forms.Button  btnDelete;
        private System.Windows.Forms.Button  btnProfilesClose;
    }
}
