
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
				int16_t raw = (int16_t)(Wire.read() << 8 | Wire.read());

				if (ADSstate == 0)
				{
					// Moisture result (AIN0-AIN1 differential)
					if (raw < 0) raw = 0;
					MoistureReading = raw;

					// Request temperature conversion (AIN2 single-ended)
					Wire.beginTransmission(ADS1115_Address);
					Wire.write(0x01);		// Config register
					Wire.write(0b11100011);	// OS=1, MUX=110 (AIN2 vs GND), PGA=001, MODE=1
					Wire.write(0b00100000);	// DR=001 (16 SPS), COMP_QUE=00
					Wire.endTransmission();
					ADSstate = 1;
				}
				else
				{
					// Temperature result (AIN2 single-ended)
					TemperatureReading = raw;

					// Request moisture conversion (AIN0-AIN1 differential)
					Wire.beginTransmission(ADS1115_Address);
					Wire.write(0x01);		// Config register
					Wire.write(0b10000011);	// OS=1, MUX=000 (AIN0-AIN1 diff), PGA=001, MODE=1
					Wire.write(0b00100000);	// DR=001 (16 SPS), COMP_QUE=00
					Wire.endTransmission();
					ADSstate = 0;
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





