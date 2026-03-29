using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmMenuLanguage
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
            this.lblNote       = new System.Windows.Forms.Label();
            this.btnEn         = new System.Windows.Forms.Button();
            this.btnDe         = new System.Windows.Forms.Button();
            this.btnFr         = new System.Windows.Forms.Button();
            this.btnHu         = new System.Windows.Forms.Button();
            this.btnNl         = new System.Windows.Forms.Button();
            this.btnPl         = new System.Windows.Forms.Button();
            this.btnRu         = new System.Windows.Forms.Button();
            this.btnLt         = new System.Windows.Forms.Button();
            this.btnSaveRestart = new System.Windows.Forms.Button();
            this.btnClose      = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = Lang.lgTitleLanguage;
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

            this.pnlTitle.Controls.Add(this.btnTitleClose);
            this.pnlTitle.Controls.Add(this.lblTitle);

            // ── Content ───────────────────────────────────────────────────────
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;

            var bf = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);

            this.lblNote.Text      = Lang.lgLangNote;
            this.lblNote.Font      = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblNote.AutoSize  = false;
            this.lblNote.Location  = new System.Drawing.Point(8, 8);
            this.lblNote.Size      = new System.Drawing.Size(440, 20);
            this.lblNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Row 1
            this.btnEn.Text = "English";   this.btnEn.Tag = "en"; this.btnEn.Font = bf; this.btnEn.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnEn.Size = new System.Drawing.Size(210, 44); this.btnEn.Location = new System.Drawing.Point(8,   36);
            this.btnDe.Text = "Deutsch";   this.btnDe.Tag = "de"; this.btnDe.Font = bf; this.btnDe.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDe.Size = new System.Drawing.Size(210, 44); this.btnDe.Location = new System.Drawing.Point(238, 36);
            // Row 2
            this.btnFr.Text = "Français";  this.btnFr.Tag = "fr"; this.btnFr.Font = bf; this.btnFr.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnFr.Size = new System.Drawing.Size(210, 44); this.btnFr.Location = new System.Drawing.Point(8,   88);
            this.btnHu.Text = "Magyar";    this.btnHu.Tag = "hu"; this.btnHu.Font = bf; this.btnHu.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnHu.Size = new System.Drawing.Size(210, 44); this.btnHu.Location = new System.Drawing.Point(238, 88);
            // Row 3
            this.btnNl.Text = "Nederlands"; this.btnNl.Tag = "nl"; this.btnNl.Font = bf; this.btnNl.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnNl.Size = new System.Drawing.Size(210, 44); this.btnNl.Location = new System.Drawing.Point(8,   140);
            this.btnPl.Text = "Polski";    this.btnPl.Tag = "pl"; this.btnPl.Font = bf; this.btnPl.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnPl.Size = new System.Drawing.Size(210, 44); this.btnPl.Location = new System.Drawing.Point(238, 140);
            // Row 4
            this.btnRu.Text = "Русский";   this.btnRu.Tag = "ru"; this.btnRu.Font = bf; this.btnRu.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnRu.Size = new System.Drawing.Size(210, 44); this.btnRu.Location = new System.Drawing.Point(8,   192);
            this.btnLt.Text = "Lietuvių";  this.btnLt.Tag = "lt"; this.btnLt.Font = bf; this.btnLt.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnLt.Size = new System.Drawing.Size(210, 44); this.btnLt.Location = new System.Drawing.Point(238, 192);

            this.btnEn.Click += new System.EventHandler(this.btnLang_Click);
            this.btnDe.Click += new System.EventHandler(this.btnLang_Click);
            this.btnFr.Click += new System.EventHandler(this.btnLang_Click);
            this.btnHu.Click += new System.EventHandler(this.btnLang_Click);
            this.btnNl.Click += new System.EventHandler(this.btnLang_Click);
            this.btnPl.Click += new System.EventHandler(this.btnLang_Click);
            this.btnRu.Click += new System.EventHandler(this.btnLang_Click);
            this.btnLt.Click += new System.EventHandler(this.btnLang_Click);

            // Save & Restart / Close
            var btnFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);

            this.btnSaveRestart.Text      = Lang.lgRestart;
            this.btnSaveRestart.Font      = btnFont;
            this.btnSaveRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveRestart.Size      = new System.Drawing.Size(210, 36);
            this.btnSaveRestart.Location  = new System.Drawing.Point(8, 256);
            this.btnSaveRestart.Click    += new System.EventHandler(this.btnSaveRestart_Click);

            this.btnClose.Text      = Lang.lgClose;
            this.btnClose.Font      = btnFont;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Size      = new System.Drawing.Size(210, 36);
            this.btnClose.Location  = new System.Drawing.Point(238, 256);
            this.btnClose.Click    += new System.EventHandler(this.btnClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblNote,
                btnEn, btnDe, btnFr, btnHu, btnNl, btnPl, btnRu, btnLt,
                btnSaveRestart, btnClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 350);
            this.MinimumSize     = new System.Drawing.Size(456, 350);
            this.MaximumSize     = new System.Drawing.Size(456, 350);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name            = "frmMenuLanguage";
            this.Text            = "Language";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuLanguage_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel   pnlTitle;
        private System.Windows.Forms.Label   lblTitle;
        private System.Windows.Forms.Button  btnTitleClose;
        private System.Windows.Forms.Panel   pnlContent;
        private System.Windows.Forms.Label   lblNote;
        private System.Windows.Forms.Button  btnEn;
        private System.Windows.Forms.Button  btnDe;
        private System.Windows.Forms.Button  btnFr;
        private System.Windows.Forms.Button  btnHu;
        private System.Windows.Forms.Button  btnNl;
        private System.Windows.Forms.Button  btnPl;
        private System.Windows.Forms.Button  btnRu;
        private System.Windows.Forms.Button  btnLt;
        private System.Windows.Forms.Button  btnSaveRestart;
        private System.Windows.Forms.Button  btnClose;
    }
}
