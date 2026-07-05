
void DoSetup()
{
	uint8_t ErrorCount = 0;
	Serial.begin(38400);
	delay(2000);
	Serial.println();
	Serial.println();
	Serial.println(InoDescription);
	Serial.println();

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
	Serial.println(ModuleID);
	Serial.println("");

	// I2C
	Wire.begin();			// I2C1 on pins SCL PB6, SDA PB7
	Wire.setClock(400000);	// Increase I2C data rate to 400kHz

	// ADS1115
	if (ADS1115Enabled)
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
			pinMode(AlertPin, INPUT_PULLUP);
			attachInterrupt(digitalPinToInterrupt(AlertPin), onADSReady, FALLING);

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
	pinMode(MainPin, INPUT);
	attachInterrupt(digitalPinToInterrupt(MainPin), onSensorEdge, CHANGE);
	if (UseCompSignal)
	{
		pinMode(CompPin, INPUT);
		attachInterrupt(digitalPinToInterrupt(CompPin), onSensorEdge, CHANGE);
	}
	BeamBlocked = (digitalRead(MainPin) == LOW);	// HIGH = clear, LOW = blocked
	LastEdgeUs = micros();
	Serial.println(UseCompSignal ? "OK (Main + Comp)." : "OK (Main only).");

	// RPM sensor
	if (RPMEnabled)
	{
		Serial.print("Starting RPM sensor ... ");
		pinMode(RPMPin, INPUT);
		attachInterrupt(digitalPinToInterrupt(RPMPin), onRPMedge, RISING);
		LastRPMedgeUs = micros();
		Serial.println("OK.");
	}

	// CAN bus — bxCAN on PB8/PB9, single-shot TX (no auto-retransmission)
	Serial.println("Starting CAN ...");
	Can.begin();
	Can.setBaudRate(250000);
	EnableAutoBusOffRecovery();
	Serial.println("CAN started at 250kbps.");

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}
