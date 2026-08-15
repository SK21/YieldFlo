// WiFi settings page. Separate from the main page for a structural reason
// rather than a cosmetic one: the main page is a single form that saves and
// reboots on submit, so a scan/pick/scan-again loop cannot live on it without
// either rebooting the module or discarding the user's unsaved edits.
//
// Everything WiFi-related lives here — station credentials, the "Use this
// Network" tick, the hotspot password, the network scan, and the manual retry.

// An SSID is text chosen by whoever owns the other access point, and it lands in
// our HTML and our URLs. Escape it in both places rather than trusting it.
static String HtmlEscape(const String& s)
{
    String out;
    out.reserve(s.length() + 8);
    for (unsigned int i = 0; i < s.length(); i++)
    {
        char c = s[i];
        switch (c)
        {
        case '&':  out += "&amp;";  break;
        case '<':  out += "&lt;";   break;
        case '>':  out += "&gt;";   break;
        case '"':  out += "&quot;"; break;
        case '\'': out += "&#39;";  break;
        default:   out += c;        break;
        }
    }
    return out;
}

static String UrlEncode(const String& s)
{
    String out;
    out.reserve(s.length() + 8);
    for (unsigned int i = 0; i < s.length(); i++)
    {
        char c = s[i];
        if (isalnum((unsigned char)c) || c == '-' || c == '_' || c == '.' || c == '~')
        {
            out += c;
        }
        else
        {
            char b[4];
            sprintf(b, "%%%02X", (unsigned char)c);
            out += b;
        }
    }
    return out;
}

// Shown while an async scan is in flight. A meta refresh rather than AJAX keeps
// this page in the same server-rendered style as the rest of the portal.
String GetPageScanning()
{
    String st = "<HTML><head>";
    st += "<META content='text/html; charset=utf-8' http-equiv=Content-Type>";
    st += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
    st += "<meta http-equiv='refresh' content='4;url=/wifi'>";
    st += "<title>Scanning</title>";
    st += GetPageStyle();
    st += "</head><BODY>";
    st += "<h1>Scanning</h1>";
    st += "<p class='status'>Looking for networks.</p>";
    st += "<p class='status'>The hotspot pauses for a few seconds while the radio scans.</p>";
    st += "<p><a href='/wifi'>Continue</a></p>";
    st += "</BODY></HTML>";
    return st;
}

// The results of the last completed scan, strongest first, one row per network
// name. Duplicates are collapsed because a mesh or a repeater lists the same
// name from every radio it has, which is noise to someone picking a network.
static String ScanResultsHtml()
{
    int16_t n = WiFi.scanComplete();

    if (n == WIFI_SCAN_RUNNING)
    {
        String st = "<meta http-equiv='refresh' content='3;url=/wifi'>";
        st += "<p class='status'>Scanning ...</p>";
        return st;
    }

    if (n < 0) return "";		// no scan has been run this session
    if (n == 0) return "<p class='status'>No networks found.</p>";

    String st = "<h1 class='subhead'>Networks Found</h1>";
    st += "<table class='nets'>";

    for (int16_t i = 0; i < n; i++)
    {
        String ssid = WiFi.SSID(i);
        if (ssid.length() == 0) continue;		// hidden network — no name to offer

        // Keep only the strongest entry for each name. The tie-break on index
        // guarantees exactly one survivor rather than none or both.
        bool weaker = false;
        for (int16_t j = 0; j < n; j++)
        {
            if (j == i) continue;
            if (WiFi.SSID(j) != ssid) continue;
            if (WiFi.RSSI(j) > WiFi.RSSI(i) || (WiFi.RSSI(j) == WiFi.RSSI(i) && j < i))
            {
                weaker = true;
                break;
            }
        }
        if (weaker) continue;

        int32_t rssi = WiFi.RSSI(i);
        const char* bars = (rssi >= -55) ? "||||" : (rssi >= -65) ? "|||" : (rssi >= -75) ? "||" : "|";
        bool locked = (WiFi.encryptionType(i) != WIFI_AUTH_OPEN);

        st += "<tr><td><a href='/wifi?pick=" + UrlEncode(ssid);
        st += "&ch=" + String(WiFi.channel(i)) + "'>";
        st += HtmlEscape(ssid);
        st += "</a></td>";
        st += "<td class='sig'>";
        if (locked) st += "&#128274; ";		// padlock
        st += String(bars) + " " + String(rssi) + " dBm ch" + String(WiFi.channel(i));
        st += "</td></tr>";
    }

    st += "</table>";
    st += "<p class='hint'>Tap a network to fill in its name, then enter the password below.</p>";
    return st;
}

String GetPageWifi()
{
    // A picked network arrives in the query string rather than being staged on
    // the module, so two browser tabs cannot overwrite each other's choice.
    String pickSSID = server.hasArg("pick") ? server.arg("pick") : "";
    String pickCh   = server.hasArg("ch")   ? server.arg("ch")   : "";
    String ssidValue = pickSSID.length() ? pickSSID : String(MDL.SSID);

    String st = "<HTML>";
    st += "<head>";
    st += "<META content='text/html; charset=utf-8' http-equiv=Content-Type>";
    st += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
    st += "<title>YieldFlo WiFi</title>";
    st += GetPageStyle();
    st += "</head>";
    st += "<BODY>";
    st += "<h1>WiFi Network</h1>";

    // Status first — it is the reason most people open this page.
    if (WiFi.isConnected())
    {
        st += "<p class='status'>Connected to " + HtmlEscape(String(MDL.SSID));
        st += " (" + WiFi.localIP().toString() + ") on channel " + String(WiFi.channel()) + "</p>";
    }
    else if (MDL.WifiModeUseStation && StaAuthRejected())
    {
        // Name the actual fault. "Not connected" otherwise covers a wrong
        // password, an absent router and a dead aerial alike, and only one of
        // those is fixed on this page.
        st += "<p class='status'>Password refused by " + HtmlEscape(String(MDL.SSID)) + ".</p>";
        st += "<p class='hint'>Check the password below, then Save. Still trying every 10 minutes.</p>";
    }
    else if (MDL.WifiModeUseStation)
    {
        st += "<p class='status'>Not connected — retrying in the background.</p>";
    }
    else
    {
        st += "<p class='status'>Hotspot only. Tick Use this Network to join a network.</p>";
    }

    st += "<form method=post action='/wifi'>";
    st += "<table class='center'>";

    st += "<tr>";
    st += "  <td class='label-col'>Network</td>";
    st += "  <td class='input-col'><div class='control-width'><input class='InputCell' name='prop1' value='" + HtmlEscape(ssidValue) + "'></div></td>";
    st += "</tr>";
    st += "<tr>";
    st += "  <td class='label-col'>Password</td>";
    st += "  <td class='input-col'><div class='control-width'><input class='InputCell' name='prop2' value='" + HtmlEscape(String(MDL.Password)) + "'></div></td>";
    st += "</tr>";
    st += "<tr>";
    st += "  <td class='label-col'>Use this Network</td>";
    st += "  <td class='input-col'><div class='control-width'><div class='checkbox-row'>";
    st += "    <input class='styled' type='checkbox' name='connect' value='1'" + String(MDL.WifiModeUseStation ? " checked" : "") + ">";
    st += "  </div></div></td>";
    st += "</tr>";
    st += "<tr><td colspan='2'><div class='control-width'><div class='hint'>Stays on until you turn it off. The module keeps its own hotspot either way.</div></div></td></tr>";

    st += "<tr><td colspan='2'><hr></td></tr>";

    st += "<tr><td colspan='2' style='text-align:center; padding:0;'><h1 class='subhead'>Hotspot</h1></td></tr>";
    st += "<tr>";
    st += "  <td class='label-col'>Password</td>";
    st += "  <td class='input-col'><div class='control-width'><input class='InputCell' name='prop3' value='" + HtmlEscape(String(MDL.APpassword)) + "'></div></td>";
    st += "</tr>";
    st += "<tr><td colspan='2'><div class='control-width'><div class='hint'>Module access point. Use 8-10 characters. Leave empty for an open hotspot.</div></div></td></tr>";

    st += "</table>";

    // Carried through the submit so the channel can be cached before the reboot,
    // making the first join a directed attempt instead of a full scan. Paired
    // with the name so an edited SSID does not inherit the wrong channel.
    st += "<input type='hidden' name='pickssid' value='" + HtmlEscape(pickSSID) + "'>";
    st += "<input type='hidden' name='pickch' value='" + HtmlEscape(pickCh) + "'>";

    st += "<p><div class='control-width'><input class='button-72' type='submit' value='Save / Restart'></div></p>";
    st += "</form>";

    // Outside the form: these navigate rather than submit, so they cannot
    // trigger the save-and-reboot path.
    st += "<p><a class='button-72' href='/wifi?scan=1'>Scan for Networks</a></p>";
    if (MDL.WifiModeUseStation && !WiFi.isConnected())
        st += "<p><a class='button-72' href='/wifi?retry=1'>Retry Connection Now</a></p>";

    st += ScanResultsHtml();

    st += "<p><a href='/'>Back</a></p>";
    st += "</BODY></HTML>";
    return st;
}

void HandleWifiSettings()
{
    String oldSSID     = String(MDL.SSID);
    String oldPassword = String(MDL.Password);
    String oldAPpw     = String(MDL.APpassword);
    bool   oldStation  = MDL.WifiModeUseStation;
    uint8_t oldChannel = MDL.StaChannelCache;

    String newSSID = server.arg("prop1");
    newSSID.trim();
    String newPassword = server.arg("prop2");
    newPassword.trim();
    newSSID.toCharArray(MDL.SSID, sizeof(MDL.SSID));
    newPassword.toCharArray(MDL.Password, sizeof(MDL.Password));

    MDL.WifiModeUseStation = server.hasArg("connect");

    // AP / Hotspot password (prop3 — may be empty for open network)
    if (server.hasArg("prop3"))
    {
        String newAPpw = server.arg("prop3");
        newAPpw.trim();
        const size_t kMaxApLen = 10;
        if (newAPpw.length() > kMaxApLen) newAPpw.remove(kMaxApLen);
        newAPpw.toCharArray(MDL.APpassword, sizeof(MDL.APpassword));
    }

    // Channel cache. The scan already knows where the chosen network lives, so
    // priming it here makes the first join after the reboot a directed attempt.
    // Only honoured when the submitted name still matches the one that was
    // tapped — otherwise the user typed something else and the channel is not
    // theirs. A changed network with no pick clears the cache rather than
    // pointing the next attempt at the old network's channel.
    if (server.hasArg("pickch") && server.arg("pickssid") == newSSID && newSSID.length())
    {
        int ch = server.arg("pickch").toInt();
        if (ch >= 1 && ch <= 13) MDL.StaChannelCache = (uint8_t)ch;
    }
    else if (newSSID != oldSSID)
    {
        MDL.StaChannelCache = 0;
    }

    server.send(200, "text/html", GetPageWifi());

    bool changed =
        (MDL.WifiModeUseStation != oldStation)  ||
        (String(MDL.SSID)       != oldSSID)     ||
        (String(MDL.Password)   != oldPassword) ||
        (String(MDL.APpassword) != oldAPpw)     ||
        (MDL.StaChannelCache    != oldChannel);

    if (changed)
    {
        SaveData();   // writes EEPROM then calls ESP.restart()
    }
}

void HandleWifiPage()
{
    NotePortalRequest();

    if (server.hasArg("prop1"))		// form submitted
    {
        HandleWifiSettings();
        return;
    }

    if (server.hasArg("scan"))
    {
        // Async. A blocking scan stalls loop() for ~2 s — no web server, no DNS,
        // no UDP — while trying to serve this very response.
        if (WiFi.scanComplete() != WIFI_SCAN_RUNNING) WiFi.scanNetworks(true);
        server.send(200, "text/html", GetPageScanning());
        return;
    }

    if (server.hasArg("retry"))
    {
        RequestNetworkRetry();
        server.sendHeader("Location", "/wifi");
        server.send(303, "text/plain", "");
        return;
    }

    server.send(200, "text/html", GetPageWifi());
}
