void HandleRoot()
{
    if (server.hasArg("commmode"))
    {
        handleSettings();
    }
    else
    {
        server.send(200, "text/html", GetPageMain());
    }
}

void handleSettings()
{
    String oldSSID     = String(MDL.SSID);
    String oldPassword = String(MDL.Password);
    bool   oldStation  = MDL.WifiModeUseStation;
    bool   oldCanComm  = MDL.UseCanComm;
    String oldAPpw     = String(MDL.APpassword);
    bool   oldUseComp  = MDL.UseCompSignal;

    // Comm mode
    String commmode = server.arg("commmode");
    commmode.trim();
    MDL.UseCanComm = (commmode == "can");

    // Optical sensor signal mode
    String sensormode = server.arg("sensormode");
    sensormode.trim();
    MDL.UseCompSignal = (sensormode != "main");

    // WiFi station credentials
    String newSSID = server.arg("prop1");
    newSSID.trim();
    String newPassword = server.arg("prop2");
    newPassword.trim();
    newSSID.toCharArray(MDL.SSID, sizeof(MDL.SSID));
    newPassword.toCharArray(MDL.Password, sizeof(MDL.Password));

    MDL.WifiModeUseStation = server.hasArg("connect");

    // AP / Hotspot password (prop3 — may be empty for open network)
    String newAPpw = oldAPpw;
    if (server.hasArg("prop3"))
    {
        newAPpw = server.arg("prop3");
        newAPpw.trim();
        const size_t kMaxApLen = 10;
        if (newAPpw.length() > kMaxApLen) newAPpw.remove(kMaxApLen);
        newAPpw.toCharArray(MDL.APpassword, sizeof(MDL.APpassword));
    }

    server.send(200, "text/html", GetPageMain());

    bool changed =
        (MDL.UseCanComm        != oldCanComm)  ||
        (MDL.UseCompSignal     != oldUseComp)  ||
        (MDL.WifiModeUseStation != oldStation)  ||
        (String(MDL.SSID)       != oldSSID)     ||
        (String(MDL.Password)   != oldPassword) ||
        (String(MDL.APpassword) != oldAPpw);

    if (changed)
    {
        SaveData();   // writes EEPROM then calls ESP.restart()
    }
}
