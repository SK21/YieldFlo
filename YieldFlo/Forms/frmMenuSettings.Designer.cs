using YieldFlo.Language;

namespace YieldFlo.Forms
{
    partial class frmMenuSettings
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblUnits = new System.Windows.Forms.Label();
            this.btnImperial = new System.Windows.Forms.Button();
            this.btnMetric = new System.Windows.Forms.Button();
            this.lblResumeJob = new System.Windows.Forms.Label();
            this.btnResumeOn = new System.Windows.Forms.Button();
            this.btnResumeOff = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnSettingsClose = new System.Windows.Forms.Button();
            this.pnlTitle.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTitle
            //
            this.pnlTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(2, 2);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(560, 48);
            this.pnlTitle.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(560, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Settings";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlContent
            //
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.pnlContent.Controls.Add(this.lblUnits);
            this.pnlContent.Controls.Add(this.btnImperial);
            this.pnlContent.Controls.Add(this.btnMetric);
            this.pnlContent.Controls.Add(this.lblResumeJob);
            this.pnlContent.Controls.Add(this.btnResumeOn);
            this.pnlContent.Controls.Add(this.btnResumeOff);
            this.pnlContent.Controls.Add(this.btnSaveSettings);
            this.pnlContent.Controls.Add(this.btnSettingsClose);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(2, 50);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(560, 246);
            this.pnlContent.TabIndex = 1;
            //
            // lblUnits
            //
            this.lblUnits.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblUnits.ForeColor = System.Drawing.Color.Silver;
            this.lblUnits.Location = new System.Drawing.Point(8, 10);
            this.lblUnits.Name = "lblUnits";
            this.lblUnits.Size = new System.Drawing.Size(300, 26);
            this.lblUnits.TabIndex = 0;
            this.lblUnits.Text = "Units";
            //
            // btnImperial
            //
            this.btnImperial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnImperial.FlatAppearance.BorderSize = 0;
            this.btnImperial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImperial.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnImperial.ForeColor = System.Drawing.Color.White;
            this.btnImperial.Location = new System.Drawing.Point(8, 40);
            this.btnImperial.Name = "btnImperial";
            this.btnImperial.Size = new System.Drawing.Size(267, 48);
            this.btnImperial.TabIndex = 1;
            this.btnImperial.Text = global::YieldFlo.Language.Lang.lgImperial;
            this.btnImperial.UseVisualStyleBackColor = false;
            this.btnImperial.Click += new System.EventHandler(this.btnImperial_Click);
            //
            // btnMetric
            //
            this.btnMetric.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnMetric.FlatAppearance.BorderSize = 0;
            this.btnMetric.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMetric.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnMetric.ForeColor = System.Drawing.Color.White;
            this.btnMetric.Location = new System.Drawing.Point(285, 40);
            this.btnMetric.Name = "btnMetric";
            this.btnMetric.Size = new System.Drawing.Size(267, 48);
            this.btnMetric.TabIndex = 2;
            this.btnMetric.Text = global::YieldFlo.Language.Lang.lgMetric;
            this.btnMetric.UseVisualStyleBackColor = false;
            this.btnMetric.Click += new System.EventHandler(this.btnMetric_Click);
            //
            // lblResumeJob
            //
            this.lblResumeJob.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblResumeJob.ForeColor = System.Drawing.Color.Silver;
            this.lblResumeJob.Location = new System.Drawing.Point(8, 100);
            this.lblResumeJob.Name = "lblResumeJob";
            this.lblResumeJob.Size = new System.Drawing.Size(400, 26);
            this.lblResumeJob.TabIndex = 3;
            this.lblResumeJob.Text = "Resume Job on Start";
            //
            // btnResumeOn
            //
            this.btnResumeOn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnResumeOn.FlatAppearance.BorderSize = 0;
            this.btnResumeOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResumeOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnResumeOn.ForeColor = System.Drawing.Color.White;
            this.btnResumeOn.Location = new System.Drawing.Point(8, 130);
            this.btnResumeOn.Name = "btnResumeOn";
            this.btnResumeOn.Size = new System.Drawing.Size(267, 48);
            this.btnResumeOn.TabIndex = 4;
            this.btnResumeOn.Text = global::YieldFlo.Language.Lang.lgOn;
            this.btnResumeOn.UseVisualStyleBackColor = false;
            this.btnResumeOn.Click += new System.EventHandler(this.btnResumeOn_Click);
            //
            // btnResumeOff
            //
            this.btnResumeOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnResumeOff.FlatAppearance.BorderSize = 0;
            this.btnResumeOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResumeOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnResumeOff.ForeColor = System.Drawing.Color.White;
            this.btnResumeOff.Location = new System.Drawing.Point(285, 130);
            this.btnResumeOff.Name = "btnResumeOff";
            this.btnResumeOff.Size = new System.Drawing.Size(267, 48);
            this.btnResumeOff.TabIndex = 5;
            this.btnResumeOff.Text = global::YieldFlo.Language.Lang.lgOff;
            this.btnResumeOff.UseVisualStyleBackColor = false;
            this.btnResumeOff.Click += new System.EventHandler(this.btnResumeOff_Click);
            //
            // btnSaveSettings
            //
            this.btnSaveSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(110)))), ((int)(((byte)(0)))));
            this.btnSaveSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnSaveSettings.ForeColor = System.Drawing.Color.White;
            this.btnSaveSettings.Location = new System.Drawing.Point(8, 190);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(130, 44);
            this.btnSaveSettings.TabIndex = 6;
            this.btnSaveSettings.Text = global::YieldFlo.Language.Lang.lgSave;
            this.btnSaveSettings.UseVisualStyleBackColor = false;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            //
            // btnSettingsClose
            //
            this.btnSettingsClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnSettingsClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettingsClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnSettingsClose.ForeColor = System.Drawing.Color.White;
            this.btnSettingsClose.Location = new System.Drawing.Point(422, 190);
            this.btnSettingsClose.Name = "btnSettingsClose";
            this.btnSettingsClose.Size = new System.Drawing.Size(130, 44);
            this.btnSettingsClose.TabIndex = 7;
            this.btnSettingsClose.Text = global::YieldFlo.Language.Lang.lgClose;
            this.btnSettingsClose.UseVisualStyleBackColor = false;
            this.btnSettingsClose.Click += new System.EventHandler(this.btnSettingsClose_Click);
            //
            // frmMenuSettings
            //
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(564, 298);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitle);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMenuSettings";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmMenuSettings_Load);
            this.pnlTitle.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel    pnlTitle;
        private System.Windows.Forms.Label    lblTitle;
        private System.Windows.Forms.Panel    pnlContent;
        private System.Windows.Forms.Label    lblUnits;
        private System.Windows.Forms.Button   btnImperial;
        private System.Windows.Forms.Button   btnMetric;
        private System.Windows.Forms.Label    lblResumeJob;
        private System.Windows.Forms.Button   btnResumeOn;
        private System.Windows.Forms.Button   btnResumeOff;
        private System.Windows.Forms.Button   btnSaveSettings;
        private System.Windows.Forms.Button   btnSettingsClose;
    }
}
