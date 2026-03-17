namespace YieldFlo.Forms
{
    partial class frmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.btnMenu = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();

            this.pnlGauges = new System.Windows.Forms.Panel();
            this.pnlYield = new System.Windows.Forms.Panel();
            this.lblYieldTitle = new System.Windows.Forms.Label();
            this.lblYield = new System.Windows.Forms.Label();
            this.lblYieldUnit = new System.Windows.Forms.Label();

            this.pnlMoisture = new System.Windows.Forms.Panel();
            this.lblMoistureTitle = new System.Windows.Forms.Label();
            this.lblMoisture = new System.Windows.Forms.Label();
            this.lblMoistureUnit = new System.Windows.Forms.Label();

            this.pnlSpeedPanel = new System.Windows.Forms.Panel();
            this.lblSpeedTitle = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblSpeedUnit = new System.Windows.Forms.Label();

            this.pnlTotals = new System.Windows.Forms.Panel();
            this.lblTotArea  = new System.Windows.Forms.Label();
            this.lblTotTotal = new System.Windows.Forms.Label();
            this.lblTotRate  = new System.Windows.Forms.Label();

            this.pnlSensors = new System.Windows.Forms.Panel();
            this.lblSensorHeader = new System.Windows.Forms.Label();
            this.lblSensor1Title = new System.Windows.Forms.Label();
            this.pnlSensor1 = new System.Windows.Forms.Panel();
            this.pnlSensor1Fill = new System.Windows.Forms.Panel();
            this.lblSensor1Value = new System.Windows.Forms.Label();
            this.lblSensor2Title = new System.Windows.Forms.Label();
            this.pnlSensor2 = new System.Windows.Forms.Panel();
            this.pnlSensor2Fill = new System.Windows.Forms.Panel();
            this.lblSensor2Value = new System.Windows.Forms.Label();

            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatusGPS = new System.Windows.Forms.Label();
            this.lblStatusModule = new System.Windows.Forms.Label();
            this.lblStatusComm = new System.Windows.Forms.Label();
            this.lblStatusJob = new System.Windows.Forms.Label();
            this.lblStatusMsg = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();

            this.pnlToolbar.SuspendLayout();
            this.pnlGauges.SuspendLayout();
            this.SuspendLayout();

            // ── Toolbar (drag handle + buttons) ───────────────────────────────
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Height = 44;
            this.pnlToolbar.Controls.AddRange(new System.Windows.Forms.Control[] {
                btnMenu, btnStart, btnPause, btnStop, btnExit, lblTitle });

            // Buttons: x, y, w=68, h=34
            SetupButton(btnMenu,  "Menu",   4,  5, 60, 34);
            SetupButton(btnStart, "Start", 67,  5, 68, 34, System.Drawing.Color.FromArgb(0, 110, 0));
            SetupButton(btnPause, "Pause",138,  5, 68, 34, System.Drawing.Color.FromArgb(110, 88, 0));
            SetupButton(btnStop,  "Stop", 209,  5, 68, 34, System.Drawing.Color.FromArgb(130, 0, 0));
            SetupButton(btnExit,  "Exit", 280,  5, 52, 34, System.Drawing.Color.FromArgb(55, 55, 55));

            this.lblTitle.Text = "YieldFlo";
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(180, 200, 220);
            this.lblTitle.AutoSize = false;
            this.lblTitle.Size = new System.Drawing.Size(164, 44);
            this.lblTitle.Location = new System.Drawing.Point(332, 0);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── Gauges ────────────────────────────────────────────────────────
            this.pnlGauges.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGauges.Height = 148;

            // col widths inside 500px form (minus 2px border each side = 496)
            // Yield: 0–188, Moisture: 190–369, Speed: 371–495
            SetupGaugePanel(pnlYield,     pnlGauges,   2,  2, 188, 144,
                lblYieldTitle, "YIELD",    lblYield,    "--.-", lblYieldUnit,    "bu/ac");
            SetupGaugePanel(pnlMoisture,  pnlGauges, 192,  2, 154, 144,
                lblMoistureTitle, "MOISTURE", lblMoisture, "--.-", lblMoistureUnit, "%");
            SetupGaugePanel(pnlSpeedPanel,pnlGauges, 348,  2, 148, 144,
                lblSpeedTitle, "SPEED",    lblSpeed,    "--.-", lblSpeedUnit,    "km/h");

            // ── Totals ────────────────────────────────────────────────────────
            this.pnlTotals.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTotals.Height = 26;
            SetupTotalsLabel(lblTotRate,    "0.0 bu/ac",   0, 166);
            SetupTotalsLabel(lblTotTotal,   "0 bu",      166, 166);
            SetupTotalsLabel(lblTotArea,    "0.00 ac",   332, 164);
            this.pnlTotals.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblTotArea, lblTotTotal, lblTotRate });

            // ── Sensor bars ───────────────────────────────────────────────────
            this.pnlSensors.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSensors.Height = 52;

            this.lblSensorHeader.Text = "Sensors:";
            this.lblSensorHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            this.lblSensorHeader.ForeColor = System.Drawing.Color.Silver;
            this.lblSensorHeader.Location = new System.Drawing.Point(4, 2);
            this.lblSensorHeader.AutoSize = true;
            this.pnlSensors.Controls.Add(lblSensorHeader);

            // bar track width = form(498) - label(62) - value(38) - margins
            SetupSensorBar(pnlSensor1, pnlSensor1Fill, lblSensor1Value, pnlSensors,
                lblSensor1Title, "Elev Flow", 4, 14, 358, 15);
            SetupSensorBar(pnlSensor2, pnlSensor2Fill, lblSensor2Value, pnlSensors,
                lblSensor2Title, "Moisture ", 4, 33, 358, 15);

            // ── Status bar ────────────────────────────────────────────────────
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.Height = 22;
            this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(28, 28, 28);

            SetupStatusLabel(lblStatusGPS,    "GPS",      4,  4, 36);
            SetupStatusLabel(lblStatusModule, "Module",  44,  4, 54);
            SetupStatusLabel(lblStatusComm,   "UDP",    102,  4, 36);
            SetupStatusLabel(lblStatusJob,    "No Job", 142,  4, 150);
            SetupStatusLabel(lblStatusMsg,    "",       296,  4, 170);

            this.lblVersion.Text = "v1.0";
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(160, 160, 160);
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(468, 6);
            this.pnlStatus.Controls.Add(lblVersion);
            this.pnlStatus.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblStatusGPS, lblStatusModule, lblStatusComm,
                lblStatusJob, lblStatusMsg, lblVersion });

            // ── frmMain ───────────────────────────────────────────────────────
            this.ClientSize = new System.Drawing.Size(500, 294);
            this.MinimumSize = new System.Drawing.Size(500, 294);
            this.MaximumSize = new System.Drawing.Size(500, 294);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Padding = new System.Windows.Forms.Padding(2);  // shows as border
            this.BackColor = System.Drawing.Color.White;          // border color
            this.TopMost = true;
            this.Name = "frmMain";
            this.Text = "YieldFlo";
            this.Controls.Add(pnlSensors);
            this.Controls.Add(pnlTotals);
            this.Controls.Add(pnlGauges);
            this.Controls.Add(pnlToolbar);
            this.Controls.Add(pnlStatus);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);

            this.pnlToolbar.ResumeLayout(false);
            this.pnlGauges.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void SetupButton(System.Windows.Forms.Button btn, string text,
            int x, int y, int w, int h,
            System.Drawing.Color? backColor = null)
        {
            btn.Text = text;
            btn.Location = new System.Drawing.Point(x, y);
            btn.Size = new System.Drawing.Size(w, h);
            btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.ForeColor = System.Drawing.Color.White;
            btn.BackColor = backColor ?? System.Drawing.Color.FromArgb(60, 60, 60);
            btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);

            if (btn == btnMenu)  btn.Click += new System.EventHandler(this.btnMenu_Click);
            else if (btn == btnStart) btn.Click += new System.EventHandler(this.btnStart_Click);
            else if (btn == btnPause) btn.Click += new System.EventHandler(this.btnPause_Click);
            else if (btn == btnStop)  btn.Click += new System.EventHandler(this.btnStop_Click);
            else if (btn == btnExit)  btn.Click += new System.EventHandler(this.btnExit_Click);
        }

        private void SetupGaugePanel(
            System.Windows.Forms.Panel panel,
            System.Windows.Forms.Panel parent,
            int x, int y, int w, int h,
            System.Windows.Forms.Label titleLbl, string titleText,
            System.Windows.Forms.Label valueLbl, string valueText,
            System.Windows.Forms.Label unitLbl,  string unitText)
        {
            panel.Location = new System.Drawing.Point(x, y);
            panel.Size = new System.Drawing.Size(w, h);
            panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            titleLbl.Text = titleText;
            titleLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            titleLbl.Dock = System.Windows.Forms.DockStyle.Top;
            titleLbl.Height = 18;
            titleLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            valueLbl.Text = valueText;
            valueLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold);
            valueLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            valueLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            panel.Controls.Add(valueLbl);
            panel.Controls.Add(titleLbl);

            if (unitLbl != null)
            {
                unitLbl.Text = unitText;
                unitLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
                unitLbl.ForeColor = System.Drawing.Color.White;
                unitLbl.Dock = System.Windows.Forms.DockStyle.Bottom;
                unitLbl.Height = 22;
                unitLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                panel.Controls.Add(unitLbl);
            }

            parent.Controls.Add(panel);
        }

        private void SetupSensorBar(
            System.Windows.Forms.Panel track,
            System.Windows.Forms.Panel fill,
            System.Windows.Forms.Label valueLbl,
            System.Windows.Forms.Panel parent,
            System.Windows.Forms.Label lbl,
            string labelText, int x, int y, int w, int h)
        {
            lbl.Text = labelText;
            lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            lbl.ForeColor = System.Drawing.Color.Silver;
            lbl.Location = new System.Drawing.Point(x, y + 1);
            lbl.AutoSize = false;
            lbl.Width = 62;

            track.Location = new System.Drawing.Point(x + 66, y);
            track.Size = new System.Drawing.Size(w, h);
            track.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            track.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            fill.Location = new System.Drawing.Point(0, 0);
            fill.Size = new System.Drawing.Size(0, h);
            fill.BackColor = System.Drawing.Color.FromArgb(50, 200, 50);
            track.Controls.Add(fill);

            valueLbl.Text = "0%";
            valueLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            valueLbl.ForeColor = System.Drawing.Color.White;
            valueLbl.Location = new System.Drawing.Point(x + 66 + w + 3, y + 1);
            valueLbl.Width = 38;

            parent.Controls.Add(lbl);
            parent.Controls.Add(track);
            parent.Controls.Add(valueLbl);
        }

        private void SetupTotalsLabel(System.Windows.Forms.Label lbl, string text, int x, int w)
        {
            lbl.Text = text;
            lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            lbl.ForeColor = System.Drawing.Color.Silver;
            lbl.Location = new System.Drawing.Point(x, 0);
            lbl.Size = new System.Drawing.Size(w, 26);
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl.AutoSize = false;
        }

        private void SetupStatusLabel(System.Windows.Forms.Label lbl, string text,
            int x, int y, int width = 60)
        {
            lbl.Text = text;
            lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            lbl.ForeColor = System.Drawing.Color.Silver;
            lbl.Location = new System.Drawing.Point(x, y);
            lbl.Width = width;
            lbl.AutoSize = false;
        }

        // ── Controls ──────────────────────────────────────────────────────────
        private System.Windows.Forms.Panel pnlToolbar;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblTitle;

        private System.Windows.Forms.Panel pnlGauges;
        private System.Windows.Forms.Panel pnlYield;
        private System.Windows.Forms.Label lblYieldTitle;
        private System.Windows.Forms.Label lblYield;
        private System.Windows.Forms.Label lblYieldUnit;

        private System.Windows.Forms.Panel pnlMoisture;
        private System.Windows.Forms.Label lblMoistureTitle;
        private System.Windows.Forms.Label lblMoisture;
        private System.Windows.Forms.Label lblMoistureUnit;

        private System.Windows.Forms.Panel pnlSpeedPanel;
        private System.Windows.Forms.Label lblSpeedTitle;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblSpeedUnit;

        private System.Windows.Forms.Panel pnlTotals;
        private System.Windows.Forms.Label lblTotArea;
        private System.Windows.Forms.Label lblTotTotal;
        private System.Windows.Forms.Label lblTotRate;

        private System.Windows.Forms.Panel pnlSensors;
        private System.Windows.Forms.Label lblSensorHeader;
        private System.Windows.Forms.Label lblSensor1Title;
        private System.Windows.Forms.Panel pnlSensor1;
        private System.Windows.Forms.Panel pnlSensor1Fill;
        private System.Windows.Forms.Label lblSensor1Value;
        private System.Windows.Forms.Label lblSensor2Title;
        private System.Windows.Forms.Panel pnlSensor2;
        private System.Windows.Forms.Panel pnlSensor2Fill;
        private System.Windows.Forms.Label lblSensor2Value;

        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatusGPS;
        private System.Windows.Forms.Label lblStatusModule;
        private System.Windows.Forms.Label lblStatusComm;
        private System.Windows.Forms.Label lblStatusJob;
        private System.Windows.Forms.Label lblStatusMsg;
        private System.Windows.Forms.Label lblVersion;
    }
}
