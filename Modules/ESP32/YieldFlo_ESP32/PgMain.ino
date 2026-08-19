// Shared by GetPageMain() and GetPageWifi() so the two cannot drift apart.
// PgUpdate.ino deliberately keeps its own copy — it is laid out differently and
// carries the upload progress bar.
String GetPageStyle()
{
    String st = "<style>";
    st += "html { font-family: Helvetica, Arial, sans-serif; display:inline-block; margin:0 auto; text-align:center; }";
    st += "body { margin-top:50px; background-color:wheat; }";
    st += "h1 { color:#444; margin:50px auto 12px; text-decoration:underline; }";
    st += "h1.subhead { margin:20px auto 12px; }";
    // Proportional columns, not the 200px + 320px they used to be. That came to
    // 568px with the padding, against a 360-412px phone viewport — a browser you
    // can pinch-zoom hides it, the captive-portal mini browser cannot, and the
    // right-hand column was simply clipped. The max-width keeps the old
    // appearance on anything wide enough to have shown it correctly before.
    st += "table.center { margin-left:auto; margin-right:auto; border-collapse:collapse; table-layout:fixed; width:100%; max-width:568px; }";
    st += "td.label-col { width:40%; text-align:left; padding:8px 12px; vertical-align:middle; }";
    st += "td.input-col { width:60%; padding:8px 12px; vertical-align:middle; }";
    st += ".control-width { width:320px; max-width:90%; margin:0 auto; box-sizing:border-box; }";
    st += ".InputCell { display:block; width:100%; height:36px; box-sizing:border-box; text-align:center; font-size:18px; font-weight:700; padding:4px 6px; }";
    st += ".button-72 { align-items:center; background-color:initial;";
    st += "  background-image:linear-gradient(rgba(179,132,201,.84),rgba(57,31,91,.84) 50%);";
    st += "  border-radius:42px; border-width:0;";
    st += "  box-shadow:rgba(57,31,91,0.24) 0 2px 2px, rgba(179,132,201,0.4) 0 8px 12px;";
    st += "  color:#FFF; cursor:pointer; display:inline-flex; font-size:18px; font-weight:700;";
    st += "  justify-content:center; letter-spacing:.04em; line-height:16px;";
    st += "  margin:12px auto; padding:12px 18px; text-align:center; text-decoration:none;";
    st += "  user-select:none; touch-action:manipulation; width:320px; max-width:90%; }";
    // Wraps because the three comm-mode radios come to ~310px, which no longer
    // fits the input column once it is a proportion of a phone screen.
    st += ".radio-row { display:flex; flex-wrap:wrap; align-items:center; gap:16px; min-height:44px; }";
    st += ".radio-row label { font-size:18px; font-weight:700; display:flex; align-items:center; gap:6px; cursor:pointer; }";
    st += ".checkbox-row { display:flex; align-items:center; height:44px; }";
    st += "input[type=checkbox].styled, input[type=radio].styled {";
    st += "  -webkit-appearance:none; appearance:none; width:44px; height:44px; display:inline-block;";
    st += "  position:relative; margin:0; padding:0; box-sizing:border-box; border-radius:10px;";
    st += "  background-image:linear-gradient(rgba(179,132,201,.84),rgba(57,31,91,.84) 50%);";
    st += "  box-shadow:rgba(57,31,91,0.24) 0 2px 2px, rgba(179,132,201,0.4) 0 8px 12px;";
    st += "  cursor:pointer; outline:none; border:1px solid rgba(57,31,91,0.25); }";
    st += "input[type=checkbox].styled::after, input[type=radio].styled::after {";
    st += "  content:''; position:absolute; left:50%; top:50%;";
    st += "  width:12px; height:22px;";
    st += "  border-right:4px solid white; border-bottom:4px solid white;";
    st += "  transform:translate(-50%,-60%) rotate(45deg) scale(0);";
    st += "  transform-origin:center; transition:transform 0.12s ease-in-out; border-radius:2px; }";
    st += "input[type=checkbox].styled:checked::after, input[type=radio].styled:checked::after { transform:translate(-50%,-60%) rotate(45deg) scale(1); }";
    st += ".hint { font-size:12px; color:#333; margin-top:4px; }";
    st += ".status { margin:2px auto 16px; font-size:16px; }";
    st += "a:link { font-size:150%; }";
    // Network scan results — rows are links, sized for a gloved finger.
    st += "table.nets { margin:0 auto; border-collapse:collapse; width:320px; max-width:90%; }";
    st += "table.nets td { padding:10px 8px; border-bottom:1px solid rgba(57,31,91,0.2); text-align:left; font-size:16px; }";
    st += "table.nets td.sig { text-align:right; white-space:nowrap; color:#333; font-size:14px; }";
    st += "table.nets a { font-size:100%; font-weight:700; text-decoration:none; color:#391f5b; }";
    st += "</style>";
    return st;
}

String GetPageMain()
{
    // Decode firmware version from InoID (DDMMY format)
    uint16_t yr   = InoID % 10 + 2020;
    uint16_t rest = InoID / 10;
    uint8_t  mn   = rest % 100;
    uint16_t dy   = rest / 100;
    String fwVer = "v" + String(yr) + ".";
    if (mn < 10) fwVer += "0";
    fwVer += String(mn) + ".";
    if (dy < 10) fwVer += "0";
    fwVer += String(dy);

    String st = "<HTML>";
    st += "<head>";
    st += "<META content='text/html; charset=utf-8' http-equiv=Content-Type>";
    st += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
    st += "<title>YieldFlo Module</title>";
    st += GetPageStyle();
    st += "</head>";
    st += "<BODY>";
    st += "<h1>YieldFlo Module</h1>";
    st += "<p class='status'>" + fwVer + "</p>";

    st += "<form id=FORM1 method=post action='/'>";
    st += "<table class='center'>";

    // Comm mode
    st += "<tr><td colspan='2' style='text-align:center; padding:0;'><h1 class='subhead'>Communication</h1></td></tr>";
    st += "<tr>";
    st += "  <td class='label-col'>Mode</td>";
    st += "  <td class='input-col'><div class='control-width'><div class='radio-row'>";
    st += "    <label><input class='styled' type='radio' name='commmode' value='wifi'" + String(MDL.CommMode == CommModeWifi ? " checked" : "") + "> WiFi</label>";
    st += "    <label><input class='styled' type='radio' name='commmode' value='can'"  + String(MDL.CommMode == CommModeCan  ? " checked" : "") + "> CAN</label>";
    st += "    <label><input class='styled' type='radio' name='commmode' value='eth'"  + String(MDL.CommMode == CommModeEth  ? " checked" : "") + "> Ethernet</label>";
    st += "  </div></div></td>";
    st += "</tr>";

    // Ethernet settings
    String ethSubnet = String(MDL.EthIP0) + "." + String(MDL.EthIP1) + "." + String(MDL.EthIP2);
    st += "<tr>";
    st += "  <td class='label-col'>Ethernet subnet</td>";
    st += "  <td class='input-col'><div class='control-width'><input class='InputCell' name='ethsubnet' value='" + ethSubnet + "'></div></td>";
    st += "</tr>";
    st += "<tr><td colspan='2'><div class='control-width'><div class='hint'>Module IP: " + ethSubnet + "." + String(50 + MDL.ID) + " — the PC's wired adapter must be on the same subnet.</div></div></td></tr>";
    if (MDL.CommMode == CommModeEth)
    {
        st += "<tr><td colspan='2' style='text-align:center;'>";
        if (!EthChipFound)
            st += "<div class='status'>Ethernet hardware (W5500) not found</div>";
        else if (Ethernet.linkStatus() == LinkON)
            st += "<div class='status'>Ethernet connected (" + Ethernet.localIP().toString() + ")</div>";
        else
            st += "<div class='status'>Ethernet cable not connected</div>";
        st += "</td></tr>";
    }

    // Divider
    st += "<tr><td colspan='2'><hr></td></tr>";

    // Optical sensor signal mode
    st += "<tr><td colspan='2' style='text-align:center; padding:0;'><h1 class='subhead'>Optical Sensor</h1></td></tr>";
    st += "<tr>";
    st += "  <td class='label-col'>Signals</td>";
    st += "  <td class='input-col'><div class='control-width'><div class='radio-row'>";
    st += "    <label><input class='styled' type='radio' name='sensormode' value='both'" + String(MDL.UseCompSignal ? " checked" : "") + "> Main + Comp</label>";
    st += "    <label><input class='styled' type='radio' name='sensormode' value='main'" + String(MDL.UseCompSignal ? "" : " checked") + "> Main only</label>";
    st += "  </div></div></td>";
    st += "</tr>";
    st += "<tr><td colspan='2'><div class='control-width'><div class='hint'>Main only: complementary wire not connected. Choose it unless Comp is definitely wired &mdash; Main + Comp without it discards every paddle and measures nothing.</div></div></td></tr>";
    st += "<tr>";
    st += "  <td class='label-col'>Polarity</td>";
    st += "  <td class='input-col'><div class='control-width'><div class='radio-row'>";
    st += "    <label><input class='styled' type='radio' name='polarity' value='pnp'" + String(MDL.InvertSensor ? "" : " checked") + "> PNP</label>";
    st += "    <label><input class='styled' type='radio' name='polarity' value='npn'" + String(MDL.InvertSensor ? " checked" : "") + "> NPN</label>";
    st += "  </div></div></td>";
    st += "</tr>";
    st += "<tr><td colspan='2'><div class='control-width'><div class='hint'>PNP: output HIGH with beam clear. NPN: inverted — select NPN if flow reads high with no grain.</div></div></td></tr>";

    st += "</table>";
    st += "<p><div class='control-width'><input class='button-72' type='submit' value='Save / Restart'></div></p>";
    st += "</form>";

    // Everything WiFi lives on its own page — see PgWifi.ino for why.
    st += "<p><a href='/wifi'>WiFi Network</a></p>";
    if (WiFi.isConnected())
        st += "<p class='status'>Connected to " + String(MDL.SSID) + " (" + WiFi.localIP().toString() + ")</p>";
    else if (MDL.WifiModeUseStation)
        st += "<p class='status'>Network not connected</p>";
    st += "<p><a href='/update'>Update Firmware</a></p>";
    st += "</BODY></HTML>";

    return st;
}
