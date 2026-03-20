namespace ModuleSimulator
{
    partial class frmSimulator
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblYieldSlider = new System.Windows.Forms.Label();
            this.trkYield = new System.Windows.Forms.TrackBar();
            this.lblMoistureSlider = new System.Windows.Forms.Label();
            this.trkMoisture = new System.Windows.Forms.TrackBar();
            this.chkSineWave = new System.Windows.Forms.CheckBox();
            this.lblSensor1 = new System.Windows.Forms.Label();
            this.lblSensor2 = new System.Windows.Forms.Label();
            this.lblMoistureVal = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.chkSections = new System.Windows.Forms.CheckBox();
            this.lblVariationSlider = new System.Windows.Forms.Label();
            this.trkVariation = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trkYield)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMoisture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkVariation)).BeginInit();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Text = "YieldFlo Simulator";
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(10, 10);
            this.lblTitle.AutoSize = true;

            // lblYieldSlider
            this.lblYieldSlider.Text = "Yield Flow (0–100%):";
            this.lblYieldSlider.Location = new System.Drawing.Point(10, 55);
            this.lblYieldSlider.AutoSize = true;

            // trkYield
            this.trkYield.Location = new System.Drawing.Point(10, 75);
            this.trkYield.Size = new System.Drawing.Size(360, 45);
            this.trkYield.Minimum = 0;
            this.trkYield.Maximum = 100;
            this.trkYield.Value = 75;
            this.trkYield.TickFrequency = 10;

            // lblMoistureSlider
            this.lblMoistureSlider.Text = "Moisture (0–30%):";
            this.lblMoistureSlider.Location = new System.Drawing.Point(10, 130);
            this.lblMoistureSlider.AutoSize = true;

            // trkMoisture
            this.trkMoisture.Location = new System.Drawing.Point(10, 150);
            this.trkMoisture.Size = new System.Drawing.Size(360, 45);
            this.trkMoisture.Minimum = 0;
            this.trkMoisture.Maximum = 300;
            this.trkMoisture.Value = 140;   // 14.0%
            this.trkMoisture.TickFrequency = 30;

            // chkSections
            this.chkSections.Text      = "Harvesting";
            this.chkSections.Location  = new System.Drawing.Point(10, 205);
            this.chkSections.AutoSize  = true;
            this.chkSections.Checked   = false;
            this.chkSections.Font      = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.chkSections.ForeColor = System.Drawing.Color.DarkGreen;
            this.chkSections.CheckedChanged += new System.EventHandler(this.chkSections_CheckedChanged);

            // chkSineWave
            this.chkSineWave.Text = "Yield variation";
            this.chkSineWave.Location = new System.Drawing.Point(10, 232);
            this.chkSineWave.AutoSize = true;
            this.chkSineWave.Checked = true;

            // lblVariationSlider
            this.lblVariationSlider.Text = "Variation: 5%";
            this.lblVariationSlider.Location = new System.Drawing.Point(200, 232);
            this.lblVariationSlider.AutoSize = true;

            // trkVariation
            this.trkVariation.Location = new System.Drawing.Point(200, 249);
            this.trkVariation.Size = new System.Drawing.Size(180, 45);
            this.trkVariation.Minimum = 1;
            this.trkVariation.Maximum = 50;
            this.trkVariation.Value = 5;
            this.trkVariation.TickFrequency = 5;

            // lblSensor1
            this.lblSensor1.Text = "S1: 0.000";
            this.lblSensor1.Font = new System.Drawing.Font("Courier New", 11F);
            this.lblSensor1.Location = new System.Drawing.Point(10, 305);
            this.lblSensor1.AutoSize = true;

            // lblSensor2
            this.lblSensor2.Text = "S2: 0.000";
            this.lblSensor2.Font = new System.Drawing.Font("Courier New", 11F);
            this.lblSensor2.Location = new System.Drawing.Point(150, 305);
            this.lblSensor2.AutoSize = true;

            // lblMoistureVal
            this.lblMoistureVal.Text = "Mst: 0.0%";
            this.lblMoistureVal.Font = new System.Drawing.Font("Courier New", 11F);
            this.lblMoistureVal.Location = new System.Drawing.Point(290, 305);
            this.lblMoistureVal.AutoSize = true;

            // lblStatus
            this.lblStatus.Text = "Initializing...";
            this.lblStatus.Location = new System.Drawing.Point(10, 340);
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.DarkGreen;

            // frmSimulator
            this.ClientSize = new System.Drawing.Size(400, 370);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblTitle, lblYieldSlider, trkYield,
                lblMoistureSlider, trkMoisture,
                chkSections,
                chkSineWave, lblVariationSlider, trkVariation,
                lblSensor1, lblSensor2, lblMoistureVal,
                lblStatus });
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmSimulator";
            this.Text = "YieldFlo Simulator";
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.Load += new System.EventHandler(this.frmSimulator_Load);

            ((System.ComponentModel.ISupportInitialize)(this.trkYield)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMoisture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkVariation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblYieldSlider;
        private System.Windows.Forms.TrackBar trkYield;
        private System.Windows.Forms.Label lblMoistureSlider;
        private System.Windows.Forms.TrackBar trkMoisture;
        private System.Windows.Forms.CheckBox chkSections;
        private System.Windows.Forms.CheckBox chkSineWave;
        private System.Windows.Forms.Label lblVariationSlider;
        private System.Windows.Forms.TrackBar trkVariation;
        private System.Windows.Forms.Label lblSensor1;
        private System.Windows.Forms.Label lblSensor2;
        private System.Windows.Forms.Label lblMoistureVal;
        private System.Windows.Forms.Label lblStatus;
    }
}
