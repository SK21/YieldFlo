
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

	CheckExtiConflicts();

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
	BeamBlocked = ((digitalRead(MainPin) == HIGH) == InvertSensor);	// PNP: HIGH = clear; NPN inverted
	LastEdgeUs = micros();
	SegStartUs = LastEdgeUs;
	Serial.println(UseCompSignal ? "OK (Main + Comp)." : "OK (Main only).");

	// Debug LED — active-low onboard LED on PC13 mirrors the beam state
	if (DebugLED)
	{
		pinMode(LedPin, OUTPUT);
		digitalWrite(LedPin, BeamBlocked ? LOW : HIGH);	// LOW = lit when blocked
	}

	// RPM sensor
	if (RPMEnabled)
	{
		Serial.print("Starting RPM sensor ... ");
		pinMode(RPMPin, INPUT);
		attachInterrupt(digitalPinToInterrupt(RPMPin), onRPMedge, RISING);
		LastRPMedgeUs = micros();
		Serial.println("OK.");
	}

	// CAN bus — bxCAN on PB8/PB9 via the Can.h register driver.
	// remap 2 = CAN_RX on PB8, CAN_TX on PB9. CANInit() configures the GPIO,
	// bit timing, accept-all filter, single-shot TX (NART) and automatic
	// Bus-Off recovery (ABOM), then leaves the controller in normal mode.
	// It returns false if the peripheral never reaches normal mode (usually
	// a wiring/transceiver fault or missing 120R termination).
	Serial.println("Starting CAN ...");
	if (CANInit(CAN_250KBPS, 2))
	{
		Serial.println("CAN started at 250kbps.");
	}
	else
	{
		Serial.println("CAN init FAILED — check transceiver, wiring and termination.");
	}

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}

// EXTI lines are shared across ports by pin ordinal (PA5 and PB5 both use
// EXTI5), and attachInterrupt() silently reassigns a line that is already
// taken — the earlier pin just stops interrupting. Warn loudly at boot if
// any two ENABLED interrupt pins clash.
void CheckExtiConflicts()
{
	uint32_t pins[4];
	const char* names[4];
	uint8_t n = 0;
	pins[n] = MainPin; names[n] = "MainPin"; n++;
	if (UseCompSignal)  { pins[n] = CompPin;  names[n] = "CompPin";  n++; }
	if (RPMEnabled)     { pins[n] = RPMPin;   names[n] = "RPMPin";   n++; }
	if (ADS1115Enabled) { pins[n] = AlertPin; names[n] = "AlertPin"; n++; }

	for (uint8_t i = 0; i < n; i++)
	{
		for (uint8_t j = i + 1; j < n; j++)
		{
			if (STM_PIN(digitalPinToPinName(pins[i])) == STM_PIN(digitalPinToPinName(pins[j])))
			{
				Serial.print("WARNING: EXTI conflict — ");
				Serial.print(names[i]);
				Serial.print(" and ");
				Serial.print(names[j]);
				Serial.println(" share one EXTI line; one of them will NOT interrupt.");
			}
		}
	}
}
