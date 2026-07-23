using YieldFlo.Language;

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
            this.pnlTitle.Height = 48;

            this.lblTitle.Text      = Lang.lgTitleCrops;
            this.lblTitle.Font      = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.AutoSize  = false;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.pnlTitle.Controls.Add(this.lblTitle);

            // ── Content ───────────────────────────────────────────────────────
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;

            this.lbCrops.Location      = new System.Drawing.Point(4, 4);
            this.lbCrops.Size          = new System.Drawing.Size(552, 170);
            this.lbCrops.Font          = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lbCrops.SelectedIndexChanged += new System.EventHandler(this.lbCrops_SelectedIndexChanged);

            // Edit panel
            this.pnlEdit.Location  = new System.Drawing.Point(4, 180);
            this.pnlEdit.Size      = new System.Drawing.Size(552, 164);

            var lf = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            var vf = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);

            // Name
            this.lblCropName.Text = Lang.lgName; this.lblCropName.Font = lf; this.lblCropName.Location = new System.Drawing.Point(8, 4); this.lblCropName.AutoSize = false; this.lblCropName.Size = new System.Drawing.Size(220, 32);
            this.txtCropName.Font = vf; this.txtCropName.Location = new System.Drawing.Point(236, 4); this.txtCropName.Height = 32; this.txtCropName.Width = 310;
            pnlEdit.Controls.Add(this.lblCropName); pnlEdit.Controls.Add(this.txtCropName);
            // Category
            this.lblCategory.Text = Lang.lgCategoryLabel; this.lblCategory.Font = lf; this.lblCategory.Location = new System.Drawing.Point(8, 44); this.lblCategory.AutoSize = false; this.lblCategory.Size = new System.Drawing.Size(220, 32);
            this.cboCropCategory.Font = vf; this.cboCropCategory.Location = new System.Drawing.Point(236, 44); this.cboCropCategory.Width = 180;
            this.cboCropCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCropCategory.Items.AddRange(new object[] { "Cereal", "OilSeed", "Corn", "Pulse", "Other" });
            this.cboCropCategory.SelectedIndex = 0;
            pnlEdit.Controls.Add(this.lblCategory); pnlEdit.Controls.Add(this.cboCropCategory);
            // Test Weight
            this.lblTestWeight.Text = Lang.lgTestWeight; this.lblTestWeight.Font = lf; this.lblTestWeight.Location = new System.Drawing.Point(8, 84); this.lblTestWeight.AutoSize = false; this.lblTestWeight.Size = new System.Drawing.Size(220, 32);
            this.numTestWeight.Font = vf; this.numTestWeight.Location = new System.Drawing.Point(236, 84); this.numTestWeight.Width = 100; this.numTestWeight.Height = 32; this.numTestWeight.Minimum = 0; this.numTestWeight.Maximum = 200; this.numTestWeight.Increment = 1; this.numTestWeight.Value = 60; this.numTestWeight.DecimalPlaces = 0;
            this.lblTestWeightUnit.Text = "lb/bu"; this.lblTestWeightUnit.Font = vf; this.lblTestWeightUnit.Location = new System.Drawing.Point(344, 88); this.lblTestWeightUnit.AutoSize = true;
            pnlEdit.Controls.Add(this.lblTestWeight); pnlEdit.Controls.Add(this.numTestWeight); pnlEdit.Controls.Add(this.lblTestWeightUnit);
            // Market Moisture
            this.lblMktMoisture.Text = Lang.lgMktMoisture; this.lblMktMoisture.Font = lf; this.lblMktMoisture.Location = new System.Drawing.Point(8, 124); this.lblMktMoisture.AutoSize = false; this.lblMktMoisture.Size = new System.Drawing.Size(220, 32);
            this.numMarketMoisture.Font = vf; this.numMarketMoisture.Location = new System.Drawing.Point(236, 124); this.numMarketMoisture.Width = 100; this.numMarketMoisture.Height = 32; this.numMarketMoisture.Minimum = 0; this.numMarketMoisture.Maximum = 40; this.numMarketMoisture.Increment = 1; this.numMarketMoisture.Value = 14; this.numMarketMoisture.DecimalPlaces = 0;
            this.lblMktMoistureUnit.Text = "%"; this.lblMktMoistureUnit.Font = vf; this.lblMktMoistureUnit.Location = new System.Drawing.Point(344, 128); this.lblMktMoistureUnit.AutoSize = true;
            pnlEdit.Controls.Add(this.lblMktMoisture); pnlEdit.Controls.Add(this.numMarketMoisture); pnlEdit.Controls.Add(this.lblMktMoistureUnit);

            this.pnlContent.Controls.Add(this.lbCrops);
            this.pnlContent.Controls.Add(this.pnlEdit);

            // Buttons row
            this.btnNew.Text = Lang.lgNew; this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold); this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnNew.Location = new System.Drawing.Point(8, 352); this.btnNew.Size = new System.Drawing.Size(130, 44);
            this.btnSave.Text = Lang.lgSave; this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold); this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnSave.Location = new System.Drawing.Point(146, 352); this.btnSave.Size = new System.Drawing.Size(130, 44);
            this.btnDelete.Text = Lang.lgDelete; this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold); this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnDelete.Location = new System.Drawing.Point(284, 352); this.btnDelete.Size = new System.Drawing.Size(130, 44);
            this.btnCropsClose.Text = Lang.lgClose; this.btnCropsClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold); this.btnCropsClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat; this.btnCropsClose.Location = new System.Drawing.Point(422, 352); this.btnCropsClose.Size = new System.Drawing.Size(130, 44);
            this.pnlContent.Controls.AddRange(new System.Windows.Forms.Control[] { btnNew, btnSave, btnDelete, btnCropsClose });

            btnNew.Click        += new System.EventHandler(this.btnNew_Click);
            btnSave.Click       += new System.EventHandler(this.btnSave_Click);
            btnDelete.Click     += new System.EventHandler(this.btnDelete_Click);
            btnCropsClose.Click += new System.EventHandler(this.btnCropsClose_Click);

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
            this.Name            = "frmMenuCrops";
            this.Text            = "Crops";
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Load += new System.EventHandler(this.frmMenuCrops_Load);

            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel         pnlTitle;
        private System.Windows.Forms.Label         lblTitle;
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
