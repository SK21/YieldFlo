using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using YieldFlo.Forms;

namespace YieldFlo.Classes
{
    // Offers module WiFi setup when a module is in range but isn't sending data: a new
    // install, a new module, or one whose settings a firmware update wiped. Tablet hotspot
    // mode only, where setup needs nothing typed.
    //
    // It asks rather than doing it: setup takes the PC's WiFi off the hotspot for about a
    // minute, which also stops GPS corrections if the internet comes over WiFi. Each module
    // is offered once per session, never while a job is recording or the setup page is
    // already open.
    //
    // A module's own hotspot stays up after it has joined a network, so "in range" alone
    // means nothing; it's "in range AND no data" that says it needs setting up.
    //
    // The same pass checks the firewall first. With the hotspot on by default a new user
    // never presses Save on the Comm page, which is the only other place the rule gets
    // added, and Windows' first-run prompt leaves Public unticked by default — that
    // creates a Block rule on this exe and every packet from the hotspot is dropped.
    // Deliberately not at launch: Windows' own prompt appears when the app opens its port,
    // and a rule added before the user answers it would be overridden by the Block it
    // creates. Asked once per session per state (Missing, then Blocked if it changes).
    public class clsModuleOffer
    {
        // Quick at first, so a first run gets to setup in well under a minute: a module
        // that is already set up joins within ~10 s of the hotspot starting (firmware 1116+),
        // so 20 s tells the two apart. After 5 minutes it settles to once a minute.
        private const int StartGraceSec = 20;       // let a set-up module connect first
        private const int FirstCheckMs = 20000;
        private const int EarlyCheckMs = 15000;
        private const int EarlyPeriodSec = 300;
        private const int CheckIntervalMs = 60000;
        private const int NoDataSec = 20;
        private const string ModulePrefix = "YieldFlo";

        private readonly System.Windows.Forms.Timer Timer;
        private readonly DateTime Started = DateTime.UtcNow;
        private readonly HashSet<string> Offered = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private bool Scanning;
        private readonly HashSet<clsTabletHotspot.FirewallStatus> FirewallAsked = new HashSet<clsTabletHotspot.FirewallStatus>();
        private readonly HashSet<string> BandTold = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public clsModuleOffer()
        {
            // create on the UI thread: the tick shows the question
            Timer = new System.Windows.Forms.Timer();
            Timer.Interval = FirstCheckMs;
            Timer.Tick += Timer_Tick;
            Timer.Enabled = true;
        }

        private static bool ModuleSending()
        {
            return Core.ModuleConnected && (DateTime.UtcNow - Core.LastModuleReceive).TotalSeconds < NoDataSec;
        }

        private static bool SetupOpen()
        {
            foreach (Form f in Application.OpenForms)
                if (f is frmWifiSetup) return true;
            return false;
        }

        // Hotspot state deliberately not required here: a 5 GHz uplink can be the very reason
        // it isn't on, and that check comes first. The firewall and module checks need it On.
        private bool ShouldLook()
        {
            if (Scanning) return false;
            if (Core.Hotspot == null || !Core.Hotspot.Enabled || Core.Hotspot.Paused) return false;
            if ((DateTime.UtcNow - Started).TotalSeconds < StartGraceSec) return false;
            if (ModuleSending()) return false;
            if (Core.Collector != null && Core.Collector.IsRecording) return false;
            if (SetupOpen()) return false;
            return true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                int interval = (DateTime.UtcNow - Started).TotalSeconds < EarlyPeriodSec ? EarlyCheckMs : CheckIntervalMs;
                if (Timer.Interval != interval) Timer.Interval = interval;

                if (!ShouldLook()) return;
                if (BandNeedsFixing()) return;          // the module can't join until it's fixed
                if (Core.Hotspot.State != clsTabletHotspot.StateOn) return;
                if (FirewallNeedsFixing()) return;      // a module couldn't get data through anyway

                Wlan.Adapter adapter = Wlan.PreferredAdapter();
                if (adapter == null) return;

                // A scan takes a few seconds in the driver, so off the UI thread.
                // No module is using the hotspot at this point, so the brief radio
                // excursion costs nothing.
                Scanning = true;
                Guid guid = adapter.Guid;
                var sync = SynchronizationContext.Current;
                ThreadPool.QueueUserWorkItem(o =>
                {
                    List<Wlan.Network> nets = null;
                    try
                    {
                        Wlan.RequestScan(guid);
                        Thread.Sleep(3000);
                        nets = Wlan.Networks(guid);
                    }
                    catch (Exception ex)
                    {
                        Props.WriteErrorLog("clsModuleOffer/Scan: " + ex.Message);
                    }
                    if (sync != null) sync.Post(s => Found(nets), null);
                    else Scanning = false;
                });
            }
            catch (Exception ex)
            {
                Scanning = false;
                Props.WriteErrorLog("clsModuleOffer/Tick: " + ex.Message);
            }
        }

        /// <summary>
        /// True if this PC's own WiFi is on 5 GHz with no module data arriving. Tells the user
        /// how to fix it once per network per session; setup wouldn't help until then.
        /// </summary>
        private bool BandNeedsFixing()
        {
            string ssid;
            if (!clsTabletHotspot.BandProblem(out ssid)) return false;
            if (BandTold.Contains(ssid)) return true;
            BandTold.Add(ssid);

            Scanning = true;    // keeps the next tick out while the message is up
            try
            {
                Props.WriteErrorLog("clsModuleOffer/Band: PC on " + ssid + " at 5 GHz, no module data");
                frmMsgBox.Inform(Core.MainForm, clsTabletHotspot.BandFixText(ssid), "Tablet Hotspot");
            }
            finally
            {
                Scanning = false;
            }
            return true;
        }

        /// <summary>
        /// True if the firewall would drop module data from the hotspot. Asks to fix it the
        /// first time each state is seen this session (one admin prompt if the user agrees).
        /// </summary>
        private bool FirewallNeedsFixing()
        {
            var status = clsTabletHotspot.CheckFirewall();
            if (status != clsTabletHotspot.FirewallStatus.Missing && status != clsTabletHotspot.FirewallStatus.Blocked)
                return false;
            if (FirewallAsked.Contains(status)) return true;
            FirewallAsked.Add(status);

            Scanning = true;    // keeps the next tick out while the question is up
            try
            {
                status = clsTabletHotspot.OfferFirewallFix(Core.MainForm);
            }
            finally
            {
                Scanning = false;
            }
            return status != clsTabletHotspot.FirewallStatus.Allowed;
        }

        private void Found(List<Wlan.Network> nets)
        {
            Scanning = false;
            try
            {
                if (nets == null) return;

                string ssid = null;
                foreach (var n in nets)
                {
                    if (!n.Ssid.StartsWith(ModulePrefix, StringComparison.OrdinalIgnoreCase)) continue;
                    if (Offered.Contains(n.Ssid)) continue;
                    ssid = n.Ssid;
                    break;
                }
                if (ssid == null) return;

                // the scan took a few seconds; conditions may have changed
                Scanning = true;        // keeps the next tick out while the question is up
                if (!ShouldLookAfterScan()) return;
                Offered.Add(ssid);

                bool yes = frmMsgBox.Ask(Core.MainForm,
                    "A module is in range but not sending data:  " + ssid + Environment.NewLine + Environment.NewLine
                    + "Set up its WiFi now? It takes about a minute.",
                    "Module WiFi Setup");

                // Yes is the go-ahead: the setup page starts by itself once it has found
                // the module, rather than asking for Set Up Module and a second Yes
                if (yes && !ModuleSending())
                    FormManager.ShowForm(new frmWifiSetup(ssid, true));
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsModuleOffer/Found: " + ex.Message);
            }
            finally
            {
                Scanning = false;
            }
        }

        private static bool ShouldLookAfterScan()
        {
            if (ModuleSending()) return false;
            if (Core.Collector != null && Core.Collector.IsRecording) return false;
            if (SetupOpen()) return false;
            return true;
        }
    }
}
