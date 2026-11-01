using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using YieldFlo.Classes;
using YieldFlo.Language;

namespace YieldFlo.Forms
{
    // Module communication, split out of Settings: one tab per transport, as RateController's
    // frmMenuComm. The two Enabled boxes are exclusive — the app talks to the module over
    // WiFi/Ethernet (UDP) or CAN, never both. Nothing applies until Save; a change of
    // transport, driver or port still asks for a restart, the tablet hotspot applies at once.
    public partial class frmMenuComm : Form
    {
        private bool _dragging;
        private Point _dragStart;
        private string _originalCommType;
        private string _originalCanDriver;
        private string _originalCanPort;
        private clsTabletHotspot.FirewallStatus _firewall = clsTabletHotspot.FirewallStatus.Unknown;

        // PC's own WiFi on 5 GHz with no module data (see clsTabletHotspot.BandProblem).
        // Re-read every few seconds rather than on every 500 ms tick: it's several WLAN calls
        private string _moduleIP = "";      // the module's address while connected over WiFi/Ethernet
        private bool _bandProblem;
        private string _bandSsid = "";
        private int _bandTicks;

        private static readonly Color ActiveColour = Color.FromArgb(0, 80, 160);
        private static readonly Color InactiveColour = Color.FromArgb(60, 60, 60);

        public frmMenuComm()
        {
            InitializeComponent();
        }

        private void frmMenuComm_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            FormPositions.Restore(this);
            this.FormClosed += (s2, ev2) => FormPositions.Save(this);
            foreach (Control c in new Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) { _dragging = true; _dragStart = ev.Location; } };
                c.MouseMove += (s, ev) => { if (_dragging) { Left += ev.X - _dragStart.X; Top += ev.Y - _dragStart.Y; } };
                c.MouseUp += (s, ev) => _dragging = false;
            }

            LoadCurrentSettings();
            _originalCommType = Properties.Settings.Default.ModuleCommType;
            _originalCanDriver = Properties.Settings.Default.CanDriver;
            _originalCanPort = Properties.Settings.Default.CanPort;

            // open on the transport in use
            tabComm.SelectedTab = ckCan.Checked ? tabCan : tabWifi;
            UpdateStatus();

            this.Shown += (s, ev) => BeginInvoke((Action)OfferFixesOnOpen);
        }

        // Opening this page is where someone comes to find out why there's no data, so if the
        // firewall or a 5 GHz connection is what's in the way, say so straight away instead of
        // waiting for Save. Not when the hotspot isn't in use, and not when module data is
        // arriving (then nothing is blocked in practice, whatever it looks like).
        private void OfferFixesOnOpen()
        {
            if (IsDisposed || !Core.Hotspot.Enabled) return;
            if (Core.ModuleConnected && (DateTime.UtcNow - Core.LastModuleReceive).TotalSeconds < 5) return;

            if (_firewall == clsTabletHotspot.FirewallStatus.Missing || _firewall == clsTabletHotspot.FirewallStatus.Blocked)
            {
                tabComm.SelectedTab = tabWifi;
                _firewall = clsTabletHotspot.OfferFirewallFix(this);
                UpdateStatus();
            }

            _bandProblem = clsTabletHotspot.BandProblem(out _bandSsid);
            if (_bandProblem && !IsDisposed)
            {
                tabComm.SelectedTab = tabWifi;
                UpdateStatus();
                frmMsgBox.Inform(this, clsTabletHotspot.BandFixText(_bandSsid), "Tablet Hotspot");
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

            // A TabPage ignores BackColor while UseVisualStyleBackColor is on
            foreach (TabPage page in tabComm.TabPages)
            {
                page.UseVisualStyleBackColor = false;
                page.BackColor = back;
                foreach (Control c in page.Controls)
                {
                    if (c is Label) continue;           // labels keep their Designer colours
                    c.ForeColor = fore;
                    if (c is Button btn && btn != btnRescanPorts)
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
            }

            // the strip itself is painted by DarkTabControl
            tabComm.HeaderBack = back;
            tabComm.PageBack = back;
            tabComm.TabBack = ctrl;
            tabComm.TabSelected = Color.FromArgb(0, 70, 120);
            tabComm.TabFore = Color.Silver;
            tabComm.TabForeSelected = Color.White;
            tabComm.BorderColour = Color.FromArgb(150, 150, 150);
            tabComm.Invalidate();

            btnSave.BackColor = Color.FromArgb(0, 110, 0);
            btnSave.ForeColor = Color.White;
            btnClose.BackColor = InactiveColour;
            btnClose.ForeColor = Color.White;
        }

        private void LoadCurrentSettings()
        {
            bool can = Properties.Settings.Default.ModuleCommType == "CAN";
            ckCan.Checked = can;
            ckWifi.Checked = !can;

            cbCanDriver.SelectedIndex = cbCanDriver.FindStringExact(Properties.Settings.Default.CanDriver);
            if (cbCanDriver.SelectedIndex < 0) cbCanDriver.SelectedIndex = 0;

            LoadCanPortCombo();
            cbCanPort.SelectedIndex = cbCanPort.FindStringExact(Properties.Settings.Default.CanPort);

            // tablet hotspot: Windows' current state, and whether the firewall lets module
            // data in from it
            SetToggle(btnHotspotOn, btnHotspotOff, Properties.Settings.Default.TabletHotspot);
            _firewall = clsTabletHotspot.CheckFirewall();
            if (!Core.Hotspot.Busy) Core.Hotspot.RefreshAsync(UpdateStatus);
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

        private void SetToggle(Button active, Button inactive, bool firstActive)
        {
            active.BackColor = firstActive ? ActiveColour : InactiveColour;
            inactive.BackColor = firstActive ? InactiveColour : ActiveColour;
        }

        private void ckWifi_CheckedChanged(object sender, EventArgs e) => ckCan.Checked = !ckWifi.Checked;
        private void ckCan_CheckedChanged(object sender, EventArgs e) => ckWifi.Checked = !ckCan.Checked;

        private void btnHotspotOn_Click(object sender, EventArgs e) => SetToggle(btnHotspotOn, btnHotspotOff, true);
        private void btnHotspotOff_Click(object sender, EventArgs e) => SetToggle(btnHotspotOn, btnHotspotOff, false);

        // Not modal: this form is TopMost, and a modal child of a TopMost form is the
        // arrangement that has caused z-order trouble here before. FormManager keeps
        // one instance, which is enough.
        private void btnWifiSetup_Click(object sender, EventArgs e) => FormManager.ShowForm(new frmWifiSetup());

        private void btnRescanPorts_Click(object sender, EventArgs e)
        {
            string current = cbCanPort.SelectedItem?.ToString() ?? "";
            LoadCanPortCombo();
            cbCanPort.SelectedIndex = cbCanPort.FindStringExact(current);
        }

        private void tmrStatus_Tick(object sender, EventArgs e) => UpdateStatus();

        private void UpdateStatus()
        {
            if (IsDisposed) return;

            // Module connected: same test as the main screen's indicator (data within 5 s).
            // Shown on the tab of the transport actually running; the other says so
            bool modOk = Core.ModuleConnected && (DateTime.UtcNow - Core.LastModuleReceive).TotalSeconds < 5;
            bool canRunning = Props.CanEnabled;

            string ip = Core.UDPmodule.ModuleIP;
            _moduleIP = modOk ? ip : "";
            ShowModule(lblModuleIP, !canRunning, modOk, _moduleIP != "" ? "  " + _moduleIP : "");
            ShowModule(lblCanModule, canRunning, modOk, "");

            if (_bandTicks-- <= 0)
            {
                _bandTicks = 10;    // 5 s
                _bandProblem = clsTabletHotspot.BandProblem(out _bandSsid);
            }

            // Reflects the CAN adapter actually running (started from previously saved
            // settings), not whatever is selected in the combo boxes — those only take
            // effect after Save + restart.
            bool connected = Core.CanModule != null && Core.CanModule.AdapterConnected;
            lblCanStatus.Text = "Adapter: " + (connected ? "Connected" : "Not Connected");
            lblCanStatus.ForeColor = connected ? OkabeIto.BluishGreen : OkabeIto.Vermillion;

            UpdateHotspotStatus();
        }

        private static void ShowModule(Label lbl, bool inUse, bool connected, string suffix)
        {
            string text;
            Color colour;
            if (!inUse)
            {
                text = "Module: Not in use";
                colour = Color.Silver;
            }
            else
            {
                text = "Module: " + (connected ? "Connected" : "Not Connected") + suffix;
                colour = connected ? OkabeIto.BluishGreen : OkabeIto.Vermillion;
            }

            // Only on change: this runs every 500 ms, and setting a LinkLabel's Text resets
            // its link and repaints it, which would flicker and reset a hover
            if (lbl.Text == text && lbl.ForeColor == colour) return;
            lbl.Text = text;
            lbl.ForeColor = colour;

            // On the WiFi tab the IP address (the suffix) is a link to the module's page
            if (lbl is LinkLabel link)
            {
                string ip = suffix.Trim();
                link.LinkArea = ip.Length > 0
                    ? new LinkArea(text.Length - ip.Length, ip.Length)
                    : new LinkArea(0, 0);
            }
        }

        // The module's settings page, at the address it has on this network. Its web server
        // answers on that address as well as on its own hotspot, so the PC can stay where
        // it is. The page stays open: closing it would lose unsaved changes, and the app's
        // other windows are TopMost too, so closing this one alone wouldn't clear the way.
        private void lblModuleIP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_moduleIP == "") return;
            try
            {
                System.Diagnostics.Process.Start("http://" + _moduleIP + "/");
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuComm/OpenModulePage: " + ex.Message);
                frmMsgBox.Inform(this, "Could not open the browser: " + ex.Message, "Module Page");
            }
        }

        private void UpdateHotspotStatus()
        {
            clsTabletHotspot H = Core.Hotspot;
            string text;
            Color colour = Color.Silver;

            if (H.Changing)
            {
                text = "Hotspot: working...";
            }
            else if (_bandProblem)
            {
                text = "Hotspot: " + H.State + ".  This PC is on " + _bandSsid
                     + " at 5 GHz. Set the PC's WiFi to 2.4 GHz (see Help).";
                colour = OkabeIto.Vermillion;
            }
            else if (H.State == clsTabletHotspot.StateOn)
            {
                text = "Hotspot: On  " + H.Ssid;
                colour = OkabeIto.BluishGreen;
                if (H.Band != "" && !H.Is24GHz)
                {
                    text += "  (not 2.4 GHz, module can't join)";
                    colour = OkabeIto.Vermillion;
                }
                else if (H.Enabled && (_firewall == clsTabletHotspot.FirewallStatus.Missing
                                    || _firewall == clsTabletHotspot.FirewallStatus.Blocked))
                {
                    text += "  (firewall is blocking module data, press Save to allow it)";
                    colour = OkabeIto.Vermillion;
                }
            }
            else if (H.State == clsTabletHotspot.StateOff)
            {
                text = "Hotspot: Off";
                if (H.Enabled && H.Failing)
                {
                    text += ", retrying" + (H.Result != "" && H.Result != "Success" ? " (" + H.Result + ")" : "");
                    colour = OkabeIto.Vermillion;
                }
            }
            else if (H.State == "InTransition")
            {
                text = "Hotspot: starting...";
            }
            else
            {
                text = "Hotspot: --";
            }

            lblHotspotStatus.Text = text;
            lblHotspotStatus.ForeColor = colour;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool wifi = ckWifi.Checked;
            Properties.Settings.Default.ModuleCommType = wifi ? "UDP" : "CAN";

            if (!wifi)
            {
                string driver = cbCanDriver.SelectedItem?.ToString() ?? "SLCAN";
                string port = cbCanPort.SelectedItem?.ToString() ?? "";
                Properties.Settings.Default.CanDriver = driver;
                Properties.Settings.Default.CanPort = port;
                Props.CurrentCanDriver = Enum.TryParse(driver, out CanDriver cd) ? cd : CanDriver.SLCAN;
                Props.CanPort = port;
            }
            Props.CanEnabled = !wifi;

            Properties.Settings.Default.TabletHotspot = btnHotspotOn.BackColor == ActiveColour;

            Properties.Settings.Default.Save();
            ApplyHotspot(Properties.Settings.Default.TabletHotspot && wifi);
            Props.ShowMessage(Lang.lgSettingsSaved);

            bool commChanged = Properties.Settings.Default.ModuleCommType != _originalCommType
                            || Properties.Settings.Default.CanDriver != _originalCanDriver
                            || Properties.Settings.Default.CanPort != _originalCanPort;

            if (commChanged)
            {
                if (frmMsgBox.Ask(this, "Comm settings changed. Restart now to apply?", "Restart Required"))
                    Core.RequestRestart();
                else
                    _originalCommType = _originalCanDriver = _originalCanPort = null;
            }
        }

        // Applied at once, no restart. Turning it on starts the hotspot (2.4 GHz forced) and,
        // if the firewall would drop module data from it, asks Windows for admin once.
        // Turning it off stops the hotspot only if the app was the one keeping it on
        private void ApplyHotspot(bool on)
        {
            clsTabletHotspot H = Core.Hotspot;
            bool wasOn = H.Enabled;
            H.Enabled = on;

            if (on)
            {
                H.ResetWatch();
                _firewall = clsTabletHotspot.OfferFirewallFix(this);
                H.StartAsync(UpdateStatus);
            }
            else if (wasOn)
            {
                H.StopAsync(UpdateStatus);
            }
            UpdateStatus();
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}
