
void IRAM_ATTR onSensorEdge()
{
	// Complementary outputs must always be opposite logic levels.
	// If both read the same, it is a noise glitch on one wire — discard.
	// Main-only mode has no pair to compare, so the check is skipped.
	bool mainHigh = digitalRead(MDL.MainPin);
	if (MDL.UseCompSignal)
	{
		bool compHigh = digitalRead(MDL.CompPin);
		if (mainHigh == compHigh)
		{
			NoiseCount++;
			// Saturate rather than wrap: at 18 rejects/s a uint16 would roll over
			// in an hour and drop back under the fault threshold.
			if (CompRejectsSinceCommit < 0xFFFF) CompRejectsSinceCommit++;
			return;
		}
	}

	// PNP: HIGH on main = beam clear, LOW = blocked. NPN sensors are inverted.
	// If state hasn't changed (including a held pending transition), this is
	// a duplicate interrupt — discard.
	bool nowBlocked = (mainHigh == MDL.InvertSensor);
	bool effBlocked = PendingValid ? PendingBlocked : BeamBlocked;
	if (nowBlocked == effBlocked) return;

	uint32_t now = micros();

	// Glitch filter: this edge returns to the pre-pending state within
	// GlitchMinUs of the pending edge — an EMI pulse pair, not a paddle.
	// Drop the pending transition; nothing was committed, so no trace is
	// left in the cycle accounting.
	if (PendingValid && (now - PendingTimeUs) < GlitchMinUs)
	{
		PendingValid = false;
		NoiseCount++;
		return;
	}

	// The pending transition outlived the glitch window — it was a real
	// edge. Commit it with its original timestamp, then hold this edge as
	// the new pending transition.
	if (PendingValid) CommitEdge(PendingBlocked, PendingTimeUs);

	PendingBlocked = nowBlocked;
	PendingTimeUs = now;
	PendingValid = true;
}

// Fold a confirmed transition into the cycle accounting. 'at' is the edge's
// original micros() timestamp, so the deferred commit costs no timing accuracy.
void IRAM_ATTR CommitEdge(bool blocked, uint32_t EdgeUs)
{
	// A real edge survived the whole chain, so the comp pair is doing its job.
	// Cleared here rather than anywhere else because this is the only place that
	// fact is known.
	CompRejectsSinceCommit = 0;

	if (blocked)
	{
		// Feed the period estimator first, with the raw interval since the last
		// leading edge — before the gate has any say. This is what keeps the
		// estimator independent of the gate's own decisions.
		if (LastLeadingUs != 0)
		{
			uint32_t sinceLead = EdgeUs - LastLeadingUs;
			if (sinceLead < 2000000)		// same absurd-interval guard as the cycle sums
			{
				GateRing[GateRingIndex] = sinceLead;
				GateRingIndex = (GateRingIndex + 1) % GateRingSize;
				if (GateRingCount < GateRingSize) GateRingCount++;
			}
		}
		LastLeadingUs = EdgeUs;

		// Too early to be a paddle — a kernel bridged the inter-paddle gap.
		// Leave the cycle open: SegStartUs below starts a blocked segment that
		// the next blocked→clear edge folds into this cycle's blocked total,
		// so the obstruction is counted where it belongs instead of becoming a
		// false boundary. CycStartUs deliberately does not move, which is what
		// preserves the phase reference for the paddle that is actually due.
		if (GateMinCycUs != 0 && CycStartUs != 0 && (EdgeUs - CycStartUs) < GateMinCycUs)
		{
			if (GateRejects < 0xFFFF) GateRejects++;
			BeamBlocked = blocked;
			SegStartUs = EdgeUs;
			LastEdgeUs = EdgeUs;
			return;
		}

		// clear→blocked edge: the cycle that started at the previous
		// clear→blocked edge is complete — fold it into the window sums.
		if (CycStartUs != 0)
		{
			uint32_t cyc = EdgeUs - CycStartUs;
			if (cyc < 2000000)		// discard absurd cycles (boot, signal resumed after a stall)
			{
				WinBlockedUs += CycBlockedUs;
				WinTotalUs   += cyc;
				PaddleCycles++;
				if (cyc < MinCycleUs) MinCycleUs = cyc;
			}
		}
		CycStartUs = EdgeUs;
		CycBlockedUs = 0;
	}
	else
	{
		// blocked→clear edge: add the blocked segment to the current cycle
		CycBlockedUs += EdgeUs - SegStartUs;
	}

	BeamBlocked = blocked;
	SegStartUs = EdgeUs;
	LastEdgeUs = EdgeUs;
}

void IRAM_ATTR onRPMedge()
{
	// Debounce: ignore edges closer than 50ms (~1200 RPM max, well above elevator speed).
	// Guards against contact bounce on a reed switch or magnet sensor ringing.
	uint32_t now = micros();
	if (now - LastRPMedgeUs < 50000) return;
	LastRPMedgeUs = now;
	RPMpulseCount++;
}

// Paddle rate for the 1 Hz packet: completed cycles since the last call,
// normalized by the actual elapsed time so a skipped send (CAN recovering)
// doesn't double the next reading. Rounded to whole Hz — the app averages
// several packets to recover the fraction.
uint8_t TakePaddleHz()
{
	static uint32_t lastMs = 0;
	noInterrupts();
	uint16_t c = PaddleCycles;  PaddleCycles = 0;
	interrupts();

	uint32_t now = millis();
	uint32_t elapsed = now - lastMs;
	lastMs = now;
	if (elapsed == 0) return 0;

	uint32_t hz = ((uint32_t)c * 1000 + elapsed / 2) / elapsed;
	return (hz > 255) ? 255 : (uint8_t)hz;
}

// Shortest completed paddle cycle for the 1 Hz packet, in ms (255 = none
// completed this window, or genuinely >=255ms). A cycle much shorter than
// the elevator's normal paddle period means a spurious edge — e.g. a grain
// kernel bridging the inter-paddle gap — got committed as if it were a
// full paddle cycle.
uint8_t TakeMinCycleMs()
{
	noInterrupts();
	uint32_t m = MinCycleUs;  MinCycleUs = 0xFFFFFFFF;
	interrupts();

	if (m == 0xFFFFFFFF) return 255;
	uint32_t ms = m / 1000;
	return (ms > 255) ? 255 : (uint8_t)ms;
}

// Gate rejections for the 1 Hz packet. A steady low count at high flow is the
// gate doing its job; zero means either a clean signal or a threshold set too
// loose to catch anything, which min_cycle_ms distinguishes.
uint8_t TakeGateRejects()
{
	noInterrupts();
	uint16_t r = GateRejects;  GateRejects = 0;
	interrupts();
	return (r > 255) ? 255 : (uint8_t)r;
}

// Gate period estimate for the 1 Hz packet, in ms. A level, not an accumulator,
// so this reads without clearing. 0 = the estimator is unarmed (ring below
// GateMinSamples, or cleared by a sensor dropout) and the gate is passing
// everything — the one state min_cycle_ms and gate_rejects together cannot
// distinguish from a clean signal. 255 = clipped, same ceiling as min_cycle_ms.
uint8_t GetMedianCycleMs()
{
	uint32_t ms = GateMedianUsCache / 1000;
	return (ms > 255) ? 255 : (uint8_t)ms;
}

// Median of the raw leading-edge intervals. Called from ReadFlow at 5 Hz, well
// outside interrupt context — sorting 32 entries is far too slow for the ISR,
// which only ever appends to the ring and reads the cached threshold.
// Copying slots 0..n-1 is correct whether or not the ring has wrapped: a median
// needs the multiset, not the order.
static uint32_t GateMedianUs()
{
	uint32_t buf[GateRingSize];
	uint8_t n;

	noInterrupts();
	n = GateRingCount;
	for (uint8_t i = 0; i < n; i++) buf[i] = GateRing[i];
	interrupts();

	if (n < GateMinSamples) return 0;

	for (uint8_t i = 1; i < n; i++)
	{
		uint32_t v = buf[i];
		int8_t j = (int8_t)i - 1;
		while (j >= 0 && buf[j] > v) { buf[j + 1] = buf[j]; j--; }
		buf[j + 1] = v;
	}
	return buf[n / 2];
}

void ReadFlow()
{
	// Snapshot the completed-cycle sums.
	noInterrupts();
	uint32_t wb = WinBlockedUs;  WinBlockedUs = 0;
	uint32_t wt = WinTotalUs;    WinTotalUs = 0;
	uint32_t lastEdge = LastEdgeUs;
	uint16_t compRej = CompRejectsSinceCommit;
	interrupts();

	// Comp-wire fault: edges are STILL being discarded by the cross-check, and
	// none has committed for a long time. Both halves are required. The count
	// alone latches once the elevator stops — nothing commits and nothing
	// rejects, so a stale count would go on asserting a fault on a parked
	// machine. Requiring it to still be moving means this reports only while the
	// signal is genuinely arriving, which is also why it needs no "is the machine
	// harvesting" gate the way SensorOK does: a still elevator produces no edges
	// and so cannot raise it.
	//
	// Not airtight in one case: sustained EMI with the elevator stopped and Comp
	// correctly wired also rejects without committing. It needs the machine
	// parked with something electrically noisy running, and it costs a warning
	// label rather than any data, so the trade is worth it against missing a
	// fault that silently records nothing all day.
	static uint16_t lastCompRej = 0;
	CompFault = MDL.UseCompSignal
	         && (compRej > CompFaultRejects)
	         && (compRej != lastCompRej);
	lastCompRej = compRej;

	// Sensor health: no valid edge for 500ms — elevator stopped, beam stuck or
	// sensor missing. LastEdgeUs must only be written by the ISR: this function
	// once reused it as a window marker, which reset it every read and made
	// this check always pass, so a beam latched blocked (paddle or grain
	// parked in front of the sensor) reported 100% flow with no pulses.
	SensorOK = ((micros() - lastEdge) < 500000);

	if (!SensorOK)
	{
		SensorRatio = 0;

		// Discard the period history with it. Whatever the elevator does next —
		// restart, different speed — must be measured fresh rather than gated
		// against a period that no longer applies. Clearing LastLeadingUs also
		// stops the stall itself entering the ring as one enormous interval.
		noInterrupts();
		GateRingCount = 0;
		GateRingIndex = 0;
		GateMinCycUs = 0;
		LastLeadingUs = 0;
		interrupts();
		GateMedianUsCache = 0;
	}
	else
	{
		// Refresh the gate threshold from the current period distribution.
		// Returns 0 until the ring holds GateMinSamples, which leaves the gate
		// open through startup — the same behaviour as before it existed.
		uint32_t med = GateMedianUs();
		uint32_t thresh = (med > 0) ? (uint32_t)(((uint64_t)med * GatePercent) / 100) : 0;
		noInterrupts();
		GateMinCycUs = thresh;
		interrupts();
		GateMedianUsCache = med;

		if (wt > 0)
			SensorRatio = (uint16_t)(((uint64_t)wb * 1000) / wt);
		// else: no cycle completed this window (slow pulse rate) — hold the last
		// value; SensorOK zeroes it if the signal actually stops.
	}
}
