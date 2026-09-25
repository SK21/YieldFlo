using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace YieldFlo.Classes
{
    // ── Module WiFi provisioning ──────────────────────────────────────────────
    //
    // The module's own settings page is the only way to give it the credentials of
    // the network it should join, and that page lives on the module's hotspot — so
    // configuring it has always meant a phone, an SSID list and a typed password,
    // once per module. This does the same thing from the app: hop the PC's WiFi
    // adapter onto the module's hotspot, POST the credentials to the same form the
    // phone submits, and hop back.
    //
    // The PC has to know the network anyway, because that is where NTRIP comes
    // from, so nothing new is being asked of the user except the passphrase itself
    // — and that is asked once and kept.
    //
    // Why wlanapi.dll and not "netsh wlan": netsh prints localized field names.
    // "State", "SSID" and "Key Content" are translated on a German or Polish
    // Windows, and this app ships in seven languages, so parsing that output would
    // work on the development machine and fail in the field. The native API returns
    // structs, which are the same everywhere.
    internal static class Wlan
    {
        private const int WLAN_MAX_NAME_LENGTH = 256;

        // WLAN_INTERFACE_STATE
        public const uint InterfaceStateConnected = 1;

        // WLAN_AVAILABLE_NETWORK dwFlags
        private const uint NetworkFlagConnected = 0x00000001;

        // WlanGetAvailableNetworkList dwFlags — include hidden networks that have a
        // manually created profile, so a shed router set up as hidden still appears.
        private const uint IncludeAllManualHiddenProfiles = 0x00000002;

        // WlanSetProfile dwFlags. A per-user profile can be written by an ordinary
        // user; an all-user profile (flags 0) needs administrator rights, and the
        // app has no business asking for those to set up a hotspot.
        private const uint ProfileUser = 0x00000002;

        // WLAN_INTF_OPCODE
        private const uint OpcodeChannelNumber = 8;

        private const uint ErrorSuccess = 0;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WLAN_INTERFACE_INFO
        {
            public Guid InterfaceGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WLAN_MAX_NAME_LENGTH)]
            public string strInterfaceDescription;
            public uint isState;
        }

        // DOT11_SSID is inlined as uSSIDLength + ucSSID rather than nested: the
        // layout is identical and a nested struct holding a ByValArray is one more
        // thing to get wrong.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WLAN_AVAILABLE_NETWORK
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WLAN_MAX_NAME_LENGTH)]
            public string strProfileName;
            public uint uSSIDLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] ucSSID;
            public uint dot11BssType;
            public uint uNumberOfBssids;
            [MarshalAs(UnmanagedType.Bool)] public bool bNetworkConnectable;
            public uint wlanNotConnectableReason;
            public uint uNumberOfPhyTypes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] dot11PhyTypes;
            [MarshalAs(UnmanagedType.Bool)] public bool bMorePhyTypes;
            public uint wlanSignalQuality;
            [MarshalAs(UnmanagedType.Bool)] public bool bSecurityEnabled;
            public uint dot11DefaultAuthAlgorithm;
            public uint dot11DefaultCipherAlgorithm;
            public uint dwFlags;
            public uint dwReserved;
        }

        // One radio of one access point, needed for ulChCenterFrequency — the only call
        // that reports real frequencies. The awkward field order is the native layout
        // and must not be tidied: the single-byte bInRegDomain followed by a USHORT and
        // then two 8-byte-aligned timestamps is what makes this 360 bytes, which is the
        // size to check against if marshalling ever looks wrong.
        [StructLayout(LayoutKind.Sequential)]
        private struct WLAN_BSS_ENTRY
        {
            public uint uSSIDLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] ucSSID;
            public uint uPhyId;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public byte[] dot11Bssid;
            public uint dot11BssType;
            public uint dot11BssPhyType;
            public int lRssi;
            public uint uLinkQuality;
            public byte bInRegDomain;
            public ushort usBeaconPeriod;
            public ulong ullTimestamp;
            public ulong ullHostTimestamp;
            public ushort usCapabilityInformation;
            public uint ulChCenterFrequency;
            public uint uRateSetLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 126)] public ushort[] usRateSet;
            public uint ulIeOffset;
            public uint ulIeSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WLAN_CONNECTION_PARAMETERS
        {
            public uint wlanConnectionMode;     // 0 = wlan_connection_mode_profile
            [MarshalAs(UnmanagedType.LPWStr)] public string strProfile;
            public IntPtr pDot11Ssid;
            public IntPtr pDesiredBssidList;
            public uint dot11BssType;           // 1 = infrastructure
            public uint dwFlags;
        }

        [DllImport("wlanapi.dll")]
        private static extern uint WlanOpenHandle(uint dwClientVersion, IntPtr pReserved,
            out uint pdwNegotiatedVersion, out IntPtr phClientHandle);

        [DllImport("wlanapi.dll")]
        private static extern uint WlanCloseHandle(IntPtr hClientHandle, IntPtr pReserved);

        [DllImport("wlanapi.dll")]
        private static extern void WlanFreeMemory(IntPtr pMemory);

        [DllImport("wlanapi.dll")]
        private static extern uint WlanEnumInterfaces(IntPtr hClientHandle, IntPtr pReserved,
            out IntPtr ppInterfaceList);

        [DllImport("wlanapi.dll")]
        private static extern uint WlanScan(IntPtr hClientHandle, ref Guid pInterfaceGuid,
            IntPtr pDot11Ssid, IntPtr pIeData, IntPtr pReserved);

        [DllImport("wlanapi.dll")]
        private static extern uint WlanGetAvailableNetworkList(IntPtr hClientHandle,
            ref Guid pInterfaceGuid, uint dwFlags, IntPtr pReserved, out IntPtr ppAvailableNetworkList);

        [DllImport("wlanapi.dll")]
        private static extern uint WlanGetNetworkBssList(IntPtr hClientHandle, ref Guid pInterfaceGuid,
            IntPtr pDot11Ssid, uint dot11BssType, bool bSecurityEnabled, IntPtr pReserved,
            out IntPtr ppWlanBssList);

        [DllImport("wlanapi.dll")]
        private static extern uint WlanQueryInterface(IntPtr hClientHandle, ref Guid pInterfaceGuid,
            uint OpCode, IntPtr pReserved, out uint pdwDataSize, out IntPtr ppData,
            IntPtr pWlanOpcodeValueType);

        [DllImport("wlanapi.dll", CharSet = CharSet.Unicode)]
        private static extern uint WlanSetProfile(IntPtr hClientHandle, ref Guid pInterfaceGuid,
            uint dwFlags, string strProfileXml, string strAllUserProfileSecurity, bool bOverwrite,
            IntPtr pReserved, out uint pdwReasonCode);

        [DllImport("wlanapi.dll")]
        private static extern uint WlanConnect(IntPtr hClientHandle, ref Guid pInterfaceGuid,
            ref WLAN_CONNECTION_PARAMETERS pConnectionParameters, IntPtr pReserved);

        [DllImport("wlanapi.dll", CharSet = CharSet.Unicode)]
        private static extern uint WlanReasonCodeToString(uint dwReasonCode, uint dwBufferSize,
            StringBuilder pStringBuffer, IntPtr pReserved);

        // ── Handle ────────────────────────────────────────────────────────────

        private static IntPtr Open()
        {
            uint negotiated;
            IntPtr handle;
            // Client version 2 = Vista and later. Version 1 changes the meaning of
            // several structures, so it is not a safe fallback.
            if (WlanOpenHandle(2, IntPtr.Zero, out negotiated, out handle) != ErrorSuccess)
                return IntPtr.Zero;
            return handle;
        }

        private static void Close(IntPtr handle)
        {
            if (handle != IntPtr.Zero) WlanCloseHandle(handle, IntPtr.Zero);
        }

        public static string ReasonToString(uint reasonCode)
        {
            try
            {
                var sb = new StringBuilder(512);
                if (WlanReasonCodeToString(reasonCode, (uint)sb.Capacity, sb, IntPtr.Zero) == ErrorSuccess)
                    return sb.ToString();
            }
            catch { }
            return "reason " + reasonCode;
        }

        // ── Interfaces ────────────────────────────────────────────────────────

        public sealed class Adapter
        {
            public Guid Guid;
            public string Description;
            public uint State;
            public bool IsConnected { get { return State == InterfaceStateConnected; } }
        }

        public static List<Adapter> Adapters()
        {
            var list = new List<Adapter>();
            IntPtr h = Open();
            if (h == IntPtr.Zero) return list;
            try
            {
                IntPtr p;
                if (WlanEnumInterfaces(h, IntPtr.Zero, out p) != ErrorSuccess) return list;
                try
                {
                    int count = Marshal.ReadInt32(p, 0);
                    int stride = Marshal.SizeOf(typeof(WLAN_INTERFACE_INFO));
                    for (int i = 0; i < count; i++)
                    {
                        // dwNumberOfItems + dwIndex occupy the first 8 bytes.
                        IntPtr item = new IntPtr(p.ToInt64() + 8 + (long)i * stride);
                        var info = (WLAN_INTERFACE_INFO)Marshal.PtrToStructure(item, typeof(WLAN_INTERFACE_INFO));
                        list.Add(new Adapter
                        {
                            Guid = info.InterfaceGuid,
                            Description = info.strInterfaceDescription,
                            State = info.isState
                        });
                    }
                }
                finally { WlanFreeMemory(p); }
            }
            catch (Exception ex) { Props.WriteErrorLog("Wlan/Adapters " + ex.Message); }
            finally { Close(h); }
            return list;
        }

        /// <summary>
        /// The adapter to work with: the one that is connected, since that is the one
        /// carrying NTRIP. Falls back to the first present so a disconnected machine
        /// can still scan.
        /// </summary>
        public static Adapter PreferredAdapter()
        {
            var all = Adapters();
            foreach (var a in all) if (a.IsConnected) return a;
            return all.Count > 0 ? all[0] : null;
        }

        public static uint StateOf(Guid guid)
        {
            foreach (var a in Adapters()) if (a.Guid == guid) return a.State;
            return 0;
        }

        // ── Scanning ──────────────────────────────────────────────────────────

        public sealed class Network
        {
            public string Ssid;
            public string ProfileName;      // empty when no profile is saved
            public int SignalPercent;
            public bool Secured;
            public bool Connected;
        }

        /// <summary>
        /// Asks the driver for a fresh scan. Returns immediately — results take a
        /// couple of seconds to arrive, so callers wait and then read the list.
        /// </summary>
        public static void RequestScan(Guid guid)
        {
            IntPtr h = Open();
            if (h == IntPtr.Zero) return;
            try { WlanScan(h, ref guid, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero); }
            catch (Exception ex) { Props.WriteErrorLog("Wlan/RequestScan " + ex.Message); }
            finally { Close(h); }
        }

        /// <summary>
        /// Visible networks, one row per name.
        /// <para>
        /// The API does not return one entry per network — it returns one per
        /// (network, profile) pair, so a network that has a saved profile comes back
        /// twice: once matched to the profile and once as an unprofiled sighting of
        /// the same SSID. Measured on the development machine, both Starlink and
        /// Net23_EXT2G appeared twice. Left alone that puts every previously
        /// configured module in the list twice, so the rows are merged here rather
        /// than in each caller — and the merge keeps the profile name, which is what
        /// WlanConnect actually needs.
        /// </para>
        /// </summary>
        public static List<Network> Networks(Guid guid)
        {
            var merged = new Dictionary<string, Network>(StringComparer.Ordinal);
            var list = new List<Network>();
            IntPtr h = Open();
            if (h == IntPtr.Zero) return list;
            try
            {
                IntPtr p;
                if (WlanGetAvailableNetworkList(h, ref guid, IncludeAllManualHiddenProfiles,
                        IntPtr.Zero, out p) != ErrorSuccess) return list;
                try
                {
                    int count = Marshal.ReadInt32(p, 0);
                    int stride = Marshal.SizeOf(typeof(WLAN_AVAILABLE_NETWORK));
                    for (int i = 0; i < count; i++)
                    {
                        IntPtr item = new IntPtr(p.ToInt64() + 8 + (long)i * stride);
                        var n = (WLAN_AVAILABLE_NETWORK)Marshal.PtrToStructure(item, typeof(WLAN_AVAILABLE_NETWORK));

                        int len = (int)n.uSSIDLength;
                        if (len <= 0 || len > 32) continue;      // hidden, or nothing to show
                        string ssid = Encoding.UTF8.GetString(n.ucSSID, 0, len);
                        if (ssid.Length == 0) continue;

                        var row = new Network
                        {
                            Ssid = ssid,
                            ProfileName = n.strProfileName ?? "",
                            SignalPercent = (int)n.wlanSignalQuality,
                            Secured = n.bSecurityEnabled,
                            Connected = (n.dwFlags & NetworkFlagConnected) != 0
                        };

                        Network seen;
                        if (!merged.TryGetValue(ssid, out seen)) { merged[ssid] = row; continue; }

                        if (row.SignalPercent > seen.SignalPercent)
                        {
                            seen.SignalPercent = row.SignalPercent;
                            seen.Secured = row.Secured;
                        }
                        if (seen.ProfileName.Length == 0) seen.ProfileName = row.ProfileName;
                        seen.Connected = seen.Connected || row.Connected;
                    }
                }
                finally { WlanFreeMemory(p); }
            }
            catch (Exception ex) { Props.WriteErrorLog("Wlan/Networks " + ex.Message); }
            finally { Close(h); }

            list.AddRange(merged.Values);
            list.Sort((a, b) => b.SignalPercent.CompareTo(a.SignalPercent));
            return list;
        }

        public static Network ConnectedNetwork(Guid guid)
        {
            foreach (var n in Networks(guid)) if (n.Connected) return n;
            return null;
        }

        /// <summary>
        /// Channel of the current connection, or 0 if unknown. Used only for the channel
        /// hint sent with the credentials: a 2.4 GHz channel lets the module's first join
        /// be a directed attempt instead of a full scan. A 5 GHz channel number falls
        /// outside the 1..13 the firmware accepts, so no hint is sent and the module
        /// scans for itself — which is the right outcome, because nothing here can tell
        /// whether the same name also has a 2.4 GHz radio.
        /// </summary>
        public static int CurrentChannel(Guid guid)
        {
            IntPtr h = Open();
            if (h == IntPtr.Zero) return 0;
            try
            {
                uint size;
                IntPtr data;
                if (WlanQueryInterface(h, ref guid, OpcodeChannelNumber, IntPtr.Zero,
                        out size, out data, IntPtr.Zero) != ErrorSuccess) return 0;
                try
                {
                    if (size < 4 || data == IntPtr.Zero) return 0;
                    return Marshal.ReadInt32(data);
                }
                finally { WlanFreeMemory(data); }
            }
            catch (Exception ex) { Props.WriteErrorLog("Wlan/CurrentChannel " + ex.Message); return 0; }
            finally { Close(h); }
        }

        private static int ChannelFromKhz(uint khz)
        {
            int mhz = (int)(khz / 1000);
            if (mhz == 2484) return 14;                              // Japan
            if (mhz >= 2412 && mhz <= 2472) return (mhz - 2407) / 5; // 1..13
            return 0;                                               // 5 GHz, or not a channel we name
        }

        /// <summary>
        /// The 2.4 GHz channel a named network is on, or 0 if it cannot be established.
        /// Picks the strongest radio if the network has several.
        /// <para>
        /// This is worth the extra interop for one reason: a module with no cached
        /// channel has no cheap retry available, so every attempt is a full scan, and
        /// the firmware rations those to one every five minutes while a client is on its
        /// hotspot (StaRetryScanOnlyMs). That is the state every firmware update leaves
        /// behind, and it cost a field session on 2026-09-25. Sending a real channel with
        /// the credentials means the module never enters it.
        /// </para>
        /// <para>
        /// Call this only while associated to something OTHER than the network being
        /// asked about. The driver does not report the bands of the network it is itself
        /// joined to — measured: associated to 'Phone' on channel 161, Phone's 2.4 GHz
        /// radio is invisible, while other networks report both bands correctly. In the
        /// provisioning flow that is satisfied for free, because it is called while the
        /// PC sits on the module's own hotspot.
        /// </para>
        /// </summary>
        public static int Channel24For(Guid guid, string ssid)
        {
            if (string.IsNullOrEmpty(ssid)) return 0;

            IntPtr h = Open();
            if (h == IntPtr.Zero) return 0;
            try
            {
                IntPtr p;
                // Unfiltered, bss type 'any': marshalling a DOT11_SSID to filter in the
                // driver buys nothing on a list this small, and it is one less pointer
                // to get wrong.
                if (WlanGetNetworkBssList(h, ref guid, IntPtr.Zero, 3, false, IntPtr.Zero, out p) != ErrorSuccess)
                    return 0;
                try
                {
                    // Header is dwTotalSize then dwNumberOfItems — note the order differs
                    // from the interface and available-network lists, which put the count
                    // first.
                    int count = Marshal.ReadInt32(p, 4);
                    int stride = Marshal.SizeOf(typeof(WLAN_BSS_ENTRY));
                    int bestRssi = int.MinValue;
                    int channel = 0;

                    for (int i = 0; i < count; i++)
                    {
                        IntPtr item = new IntPtr(p.ToInt64() + 8 + (long)i * stride);
                        var e = (WLAN_BSS_ENTRY)Marshal.PtrToStructure(item, typeof(WLAN_BSS_ENTRY));

                        int len = (int)e.uSSIDLength;
                        if (len <= 0 || len > 32) continue;
                        if (Encoding.UTF8.GetString(e.ucSSID, 0, len) != ssid) continue;

                        int ch = ChannelFromKhz(e.ulChCenterFrequency);
                        if (ch == 0) continue;                  // a 5 GHz radio of the same name
                        if (e.lRssi <= bestRssi) continue;
                        bestRssi = e.lRssi;
                        channel = ch;
                    }
                    return channel;
                }
                finally { WlanFreeMemory(p); }
            }
            catch (Exception ex) { Props.WriteErrorLog("Wlan/Channel24For " + ex.Message); return 0; }
            finally { Close(h); }
        }

        // ── Profiles and connecting ───────────────────────────────────────────

        private static string Xml(string s)
        {
            return (s ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
                            .Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        /// <summary>
        /// connectionMode is deliberately 'manual'. An auto profile makes Windows
        /// treat the module's hotspot as a preferred network and roam onto it on its
        /// own later — in the middle of harvest, taking the PC off the network that
        /// carries NTRIP. The hotspot is somewhere to visit, never somewhere to live.
        /// </summary>
        private static string ProfileXml(string ssid, string key)
        {
            bool open = string.IsNullOrEmpty(key);
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\"?>");
            sb.Append("<WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\">");
            sb.Append("<name>").Append(Xml(ssid)).Append("</name>");
            sb.Append("<SSIDConfig><SSID><name>").Append(Xml(ssid)).Append("</name></SSID></SSIDConfig>");
            sb.Append("<connectionType>ESS</connectionType>");
            sb.Append("<connectionMode>manual</connectionMode>");
            sb.Append("<MSM><security><authEncryption>");
            sb.Append("<authentication>").Append(open ? "open" : "WPA2PSK").Append("</authentication>");
            sb.Append("<encryption>").Append(open ? "none" : "AES").Append("</encryption>");
            sb.Append("<useOneX>false</useOneX>");
            sb.Append("</authEncryption>");
            if (!open)
            {
                sb.Append("<sharedKey><keyType>passPhrase</keyType><protected>false</protected>");
                sb.Append("<keyMaterial>").Append(Xml(key)).Append("</keyMaterial></sharedKey>");
            }
            sb.Append("</security></MSM></WLANProfile>");
            return sb.ToString();
        }

        /// <summary>
        /// Writes a per-user profile for the hotspot. Failure is not fatal and is not
        /// reported as an error: the usual reason is that a profile of that name
        /// already exists as an all-user profile — which is the normal case for a
        /// module that has been set up from this PC before, and which already holds
        /// the right key. Connecting is what decides.
        /// </summary>
        public static bool TrySetProfile(Guid guid, string ssid, string key, out string note)
        {
            note = "";
            IntPtr h = Open();
            if (h == IntPtr.Zero) { note = "WLAN service unavailable"; return false; }
            try
            {
                uint reason;
                uint r = WlanSetProfile(h, ref guid, ProfileUser, ProfileXml(ssid, key), null, true,
                                        IntPtr.Zero, out reason);
                if (r == ErrorSuccess) return true;
                note = reason != 0 ? ReasonToString(reason) : "error " + r;
                return false;
            }
            catch (Exception ex) { note = ex.Message; return false; }
            finally { Close(h); }
        }

        public static bool BeginConnect(Guid guid, string profileName, out string error)
        {
            error = "";
            IntPtr h = Open();
            if (h == IntPtr.Zero) { error = "WLAN service unavailable"; return false; }
            try
            {
                var p = new WLAN_CONNECTION_PARAMETERS
                {
                    wlanConnectionMode = 0,     // by profile
                    strProfile = profileName,
                    pDot11Ssid = IntPtr.Zero,
                    pDesiredBssidList = IntPtr.Zero,
                    dot11BssType = 1,           // infrastructure
                    dwFlags = 0
                };
                uint r = WlanConnect(h, ref guid, ref p, IntPtr.Zero);
                if (r == ErrorSuccess) return true;
                error = "error " + r;
                return false;
            }
            catch (Exception ex) { error = ex.Message; return false; }
            finally { Close(h); }
        }

        /// <summary>
        /// Connects and waits until the adapter reports itself associated to that
        /// SSID. The interface state alone is not enough — it says "connected", not
        /// "connected to the network you asked for", and on a failed join Windows
        /// will happily fall back to a different saved network.
        /// </summary>
        public static bool ConnectAndWait(Guid guid, string profileName, string expectSsid,
                                          int timeoutMs, out string error)
        {
            if (!BeginConnect(guid, profileName, out error)) return false;

            int waited = 0;
            while (waited < timeoutMs)
            {
                Thread.Sleep(500);
                waited += 500;
                var n = ConnectedNetwork(guid);
                if (n != null && string.Equals(n.Ssid, expectSsid, StringComparison.Ordinal)) return true;
            }
            error = "timed out waiting to join " + expectSsid;
            return false;
        }

        // ── IP ────────────────────────────────────────────────────────────────

        /// <summary>
        /// The adapter's IPv4 default gateway, which on the module's hotspot is the
        /// module itself. Taken from DHCP rather than computed as
        /// 192.168.(ID+200).1 — the app does not know the module's ID, and asking
        /// the network is right in any case.
        /// </summary>
        public static IPAddress WaitForGateway(Guid guid, int timeoutMs)
        {
            int waited = 0;
            while (true)
            {
                try
                {
                    foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        Guid id;
                        try { id = new Guid(ni.Id); } catch { continue; }
                        if (id != guid) continue;
                        if (ni.OperationalStatus != OperationalStatus.Up) continue;

                        foreach (var gw in ni.GetIPProperties().GatewayAddresses)
                        {
                            if (gw.Address == null) continue;
                            if (gw.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;
                            if (gw.Address.Equals(IPAddress.Any)) continue;
                            return gw.Address;
                        }
                    }
                }
                catch (Exception ex) { Props.WriteErrorLog("Wlan/WaitForGateway " + ex.Message); }

                if (waited >= timeoutMs) return null;
                Thread.Sleep(500);
                waited += 500;
            }
        }
    }

    // ── Stored credentials ────────────────────────────────────────────────────
    //
    // The passphrase is kept so it is typed once rather than once per module, and
    // it is kept encrypted because user.config is a plain XML file in the user's
    // profile. DPAPI at CurrentUser scope ties the ciphertext to this Windows
    // account on this machine, which is the right strength here: it stops the
    // password being readable by anything that merely opens the file, and it does
    // not pretend to survive the user's account being compromised.
    //
    // A blob that will not decrypt is treated as absent, not as an error — that is
    // what a copied user.config from another machine looks like, and the answer is
    // to ask for the password again.
    public static class WifiStore
    {
        public static string HomeSsid
        {
            get { return Properties.Settings.Default.WifiHomeSsid ?? ""; }
            set { Properties.Settings.Default.WifiHomeSsid = value ?? ""; }
        }

        public static string HomeKey
        {
            get { return Unprotect(Properties.Settings.Default.WifiHomeKey); }
            set { Properties.Settings.Default.WifiHomeKey = Protect(value); }
        }

        public static string ModuleKey
        {
            get { return Unprotect(Properties.Settings.Default.WifiModuleKey); }
            set { Properties.Settings.Default.WifiModuleKey = Protect(value); }
        }

        public static void Save() { Properties.Settings.Default.Save(); }

        private static string Protect(string plain)
        {
            if (string.IsNullOrEmpty(plain)) return "";
            try
            {
                byte[] cipher = System.Security.Cryptography.ProtectedData.Protect(
                    Encoding.UTF8.GetBytes(plain), null,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(cipher);
            }
            catch (Exception ex) { Props.WriteErrorLog("WifiStore/Protect " + ex.Message); return ""; }
        }

        private static string Unprotect(string stored)
        {
            if (string.IsNullOrEmpty(stored)) return "";
            try
            {
                byte[] plain = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(stored), null,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(plain);
            }
            catch { return ""; }
        }
    }

    // ── The provisioning run ──────────────────────────────────────────────────

    public enum ProvisionOutcome
    {
        Success,            // module packets arrived on the home network
        AuthRejected,       // module says the router refused the password
        NotConnected,       // module is trying but has not joined
        NoPackets,          // module joined but nothing reached the app
        HotspotFailed,      // could not get onto the module's hotspot
        PortalFailed,       // hotspot fine, settings page did not answer
        HomeReturnFailed,   // credentials sent, but the PC did not get back
        Aborted
    }

    public sealed class ProvisionRequest
    {
        public Guid Adapter;
        public string ModuleSsid;           // the hotspot to configure
        public string ModuleKey = "";       // hotspot password; empty for an open hotspot
        public string HomeSsid;             // network the module should join
        public string HomeProfile;          // Windows profile name for that network
        public string HomeKey = "";         // passphrase handed to the module
        public int HomeChannel;             // 0 if unknown; primes the module's channel cache
    }

    public sealed class ProvisionResult
    {
        public ProvisionOutcome Outcome;
        public string Detail = "";
    }

    public sealed class ModuleProvisioner
    {
        private readonly Action<string> _say;

        public ModuleProvisioner(Action<string> progress) { _say = progress; }

        private void Say(string s) { if (_say != null) _say(s); }

        // Timings. The hotspot join is the slow one: the module's radio is shared
        // between its hotspot and its own station attempts, so it can be briefly
        // deaf when we knock.
        private const int JoinTimeoutMs = 25000;
        private const int GatewayTimeoutMs = 15000;
        private const int HttpTimeoutMs = 8000;
        private const int ScanSettleMs = 3500;      // driver needs a few seconds to finish a scan
        private const int RebootWaitMs = 6000;      // SaveData() delays 3 s, then boots
        // Long enough to outlast one backoff cycle. The module's retry interval stretches
        // from 10 s to 60 s after three failures (StaRetrySlowMs), and its first three
        // attempts all happen while the PC is away on its hotspot — so a shorter wait can
        // expire in the gap between attempts on a network that is perfectly fine.
        private const int PacketWaitMs = 60000;
        private const int NudgeWaitMs = 30000;      // second window, after forcing a retry

        public ProvisionResult Run(ProvisionRequest req)
        {
            var result = new ProvisionResult();

            // 1 ── onto the module's hotspot
            Say("Joining " + req.ModuleSsid + " ...");
            string note;
            if (!Wlan.TrySetProfile(req.Adapter, req.ModuleSsid, req.ModuleKey, out note) && note.Length > 0)
                Say("  (using the saved profile: " + note + ")");

            string error;
            if (!Wlan.ConnectAndWait(req.Adapter, req.ModuleSsid, req.ModuleSsid, JoinTimeoutMs, out error))
            {
                Say("Could not join the hotspot: " + error);
                ReturnHome(req);
                result.Outcome = ProvisionOutcome.HotspotFailed;
                result.Detail = error;
                return result;
            }

            IPAddress gw = Wlan.WaitForGateway(req.Adapter, GatewayTimeoutMs);
            if (gw == null)
            {
                Say("Joined, but the hotspot handed out no address.");
                ReturnHome(req);
                result.Outcome = ProvisionOutcome.HotspotFailed;
                result.Detail = "no DHCP lease";
                return result;
            }
            Say("  module at " + gw);

            // 2 ── find the target network's 2.4 GHz channel, from here
            //
            // Here specifically, because the driver will not report the bands of the
            // network it is itself joined to — and sitting on the module's hotspot is the
            // one moment in this flow when the target network is not that. Worth the
            // detour: without a channel to cache, the module has no cheap retry, so every
            // attempt is a full scan and the firmware rations those to one per five
            // minutes while anything is on its hotspot. That is the state a firmware
            // update leaves behind.
            int channel = req.HomeChannel;
            int found = ScanForChannel(req);
            if (found > 0)
            {
                channel = found;
                Say("  " + req.HomeSsid + " is on 2.4 GHz channel " + channel);
            }

            // 3 ── send the credentials to the same form the phone submits
            string url = "http://" + gw + "/wifi";
            Say("Sending network name and password ...");

            // prop3 is the hotspot's own password and is deliberately NOT sent:
            // HandleWifiSettings() only touches MDL.APpassword when the argument is
            // present, so leaving it out preserves whatever the module has. Sending
            // it back would mean reading it off the page first and risking clearing
            // it on a parse miss.
            var body = new StringBuilder();
            body.Append("prop1=").Append(Uri.EscapeDataString(req.HomeSsid));
            body.Append("&prop2=").Append(Uri.EscapeDataString(req.HomeKey));
            body.Append("&connect=1");
            if (channel >= 1 && channel <= 13)
            {
                // Primes StaChannelCache so the module's first join after the reboot is a
                // directed attempt rather than a full scan, and so it never sits in the
                // five-minute full-scan rationing. Only honoured by the firmware when
                // pickssid matches the submitted name.
                body.Append("&pickssid=").Append(Uri.EscapeDataString(req.HomeSsid));
                body.Append("&pickch=").Append(channel);
            }
            else
            {
                Say("  no 2.4 GHz channel found — the module will scan for itself");
            }

            if (!HttpPost(url, body.ToString(), HttpTimeoutMs, out error))
            {
                Say("The module's settings page did not answer: " + error);
                ReturnHome(req);
                result.Outcome = ProvisionOutcome.PortalFailed;
                result.Detail = error;
                return result;
            }
            Say("  saved — the module is restarting");

            // 4 ── back to the network that carries NTRIP
            Thread.Sleep(RebootWaitMs);
            if (!ReturnHome(req))
            {
                result.Outcome = ProvisionOutcome.HomeReturnFailed;
                result.Detail = "could not rejoin " + req.HomeSsid;
                return result;
            }

            // 5 ── did it work? The only answer that counts is data arriving.
            Say("Waiting up to " + (PacketWaitMs / 1000) + " s for the module on " + req.HomeSsid + " ...");
            if (WaitForPackets(PacketWaitMs))
            {
                result.Outcome = ProvisionOutcome.Success;
                return result;
            }

            // 6 ── nothing arrived. "No packets" covers a refused password, a network
            // the module cannot see, and a firewall eating the traffic, and the user
            // can only act on one of those. The module itself knows which, so go back
            // and ask it — this hop happens only on failure, never on success.
            Say("Nothing arrived. Asking the module why ...");
            bool nudged;
            result.Outcome = ProvisionOutcome.NoPackets;
            result.Detail = Diagnose(req, out result.Outcome, out nudged);
            ReturnHome(req);

            // The module was told to try again, so give it one more window. Worth doing
            // because the most likely reason for the first window coming up empty is
            // timing, not configuration: a phone hotspot stops advertising while it has
            // no clients, and the PC — its only client — was away on the module's
            // hotspot for the whole of the module's first few attempts. Three failures
            // is all it takes for ServiceWifiStation() to stretch its interval to 60 s,
            // which can outlast the wait above even once the hotspot is back.
            if (nudged)
            {
                Say("Asked the module to try again. Waiting up to " + (NudgeWaitMs / 1000) + " s ...");
                if (WaitForPackets(NudgeWaitMs))
                {
                    result.Outcome = ProvisionOutcome.Success;
                    result.Detail = "";
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// True as soon as a module packet arrives after this call started. Nothing the
        /// module says about itself is as good as data actually turning up.
        /// </summary>
        private bool WaitForPackets(int timeoutMs)
        {
            DateTime start = DateTime.UtcNow;
            int waited = 0;
            while (waited < timeoutMs)
            {
                Thread.Sleep(500);
                waited += 500;
                if (Core.LastModuleReceive > start)
                {
                    Say("Module is sending data. Setup complete.");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Scans from the module's hotspot and returns the target network's 2.4 GHz
        /// channel, or 0 if it cannot be found. Re-confirms the hotspot association
        /// afterwards: a scan takes this PC's radio off the hotspot's channel for a couple
        /// of seconds, and the POST that follows needs that association back.
        /// </summary>
        private int ScanForChannel(ProvisionRequest req)
        {
            Wlan.RequestScan(req.Adapter);
            Thread.Sleep(ScanSettleMs);
            int channel = Wlan.Channel24For(req.Adapter, req.HomeSsid);

            var cur = Wlan.ConnectedNetwork(req.Adapter);
            if (cur == null || cur.Ssid != req.ModuleSsid)
            {
                Say("  rejoining the hotspot after the scan ...");
                string error;
                Wlan.ConnectAndWait(req.Adapter, req.ModuleSsid, req.ModuleSsid, JoinTimeoutMs, out error);
                Wlan.WaitForGateway(req.Adapter, GatewayTimeoutMs);
            }
            return channel;
        }

        private bool ReturnHome(ProvisionRequest req)
        {
            if (string.IsNullOrEmpty(req.HomeProfile)) return false;

            var cur = Wlan.ConnectedNetwork(req.Adapter);
            if (cur != null && cur.Ssid == req.HomeSsid) return true;

            Say("Reconnecting to " + req.HomeSsid + " ...");
            string error;
            if (!Wlan.ConnectAndWait(req.Adapter, req.HomeProfile, req.HomeSsid, JoinTimeoutMs, out error))
            {
                Say("Could not rejoin " + req.HomeSsid + ": " + error);
                return false;
            }
            Wlan.WaitForGateway(req.Adapter, GatewayTimeoutMs);
            return true;
        }

        /// <summary>
        /// Reads the verdict off the module's own settings page. The firmware already
        /// distinguishes a refused password from an absent network and says so in
        /// words; scraping those two phrases is cheaper than adding an endpoint, at
        /// the cost of breaking if the wording changes — so a miss falls back to the
        /// undiagnosed answer rather than guessing.
        /// </summary>
        private string Diagnose(ProvisionRequest req, out ProvisionOutcome outcome, out bool nudged)
        {
            outcome = ProvisionOutcome.NoPackets;
            nudged = false;

            string note, error;
            Wlan.TrySetProfile(req.Adapter, req.ModuleSsid, req.ModuleKey, out note);
            if (!Wlan.ConnectAndWait(req.Adapter, req.ModuleSsid, req.ModuleSsid, JoinTimeoutMs, out error))
                return "could not get back onto the hotspot to ask";

            IPAddress gw = Wlan.WaitForGateway(req.Adapter, GatewayTimeoutMs);
            if (gw == null) return "hotspot gave no address on the second visit";

            string html;
            if (!HttpGet("http://" + gw + "/wifi", HttpTimeoutMs, out html, out error))
                return "settings page did not answer: " + error;

            if (html.IndexOf("Password refused", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                outcome = ProvisionOutcome.AuthRejected;
                return req.HomeSsid + " refused the password.";
            }
            if (html.IndexOf("Connected to", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Joined, yet nothing reached the app: the packets are being dropped
                // between the two, not misaddressed by the module.
                outcome = ProvisionOutcome.NoPackets;
                return "The module joined " + req.HomeSsid + " but its data is not reaching the PC. "
                     + "Check the Windows Firewall rule for UDP 30100 on this network, "
                     + "and whether the access point blocks traffic between clients.";
            }
            if (html.IndexOf("Not connected", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                outcome = ProvisionOutcome.NotConnected;

                // Press the portal's own "Retry Connection Now" before leaving. Two
                // reasons, and the second is the important one:
                //  - it forces an immediate full scan instead of waiting out a backoff
                //    that may have stretched to 60 s while the hotspot was dormant;
                //  - loading any portal page called NotePortalRequest(), which suspends
                //    ordinary retries for PortalActiveMs (60 s). So having come here to
                //    ask, we have just delayed the very thing we are waiting for. The
                //    forced retry is the only kind that ignores that suppression.
                string ignored;
                if (HttpGet("http://" + gw + "/wifi?retry=1", HttpTimeoutMs, out ignored, out error))
                    nudged = true;

                // Leading advice is to get off the module's hotspot, because that is what
                // actually blocks it. One radio serves both the hotspot and the search,
                // so with a client associated and no channel cached — which is the state
                // a firmware update leaves behind — ServiceWifiStation() rations full
                // scans to one every five minutes (StaRetryScanOnlyMs). Measured in the
                // field 2026-09-25: the module found the network the moment the PC left
                // its hotspot. The forced retry above is the same remedy applied from
                // here, since it bypasses that interval.
                return "The module has the credentials but has not joined " + req.HomeSsid + "."
                     + Environment.NewLine + Environment.NewLine
                     + "The module uses one radio for its own hotspot and for finding the network, "
                     + "so it searches slowly while anything is connected to its hotspot. Stay off "
                     + "the module's hotspot and give it a minute."
                     + Environment.NewLine + Environment.NewLine
                     + "Otherwise check the password, and that the network is in range of the "
                     + "module. A network with no 2.4 GHz band cannot be joined at all.";
            }
            return "the module gave no clear status";
        }

        // ── HTTP ──────────────────────────────────────────────────────────────
        // Proxy is set to null on purpose. A system proxy configured for the office
        // network would otherwise be used for a request to 192.168.x.1, and the call
        // fails or stalls for no visible reason.

        private static bool HttpGet(string url, int timeoutMs, out string body, out string error)
        {
            body = ""; error = "";
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Proxy = null;
                req.KeepAlive = false;
                req.Timeout = timeoutMs;
                req.ReadWriteTimeout = timeoutMs;
                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var sr = new StreamReader(resp.GetResponseStream()))
                    body = sr.ReadToEnd();
                return true;
            }
            catch (Exception ex) { error = ex.Message; return false; }
        }

        private static bool HttpPost(string url, string form, int timeoutMs, out string error)
        {
            error = "";
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Proxy = null;
                req.KeepAlive = false;
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Timeout = timeoutMs;
                req.ReadWriteTimeout = timeoutMs;

                byte[] data = Encoding.UTF8.GetBytes(form);
                req.ContentLength = data.Length;
                using (var s = req.GetRequestStream()) s.Write(data, 0, data.Length);

                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var sr = new StreamReader(resp.GetResponseStream()))
                    sr.ReadToEnd();
                return true;
            }
            catch (Exception ex) { error = ex.Message; return false; }
        }
    }
}
