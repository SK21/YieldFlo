
// ── Shared 8-byte data body ──────────────────────────────────────────────
// Identical to the ESP32 module — the PC app needs no changes.
// Layout:
//   [0]   status_flags  bit0=SensorOK, bit1=RPMPresent, bit2=MoistureOK
//   [1-2] sensor_ratio  uint16 LE  (ratio × 1000, 0–1000 = 0.0–100.0%)
//   [3-4] moisture_raw  uint16 LE  (raw ADS1115 AIN0-AIN1 differential count)
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
	uint16_t rpm = RPMEnabled ? (uint16_t)(pulses * 300) : 200;

	byte flags = 0;
	if (SensorOK)   flags |= 0x01;  // bit 0 — SensorOK
	if (RPMEnabled) flags |= 0x02;  // bit 1 — RPM sensor present
	if (ADSfound)   flags |= 0x04;  // bit 2 — MoistureOK

	body[0] = flags;
	body[1] = SensorRatio & 0xFF;
	body[2] = SensorRatio >> 8;
	body[3] = MoistureReading & 0xFF;            // moisture lo
	body[4] = MoistureReading >> 8;              // moisture hi
	body[5] = rpm & 0xFF;
	body[6] = rpm >> 8;
	body[7] = (noise > 255) ? 255 : (uint8_t)noise;
}

// ── CAN bus health ───────────────────────────────────────────────────────
// bxCAN differs from the ESP32 TWAI driver:
//  - ABOM (automatic bus-off management) recovers from Bus Off in hardware
//    after 128 × 11 recessive bits — no recovery state machine needed.
//  - TX is single-shot (auto-retransmission disabled in Can.begin()), so a
//    frame that gets no ACK is dropped instead of flooding the bus; the next
//    one goes out 200ms later. No WiFi fallback — there is nothing to fall
//    back to on this board.

// Set the ABOM bit. MCR is only writable in initialisation mode, so briefly
// re-enter it; called once after Can.begin()/setBaudRate().
void EnableAutoBusOffRecovery()
{
	uint32_t t = millis();
	CAN1->MCR |= CAN_MCR_INRQ;
	while (!(CAN1->MSR & CAN_MSR_INAK)) { if (millis() - t > 10) return; }
	CAN1->MCR |= CAN_MCR_ABOM;
	CAN1->MCR &= ~CAN_MCR_INRQ;
	t = millis();
	while (CAN1->MSR & CAN_MSR_INAK) { if (millis() - t > 10) return; }
}

static uint32_t LastBusOffMs = 0;

void CheckCanBus()
{
	if (CAN1->ESR & CAN_ESR_BOFF)
	{
		if (millis() - LastBusOffMs > 3000)
		{
			LastBusOffMs = millis();
			Serial.println("CAN Bus Off — hardware recovery in progress (ABOM).");
		}
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
	if (CAN1->ESR & CAN_ESR_BOFF) return;   // ABOM is recovering the controller

	SendLastPK1 = millis();

	byte body[8];
	BuildDataBody(body);

	CAN_message_t msg = {};
	msg.id = 0x18FF00F8;
	msg.flags.extended = 1;
	msg.len = 8;
	memcpy(msg.buf, body, 8);

	Can.write(msg);
}

// ── Second packet: temperature (1 Hz) ────────────────────────────────────
// ID 0x18FF01F8, DLC=8, [0]=flags bit0=TempOK, [1-2]=temp_raw int16 LE, [3-7]=0
void SendCANPK2()
{
	if (millis() - SendLastPK2 > SendTimePK2)
	{
		SendLastPK2 = millis();
		if (CAN1->ESR & CAN_ESR_BOFF) return;

		byte flags = ADSfound ? 0x01 : 0x00;
		int16_t temp = TemperatureReading;

		CAN_message_t msg = {};
		msg.id = 0x18FF01F8;
		msg.flags.extended = 1;
		msg.len = 8;
		msg.buf[0] = flags;
		msg.buf[1] = (byte)(temp & 0xFF);
		msg.buf[2] = (byte)((temp >> 8) & 0xFF);

		Can.write(msg);
	}
}

// ── Receive (drain incoming frames) ──────────────────────────────────────
void DrainCanRx()
{
	CAN_message_t rx;
	while (Can.read(rx))
	{
		// no downstream commands defined yet
	}
}
