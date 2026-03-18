namespace YieldFlo.Forms
{
    partial class frmMenuCrops
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
            this.lbCrops       = new System.Windows.Forms.ListBox();
            this.pnlEdit       = new System.Windows.Forms.Panel();
            this.lblCropName   = new System.Windows.Forms.Label();
            this.txtCropName   = new System.Windows.Forms.TextBox();
            this.lblCategory   = new System.Windows.Forms.Label();
            this.cboCropCategory = new System.Windows.Forms.ComboBox();
            this.lblTestWeight = new System.Windows.Forms.Label();
            this.numTestWeight = new System.Windows.Forms.NumericUpDown();
            this.lblTestWeightUnit = new System.Windows.Forms.Label();
            this.lblMktMoisture  = new System.Windows.Forms.Label();
            this.numMarketMoisture = new System.Windows.Forms.NumericUpDown();
            this.lblMktMoistureUnit = new System.Windows.Forms.Label();
            this.btnNew        = new System.Windows.Forms.Button();
            this.btnSave       = new System.Windows.Forms.Button();
            this.btnDelete     = new System.Windows.Forms.Button();
            this.btnCropsClose = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── Title bar ────────────────────────────────────────────────────
            this.pnlTitle.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Height = 40;

            this.lblTitle.Text      = "Crops";
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

            this.lbCrops.Location      = new System.Drawing.Point(4, 4);
            this.lbCrops.Size          = new System.Drawing.Size(448, 152);
            this.lbCrops.Font          = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lbCrops.SelectedIndexChanged += new System.EventHandler(this.lbCrops_SelectedIndexChanged);

            // Edit panel (transparent background, positioned below list)
            this.pnlEdit.Location  = new System.Drawing.Point(0, 160);
            this.pnlEdit.Size      = new System.Drawing.Size(456, 160);

            var lf = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            var vf = new System.Drawing.Font("Microsoft Sans Serif", 9F);

            // Name
            this.lblCropName.Text = "Name:"; this.lblCropName.Font = lf; this.lblCropName.Location = new System.Drawing.Point(8, 3); this.lblCropName.AutoSize = false; this.lblCropName.Width = 108;
            this.txtCropName.Font = vf; this.txtCropName.Location = new System.Drawing.Point(120, 0); this.txtCropName.Height = 24; this.txtCropName.Width = 320;
            pnlEdit.Controls.Add(this.lblCropName); pnlEdit.Controls.Add(this.txtCropName);
            // Category
            this.lblCategory.Text = "Category:"; this.lblCategory.Font = lf; this.lblCategory.Location = new System.Drawing.Point(8, 35); this.lblCategory.AutoSize = false; this.lblCategory.Width = 108;
            this.cboCropCategory.Font = vf; this.cboCropCategory.Location = new System.Drawing.Point(120, 32); this.cboCropCategory.Width = 160;
            this.cboCropCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCropCategory.Items.AddRange(new object[] { "Cereal", "OilSeed", "Corn", "Pulse", "Other" });
            this.cboCropCategory.SelectedIndex = 0;
            pnlEdit.Controls.Add(this.lblCategory); pnlEdit.Controls.Add(this.cboCropCategory);
            // Test Weight
            this.lblTestWeight.Text = "Test Wt:"; this.lblTestWeight.Font = lf; this.lblTestWeight.Location = new System.Drawing.Point(8, 67); this.lblTestWeight.AutoSize = false; this.lblTestWeight.Width = 108;
            this.numTestWeight.Font = vf; this.numTestWeight.Location = new System.Drawing.Point(120, 64); this.numTestWeight.Width = 80; this.numTestWeight.Minimum = 0; this.numTestWeight.Maximum = 200; this.numTestWeight.Increment = 1; this.numTestWeight.Value = 60; this.numTestWeight.DecimalPlaces = 0;
            this.lblTestWeightUnit.Text = "lb/bu"; this.lblTestWeightUnit.Font = vf; this.lblTestWeightUnit.Location = new System.Drawing.Point(210, 67); this.lblTestWeightUnit.AutoSize = true;
            pnlEdit.Controls.Add(this.lblTestWeight); pnlEdit.Controls.Add(this.numTestWeight); pnlEdit.Controls.Add(this.lblTestWeightUnit);
            // Market Moisture
            this.lblMktMoisture.Text = "Mkt Moisture:"; this.lblMktMoisture.Font = lf; this.lblMktMoisture.Location = new System.Drawing.Point(8, 99); this.lblMktMoisture.AutoSize = false; this.lblMktMoisture.Width = 108;
            this.numMarketMoisture.Font = vf; this.numMarketMoisture.Location = new System.Drawing.Point(120, 96); this.numMarketMoisture.Width = 80; this.numMarketMoisture.Minimum = 0; this.numMarketMoisture.Maximum = 40; this.numMarketMoisture.Increment = 1; this.numMarketMoisture.Value = 14; this.numMarketMoisture.DecimalPlaces = 0;
            this.lblMktMoistureUnit.Text = "%"; this.lblMktMoistureUnit.Font = vf; this.lblMktMoistureUnit.Location = new System.Drawing.Point(210, 99); this.lblMktMoistureUnit.AutoSize = true;
            pnlEdit.Controls.Add(this.lblMktMoisture); pnlEdit.Controls.Add(this.numMarketMoisture); pnlEdit.Controls.Add(this.lblMktMoistureUnit);

            this.pnlContent.Controls.Add(this.lbCrops);
            this.pnlContent.Controls.Add(this.pnlEdit);

            // Buttons row at bottom of pnlContent
            this.btnNew.Text = "New"; this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold); this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnNew.Location = new System.Drawing.Point(8, 328); this.btnNew.Size = new System.Drawing.Size(80, 36);
            this.btnSave.Text = "Save"; this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold); this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnSave.Location = new System.Drawing.Point(96, 328); this.btnSave.Size = new System.Drawing.Size(80, 36);
            this.btnDelete.Text = "Delete"; this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold); this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDelete.Location = new System.Drawing.Point(184, 328); this.btnDelete.Size = new System.Drawing.Size(80, 36);
            this.btnCropsClose.Text = "Close"; this.btnCropsClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold); this.btnCropsClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnCropsClose.Location = new System.Drawing.Point(368, 328); this.btnCropsClose.Size = new System.Drawing.Size(80, 36);
            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] { btnNew, btnSave, btnDelete, btnCropsClose });

            btnNew.Click        += new System.EventHandler(this.btnNew_Click);
            btnSave.Click       += new System.EventHandler(this.btnSave_Click);
            btnDelete.Click     += new System.EventHandler(this.btnDelete_Click);
            btnCropsClose.Click += new System.EventHandler(this.btnCropsClose_Click);

            // ── Form ──────────────────────────────────────────────────────────
            this.ClientSize      = new System.Drawing.Size(456, 410);
            this.MinimumSize     = new System.Drawing.Size(456, 410);
            this.MaximumSize     = new System.Drawing.Size(456, 410);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding         = new System.Windows.Forms.Padding(2);
            this.BackColor       = System.Drawing.Color.White;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Name            = "frmMenuCrops";
            this.Text            = "Crops";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuCrops_Load);

            this.ResumeLayout(false);
        }

        private void SetRow(System.Windows.Forms.Panel parent,
            System.Windows.Forms.Label lbl, string text, System.Drawing.Font lf, int y,
            System.Windows.Forms.Control input, System.Drawing.Font vf, int ix, int iy, int iw, int ih)
        {
            lbl.Text = text; lbl.Font = lf;
            lbl.Location = new System.Drawing.Point(8, y + 3); lbl.AutoSize = false; lbl.Width = 108;
            input.Font   = vf;
            input.Location = new System.Drawing.Point(ix, iy);
            if (ih > 0) input.Height = ih;
            input.Width  = iw;
            parent.Controls.Add(lbl); parent.Controls.Add(input);
        }

        private void SetNumRow(System.Windows.Forms.Panel parent,
            System.Windows.Forms.Label lbl, string text, System.Drawing.Font lf, int y,
            System.Windows.Forms.NumericUpDown num, System.Drawing.Font vf,
            int nx, int ny, int nw, decimal min, decimal max, decimal inc, decimal def,
            System.Windows.Forms.Label unitLbl, string unit, int ux, int uy)
        {
            lbl.Text = text; lbl.Font = lf;
            lbl.Location = new System.Drawing.Point(8, y + 3); lbl.AutoSize = false; lbl.Width = 108;
            num.Font = vf; num.Location = new System.Drawing.Point(nx, ny); num.Width = nw;
            num.Minimum = min; num.Maximum = max; num.Increment = inc; num.Value = def;
            num.DecimalPlaces = inc < 1 ? 1 : 0;
            unitLbl.Text = unit; unitLbl.Font = vf;
            unitLbl.Location = new System.Drawing.Point(ux, uy + 3); unitLbl.AutoSize = true;
            parent.Controls.Add(lbl); parent.Controls.Add(num); parent.Controls.Add(unitLbl);
        }

        private void SetBtn(System.Windows.Forms.Button btn, System.Windows.Forms.Panel parent,
            string text, int x, int y, int w, int h)
        {
            btn.Text = text;
            btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.Location  = new System.Drawing.Point(x, y);
            btn.Size      = new System.Drawing.Size(w, h);
            parent.Controls.Add(btn);
        }

        private System.Windows.Forms.Panel         pnlTitle;
        private System.Windows.Forms.Label         lblTitle;
        private System.Windows.Forms.Button        btnTitleClose;
        private System.Windows.Forms.Panel         pnlContent;
        private System.Windows.Forms.ListBox       lbCrops;
        private System.Windows.Forms.Panel         pnlEdit;
        private System.Windows.Forms.Label         lblCropName;
        private System.Windows.Forms.TextBox       txtCropName;
        private System.Windows.Forms.Label         lblCategory;
        private System.Windows.Forms.ComboBox      cboCropCategory;
        private System.Windows.Forms.Label         lblTestWeight;
        private System.Windows.Forms.NumericUpDown numTestWeight;
        private System.Windows.Forms.Label         lblTestWeightUnit;
        private System.Windows.Forms.Label         lblMktMoisture;
        private System.Windows.Forms.NumericUpDown numMarketMoisture;
        private System.Windows.Forms.Label         lblMktMoistureUnit;
        private System.Windows.Forms.Button        btnNew;
        private System.Windows.Forms.Button        btnSave;
        private System.Windows.Forms.Button        btnDelete;
        private System.Windows.Forms.Button        btnCropsClose;
    }
}
