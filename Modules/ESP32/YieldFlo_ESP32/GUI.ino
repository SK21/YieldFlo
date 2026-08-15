void HandleRoot()
{
    // Only a genuine page load means the user is standing at the portal, and
    // only that should suspend the station retry (see Wifi.ino). This is also
    // the onNotFound handler, so it sees traffic nobody asked for: Windows
    // polls /connecttest.txt for as long as a PC sits on the hotspot, and
    // counting that would suspend retries forever. The settings form posts to
    // '/' as well, so one check covers both the page and the submit.
    if (server.uri() == "/") NotePortalRequest();

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
    // WiFi credentials, the station tick and the hotspot password are NOT read
    // here — they live on /wifi (PgWifi.ino) and are not fields of this form.
    // Reading them here anyway would be silently destructive: server.hasArg()
    // returns false for an absent field, so every save from this page would
    // clear the user's "Use this Network" setting and blank the SSID.
    uint8_t oldCommMode = MDL.CommMode;
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

    // Optical sensor polarity (PNP = FarmTrx, NPN = inverted)
    bool oldInvert = MDL.InvertSensor;
    String polarity = server.arg("polarity");
    polarity.trim();
    MDL.InvertSensor = (polarity == "npn");

    server.send(200, "text/html", GetPageMain());

    bool changed =
        (MDL.CommMode          != oldCommMode) ||
        (MDL.UseCompSignal     != oldUseComp)  ||
        (MDL.InvertSensor      != oldInvert)   ||
        (MDL.EthIP0 != oldE0) || (MDL.EthIP1 != oldE1) || (MDL.EthIP2 != oldE2);

    if (changed)
    {
        SaveData();   // writes EEPROM then calls ESP.restart()
    }
}
