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
            ((System.ComponentModel.ISupportInitialize)(this.trkYield)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMoisture)).BeginInit();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Text = "YieldFlo Module Simulator";
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
            this.trkYield.Value = 70;
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

            // chkSineWave
            this.chkSineWave.Text = "Sine-wave modulation (simulates variable yield)";
            this.chkSineWave.Location = new System.Drawing.Point(10, 205);
            this.chkSineWave.AutoSize = true;
            this.chkSineWave.Checked = true;

            // lblSensor1
            this.lblSensor1.Text = "S1: 0.000";
            this.lblSensor1.Font = new System.Drawing.Font("Courier New", 11F);
            this.lblSensor1.Location = new System.Drawing.Point(10, 240);
            this.lblSensor1.AutoSize = true;

            // lblSensor2
            this.lblSensor2.Text = "S2: 0.000";
            this.lblSensor2.Font = new System.Drawing.Font("Courier New", 11F);
            this.lblSensor2.Location = new System.Drawing.Point(150, 240);
            this.lblSensor2.AutoSize = true;

            // lblMoistureVal
            this.lblMoistureVal.Text = "Mst: 0.0%";
            this.lblMoistureVal.Font = new System.Drawing.Font("Courier New", 11F);
            this.lblMoistureVal.Location = new System.Drawing.Point(290, 240);
            this.lblMoistureVal.AutoSize = true;

            // lblStatus
            this.lblStatus.Text = "Initializing...";
            this.lblStatus.Location = new System.Drawing.Point(10, 275);
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.DarkGreen;

            // frmSimulator
            this.ClientSize = new System.Drawing.Size(400, 310);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblTitle, lblYieldSlider, trkYield,
                lblMoistureSlider, trkMoisture,
                chkSineWave,
                lblSensor1, lblSensor2, lblMoistureVal,
                lblStatus });
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmSimulator";
            this.Text = "YieldFlo Module Simulator";
            this.Load += new System.EventHandler(this.frmSimulator_Load);

            ((System.ComponentModel.ISupportInitialize)(this.trkYield)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMoisture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblYieldSlider;
        private System.Windows.Forms.TrackBar trkYield;
        private System.Windows.Forms.Label lblMoistureSlider;
        private System.Windows.Forms.TrackBar trkMoisture;
        private System.Windows.Forms.CheckBox chkSineWave;
        private System.Windows.Forms.Label lblSensor1;
        private System.Windows.Forms.Label lblSensor2;
        private System.Windows.Forms.Label lblMoistureVal;
        private System.Windows.Forms.Label lblStatus;
    }
}
