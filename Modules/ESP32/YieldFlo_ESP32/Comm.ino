
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
	uint16_t noise  = NoiseCount;     NoiseCount    = 0;
	interrupts();

	// RPM: 1 magnet/rev, 200ms window → pulses × 300 = RPM.
	// Fixed reference 200 when no RPM sensor — app uses this to detect absence.
	uint16_t rpm = (MDL.RPMpin < NC) ? (uint16_t)(pulses * 300) : 200;

	byte flags = 0;
	if (SensorOK)        flags |= 0x01;  // bit 0 — SensorOK
	if (MDL.RPMpin < NC) flags |= 0x02;  // bit 1 — RPM sensor present
	                                      // bit 2 (MoistureOK) not set — calibration pending

	body[0] = flags;
	body[1] = SensorRatio & 0xFF;
	body[2] = SensorRatio >> 8;
	body[3] = 0;              // moisture lo — calibration pending
	body[4] = 0;              // moisture hi
	body[5] = rpm & 0xFF;
	body[6] = rpm >> 8;
	body[7] = (noise > 255) ? 255 : (uint8_t)noise;
}

// ── WiFi UDP send (5 Hz) ─────────────────────────────────────────────────
void SendComm()
{
	if (millis() - SendLast < SendTime) return;
	SendLast = millis();

	byte body[8];
	BuildDataBody(body);

	// Build 12-byte UDP packet (PGN 40001)
	byte pkt[12];
	pkt[0]  = 0x41;		// PGN 40001 low byte
	pkt[1]  = 0x9C;		// PGN 40001 high byte
	pkt[2]  = 0x01;		// packet type
	pkt[3]  = body[0];	// status_flags
	pkt[4]  = body[1];	// sensor_ratio lo
	pkt[5]  = body[2];	// sensor_ratio hi
	pkt[6]  = body[3];	// moisture_raw lo
	pkt[7]  = body[4];	// moisture_raw hi
	pkt[8]  = body[5];	// module_rpm lo
	pkt[9]  = body[6];	// module_rpm hi
	pkt[10] = body[7];	// noise_count
	pkt[11] = CRC(pkt, 11, 0);

	UDP_Wifi.beginPacket(Wifi_DestinationIP, ModuleSendPort);
	UDP_Wifi.write(pkt, 12);
	UDP_Wifi.endPacket();
}

// ── CAN bus send (5 Hz) ──────────────────────────────────────────────────
// Frame ID: 0x18FF00F8 (Extended, Priority=6, PF=0xFF ProprietaryB, PS=0x00, SA=0xF8)
void SendCAN()
{
	if (millis() - SendLast < SendTime) return;
	SendLast = millis();

	byte body[8];
	BuildDataBody(body);

	twai_message_t msg;
	memset(&msg, 0, sizeof(msg));
	msg.extd             = 1;
	msg.identifier       = 0x18FF00F8;
	msg.data_length_code = 8;
	memcpy(msg.data, body, 8);

	twai_transmit(&msg, pdMS_TO_TICKS(10));
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
