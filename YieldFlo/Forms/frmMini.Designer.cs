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
            this.pnlContent = new System.Windows.Forms.Panel();
            this.btnRestore = new System.Windows.Forms.Button();
            this.lblYield = new System.Windows.Forms.Label();
            this.lblUnit = new System.Windows.Forms.Label();
            this.pnlContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.btnRestore);
            this.pnlContent.Controls.Add(this.lblYield);
            this.pnlContent.Controls.Add(this.lblUnit);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(2, 2);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(181, 92);
            this.pnlContent.TabIndex = 0;
            // 
            // btnRestore
            // 
            this.btnRestore.FlatAppearance.BorderSize = 0;
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.btnRestore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.btnRestore.Location = new System.Drawing.Point(139, 3);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(40, 34);
            this.btnRestore.TabIndex = 1;
            this.btnRestore.Text = "▣";
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // lblYield
            // 
            this.lblYield.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold);
            this.lblYield.Location = new System.Drawing.Point(0, 40);
            this.lblYield.Name = "lblYield";
            this.lblYield.Size = new System.Drawing.Size(179, 48);
            this.lblYield.TabIndex = 2;
            this.lblYield.Text = "---.-";
            this.lblYield.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUnit
            // 
            this.lblUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblUnit.Location = new System.Drawing.Point(3, 9);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(130, 26);
            this.lblUnit.TabIndex = 3;
            this.lblUnit.Text = "bu/ac";
            this.lblUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmMini
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(185, 96);
            this.Controls.Add(this.pnlContent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMini";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "YieldFlo";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMini_FormClosing);
            this.Load += new System.EventHandler(this.frmMini_Load);
            this.pnlContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel  pnlContent;
        private System.Windows.Forms.Label  lblYield;
        private System.Windows.Forms.Label  lblUnit;
        private System.Windows.Forms.Button btnRestore;
    }
}
