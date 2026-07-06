
// ── Shared 8-byte data body ──────────────────────────────────────────────
// Both SendComm() and SendCAN() build this body from the same sensor state.
// Layout:
//   [0]   status_flags  bit0=SensorOK, bit1=RPMPresent, bit2=MoistureOK
//   [1-2] sensor_ratio  uint16 LE  (ratio × 1000, 0–1000 = 0.0–100.0%)
//   [3-4] moisture_raw  uint16 LE  (value × 10 = tenths %)
//   [5-6] module_rpm    uint16 LE
//   [7]   noise_count   uint8  (ISR-rejected edges this window, capped at 255)

static void BuildDataBody(byte body[8])
{
	noInterrupts();
	uint32_t pulses = RPMpulseCount;  RPMpulseCount = 0;
	uint16_t noise = NoiseCount;     NoiseCount = 0;
	interrupts();

	// RPM: 1 magnet/rev, 200ms window → pulses × 300 = RPM.
	// Fixed reference 200 when no RPM sensor — app uses this to detect absence.
	uint16_t rpm = (MDL.RPMpin < NC) ? (uint16_t)(pulses * 300) : 200;

	byte flags = 0;
	if (SensorOK)        flags |= 0x01;  // bit 0 — SensorOK
	if (MDL.RPMpin < NC) flags |= 0x02;  // bit 1 — RPM sensor present
	if (ADSfound)        flags |= 0x04;  // bit 2 — MoistureOK

	body[0] = flags;
	body[1] = SensorRatio & 0xFF;
	body[2] = SensorRatio >> 8;
	body[3] = MoistureReading & 0xFF;            // moisture lo 
	body[4] = MoistureReading >> 8;              // moisture hi
	body[5] = rpm & 0xFF;
	body[6] = rpm >> 8;
	body[7] = (noise > 255) ? 255 : (uint8_t)noise;
}

// ── CAN bus health monitoring ─────────────────────────────────────────────
static uint32_t LastBusOffMs = 0;
static uint8_t  BusOffCount = 0;

// Called every loop() when CAN mode is active.
// Handles Bus Off recovery and falls back to WiFi after repeated failures.
void CheckCanBus()
{
	twai_status_info_t st;
	if (twai_get_status_info(&st) != ESP_OK) return;

	if (st.state == TWAI_STATE_BUS_OFF)
	{
		if (millis() - LastBusOffMs > 3000)
		{
			LastBusOffMs = millis();
			BusOffCount++;
			Serial.print("CAN Bus Off. Recovery attempt ");
			Serial.println(BusOffCount);
			twai_initiate_recovery();   // waits for 128 × 11 recessive bits, then auto-starts
		}

		if (BusOffCount > 5)
		{
			Serial.println("CAN Bus Off: persistent. Falling back to WiFi.");
			MDL.UseCanComm = false;     // session only — EEPROM unchanged, reverts on restart
			BusOffCount = 0;
			SendLastPK1 = 0;         // send WiFi immediately on next loop
		}
	}
	else if (st.state == TWAI_STATE_STOPPED)
	{
		twai_start();
	}
}

void SendCAN()
{
	SendCANPK1();
	SendCANPK2();
}

// ── CAN bus send (5 Hz) ──────────────────────────────────────────────────
// Frame ID: 0x18FF00F8 (Extended, Priority=6, PF=0xFF ProprietaryB, PS=0x00, SA=0xF8)
void SendCANPK1()
{
	if (millis() - SendLastPK1 < SendTimePK1) return;

	// Skip transmit only if controller is stopped. Do NOT gate on tx_error_counter:
	// with no ACKing node on the bus (app not open yet) TEC rises to error-passive,
	// and TEC only falls on successful TX — a TEC guard here would block transmit
	// forever. ACK errors at error-passive cannot reach Bus Off (CAN spec), so
	// transmitting in that state is safe; CheckCanBus handles recovery if it stops.
	twai_status_info_t st;
	if (twai_get_status_info(&st) != ESP_OK)           return;
	if (st.state != TWAI_STATE_RUNNING)                return;

	SendLastPK1 = millis();

	byte body[8];
	BuildDataBody(body);

	twai_message_t msg;
	memset(&msg, 0, sizeof(msg));
	msg.extd = 1;
	msg.identifier = 0x18FF00F8;
	msg.data_length_code = 8;
	memcpy(msg.data, body, 8);

	twai_transmit(&msg, pdMS_TO_TICKS(10));
}

void SendCANPK2()
{
	if (millis() - SendLastPK2 > SendTimePK2)
	{
		SendLastPK2 = millis();
		twai_status_info_t st;
		if (twai_get_status_info(&st) != ESP_OK) return;
		if (st.state != TWAI_STATE_RUNNING)      return;

		byte flags = ADSfound ? 0x01 : 0x00;
		int16_t temp = TemperatureReading;

		twai_message_t msg;
		memset(&msg, 0, sizeof(msg));
		msg.extd = 1;
		msg.identifier = 0x18FF01F8;
		msg.data_length_code = 8;
		msg.data[0] = flags;
		msg.data[1] = (byte)(temp & 0xFF);
		msg.data[2] = (byte)((temp >> 8) & 0xFF);

		twai_transmit(&msg, pdMS_TO_TICKS(10));
	}
}

void SendWifi()
{
	SendWifiPK1();
	SendWifiPK2();
}

// ── WiFi UDP send (5 Hz) ─────────────────────────────────────────────────
void SendWifiPK1()
{
	if (millis() - SendLastPK1 < SendTimePK1) return;
	SendLastPK1 = millis();

	byte body[8];
	BuildDataBody(body);

	// Build 11-byte UDP packet (PGN 40001)
	byte pkt[11];
	pkt[0] = 0x41;		// PGN 40001 low byte
	pkt[1] = 0x9C;		// PGN 40001 high byte
	pkt[2] = body[0];	// status_flags
	pkt[3] = body[1];	// sensor_ratio lo
	pkt[4] = body[2];	// sensor_ratio hi
	pkt[5] = body[3];	// moisture_raw lo
	pkt[6] = body[4];	// moisture_raw hi
	pkt[7] = body[5];	// module_rpm lo
	pkt[8] = body[6];	// module_rpm hi
	pkt[9] = body[7];	// noise_count
	pkt[10] = CRC(pkt, 10, 0);

	UDP_Wifi.beginPacket(Wifi_DestinationIP, ModuleSendPort);
	UDP_Wifi.write(pkt, 11);
	UDP_Wifi.endPacket();
}

// ── Second packet: temperature (1 Hz) ────────────────────────────────────
// UDP  — 6 bytes, PGN 40002
//   [0-1] PGN 40002 LE  (0x42 0x9C)
//   [2]   flags  bit0=TempOK
//   [3-4] temp_raw int16 LE  (raw ADS1115 AIN2 reading)
//   [5]   CRC8
// CAN  — ID 0x18FF01F8, DLC=8, [0]=flags, [1-2]=temp_raw, [3-7]=0

void SendWifiPK2()
{
	if (millis() - SendLastPK2 > SendTimePK2)
	{
		SendLastPK2 = millis();
		byte flags = ADSfound ? 0x01 : 0x00;
		int16_t temp = TemperatureReading;

		byte pkt[6];
		pkt[0] = 0x42;
		pkt[1] = 0x9C;
		pkt[2] = flags;
		pkt[3] = (byte)(temp & 0xFF);
		pkt[4] = (byte)((temp >> 8) & 0xFF);
		pkt[5] = CRC(pkt, 5, 0);

		UDP_Wifi.beginPacket(Wifi_DestinationIP, ModuleSendPort);
		UDP_Wifi.write(pkt, 6);
		UDP_Wifi.endPacket();
	}
}


// ── Receive (drain incoming UDP) ─────────────────────────────────────────
void ReceiveComm()
{
	// Drain incoming UDP packets — no commands defined yet.
	int sz = UDP_Wifi.parsePacket();
	while (sz > 0)
	{
		UDP_Wifi.flush();
		sz = UDP_Wifi.parsePacket();
	}
}
