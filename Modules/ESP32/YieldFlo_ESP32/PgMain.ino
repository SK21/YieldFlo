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
    st += "<style>";
    st += "html { font-family: Helvetica, Arial, sans-serif; display:inline-block; margin:0 auto; text-align:center; }";
    st += "body { margin-top:50px; background-color:wheat; }";
    st += "h1 { color:#444; margin:50px auto 12px; text-decoration:underline; }";
    st += "h1.subhead { margin:20px auto 12px; }";
    st += "table.center { margin-left:auto; margin-right:auto; border-collapse:collapse; table-layout:fixed; }";
    st += "td.label-col { width:200px; text-align:left; padding:8px 12px; vertical-align:middle; }";
    st += "td.input-col { width:320px; padding:8px 12px; vertical-align:middle; }";
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
    st += ".radio-row { display:flex; align-items:center; gap:16px; height:44px; }";
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
    st += "</style>";
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
    st += "    <label><input class='styled' type='radio' name='commmode' value='wifi'" + String(MDL.UseCanComm ? "" : " checked") + "> WiFi</label>";
    st += "    <label><input class='styled' type='radio' name='commmode' value='can'"  + String(MDL.UseCanComm ? " checked" : "") + "> CAN</label>";
    st += "  </div></div></td>";
    st += "</tr>";

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
    st += "<tr><td colspan='2'><div class='control-width'><div class='hint'>Main only: complementary wire not connected, noise rejection disabled.</div></div></td></tr>";

    // Divider
    st += "<tr><td colspan='2'><hr></td></tr>";

    // WiFi station settings
    st += "<tr><td colspan='2' style='text-align:center; padding:0;'><h1 class='subhead'>WiFi Network</h1></td></tr>";
    st += "<tr>";
    st += "  <td class='label-col'>Network</td>";
    st += "  <td class='input-col'><div class='control-width'><input class='InputCell' name='prop1' value='" + String(MDL.SSID) + "'></div></td>";
    st += "</tr>";
    st += "<tr>";
    st += "  <td class='label-col'>Password</td>";
    st += "  <td class='input-col'><div class='control-width'><input class='InputCell' name='prop2' value='" + String(MDL.Password) + "'></div></td>";
    st += "</tr>";
    st += "<tr>";
    st += "  <td class='label-col'>Use this Network</td>";
    st += "  <td class='input-col'><div class='control-width'><div class='checkbox-row'>";
    st += "    <input class='styled' type='checkbox' name='connect' value='1'" + String(MDL.WifiModeUseStation ? " checked" : "") + ">";
    st += "  </div></div></td>";
    st += "</tr>";
    st += "<tr><td colspan='2' style='text-align:center;'>";
    if (WiFi.isConnected())
        st += "<div class='status'>Connected to " + String(MDL.SSID) + " (" + WiFi.localIP().toString() + ")</div>";
    else
        st += "<div class='status'>Not connected to network</div>";
    st += "</td></tr>";

    // Divider
    st += "<tr><td colspan='2'><hr></td></tr>";

    // AP / Hotspot password
    st += "<tr><td colspan='2' style='text-align:center; padding:0;'><h1 class='subhead'>Hotspot</h1></td></tr>";
    st += "<tr>";
    st += "  <td class='label-col'>Password</td>";
    st += "  <td class='input-col'><div class='control-width'><input class='InputCell' name='prop3' value='" + String(MDL.APpassword) + "'></div></td>";
    st += "</tr>";
    st += "<tr><td colspan='2'><div class='control-width'><div class='hint'>Module access point. Use 8–10 characters. Leave empty for an open hotspot.</div></div></td></tr>";

    st += "</table>";
    st += "<p><div class='control-width'><input class='button-72' type='submit' value='Save / Restart'></div></p>";
    st += "<p><a href='/update'>Update Firmware</a></p>";
    st += "</form>";
    st += "</BODY></HTML>";

    return st;
}
