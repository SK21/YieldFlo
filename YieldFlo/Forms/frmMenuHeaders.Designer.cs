using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmMenuHeaders
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
            this.lbHeaders      = new System.Windows.Forms.ListBox();
            this.pnlEdit        = new System.Windows.Forms.Panel();
            this.lblHeaderName  = new System.Windows.Forms.Label();
            this.txtHeaderName  = new System.Windows.Forms.TextBox();
            this.lblHeaderType  = new System.Windows.Forms.Label();
            this.cboHeaderType  = new System.Windows.Forms.ComboBox();
            this.lblWidth       = new System.Windows.Forms.Label();
            this.numWidth       = new System.Windows.Forms.NumericUpDown();
            this.lblWidthUnit   = new System.Windows.Forms.Label();
            this.btnNew         = new System.Windows.Forms.Button();
            this.btnSave        = new System.Windows.Forms.Button();
            this.btnDelete      = new System.Windows.Forms.Button();
            this.btnHeadersClose = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = Lang.lgTitleHeaders;
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

            this.lbHeaders.Location = new System.Drawing.Point(4, 4);
            this.lbHeaders.Size     = new System.Drawing.Size(448, 142);
            this.lbHeaders.Font     = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lbHeaders.SelectedIndexChanged += new System.EventHandler(this.lbHeaders_SelectedIndexChanged);

            this.pnlEdit.Location = new System.Drawing.Point(0, 150);
            this.pnlEdit.Size     = new System.Drawing.Size(456, 110);

            var lf = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            var vf = new System.Drawing.Font("Microsoft Sans Serif", 9F);

            // Name
            this.lblHeaderName.Text     = Lang.lgName; this.lblHeaderName.Font = lf;
            this.lblHeaderName.Location = new System.Drawing.Point(8, 3); this.lblHeaderName.AutoSize = false; this.lblHeaderName.Width = 100;
            this.txtHeaderName.Font     = vf;
            this.txtHeaderName.Location = new System.Drawing.Point(116, 0); this.txtHeaderName.Width = 326; this.txtHeaderName.Height = 24;

            // Type
            this.lblHeaderType.Text     = Lang.lgType; this.lblHeaderType.Font = lf;
            this.lblHeaderType.Location = new System.Drawing.Point(8, 35); this.lblHeaderType.AutoSize = false; this.lblHeaderType.Width = 100;
            this.cboHeaderType.Font          = vf;
            this.cboHeaderType.Location      = new System.Drawing.Point(116, 32);
            this.cboHeaderType.Width         = 160;
            this.cboHeaderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHeaderType.Items.AddRange(new object[] { "Draper", "Auger", "Corn" });
            this.cboHeaderType.SelectedIndex = 0;

            // Width
            this.lblWidth.Text     = Lang.lgWidth; this.lblWidth.Font = lf;
            this.lblWidth.Location = new System.Drawing.Point(8, 67); this.lblWidth.AutoSize = false; this.lblWidth.Width = 100;
            this.numWidth.Font     = vf;
            this.numWidth.Location = new System.Drawing.Point(116, 64); this.numWidth.Width = 80;
            this.numWidth.Minimum  = 1; this.numWidth.Maximum = 60; this.numWidth.DecimalPlaces = 2;
            this.numWidth.Increment = (decimal)0.1; this.numWidth.Value = 9;
            this.lblWidthUnit.Text = "m"; this.lblWidthUnit.Font = vf;
            this.lblWidthUnit.Location = new System.Drawing.Point(204, 67); this.lblWidthUnit.AutoSize = true;

            this.pnlEdit.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblHeaderName, txtHeaderName, lblHeaderType, cboHeaderType,
                lblWidth, numWidth, lblWidthUnit });

            this.pnlContent.Controls.Add(this.lbHeaders);
            this.pnlContent.Controls.Add(this.pnlEdit);

            // Buttons
            this.btnNew.Text      = Lang.lgNew;    this.btnNew.Font    = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location  = new System.Drawing.Point(8, 272); this.btnNew.Size = new System.Drawing.Size(106, 36);
            this.btnNew.Click    += new System.EventHandler(this.btnNew_Click);

            this.btnSave.Text      = Lang.lgSave;   this.btnSave.Font   = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location  = new System.Drawing.Point(118, 272); this.btnSave.Size = new System.Drawing.Size(106, 36);
            this.btnSave.Click    += new System.EventHandler(this.btnSave_Click);

            this.btnDelete.Text      = Lang.lgDelete; this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location  = new System.Drawing.Point(228, 272); this.btnDelete.Size = new System.Drawing.Size(106, 36);
            this.btnDelete.Click    += new System.EventHandler(this.btnDelete_Click);

            this.btnHeadersClose.Text      = Lang.lgClose; this.btnHeadersClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnHeadersClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeadersClose.Location  = new System.Drawing.Point(338, 272); this.btnHeadersClose.Size = new System.Drawing.Size(106, 36);
            this.btnHeadersClose.Click    += new System.EventHandler(this.btnHeadersClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                btnNew, btnSave, btnDelete, btnHeadersClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 356);
            this.MinimumSize     = new System.Drawing.Size(456, 356);
            this.MaximumSize     = new System.Drawing.Size(456, 356);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name            = "frmMenuHeaders";
            this.Text            = "Headers";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuHeaders_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel         pnlTitle;
        private System.Windows.Forms.Label         lblTitle;
        private System.Windows.Forms.Button        btnTitleClose;
        private System.Windows.Forms.Panel         pnlContent;
        private System.Windows.Forms.ListBox       lbHeaders;
        private System.Windows.Forms.Panel         pnlEdit;
        private System.Windows.Forms.Label         lblHeaderName;
        private System.Windows.Forms.TextBox       txtHeaderName;
        private System.Windows.Forms.Label         lblHeaderType;
        private System.Windows.Forms.ComboBox      cboHeaderType;
        private System.Windows.Forms.Label         lblWidth;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.Label         lblWidthUnit;
        private System.Windows.Forms.Button        btnNew;
        private System.Windows.Forms.Button        btnSave;
        private System.Windows.Forms.Button        btnDelete;
        private System.Windows.Forms.Button        btnHeadersClose;
    }
}
