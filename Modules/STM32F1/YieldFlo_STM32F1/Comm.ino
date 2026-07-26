
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
	static uint32_t lastMs = 0;

	noInterrupts();
	uint32_t pulses = RPMpulseCount;  RPMpulseCount = 0;
	uint16_t noise = NoiseCount;     NoiseCount = 0;
	interrupts();

	uint32_t now = millis();
	uint32_t elapsed = now - lastMs;
	lastMs = now;

	// RPM: 1 magnet/rev, normalized by the ACTUAL elapsed time since the last
	// call rather than an assumed fixed 200ms window — a skipped send (e.g.
	// CAN Bus Off recovery) lets pulses accumulate over a longer window, and
	// the old fixed-300 multiplier would report a spurious RPM spike on the
	// next send. Same fix TakePaddleHz() already applies to paddle Hz.
	// Fixed reference 200 when no RPM sensor — app uses this to detect absence.
	uint16_t rpm;
	if (RPMEnabled)
	{
		uint32_t rpmCalc = (elapsed == 0) ? 0
			: (uint32_t)(((uint64_t)pulses * 60000 + elapsed / 2) / elapsed);
		rpm = (rpmCalc > 65535) ? 65535 : (uint16_t)rpmCalc;
	}
	else
	{
		rpm = 200;
	}

	byte flags = 0;
	if (SensorOK)   flags |= 0x01;  // bit 0 — SensorOK
	if (RPMEnabled) flags |= 0x02;  // bit 1 — RPM sensor present
	if (ADSFresh()) flags |= 0x04;  // bit 2 — MoistureOK

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
//  - ABOM (automatic bus-off management, set in CANInit) recovers from Bus
//    Off in hardware after 128 × 11 recessive bits — no recovery state
//    machine needed.
//  - TX is single-shot (NART set in CANInit), so a frame that gets no ACK is
//    dropped instead of flooding the bus; the next one goes out 200ms later.
//    No WiFi fallback — there is nothing to fall back to on this board.

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
	SendCANPK3();
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

	CAN_msg_t msg = {};
	msg.id = 0x18FF00F8;
	msg.format = EXTENDED_FORMAT;
	msg.type = DATA_FRAME;
	msg.len = 8;
	memcpy(msg.data, body, 8);

	CANSend(&msg);
}

// ── Second packet: temperature + paddle rate (1 Hz) ─────────────────────
// ID 0x18FF01F8, DLC=8, [0]=flags bit0=TempOK bit1=PaddleHzPresent
// bit2=MinCycleMsPresent, [1-2]=temp_raw int16 LE, [3]=paddle_hz uint8
// (paddles/s), [4]=min_cycle_ms uint8 (shortest paddle cycle this window,
// ms; 255=none/clipped), [5-7]=0
void SendCANPK2()
{
	if (millis() - SendLastPK2 > SendTimePK2)
	{
		SendLastPK2 = millis();
		if (CAN1->ESR & CAN_ESR_BOFF) return;

		byte flags = ADSFresh() ? 0x01 : 0x00;
		flags |= 0x02;		// bit 1 — paddle_hz field present
		flags |= 0x04;		// bit 2 — min_cycle_ms field present
		int16_t temp = TemperatureReading;

		CAN_msg_t msg = {};
		msg.id = 0x18FF01F8;
		msg.format = EXTENDED_FORMAT;
		msg.type = DATA_FRAME;
		msg.len = 8;
		msg.data[0] = flags;
		msg.data[1] = (byte)(temp & 0xFF);
		msg.data[2] = (byte)((temp >> 8) & 0xFF);
		msg.data[3] = TakePaddleHz();
		msg.data[4] = TakeMinCycleMs();

		CANSend(&msg);
	}
}

// ── Third packet: paddle-event flow channel (5 Hz) ───────────────────────
// ID 0x18FF02F8, DLC=8. Sent on the same period as PK1 and filled from the
// same ReadFlow() snapshot, so a receiver can pair each PK3 with the PK1 that
// covers the same window and cross-check the two channels.
//
// This is a NEW frame rather than an extension of PK1: the shared 8-byte body
// is byte-for-byte common with the ESP32 module, and an app that never sees a
// PK3 (older firmware, ESP32 over UDP) has to keep working on the duty channel
// alone. Every field is a raw count — no calibration constant is applied here,
// so a stored log can be re-derived under any baseline afterwards.
//
//   [0]   flags  bit0=PaddleValid, bit1=Saturated, bit2=Unaccounted,
//                bit3=RepairsApplied (a cycle was merged, scaled or reseeded)
//   [1-2] flow_sum     uint16 LE  Σ(per-paddle duty × pitches) × 1000
//   [3-4] accounted_ms uint16 LE  window time inside accepted cycles, ms
//   [5]   paddles      uint8      pitches accounted this window
//   [6]   rejects      uint8      cycles repaired this window
//   [7]   sat_paddles  uint8      paddles at/above 90% duty this window
//
// Receiver math (pitch-free — multiply by paddle pitch for mm/s if wanted):
//   window_s    = accounted_ms / 1000
//   paddles/s   = paddles / window_s
//   flow_rate   = (flow_sum / 1000) / window_s      ← uncorrected obstruction/s
//   grain_rate  = flow_rate - paddles/s × baseline  ← baseline is per PADDLE
void SendCANPK3()
{
	if (millis() - SendLastPK3 < SendTimePK3) return;
	if (CAN1->ESR & CAN_ESR_BOFF) return;

	SendLastPK3 = millis();

	byte flags = 0;
	if (FlowValid)       flags |= 0x01;
	// Saturated once more than half the window's paddles are pinned at full
	// obstruction — past that point extra grain produces no extra signal.
	if (FlowValid && FlowSatPaddles * 2 > FlowPaddles) flags |= 0x02;
	if (FlowUnaccounted) flags |= 0x04;
	if (FlowRejects > 0) flags |= 0x08;

	CAN_msg_t msg = {};
	msg.id = 0x18FF02F8;
	msg.format = EXTENDED_FORMAT;
	msg.type = DATA_FRAME;
	msg.len = 8;
	msg.data[0] = flags;
	msg.data[1] = (byte)(FlowSumX1000 & 0xFF);
	msg.data[2] = (byte)(FlowSumX1000 >> 8);
	msg.data[3] = (byte)(FlowAccountedMs & 0xFF);
	msg.data[4] = (byte)(FlowAccountedMs >> 8);
	msg.data[5] = FlowPaddles;
	msg.data[6] = FlowRejects;
	msg.data[7] = FlowSatPaddles;

	CANSend(&msg);
}

// ── Receive (drain incoming frames) ──────────────────────────────────────
void DrainCanRx()
{
	CAN_msg_t rx;
	while (CANMsgAvail())
	{
		CANReceive(&rx);
		// no downstream commands defined yet
	}
}
