
void SendComm()
{
	if (millis() - SendLast < SendTime) return;
	SendLast = millis();

	// Snapshot RPM counter.
	noInterrupts();
	uint32_t pulses   = RPMpulseCount; RPMpulseCount = 0;
	uint16_t noise    = NoiseCount;    NoiseCount    = 0;
	interrupts();

	// RPM: 1 magnet per revolution, 200ms window → pulses × 300 = RPM.
	// If no RPM sensor configured, send fixed reference 200 so the app
	// can detect the absence and skip RPM normalisation.
	uint16_t rpm = (MDL.RPMpin < NC) ? (uint16_t)(pulses * 300) : 200;

	// Status flags
	byte flags = 0;
	if (SensorOK) flags |= 0x01;	// bit 0 — S1 OK
									// bit 2 (moisture OK) not set — calibration pending

	// Build 13-byte packet
	byte pkt[13];
	pkt[0]  = 0x41;				// PGN 40001 low byte
	pkt[1]  = 0x9C;				// PGN 40001 high byte
	pkt[2]  = 0x01;				// packet type
	pkt[3]  = SensorRatio & 0xFF;
	pkt[4]  = SensorRatio >> 8;
	pkt[5]  = noise & 0xFF;		// noise count — rejected ISR edges this window
	pkt[6]  = noise >> 8;
	pkt[7]  = 0;				// moisture — calibration pending
	pkt[8]  = 0;
	pkt[9]  = rpm & 0xFF;
	pkt[10] = rpm >> 8;
	pkt[11] = flags;
	pkt[12] = CRC(pkt, 12, 0);

	UDP_Wifi.beginPacket(Wifi_DestinationIP, ModuleSendPort);
	UDP_Wifi.write(pkt, 13);
	UDP_Wifi.endPacket();
}

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
