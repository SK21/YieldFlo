using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    public partial class frmMenuSettings : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private string _originalCommType;
        private string _originalCanDriver;
        private string _originalCanPort;

        private static readonly Color ActiveColour = Color.FromArgb(0, 80, 160);
        private static readonly Color InactiveColour = Color.FromArgb(60, 60, 60);

        public frmMenuSettings()
        {
            InitializeComponent();
        }

        private void frmMenuSettings_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            FormPositions.Restore(this);
            this.FormClosed += (s2, ev2) => FormPositions.Save(this);
            foreach (Control c in new System.Windows.Forms.Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) { _dragging = true; _dragStart = ev.Location; } };
                c.MouseMove += (s, ev) => { if (_dragging) { Left += ev.X - _dragStart.X; Top += ev.Y - _dragStart.Y; } };
                c.MouseUp += (s, ev) => _dragging = false;
            }
            LoadCurrentSettings();
            _originalCommType = Properties.Settings.Default.ModuleCommType;
            _originalCanDriver = Properties.Settings.Default.CanDriver;
            _originalCanPort = Properties.Settings.Default.CanPort;

            if (Core.UDPmodule.ModuleIP != "")
            {
                btnEthernet.Text = "Wifi  (" + Core.UDPmodule.ModuleIP + ")";
            }
            else
            {
                btnEthernet.Text = "Wifi";
            }
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var ctrl = Color.FromArgb(60, 60, 60);

            pnlTitle.BackColor = back;
            pnlContent.BackColor = back;
            lblTitle.ForeColor = Color.FromArgb(180, 200, 220);
            btnTitleClose.BackColor = Color.FromArgb(80, 30, 30);
            btnTitleClose.ForeColor = Color.White;

            foreach (Control c in pnlContent.Controls)
            {
                c.ForeColor = fore;
                if (c is Button btn && btn != btnSaveSettings)
                {
                    btn.BackColor = InactiveColour;
                    btn.ForeColor = Color.White;
                }
                if (c is ComboBox cb)
                {
                    cb.BackColor = ctrl;
                    cb.ForeColor = Color.White;
                }
            }

            btnSaveSettings.BackColor = Color.FromArgb(0, 110, 0);
            btnSaveSettings.ForeColor = Color.White;
            btnSettingsClose.BackColor = InactiveColour;
            btnSettingsClose.ForeColor = Color.White;
        }

        private void LoadCurrentSettings()
        {
            // Units
            SetToggle(btnImperial, btnMetric, Properties.Settings.Default.Units == "Imperial");

            // Theme
            bool isDark = Properties.Settings.Default.MainBackColour.GetBrightness() < 0.5f;
            SetToggle(btnDark, btnLight, isDark);

            // Network mode
            bool isEthernet = Properties.Settings.Default.ModuleCommType != "CAN";
            SetToggle(btnEthernet, btnCAN, isEthernet);

            // Resume Job on Start
            bool resume = Properties.Settings.Default.ResumeJobOnStart;
            SetToggle(btnResumeOn, btnResumeOff, resume);

            // CAN driver
            cbCanDriver.SelectedIndex = cbCanDriver.FindStringExact(Properties.Settings.Default.CanDriver);
            if (cbCanDriver.SelectedIndex < 0) cbCanDriver.SelectedIndex = 0;

            // CAN port
            LoadCanPortCombo();
            cbCanPort.SelectedIndex = cbCanPort.FindStringExact(Properties.Settings.Default.CanPort);

            UpdateNetworkControls(isEthernet);
        }

        private void LoadCanPortCombo()
        {
            cbCanPort.Items.Clear();
            try
            {
                foreach (string port in SerialPort.GetPortNames())
                    cbCanPort.Items.Add(port);
            }
            catch { }
        }

        private void UpdateNetworkControls(bool isWifi)
        {
            lblWifiInfo.Visible = isWifi;
            lblCanDriver.Visible = !isWifi;
            cbCanDriver.Visible = !isWifi;
            lblCanPort.Visible = !isWifi;
            cbCanPort.Visible = !isWifi;
            btnRescanPorts.Visible = !isWifi;
        }

        private void btnRescanPorts_Click(object sender, EventArgs e)
        {
            string current = cbCanPort.SelectedItem?.ToString() ?? "";
            LoadCanPortCombo();
            cbCanPort.SelectedIndex = cbCanPort.FindStringExact(current);
        }

        private void SetToggle(Button active, Button inactive, bool firstActive)
        {
            active.BackColor = firstActive ? ActiveColour : InactiveColour;
            inactive.BackColor = firstActive ? InactiveColour : ActiveColour;
        }

        private void btnImperial_Click(object sender, EventArgs e) => SetToggle(btnImperial, btnMetric, true);
        private void btnMetric_Click(object sender, EventArgs e) => SetToggle(btnImperial, btnMetric, false);
        private void btnDark_Click(object sender, EventArgs e) => SetToggle(btnDark, btnLight, true);
        private void btnLight_Click(object sender, EventArgs e) => SetToggle(btnDark, btnLight, false);
        private void btnResumeOn_Click(object sender, EventArgs e) => SetToggle(btnResumeOn, btnResumeOff, true);
        private void btnResumeOff_Click(object sender, EventArgs e) => SetToggle(btnResumeOn, btnResumeOff, false);

        private void btnEthernet_Click(object sender, EventArgs e)
        {
            SetToggle(btnEthernet, btnCAN, true);
            UpdateNetworkControls(true);
        }

        private void btnCAN_Click(object sender, EventArgs e)
        {
            SetToggle(btnEthernet, btnCAN, false);
            UpdateNetworkControls(false);
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            // Units
            bool isImperial = btnImperial.BackColor == ActiveColour;
            Properties.Settings.Default.Units = isImperial ? "Imperial" : "Metric";

            // Theme
            bool isDark = btnDark.BackColor == ActiveColour;
            if (isDark)
            {
                Properties.Settings.Default.MainBackColour = Color.FromArgb(45, 45, 45);
                Properties.Settings.Default.DisplayBackColour = Color.FromArgb(60, 60, 60);
                Properties.Settings.Default.MainForeColour = Color.White;
                Properties.Settings.Default.DisplayForeColour = Color.FromArgb(180, 200, 220);
            }
            else
            {
                Properties.Settings.Default.MainBackColour = Color.FromArgb(200, 200, 210);
                Properties.Settings.Default.DisplayBackColour = Color.FromArgb(220, 220, 230);
                Properties.Settings.Default.MainForeColour = Color.FromArgb(20, 20, 20);
                Properties.Settings.Default.DisplayForeColour = Color.FromArgb(20, 50, 100);
            }

            // Network / comm type
            bool isEthernet = btnEthernet.BackColor == ActiveColour;
            Properties.Settings.Default.ModuleCommType = isEthernet ? "UDP" : "CAN";

            if (isEthernet)
            {
                // WiFi mode — module broadcasts to its own subnet automatically; no endpoint config needed
            }
            else
            {
                string driver = cbCanDriver.SelectedItem?.ToString() ?? "SLCAN";
                string port = cbCanPort.SelectedItem?.ToString() ?? "";
                Properties.Settings.Default.CanDriver = driver;
                Properties.Settings.Default.CanPort = port;
                Props.CurrentCanDriver = Enum.TryParse(driver, out CanDriver cd) ? cd : CanDriver.SLCAN;
                Props.CanPort = port;
            }

            Props.CanEnabled = !isEthernet;

            // Resume Job on Start
            Properties.Settings.Default.ResumeJobOnStart = btnResumeOn.BackColor == ActiveColour;

            Properties.Settings.Default.Save();
            Core.RaiseColorChanged();
            Props.ShowMessage(Lang.lgSettingsSaved);

            bool commChanged = Properties.Settings.Default.ModuleCommType != _originalCommType
                            || Properties.Settings.Default.CanDriver != _originalCanDriver
                            || Properties.Settings.Default.CanPort != _originalCanPort;

            if (commChanged)
            {
                var result = MessageBox.Show(
                    "Comm settings changed. Restart now to apply?",
                    "Restart Required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    Core.RequestRestart();
                else
                    _originalCommType = _originalCanDriver = _originalCanPort = null;
            }
        }

        private void btnTitleClose_Click(object sender, EventArgs e) => this.Close();
        private void btnSettingsClose_Click(object sender, EventArgs e) => this.Close();
    }
}
