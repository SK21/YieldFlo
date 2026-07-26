
// Writes the ADS1115 config register to start a conversion on the given
// channel. Retries a transient I2C NACK before giving up — a lost write must
// not be assumed to have succeeded, or the local ADSstate desyncs from the
// ADC's actual channel and moisture/temperature silently swap.
static bool WriteADSConfig(byte hi, byte lo)
{
	for (uint8_t attempt = 0; attempt < 3; attempt++)
	{
		Wire.beginTransmission(ADS1115_Address);
		Wire.write(0x01);
		Wire.write(hi);
		Wire.write(lo);
		if (Wire.endTransmission() == 0) return true;
	}
	return false;
}

void ReadAnalog()
{
	// ADS1115 channel assignments (Moisture1 daughter board):
	//   AIN0-AIN1  differential  moisture (OEM sensor MoistureA - MoistureB)
	//   AIN2       single-ended  moisture temperature
	//
	// Config register bit fields (MSB):
	//   Bit  15      OS   0=no effect, 1=start single conversion
	//   Bits 14:12   MUX  000=AIN0-AIN1 diff, 110=AIN2 single
	//   Bits 11:9    PGA  001=4.096V full scale
	//   Bit  8       MODE 1=single shot
	// Config register bit fields (LSB):
	//   Bits 7:5     DR   001=16 SPS
	//   Bits 1:0     QUE  00=assert after 1 conversion (enables ALERT/RDY)
	//
	// Conversion-ready interrupt (MDL.AlertPin) sets ADSconversionReady.
	// ADSstate tracks which channel result is pending.

	static uint8_t ADSstate = 0;	// 0 = moisture result pending, 1 = temp result pending

	if (ADSfound)
	{
		if (ADSconversionReady)
		{
			ADSconversionReady = false;

			// Read result from conversion register
			Wire.beginTransmission(ADS1115_Address);
			Wire.write(0x00);	// Conversion register
			Wire.endTransmission();

			if (Wire.requestFrom(ADS1115_Address, 2) == 2)
			{
				uint8_t hiByte = Wire.read();
				uint8_t loByte = Wire.read();
				int16_t raw = (int16_t)((uint16_t)hiByte << 8 | loByte);
				LastADSReadMs = millis();

				if (ADSstate == 0)
				{
					// Moisture result (AIN0-AIN1 differential)
					if (raw < 0) raw = 0;
					MoistureReading = raw;

					// Request temperature conversion (AIN2 single-ended). Only
					// advance ADSstate if the switch actually took — otherwise
					// the ADC is still on the moisture channel and flipping
					// our state here would label its next (moisture) result
					// as temperature.
					// OS=1, MUX=110 (AIN2 vs GND), PGA=001, MODE=1 / DR=001 (16 SPS), COMP_QUE=00
					if (WriteADSConfig(0b11100011, 0b00100000))	ADSstate = 1;
				}
				else
				{
					// Temperature result (AIN2 single-ended)
					TemperatureReading = raw;

					// Request moisture conversion (AIN0-AIN1 differential).
					// Only advance ADSstate if the switch actually took (see
					// note above).
					// OS=1, MUX=000 (AIN0-AIN1 diff), PGA=001, MODE=1 / DR=001 (16 SPS), COMP_QUE=00
					if (WriteADSConfig(0b10000011, 0b00100000))	ADSstate = 0;
				}
			}
		}
	}
	else
	{
		// Fallback: ESP32 native ADC
		if (MDL.AnalogPin < NC) MoistureReading = (int16_t)analogRead(MDL.AnalogPin);
	}
}

// ADS1115 is only "OK" if it was actually found AND has produced a
// conversion recently — a dropped I2C connection stops ADSconversionReady
// from firing without ever clearing ADSfound, which would otherwise report
// stale/frozen moisture+temperature as good indefinitely.
const uint32_t ADS_STALE_MS = 1000;
bool ADSFresh()
{
	return ADSfound && (millis() - LastADSReadMs) < ADS_STALE_MS;
}





