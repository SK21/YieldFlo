using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

namespace YieldFlo.Classes
{
    // ── Tablet hotspot ────────────────────────────────────────────────────────
    //
    // Optional WiFi transport: the tablet runs Windows Mobile Hotspot and the module
    // joins it as an ordinary station network. The network is always 192.168.137.1/24,
    // exists whenever the tablet is on, and doesn't depend on a phone, so the phone
    // hotspot problems (iOS /28, shared radio, Public profile per phone) go away.
    // No firmware change: the module's broadcast is mask-derived, so it already sends
    // to 192.168.137.255.
    //
    // Control is ported from RateController (clsWebServer): a hidden PowerShell process
    // calling the WinRT NetworkOperatorTetheringManager. No admin, no project
    // references. Each call takes 2-5 s, so it runs on the ThreadPool.
    //
    // Additions over RC:
    //  - The band is forced to 2.4 GHz on every start (the ESP32 radio is 2.4 GHz only,
    //    and Windows may otherwise follow the tablet's own WiFi onto 5 GHz). Done on
    //    every start, not once, so a user who changes Windows Settings back to "Any
    //    available" doesn't end up with a module that silently can't see the network.
    //  - The watchdog never gives up. Power saving turns the hotspot off after 5 min with
    //    no device (the module unplugged or out of range), and RC's stop-after-3 would
    //    then leave the module unable to reconnect. After 3 fast restarts it keeps
    //    trying every 5 min.
    //  - The firewall check reads the rules through the firewall's COM API (no admin,
    //    not localized) and also looks for a Block rule on this exe, which Windows'
    //    first-run prompt creates for the Public profile when only Private is ticked.
    //    A Block rule beats any Allow rule, so it has to be found and changed.
    //
    // Measured on the bench (2026-11-01): module joins and data arrives; the hotspot starts
    // with no internet at all; the hotspot DID drop when the PC's WiFi hopped to a
    // module's AP with internet over Ethernet (did not without Ethernet). Callers that
    // take the WiFi adapter away (module setup) must call StartAsync afterwards.
    public class clsTabletHotspot
    {
        public const string StateOn = "On";
        public const string StateOff = "Off";
        public const string StateUnknown = "Unknown";

        public const string FirewallRule = "YieldFlo hotspot";
        public const int ModulePort = 30100;
        public const string HotspotSubnet = "192.168.137.0/24";

        private const int FastRestarts = 3;
        private const int SlowRetryTicks = 10;          // 10 x 30 s = 5 min
        private const int WatchIntervalMs = 30000;

        // ACTION is status, start or stop.
        // Prints state|result|band|bandnote|ssid|passphrase
        //   result    last start/stop: Success or Windows' reason, '' if nothing was done
        //   band      Auto, TwoPointFourGigahertz, FiveGigahertz, or '' if Windows has no Band
        //   bandnote  '' nothing changed, Set, Unsupported, or Error: <message>
        // On start, if 2.4 GHz is supported and not selected, it's selected first; a hotspot
        // that was already On is restarted so the new band takes effect.
        private const string Script = @"$ErrorActionPreference='Stop'
$ProgressPreference='SilentlyContinue'
Add-Type -AssemblyName System.Runtime.WindowsRuntime
$null=[Windows.Networking.Connectivity.NetworkInformation,Windows.Networking.Connectivity,ContentType=WindowsRuntime]
$null=[Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager,Windows.Networking.NetworkOperators,ContentType=WindowsRuntime]
$T=[Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager]
$N=[Windows.Networking.Connectivity.NetworkInformation]
$p=$N::GetInternetConnectionProfile()
if(-not $p){$p=$N::GetConnectionProfiles()|Where-Object{$T::GetTetheringCapabilityFromConnectionProfile($_) -eq 'Enabled'}|Select-Object -First 1}
$m=$T::CreateFromConnectionProfile($p)
$ext=[System.WindowsRuntimeSystemExtensions].GetMethods()|Where-Object{$_.Name -eq 'AsTask' -and $_.GetParameters().Count -eq 1}
$aw=($ext|Where-Object{$_.GetParameters()[0].ParameterType.Name -eq 'IAsyncOperation`1'})[0].MakeGenericMethod([Windows.Networking.NetworkOperators.NetworkOperatorTetheringOperationResult])
$aa=($ext|Where-Object{$_.GetParameters()[0].ParameterType.Name -eq 'IAsyncAction'})[0]
function Op($o){$t=$aw.Invoke($null,@($o));$null=$t.Wait(30000);[string]$t.Result.Status}
$r=''
$bn=''
$s=[string]$m.TetheringOperationalState
if('ACTION' -eq 'start'){
try{
$c=$m.GetCurrentAccessPointConfiguration()
if($c.PSObject.Properties['Band']){
if($c.IsBandSupported('TwoPointFourGigahertz')){
if([string]$c.Band -ne 'TwoPointFourGigahertz'){
$c.Band='TwoPointFourGigahertz'
$t=$aa.Invoke($null,@($m.ConfigureAccessPointAsync($c)))
$null=$t.Wait(30000)
$bn='Set'
if($s -eq 'On'){$r=Op $m.StopTetheringAsync();$s='Off'}
}
}else{$bn='Unsupported'}
}
}catch{$bn='Error: '+($_.Exception.Message -replace '[|\r\n]',' ')}
if($s -ne 'On'){$r=Op $m.StartTetheringAsync()}
}
if('ACTION' -eq 'stop' -and $s -ne 'Off'){$r=Op $m.StopTetheringAsync()}
$c=$m.GetCurrentAccessPointConfiguration()
$b=''
if($c.PSObject.Properties['Band']){$b=[string]$c.Band}
Write-Output ('{0}|{1}|{2}|{3}|{4}|{5}' -f $m.TetheringOperationalState,$r,$b,$bn,$c.Ssid,$c.Passphrase)";

        public enum FirewallStatus
        {
            Allowed,        // an Allow rule covers UDP 30100 from the hotspot subnet on Public
            Missing,        // nothing covers it
            Blocked,        // a Block rule on this exe for Public beats any Allow
            Unknown         // the check itself failed
        }

        private readonly SynchronizationContext UI;
        private readonly System.Windows.Forms.Timer Watch;
        private int Pending = 0;
        private int PendingChange = 0;
        private int Restarts = 0;
        private int SlowTicks = 0;

        public clsTabletHotspot()
        {
            // create on the UI thread: results and callbacks are posted back to it
            UI = SynchronizationContext.Current;
            Watch = new System.Windows.Forms.Timer();
            Watch.Interval = WatchIntervalMs;
            Watch.Tick += Watch_Tick;
            Watch.Enabled = true;
        }

        // the user's intent (the setting), not a mirror of Windows' switch. The watchdog
        // only acts while this is true
        public bool Enabled { get; set; }

        // Module setup takes the WiFi adapter to the module's own hotspot, which knocks the
        // tablet hotspot off. A watchdog restart then would fail or pull the PC off the
        // module mid-run, so setup pauses it and restarts the hotspot itself afterwards
        public bool Paused { get; set; }

        public string Band { get; private set; } = "";             // as Windows reports it
        public string BandNote { get; private set; } = "";         // last start's band change
        // calls may overlap (a Save while the watchdog's status check is running), so a count
        public bool Busy { get { return Pending > 0; } }

        // a start or stop in progress, the only thing worth showing as "working". The
        // watchdog's routine status check every 30 s isn't: the state it last read stands
        public bool Changing { get { return PendingChange > 0; } }
        public string Passphrase { get; private set; } = "";
        public string Result { get; private set; } = "";           // last start/stop
        public string Ssid { get; private set; } = "";
        public string State { get; private set; } = StateUnknown; // On, Off, InTransition, Unknown

        // fast restarts used up and the hotspot is still off: the watchdog is in its slow
        // 5 min retry, worth showing in the status label along with Result
        public bool Failing
        { get { return Restarts >= FastRestarts && State == StateOff; } }

        public bool Is24GHz
        { get { return Band == "TwoPointFourGigahertz"; } }

        public void ResetWatch()
        {
            // a deliberate start (Save, app start) gets fresh fast restarts
            Restarts = 0;
            SlowTicks = 0;
        }

        public void RefreshAsync(Action Done = null) { RunAsync("status", Done); }

        public void StartAsync(Action Done = null) { RunAsync("start", Done); }

        public void StopAsync(Action Done = null) { RunAsync("stop", Done); }

        public void RunAsync(string Action, Action Done)
        {
            // status, start or stop on a pool thread (2-5 s, longer for a band change);
            // results and Done on the UI thread
            bool Change = Action != "status";
            Interlocked.Increment(ref Pending);
            if (Change) Interlocked.Increment(ref PendingChange);
            ThreadPool.QueueUserWorkItem(o =>
            {
                string Output = RunScript(Action);
                Post(() =>
                {
                    Apply(Output, Action);
                    Interlocked.Decrement(ref Pending);
                    if (Change) Interlocked.Decrement(ref PendingChange);
                    Done?.Invoke();
                });
            });
        }

        public bool Run(string Action)
        {
            // synchronous version, for callers already on a worker thread (module setup).
            // Not for the UI thread. Returns whether the hotspot ended up On
            bool Change = Action != "status";
            Interlocked.Increment(ref Pending);
            if (Change) Interlocked.Increment(ref PendingChange);
            try
            {
                Apply(RunScript(Action), Action);
            }
            finally
            {
                Interlocked.Decrement(ref Pending);
                if (Change) Interlocked.Decrement(ref PendingChange);
            }
            return State == StateOn;
        }

        private void Apply(string Output, string Action)
        {
            string[] Parts = Output.Split(new char[] { '|' }, 6);
            if (Parts.Length == 6 && Parts[0] != "")
            {
                State = Parts[0];
                if (Parts[1] != "")
                {
                    Result = Parts[1];
                    if (Result != "Success") Props.WriteErrorLog("clsTabletHotspot/" + Action + ": " + Result + ", state " + State);
                }
                Band = Parts[2];
                if (Parts[3] != "")
                {
                    BandNote = Parts[3];
                    if (BandNote != "Set") Props.WriteErrorLog("clsTabletHotspot/Band: " + BandNote + " (band " + Band + ")");
                }
                Ssid = Parts[4];
                Passphrase = Parts[5];
            }
            else
            {
                State = StateUnknown;
                Props.WriteErrorLog("clsTabletHotspot/" + Action + ": unexpected output '" + Output + "'");
            }
        }

        private void Post(Action A)
        {
            try
            {
                if (UI != null) UI.Post(s => A(), null);
                else A();
            }
            catch (Exception ex)
            {
                // the queued callback never runs, so release both counts here
                Interlocked.Exchange(ref Pending, 0);
                Interlocked.Exchange(ref PendingChange, 0);
                Props.WriteErrorLog("clsTabletHotspot/Post: " + ex.Message);
            }
        }

        private string RunScript(string Action)
        {
            string Output = "";
            try
            {
                string Text = Script.Replace("ACTION", Action);
                string Encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(Text));
                ProcessStartInfo Info = new ProcessStartInfo("powershell.exe",
                    "-NoProfile -NonInteractive -ExecutionPolicy Bypass -EncodedCommand " + Encoded);
                Info.UseShellExecute = false;
                Info.CreateNoWindow = true;
                Info.RedirectStandardOutput = true;
                Info.RedirectStandardError = true;
                using (Process P = Process.Start(Info))
                {
                    // stderr read on its own thread so a full pipe can't deadlock stdout
                    var ErrTask = P.StandardError.ReadToEndAsync();
                    Output = P.StandardOutput.ReadToEnd().Trim();
                    P.WaitForExit(90000);
                    string Errors = ErrTask.Result.Trim();
                    if (Errors != "") Props.WriteErrorLog("clsTabletHotspot/RunScript " + Action + ": " + Errors);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsTabletHotspot/RunScript: " + ex.Message);
            }
            return Output;
        }

        private void Watch_Tick(object sender, EventArgs e)
        {
            if (Enabled && !Paused && !Busy) RefreshAsync(WatchDone);
        }

        private void WatchDone()
        {
            // 3 fast restarts (30 s apart) once the hotspot is seen Off, then every 5 min for
            // as long as the setting is on. Reset once it's seen On. Unknown (the check
            // itself failed) isn't counted
            if (State == StateOn)
            {
                Restarts = 0;
                SlowTicks = 0;
            }
            else if (State == StateOff && Enabled && !Paused)
            {
                if (Restarts < FastRestarts)
                {
                    Restarts++;
                    Props.WriteErrorLog("clsTabletHotspot/Watch: hotspot off, restart " + Restarts.ToString()
                        + " of " + FastRestarts.ToString() + LastReason());
                    StartAsync();
                }
                else if (++SlowTicks >= SlowRetryTicks)
                {
                    SlowTicks = 0;
                    Props.WriteErrorLog("clsTabletHotspot/Watch: hotspot still off, slow retry" + LastReason());
                    StartAsync();
                }
            }
        }

        private string LastReason()
        {
            return Result != "" && Result != "Success" ? ", last: " + Result : "";
        }

        // ── Firewall ──────────────────────────────────────────────────────────

        public static FirewallStatus CheckFirewall()
        {
            return CheckFirewall(Process.GetCurrentProcess().MainModule.FileName);
        }

        public static FirewallStatus CheckFirewall(string ExePath)
        {
            // Reads the rules through HNetCfg.FwPolicy2 (no admin, not localized like
            // netsh). The hotspot network has no profile, so it's always Public.
            // Inbound only; a Block rule on this exe for Public wins over any Allow.
            const int PublicProfile = 4;
            const int DirIn = 1;
            const int ActionAllow = 1;
            const int ProtoUdp = 17;
            const int ProtoAny = 256;

            FirewallStatus Status = FirewallStatus.Missing;
            try
            {
                Type T = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                object Policy = Activator.CreateInstance(T);
                object Rules = Get(Policy, "Rules");
                foreach (object R in (IEnumerable)Rules)
                {
                    if (!(bool)Get(R, "Enabled")) continue;
                    if ((int)Get(R, "Direction") != DirIn) continue;
                    if (((int)Get(R, "Profiles") & PublicProfile) == 0) continue;
                    int Proto = (int)Get(R, "Protocol");
                    if (Proto != ProtoUdp && Proto != ProtoAny) continue;

                    string App = Get(R, "ApplicationName") as string ?? "";
                    bool ThisApp = App != "" && string.Equals(App, ExePath, StringComparison.OrdinalIgnoreCase);
                    if (App != "" && !ThisApp) continue;

                    // a rule with no program can still be tied to a service or a Store app
                    // package (Store, Game Bar, ...). Those don't cover us
                    if (App == "" && (Opt(R, "ServiceName") != "" || Opt(R, "LocalAppPackageId") != "")) continue;

                    bool Allow = (int)Get(R, "Action") == ActionAllow;
                    if (!Allow)
                    {
                        // only a block aimed at this exe; a generic block isn't ours to touch
                        if (ThisApp) return FirewallStatus.Blocked;
                        continue;
                    }

                    if (PortMatches(Get(R, "LocalPorts") as string, ModulePort)
                        && SubnetMatches(Get(R, "RemoteAddresses") as string))
                    {
                        Status = FirewallStatus.Allowed;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsTabletHotspot/CheckFirewall: " + ex.Message);
                Status = FirewallStatus.Unknown;
            }
            return Status;
        }

        public static FirewallStatus AllowFirewall()
        {
            return AllowFirewall(Process.GetCurrentProcess().MainModule.FileName);
        }

        public static FirewallStatus AllowFirewall(string ExePath)
        {
            // One admin prompt. Adds the port-based rule (survives upgrades and folder moves)
            // limited to the hotspot subnet, and if a Block rule on this exe covers Public,
            // turns this exe's Public rules into Allow so the block stops winning.
            try
            {
                string Rule = "advfirewall firewall {0} rule name=\"" + FirewallRule + "\"";
                string Args = "/c netsh " + string.Format(Rule, "delete")
                    + " & netsh " + string.Format(Rule, "add")
                    + " dir=in action=allow protocol=UDP localport=" + ModulePort.ToString()
                    + " profile=public remoteip=" + HotspotSubnet;
                if (CheckFirewall(ExePath) == FirewallStatus.Blocked)
                {
                    Args += " & netsh advfirewall firewall set rule name=all dir=in profile=public program=\""
                        + ExePath + "\" new action=allow";
                }
                ProcessStartInfo Info = new ProcessStartInfo("cmd.exe", Args);
                Info.UseShellExecute = true;
                Info.Verb = "runas";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                using (Process P = Process.Start(Info))
                {
                    P.WaitForExit(15000);
                }
            }
            catch (Win32Exception)
            {
                // admin prompt declined
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsTabletHotspot/AllowFirewall: " + ex.Message);
            }
            return CheckFirewall(ExePath);
        }

        // ── 5 GHz uplink ──────────────────────────────────────────────────────
        //
        // Most WiFi adapters have one radio, and Windows runs the hotspot on the channel of
        // the network the PC itself is joined to. If that's a 5 GHz network (a dual-band
        // phone hotspot can be either, day to day — measured 2026-09-25 and 2026-11-01), the
        // hotspot can't be on 2.4 GHz and the 2.4 GHz-only module can't join. Adapters that
        // can run both bands at once exist, so this is only reported when module data isn't
        // arriving — then it's the likely cause, otherwise it's harmless.

        /// <summary>
        /// True when the tablet hotspot is wanted, module data isn't arriving, and this PC's
        /// own WiFi connection is on 5 GHz. ssid is that network's name.
        /// </summary>
        public static bool BandProblem(out string ssid)
        {
            ssid = "";
            if (Core.Hotspot == null || !Core.Hotspot.Enabled) return false;
            if (Core.ModuleConnected && (DateTime.UtcNow - Core.LastModuleReceive).TotalSeconds < 5) return false;
            try
            {
                Wlan.Adapter a = Wlan.PreferredAdapter();
                if (a == null || !a.IsConnected) return false;
                Wlan.Network n = Wlan.ConnectedNetwork(a.Guid);
                if (n == null) return false;
                if (Wlan.CurrentChannel(a.Guid) <= 14) return false;
                ssid = n.Ssid;
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsTabletHotspot/BandProblem: " + ex.Message);
                return false;
            }
        }

        /// <summary>What to do about it. Kept in step with the manual's Comm section.</summary>
        public static string BandFixText(string ssid)
        {
            string nl = Environment.NewLine;
            return "This PC is connected to " + ssid + " on 5 GHz, so the module may not be able to join the tablet hotspot." + nl + nl
                 + "Set this PC's WiFi to use 2.4 GHz:" + nl
                 + "1. Press Windows+R, type ncpa.cpl and press Enter." + nl
                 + "2. Right-click Wi-Fi and choose Properties, then Configure, then the Advanced tab." + nl
                 + "3. Set Preferred Band to Prefer 2.4GHz band. On some PCs it is called Band or Wireless Mode." + nl
                 + "4. Press OK and wait for the PC to reconnect to " + ssid + "." + nl + nl
                 + "Or set " + ssid + " itself to 2.4 GHz, if the phone or router has that setting.";
        }

        /// <summary>
        /// If the firewall would drop module data from the hotspot, explains it and offers the
        /// fix (one admin prompt). Shared by the Comm page and the background check so the
        /// wording is the same everywhere. Returns the firewall state afterwards.
        /// </summary>
        public static FirewallStatus OfferFirewallFix(System.Windows.Forms.IWin32Window owner)
        {
            FirewallStatus status = CheckFirewall();
            if (status != FirewallStatus.Missing && status != FirewallStatus.Blocked) return status;

            if (YieldFlo.Forms.frmMsgBox.Ask(owner,
                    "Windows Firewall is blocking module data from the tablet hotspot." + Environment.NewLine + Environment.NewLine
                    + "Allow it now? Windows will ask for permission.",
                    "Tablet Hotspot"))
            {
                status = AllowFirewall();
                Props.WriteErrorLog("clsTabletHotspot/Firewall: fix requested, firewall now " + status);
            }
            return status;
        }

        private static object Get(object Com, string Name)
        {
            return Com.GetType().InvokeMember(Name, BindingFlags.GetProperty, null, Com, null);
        }

        private static string Opt(object Com, string Name)
        {
            // optional string property: LocalAppPackageId is INetFwRule3, missing on old rules
            try
            {
                return Get(Com, Name) as string ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static bool PortMatches(string Ports, int Port)
        {
            // "*", "30100", "30000-30200", or a comma list of those
            if (string.IsNullOrEmpty(Ports) || Ports == "*") return true;
            foreach (string P in Ports.Split(','))
            {
                string S = P.Trim();
                int Dash = S.IndexOf('-');
                int Lo, Hi;
                if (Dash > 0)
                {
                    if (int.TryParse(S.Substring(0, Dash), out Lo) && int.TryParse(S.Substring(Dash + 1), out Hi)
                        && Port >= Lo && Port <= Hi) return true;
                }
                else if (int.TryParse(S, out Lo) && Lo == Port)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SubnetMatches(string Addresses)
        {
            // "*", "LocalSubnet" (the hotspot adapter's own subnet), or a list containing
            // 192.168.137.x in any form (the COM API reports 192.168.137.0/255.255.255.0)
            if (string.IsNullOrEmpty(Addresses) || Addresses == "*") return true;
            foreach (string A in Addresses.Split(','))
            {
                string S = A.Trim();
                if (S.Equals("LocalSubnet", StringComparison.OrdinalIgnoreCase)) return true;
                if (S.StartsWith("192.168.137.", StringComparison.Ordinal)) return true;
            }
            return false;
        }
    }
}
