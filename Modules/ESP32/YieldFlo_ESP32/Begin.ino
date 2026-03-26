
void DoSetup()
{
	uint8_t ErrorCount;
	Serial.begin(38400);
	delay(5000);
	Serial.println();
	Serial.println();
	Serial.println(InoDescription);
	Serial.println();

	EEPROM.begin(256);
	LoadData();


	Serial.println("");
	Serial.println(InoDescription);

	// version
	uint16_t yr = InoID % 10 + 2020;
	uint16_t rest = InoID / 10;
	uint8_t mn = rest % 100;
	uint16_t dy = rest / 100;

	String fwVer;
	if (mn <= 12 && dy <= 31)
	{
		fwVer = "Firmware Version: v";
		fwVer += String(yr);
		fwVer += ".";
		if (mn < 10) fwVer += "0";
		fwVer += String(mn);
		fwVer += ".";
		if (dy < 10) fwVer += "0";
		fwVer += String(dy);
	}
	else
	{
		fwVer = "Firmware Version: invalid";
	}
	Serial.println(fwVer);

	Serial.print("Module ID: ");
	Serial.println(MDL.ID);
	Serial.println("");

	// I2C
	Wire.begin();			// I2C on pins SCL 22, SDA 21
	Wire.setClock(400000);	//Increase I2C data rate to 400kHz

	// ADS1115
	if (MDL.ADS1115Enabled)
	{
		Serial.print("Starting ADS1115 at address ");
		Serial.println(ADS1115_Address);
		while (!ADSfound)
		{
			Wire.beginTransmission(ADS1115_Address);
			Wire.write(0b00000000);	//Point to Conversion register
			Wire.endTransmission();
			ADSfound = (Wire.requestFrom(ADS1115_Address, 2) == 2);
			Serial.print(".");
			delay(500);
			if (ErrorCount++ > 10) break;
		}
		Serial.println("");
		if (ADSfound)
		{
			Serial.println("ADS1115 found.");
			Serial.println("");

			// Configure ALERT/RDY pin for conversion-ready mode:
			// Set Hi_thresh MSB=1 and Lo_thresh MSB=0 — this puts ALERT/RDY
			// into conversion-ready mode regardless of comparator settings.
			Wire.beginTransmission(ADS1115_Address);
			Wire.write(0x02);		// Hi_thresh register
			Wire.write(0x80);		// 0x8000 MSB
			Wire.write(0x00);
			Wire.endTransmission();

			Wire.beginTransmission(ADS1115_Address);
			Wire.write(0x03);		// Lo_thresh register
			Wire.write(0x00);		// 0x0000
			Wire.write(0x00);
			Wire.endTransmission();

			// Attach interrupt — ALERT/RDY is open-drain active-low
			pinMode(MDL.AlertPin, INPUT_PULLUP);
			attachInterrupt(digitalPinToInterrupt(MDL.AlertPin), onADSReady, FALLING);

			// Start first conversion — moisture (AIN0-AIN1 differential, PGA=±4.096V, 16 SPS)
			Wire.beginTransmission(ADS1115_Address);
			Wire.write(0x01);		// Config register
			Wire.write(0b10000011);	// OS=1, MUX=000 (AIN0-AIN1 diff), PGA=001 (4.096V), MODE=1
			Wire.write(0b00100000);	// DR=001 (16 SPS), COMP_QUE=00
			Wire.endTransmission();
		}
		else
		{
			Serial.println("ADS1115 not found.");
			Serial.println("ADS1115 disabled.");
			Serial.println("");
		}
	}

	// Optical sensor
	Serial.print("Starting optical sensor ... ");
	if (MDL.MainPin < NC && MDL.CompPin < NC)
	{
		pinMode(MDL.MainPin, INPUT);
		pinMode(MDL.CompPin, INPUT);
		attachInterrupt(digitalPinToInterrupt(MDL.MainPin), onSensorEdge, CHANGE);
		attachInterrupt(digitalPinToInterrupt(MDL.CompPin), onSensorEdge, CHANGE);
		BeamBlocked = (digitalRead(MDL.MainPin) == LOW);	// HIGH = clear, LOW = blocked
		LastEdgeUs = micros();
		Serial.println("OK.");
	}
	else
	{
		Serial.println("pins not configured.");
	}

	// RPM sensor
	if (MDL.RPMpin < NC)
	{
		Serial.print("Starting RPM sensor ... ");
		pinMode(MDL.RPMpin, INPUT);
		attachInterrupt(digitalPinToInterrupt(MDL.RPMpin), onRPMedge, RISING);
		LastRPMedgeUs = micros();
		Serial.println("OK.");
	}

	// Wifi
	WiFi.mode(WIFI_MODE_APSTA);
	WiFi.disconnect(true);

	// Access Point
	Wifi_DestinationIP = IPAddress(192, 168, MDL.ID + 200, 255);
	IPAddress AP_LocalIP = IPAddress(192, 168, MDL.ID + 200, 1);
	IPAddress AP_GateWay = AP_LocalIP;
	IPAddress AP_Subnet(255, 255, 255, 0);

	uint64_t mac = ESP.getEfuseMac();
	uint32_t low32 = (uint32_t)(mac & 0xFFFFFFFF);

	char suffix[9]; // 8 hex + null
	sprintf(suffix, "%08X", low32);

	String AP = MDL.APname;
	AP += "_";
	AP += suffix;

	WiFi.softAPConfig(AP_LocalIP, AP_GateWay, AP_Subnet);
	if (strlen(MDL.APpassword) >= 8)
	{
		// WPA2-PSK
		WiFi.softAP(AP.c_str(), MDL.APpassword, 6, false, 4);
	}
	else
	{
		// Fallback: invalid WPA passphrase length -> force open
		WiFi.softAP(AP.c_str(), nullptr, 6, false, 4);
	}

	dnsServer.start(AP_DNS_PORT, "*", AP_LocalIP);

	UDP_Wifi.begin(ListeningPort);

	Serial.println("");
	Serial.print("Access Point name: ");
	Serial.println(AP);
	Serial.print("Settings Page IP: ");
	Serial.println(AP_LocalIP);

	// web server
	Serial.println();
	Serial.println("Starting Web Server");

	//server.on("/", HandleRoot);
	//server.on("/page1", HandlePage1);
	//server.on("/page2", HandlePage2);
	//server.on("/ButtonPressed", ButtonPressed);
	//server.onNotFound(HandleRoot);

	server.on("/generate_204", []() {server.send(204, "text/plain", "");	});
	server.on("/fwlink", []() { server.send(200, "text/plain", "OK"); });
	server.on("/hotspot-detect.html", HTTP_GET, []() { server.send(200, "text/html", "<html><body>Portal</body></html>"); });
	server.on("/ncsi.txt", HTTP_GET, []() { server.send(200, "text/plain", "Microsoft NCSI"); });

	// OTA
	server.on("/myurl", HTTP_GET, []() {
		server.sendHeader("Connection", "close");
		server.send(200, "text/plain", "Hello there!");
	});

	server.begin();

	/* INITIALIZE ESP2SOTA LIBRARY */
	ESP2SOTA.begin(&server);

	Serial.println("OTA started.");

	// wifi client mode
	if (MDL.WifiModeUseStation)
	{
		// connect to network
		delay(1000);
		WiFi.onEvent(WiFiStationConnected, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_CONNECTED);
		WiFi.onEvent(WiFiGotIP, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_GOT_IP);
		WiFi.onEvent(WiFiStationDisconnected, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_DISCONNECTED);
		WiFi.begin(MDL.SSID, MDL.Password);
		Serial.println();
		Serial.println("Connecting to wifi network ...");
	}

	delay(1500);

	if (ADSfound)
	{
		Serial.println(F("ADS1115: Enabled "));
	}
	else
	{
		Serial.println(F("ADS1115: Disabled "));
	}

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}


void SaveData()
{
	EEPROM.put(0, (int16_t)StructVersion);
	EEPROM.put(10, MDL);
	EEPROM.commit();

	delay(3000);

	ESP.restart();
}

void LoadData()
{
	bool IsValid = false;
	int16_t StoredStructVersion;
	EEPROM.get(0, StoredStructVersion);
	if (StoredStructVersion == StructVersion)
	{
		Serial.println("Loading stored settings.");
		EEPROM.get(10, MDL);
		IsValid = ValidData();
	}

	if (!IsValid)
	{
		Serial.println("Stored settings not valid.");
		LoadDefaults();
		SaveData();
	}
}

// valid pins for each processor
uint8_t ValidPins0[] = { 0,2,4,5,13,14,15,16,17,18,19,21,22,23,25,26,27,32,33,34,35,36,39 };

bool ValidData()
{
	bool Result = false;

	// RPM pin
	Result = (MDL.RPMpin == NC);
	if (!Result)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			if (MDL.RPMpin == ValidPins0[i])
			{
				Result = true;
				break;
			}
		}
	}

	// complementary pin
	if (Result && MDL.CompPin < NC)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			if (MDL.CompPin == ValidPins0[i])
			{
				Result = true;
				break;
			}
		}
	}

	// main pin
	if (Result && MDL.MainPin < NC)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			if (MDL.MainPin == ValidPins0[i])
			{
				Result = true;
				break;
			}
		}
	}

	// alert pin
	if (Result && MDL.AlertPin < NC)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			if (MDL.AlertPin == ValidPins0[i])
			{
				Result = true;
				break;
			}
		}
	}

	// analog pin
	if (Result && MDL.AnalogPin < NC)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			if (MDL.AnalogPin == ValidPins0[i])
			{
				Result = true;
				break;
			}
		}
	}

	return Result;
}

void LoadDefaults()
{
	Serial.println("Loading default settings.");

	strncpy(MDL.APname, "YieldFlo_ESP32", ModStringLengths);
	strncpy(MDL.APpassword, "", ModStringLengths);
	MDL.WifiModeUseStation = false;
	strncpy(MDL.SSID, "Tractor", ModStringLengths);
	strncpy(MDL.Password, "111222333", ModStringLengths);
	MDL.ADS1115Enabled = true;
	MDL.RPMpin = 35;
	MDL.CompPin = 32;
	MDL.MainPin = 33;
	MDL.AlertPin = 16;
	MDL.AnalogPin = NC;
}
