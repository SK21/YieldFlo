using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using YieldFlo.Classes;

namespace YieldFlo.Forms
{
    /// <summary>
    /// Gives a module the credentials of the network it should join, without a phone.
    /// The PC already knows that network — it is where NTRIP comes from — so all this
    /// form needs is the passphrase, once, and which module to set up.
    /// </summary>
    public partial class frmWifiSetup : Form
    {
        // Module hotspots are named APname + "_" + the low half of the MAC, and
        // APname defaults to "YieldFlo_ESP32". A module whose AP name has been
        // renamed to something else will not appear in the list; the portal is still
        // there for that case.
        private const string ModulePrefix = "YieldFlo";

        private bool _dragging;
        private Point _dragStart;
        private bool _running;

        private Wlan.Adapter _adapter;
        private string _homeSsid = "";
        private string _homeProfile = "";
        private int _homeChannel;

        private sealed class ModuleItem
        {
            public string Ssid;
            public int Signal;
            public bool Secured;
            public override string ToString()
            {
                return Ssid + "    " + Signal + "%" + (Secured ? "   locked" : "");
            }
        }

        public frmWifiSetup()
        {
            InitializeComponent();
        }

        private void frmWifiSetup_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            FormPositions.Restore(this);
            this.FormClosed += (s, ev) => FormPositions.Save(this);

            foreach (Control c in new Control[] { pnlTitle, lblTitle })
            {
                c.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) { _dragging = true; _dragStart = ev.Location; } };
                c.MouseMove += (s, ev) => { if (_dragging) { Left += ev.X - _dragStart.X; Top += ev.Y - _dragStart.Y; } };
                c.MouseUp   += (s, ev) => _dragging = false;
            }

            // Wired in Shown, not here: Wire() hooks the Enter event, which is a focus
            // event, so anything wired before the form has settled its initial focus
            // opens the keyboard the moment the form appears.
            this.Shown += (s, ev) =>
            {
                KeyboardHelper.Wire(this, txtKey, "Network password");
                KeyboardHelper.Wire(this, txtModuleKey, "Hotspot password");
            };

            txtModuleKey.Text = WifiStore.ModuleKey;

            ReadHomeNetwork();

            // Only offer the stored password back when it belongs to the network the
            // PC is actually on. Prefilling it for a different network hands the
            // module a password that cannot work and makes the result read as a
            // refused password rather than as the wrong one being sent.
            if (_homeSsid.Length > 0 && WifiStore.HomeSsid == _homeSsid)
                txtKey.Text = WifiStore.HomeKey;

            if (Properties.Settings.Default.ModuleCommType == "CAN")
                Log("Note: this app is set to CAN. A module on CAN does not send data over WiFi, so the last step will not confirm.");

            // Open with focus on nothing that asks for typing, so the form does not
            // greet the user with a keyboard. Belt and braces alongside wiring in Shown:
            // it no longer matters whether first focus is assigned before or after.
            this.ActiveControl = txtLog;

            StartScan();
        }

        private void ApplyTheme()
        {
            var back = Properties.Settings.Default.MainBackColour;
            var fore = Properties.Settings.Default.MainForeColour;
            var ctrl = Color.FromArgb(60, 60, 60);

            pnlTitle.BackColor = back;
            pnlContent.BackColor = back;
            lblTitle.ForeColor = Color.FromArgb(180, 200, 220);

            foreach (Control c in pnlContent.Controls)
            {
                c.ForeColor = fore;
                if (c is Button b) { b.BackColor = ctrl; b.ForeColor = Color.White; }
                if (c is TextBox t) { t.BackColor = ctrl; t.ForeColor = Color.White; }
                if (c is ListBox l) { l.BackColor = ctrl; l.ForeColor = Color.White; }
            }

            btnSend.BackColor = Color.FromArgb(0, 110, 0);
            btnSend.ForeColor = Color.White;
            lblHint.ForeColor = Color.Silver;
        }

        // ── State ─────────────────────────────────────────────────────────────

        private void ReadHomeNetwork()
        {
            _adapter = Wlan.PreferredAdapter();
            if (_adapter == null)
            {
                lblHome.Text = "No WiFi adapter found on this PC.";
                lblHome.ForeColor = OkabeIto.Vermillion;
                btnScan.Enabled = false;
                btnSend.Enabled = false;
                return;
            }

            var net = Wlan.ConnectedNetwork(_adapter.Guid);
            if (net == null)
            {
                _homeSsid = "";
                _homeProfile = "";
                _homeChannel = 0;
                lblHome.Text = "This PC is not on a WiFi network. Connect to the network the module should use, then reopen this page.";
                lblHome.ForeColor = OkabeIto.Vermillion;
                btnSend.Enabled = false;
                return;
            }

            _homeSsid = net.Ssid;
            _homeProfile = string.IsNullOrEmpty(net.ProfileName) ? net.Ssid : net.ProfileName;

            // Nothing here predicts whether the module can reach this network, and the
            // button is never disabled on a guess. The module's radio is 2.4 GHz only,
            // but whether a network has a 2.4 GHz radio cannot be established from this
            // side: the driver will not report the other band of the network it is
            // itself joined to, which is always the network being set up here. Measured
            // on an Intel AC 9560 — associated to 'Phone' on channel 161, a completed
            // scan reports Phone as 5 GHz only, while a scan taken while disconnected
            // sees its 2.4 GHz radio plainly at 2412 MHz.
            //
            // So let the module try. It is built for the attempt failing: with no
            // channel cached every retry is a full scan, and ServiceWifiStation()
            // already rations those to 10 s, then 60 s, then 5 minutes once a client is
            // on the hotspot. If it never joins, the run says so and names 5 GHz as a
            // possible reason — which is the right place for that guess, because by
            // then it is the answer to a question the user is actually asking.
            int channel = Wlan.CurrentChannel(_adapter.Guid);
            _homeChannel = (channel >= 1 && channel <= 13) ? channel : 0;

            string where = "Network for the module:  " + _homeSsid;
            if (channel > 0) where += "   (channel " + channel + ")";

            lblHome.Text = where;
            lblHome.ForeColor = Properties.Settings.Default.MainForeColour;
            btnSend.Enabled = true;
        }

        // ── Scanning ──────────────────────────────────────────────────────────

        private void btnScan_Click(object sender, EventArgs e)
        {
            if (_running) return;
            ReadHomeNetwork();
            StartScan();
        }

        /// <summary>
        /// A scan takes a couple of seconds to complete in the driver, so ask for one and
        /// read the results afterwards rather than reading whatever is cached.
        /// </summary>
        private void StartScan()
        {
            if (_adapter == null) return;

            btnScan.Enabled = false;
            lstModules.Items.Clear();
            lstModules.Items.Add("Scanning ...");

            Guid guid = _adapter.Guid;
            var t = new Thread(() =>
            {
                Wlan.RequestScan(guid);
                Thread.Sleep(3000);
                var nets = Wlan.Networks(guid);
                OnUi(() =>
                {
                    lstModules.Items.Clear();
                    foreach (var n in nets)
                    {
                        if (!n.Ssid.StartsWith(ModulePrefix, StringComparison.OrdinalIgnoreCase)) continue;
                        lstModules.Items.Add(new ModuleItem { Ssid = n.Ssid, Signal = n.SignalPercent, Secured = n.Secured });
                    }
                    if (lstModules.Items.Count == 0)
                        lstModules.Items.Add("No modules found — is the module powered up?");
                    else
                        lstModules.SelectedIndex = 0;
                    btnScan.Enabled = true;
                });
            });
            t.IsBackground = true;
            t.Start();
        }

        // ── Sending ───────────────────────────────────────────────────────────

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (_running) return;

            var item = lstModules.SelectedItem as ModuleItem;
            if (item == null)
            {
                MessageBox.Show("Select a module first.", "Module WiFi Setup",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(_homeSsid))
            {
                MessageBox.Show("This PC is not on a WiFi network.", "Module WiFi Setup",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtKey.Text.Length == 0)
            {
                MessageBox.Show("Enter the password for " + _homeSsid + ".", "Module WiFi Setup",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // The firmware keeps 63 characters of passphrase and 32 of name, and
            // silently drops the rest. Better to say so than to send something that
            // will come back as "password refused".
            if (txtKey.Text.Length > 63)
            {
                MessageBox.Show("That password is longer than 63 characters, which is more than the module can store.",
                    "Module WiFi Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_homeSsid.Length > 32)
            {
                MessageBox.Show("That network name is longer than 32 characters, which is more than the module can store.",
                    "Module WiFi Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Recording must not be interrupted: this takes the PC off the network
            // for about a minute, and on a phone hotspot that is also the NTRIP feed.
            if (Core.Collector != null && Core.Collector.IsRecording)
            {
                MessageBox.Show("A job is recording. Stop recording before setting up WiFi.",
                    "Module WiFi Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var answer = MessageBox.Show(
                "Send the details of " + _homeSsid + " to " + item.Ssid + "?" + Environment.NewLine + Environment.NewLine
                + "This PC leaves " + _homeSsid + " for about a minute, so anything using that network — including "
                + "GPS corrections — stops until it is back.",
                "Module WiFi Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            WifiStore.HomeSsid = _homeSsid;
            WifiStore.HomeKey = txtKey.Text;
            WifiStore.ModuleKey = txtModuleKey.Text;
            WifiStore.Save();

            var req = new ProvisionRequest
            {
                Adapter = _adapter.Guid,
                ModuleSsid = item.Ssid,
                ModuleKey = txtModuleKey.Text,
                HomeSsid = _homeSsid,
                HomeProfile = _homeProfile,
                HomeKey = txtKey.Text,
                HomeChannel = _homeChannel
            };

            SetRunning(true);
            txtLog.Clear();

            var t = new Thread(() =>
            {
                ProvisionResult result;
                try
                {
                    var p = new ModuleProvisioner(Log);
                    result = p.Run(req);
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("frmWifiSetup/Run " + ex.Message);
                    result = new ProvisionResult { Outcome = ProvisionOutcome.Aborted, Detail = ex.Message };
                }
                OnUi(() => { SetRunning(false); Report(result); });
            });
            t.IsBackground = true;
            t.Start();
        }

        private void SetRunning(bool running)
        {
            // Park focus somewhere harmless first. Disabling the control that currently
            // has focus makes WinForms hand focus to the next one in tab order, and if
            // that is one of the keyboard-wired boxes its Enter event pops the tablet
            // keyboard over a run that has already started. txtLog is a TextBox too but
            // is read-only and deliberately not wired.
            if (running) txtLog.Focus();

            _running = running;
            btnSend.Enabled = !running;
            btnScan.Enabled = !running;
            btnClose.Enabled = !running;
            lstModules.Enabled = !running;
            txtKey.Enabled = !running;
            txtModuleKey.Enabled = !running;
        }

        private void Report(ProvisionResult r)
        {
            switch (r.Outcome)
            {
                case ProvisionOutcome.Success:
                    MessageBox.Show("The module is on " + _homeSsid + " and sending data.",
                        "Module WiFi Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case ProvisionOutcome.AuthRejected:
                    MessageBox.Show(_homeSsid + " refused the password." + Environment.NewLine + Environment.NewLine
                        + "Check it and try again. The module keeps trying in the background.",
                        "Module WiFi Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case ProvisionOutcome.HotspotFailed:
                    MessageBox.Show("Could not get onto the module's hotspot." + Environment.NewLine + Environment.NewLine
                        + r.Detail + Environment.NewLine + Environment.NewLine
                        + "If the hotspot has a password, enter it above.",
                        "Module WiFi Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                default:
                    MessageBox.Show(r.Detail.Length > 0 ? r.Detail : "Setup did not complete. See the log.",
                        "Module WiFi Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

        // ── Plumbing ──────────────────────────────────────────────────────────

        private void OnUi(Action a)
        {
            if (IsDisposed || !IsHandleCreated) return;
            try { BeginInvoke(a); } catch { }
        }

        /// <summary>
        /// Called from the worker thread, so it marshals itself.
        /// </summary>
        private void Log(string line)
        {
            OnUi(() =>
            {
                txtLog.AppendText(line + Environment.NewLine);
                txtLog.SelectionStart = txtLog.TextLength;
                txtLog.ScrollToCaret();
            });
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Closing mid-run would leave the PC on the module's hotspot with nothing
            // left to put it back.
            if (_running && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
        }
    }
}
