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
            this.lblTitle          = new System.Windows.Forms.Label();
            this.lblYieldSlider    = new System.Windows.Forms.Label();
            this.trkYield          = new System.Windows.Forms.TrackBar();
            this.lblMoistureSlider = new System.Windows.Forms.Label();
            this.trkMoisture       = new System.Windows.Forms.TrackBar();
            this.lblTempSlider     = new System.Windows.Forms.Label();
            this.trkTemperature    = new System.Windows.Forms.TrackBar();
            this.chkSections       = new System.Windows.Forms.CheckBox();
            this.chkSineWave       = new System.Windows.Forms.CheckBox();
            this.lblVariationSlider = new System.Windows.Forms.Label();
            this.trkVariation      = new System.Windows.Forms.TrackBar();
            this.lblSensor1        = new System.Windows.Forms.Label();
            this.lblMoistureVal    = new System.Windows.Forms.Label();
            this.lblTempVal        = new System.Windows.Forms.Label();
            this.lblStatus         = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trkYield)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMoisture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkTemperature)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkVariation)).BeginInit();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Text     = "YieldFlo Simulator";
            this.lblTitle.Font     = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(10, 10);
            this.lblTitle.AutoSize = true;

            // Yield
            this.lblYieldSlider.Text     = "Yield Flow (0–100%):";
            this.lblYieldSlider.Location = new System.Drawing.Point(10, 55);
            this.lblYieldSlider.AutoSize = true;

            this.trkYield.Location      = new System.Drawing.Point(10, 75);
            this.trkYield.Size          = new System.Drawing.Size(360, 45);
            this.trkYield.Minimum       = 0;
            this.trkYield.Maximum       = 100;
            this.trkYield.Value         = 75;
            this.trkYield.TickFrequency = 10;

            // Moisture
            this.lblMoistureSlider.Text     = "Moisture (0–30%):";
            this.lblMoistureSlider.Location = new System.Drawing.Point(10, 130);
            this.lblMoistureSlider.AutoSize = true;

            this.trkMoisture.Location      = new System.Drawing.Point(10, 150);
            this.trkMoisture.Size          = new System.Drawing.Size(360, 45);
            this.trkMoisture.Minimum       = 0;
            this.trkMoisture.Maximum       = 300;
            this.trkMoisture.Value         = 140;   // 14.0 %
            this.trkMoisture.TickFrequency = 30;

            // Temperature
            this.lblTempSlider.Text     = "Temperature (–10 to 50°C):";
            this.lblTempSlider.Location = new System.Drawing.Point(10, 205);
            this.lblTempSlider.AutoSize = true;

            this.trkTemperature.Location      = new System.Drawing.Point(10, 225);
            this.trkTemperature.Size          = new System.Drawing.Size(360, 45);
            this.trkTemperature.Minimum       = -100;  // –10.0°C
            this.trkTemperature.Maximum       = 500;   // 50.0°C
            this.trkTemperature.Value         = 200;   // 20.0°C
            this.trkTemperature.TickFrequency = 50;

            // Harvesting checkbox
            this.chkSections.Text          = "Harvesting";
            this.chkSections.Location      = new System.Drawing.Point(10, 282);
            this.chkSections.AutoSize      = true;
            this.chkSections.Checked       = false;
            this.chkSections.Font          = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.chkSections.ForeColor     = System.Drawing.Color.DarkGreen;
            this.chkSections.CheckedChanged += new System.EventHandler(this.chkSections_CheckedChanged);

            // Sine wave checkbox + variation
            this.chkSineWave.Text     = "Yield variation";
            this.chkSineWave.Location = new System.Drawing.Point(10, 310);
            this.chkSineWave.AutoSize = true;
            this.chkSineWave.Checked  = true;

            this.lblVariationSlider.Text     = "Variation: 5%";
            this.lblVariationSlider.Location = new System.Drawing.Point(200, 310);
            this.lblVariationSlider.AutoSize = true;

            this.trkVariation.Location      = new System.Drawing.Point(200, 327);
            this.trkVariation.Size          = new System.Drawing.Size(180, 45);
            this.trkVariation.Minimum       = 1;
            this.trkVariation.Maximum       = 50;
            this.trkVariation.Value         = 5;
            this.trkVariation.TickFrequency = 5;

            // Readout labels
            this.lblSensor1.Text     = "S1: 0.000";
            this.lblSensor1.Font     = new System.Drawing.Font("Courier New", 11F);
            this.lblSensor1.Location = new System.Drawing.Point(10, 383);
            this.lblSensor1.AutoSize = true;

            this.lblMoistureVal.Text     = "Mst: 0.0%";
            this.lblMoistureVal.Font     = new System.Drawing.Font("Courier New", 11F);
            this.lblMoistureVal.Location = new System.Drawing.Point(140, 383);
            this.lblMoistureVal.AutoSize = true;

            this.lblTempVal.Text     = "Tmp: 0.0°C";
            this.lblTempVal.Font     = new System.Drawing.Font("Courier New", 11F);
            this.lblTempVal.Location = new System.Drawing.Point(270, 383);
            this.lblTempVal.AutoSize = true;

            // Status
            this.lblStatus.Text      = "Initializing...";
            this.lblStatus.Location  = new System.Drawing.Point(10, 418);
            this.lblStatus.AutoSize  = true;
            this.lblStatus.ForeColor = System.Drawing.Color.DarkGreen;

            // Form
            this.ClientSize      = new System.Drawing.Size(400, 448);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblTitle,
                lblYieldSlider, trkYield,
                lblMoistureSlider, trkMoisture,
                lblTempSlider, trkTemperature,
                chkSections,
                chkSineWave, lblVariationSlider, trkVariation,
                lblSensor1, lblMoistureVal, lblTempVal,
                lblStatus });
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.Name            = "frmSimulator";
            this.Text            = "YieldFlo Simulator";
            this.Icon            = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.Load           += new System.EventHandler(this.frmSimulator_Load);

            ((System.ComponentModel.ISupportInitialize)(this.trkYield)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkMoisture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkTemperature)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkVariation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label    lblTitle;
        private System.Windows.Forms.Label    lblYieldSlider;
        private System.Windows.Forms.TrackBar trkYield;
        private System.Windows.Forms.Label    lblMoistureSlider;
        private System.Windows.Forms.TrackBar trkMoisture;
        private System.Windows.Forms.Label    lblTempSlider;
        private System.Windows.Forms.TrackBar trkTemperature;
        private System.Windows.Forms.CheckBox chkSections;
        private System.Windows.Forms.CheckBox chkSineWave;
        private System.Windows.Forms.Label    lblVariationSlider;
        private System.Windows.Forms.TrackBar trkVariation;
        private System.Windows.Forms.Label    lblSensor1;
        private System.Windows.Forms.Label    lblMoistureVal;
        private System.Windows.Forms.Label    lblTempVal;
        private System.Windows.Forms.Label    lblStatus;
    }
}
