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
            this.lblOffset      = new System.Windows.Forms.Label();
            this.numOffset      = new System.Windows.Forms.NumericUpDown();
            this.lblOffsetUnit  = new System.Windows.Forms.Label();
            this.btnNew         = new System.Windows.Forms.Button();
            this.btnSave        = new System.Windows.Forms.Button();
            this.btnDelete      = new System.Windows.Forms.Button();
            this.btnHeadersClose = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 48;

            this.lblTitle.Text      = Lang.lgTitleHeaders;
            this.lblTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.AutoSize  = false;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.btnTitleClose.Text      = "×";
            this.btnTitleClose.Font      = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.btnTitleClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTitleClose.FlatAppearance.BorderSize = 0;
            this.btnTitleClose.Size     = new System.Drawing.Size(48, 40);
            this.btnTitleClose.Location = new System.Drawing.Point(504, 4);
            this.btnTitleClose.Click   += new System.EventHandler(this.btnTitleClose_Click);

            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Controls.Add(this.btnTitleClose);

            // ── Content ───────────────────────────────────────────────────────
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;

            this.lbHeaders.Location = new System.Drawing.Point(4, 4);
            this.lbHeaders.Size     = new System.Drawing.Size(552, 170);
            this.lbHeaders.Font     = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lbHeaders.SelectedIndexChanged += new System.EventHandler(this.lbHeaders_SelectedIndexChanged);

            this.pnlEdit.Location = new System.Drawing.Point(4, 180);
            this.pnlEdit.Size     = new System.Drawing.Size(552, 164);

            var lf = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            var vf = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            var nf = new System.Drawing.Font("Microsoft Sans Serif", 11F);

            // Name
            this.lblHeaderName.Text     = Lang.lgName; this.lblHeaderName.Font = lf;
            this.lblHeaderName.Location = new System.Drawing.Point(8, 4); this.lblHeaderName.AutoSize = false; this.lblHeaderName.Size = new System.Drawing.Size(220, 32);
            this.txtHeaderName.Font     = vf;
            this.txtHeaderName.Location = new System.Drawing.Point(236, 4); this.txtHeaderName.Width = 310; this.txtHeaderName.Height = 32;

            // Type
            this.lblHeaderType.Text     = Lang.lgType; this.lblHeaderType.Font = lf;
            this.lblHeaderType.Location = new System.Drawing.Point(8, 44); this.lblHeaderType.AutoSize = false; this.lblHeaderType.Size = new System.Drawing.Size(220, 32);
            this.cboHeaderType.Font          = vf;
            this.cboHeaderType.Location      = new System.Drawing.Point(236, 44);
            this.cboHeaderType.Width         = 180;
            this.cboHeaderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHeaderType.Items.AddRange(new object[] { "Draper", "Auger", "Corn" });
            this.cboHeaderType.SelectedIndex = 0;

            // Width
            this.lblWidth.Text     = Lang.lgWidth; this.lblWidth.Font = lf;
            this.lblWidth.Location = new System.Drawing.Point(8, 84); this.lblWidth.AutoSize = false; this.lblWidth.Size = new System.Drawing.Size(220, 32);
            this.numWidth.Font     = vf;
            this.numWidth.Location = new System.Drawing.Point(236, 84); this.numWidth.Width = 100; this.numWidth.Height = 32;
            this.numWidth.Minimum  = 1; this.numWidth.Maximum = 60; this.numWidth.DecimalPlaces = 2;
            this.numWidth.Increment = (decimal)0.1; this.numWidth.Value = 9;
            this.lblWidthUnit.Text = "m"; this.lblWidthUnit.Font = nf;
            this.lblWidthUnit.Location = new System.Drawing.Point(344, 90); this.lblWidthUnit.AutoSize = true;

            // Header position ahead of the AOG pivot point (AOG's pivot-to-header
            // distance) — pass boundaries are recorded at the header, matching
            // where AOG paints its coverage.
            this.lblOffset.Text     = Lang.lgHeaderOffset; this.lblOffset.Font = lf;
            this.lblOffset.Location = new System.Drawing.Point(8, 124); this.lblOffset.AutoSize = false;
            this.lblOffset.Size     = new System.Drawing.Size(220, 32);
            this.lblOffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.numOffset.Font     = vf;
            this.numOffset.Location = new System.Drawing.Point(236, 124); this.numOffset.Width = 100; this.numOffset.Height = 32;
            this.numOffset.Minimum  = -30; this.numOffset.Maximum = 100; this.numOffset.DecimalPlaces = 2;
            this.numOffset.Increment = (decimal)0.1; this.numOffset.Value = 0;
            this.lblOffsetUnit.Text = "m"; this.lblOffsetUnit.Font = nf;
            this.lblOffsetUnit.Location = new System.Drawing.Point(344, 130); this.lblOffsetUnit.AutoSize = true;

            this.pnlEdit.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblHeaderName, txtHeaderName, lblHeaderType, cboHeaderType,
                lblWidth, numWidth, lblWidthUnit, lblOffset, numOffset, lblOffsetUnit });

            this.pnlContent.Controls.Add(this.lbHeaders);
            this.pnlContent.Controls.Add(this.pnlEdit);

            // Buttons
            this.btnNew.Text      = Lang.lgNew;    this.btnNew.Font    = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location  = new System.Drawing.Point(8, 352); this.btnNew.Size = new System.Drawing.Size(130, 44);
            this.btnNew.Click    += new System.EventHandler(this.btnNew_Click);

            this.btnSave.Text      = Lang.lgSave;   this.btnSave.Font   = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location  = new System.Drawing.Point(146, 352); this.btnSave.Size = new System.Drawing.Size(130, 44);
            this.btnSave.Click    += new System.EventHandler(this.btnSave_Click);

            this.btnDelete.Text      = Lang.lgDelete; this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location  = new System.Drawing.Point(284, 352); this.btnDelete.Size = new System.Drawing.Size(130, 44);
            this.btnDelete.Click    += new System.EventHandler(this.btnDelete_Click);

            this.btnHeadersClose.Text      = Lang.lgClose; this.btnHeadersClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnHeadersClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeadersClose.Location  = new System.Drawing.Point(422, 352); this.btnHeadersClose.Size = new System.Drawing.Size(130, 44);
            this.btnHeadersClose.Click    += new System.EventHandler(this.btnHeadersClose_Click);

            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] {
                btnNew, btnSave, btnDelete, btnHeadersClose });

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(564, 456);
            this.MinimumSize     = new System.Drawing.Size(564, 456);
            this.MaximumSize     = new System.Drawing.Size(564, 456);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
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
        private System.Windows.Forms.Label         lblOffset;
        private System.Windows.Forms.NumericUpDown numOffset;
        private System.Windows.Forms.Label         lblOffsetUnit;
        private System.Windows.Forms.Button        btnNew;
        private System.Windows.Forms.Button        btnSave;
        private System.Windows.Forms.Button        btnDelete;
        private System.Windows.Forms.Button        btnHeadersClose;
    }
}
