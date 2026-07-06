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
    uint8_t oldCommMode = MDL.CommMode;
    String oldAPpw     = String(MDL.APpassword);
    bool   oldUseComp  = MDL.UseCompSignal;
    uint8_t oldE0 = MDL.EthIP0, oldE1 = MDL.EthIP1, oldE2 = MDL.EthIP2;

    // Comm mode
    String commmode = server.arg("commmode");
    commmode.trim();
    MDL.CommMode = CommModeWifi;
    if (commmode == "can") MDL.CommMode = CommModeCan;
    else if (commmode == "eth") MDL.CommMode = CommModeEth;

    // Ethernet subnet ("192.168.1" — module gets .50+ID on it)
    if (server.hasArg("ethsubnet"))
    {
        String sn = server.arg("ethsubnet");
        sn.trim();
        int a, b, c;
        if (sscanf(sn.c_str(), "%d.%d.%d", &a, &b, &c) == 3 &&
            a >= 0 && a <= 255 && b >= 0 && b <= 255 && c >= 0 && c <= 255)
        {
            MDL.EthIP0 = (uint8_t)a;
            MDL.EthIP1 = (uint8_t)b;
            MDL.EthIP2 = (uint8_t)c;
        }
    }

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
        (MDL.CommMode          != oldCommMode) ||
        (MDL.UseCompSignal     != oldUseComp)  ||
        (MDL.WifiModeUseStation != oldStation)  ||
        (String(MDL.SSID)       != oldSSID)     ||
        (String(MDL.Password)   != oldPassword) ||
        (String(MDL.APpassword) != oldAPpw)     ||
        (MDL.EthIP0 != oldE0) || (MDL.EthIP1 != oldE1) || (MDL.EthIP2 != oldE2);

    if (changed)
    {
        SaveData();   // writes EEPROM then calls ESP.restart()
    }
}
