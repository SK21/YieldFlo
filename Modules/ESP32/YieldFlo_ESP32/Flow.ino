
void IRAM_ATTR onSensorEdge()
{
	// Complementary outputs must always be opposite logic levels.
	// If both read the same, it is a noise glitch on one wire — discard.
	// Main-only mode has no pair to compare, so the check is skipped.
	bool mainHigh = digitalRead(MDL.MainPin);
	if (MDL.UseCompSignal)
	{
		bool compHigh = digitalRead(MDL.CompPin);
		if (mainHigh == compHigh) { NoiseCount++; return; }
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
void IRAM_ATTR CommitEdge(bool blocked, uint32_t at)
{
	if (blocked)
	{
		// clear→blocked edge: the cycle that started at the previous
		// clear→blocked edge is complete — fold it into the window sums.
		if (CycStartUs != 0)
		{
			uint32_t cyc = at - CycStartUs;
			if (cyc < 2000000)		// discard absurd cycles (boot, signal resumed after a stall)
			{
				WinBlockedUs += CycBlockedUs;
				WinTotalUs   += cyc;
			}
		}
		CycStartUs = at;
		CycBlockedUs = 0;
	}
	else
	{
		// blocked→clear edge: add the blocked segment to the current cycle
		CycBlockedUs += at - SegStartUs;
	}

	BeamBlocked = blocked;
	SegStartUs = at;
	LastEdgeUs = at;
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

void ReadFlow()
{
	// Snapshot the completed-cycle sums.
	noInterrupts();
	uint32_t wb = WinBlockedUs;  WinBlockedUs = 0;
	uint32_t wt = WinTotalUs;    WinTotalUs = 0;
	uint32_t lastEdge = LastEdgeUs;
	interrupts();

	// Sensor health: no valid edge for 500ms — elevator stopped, beam stuck or
	// sensor missing. LastEdgeUs must only be written by the ISR: this function
	// once reused it as a window marker, which reset it every read and made
	// this check always pass, so a beam latched blocked (paddle or grain
	// parked in front of the sensor) reported 100% flow with no pulses.
	SensorOK = ((micros() - lastEdge) < 500000);

	if (!SensorOK)
		SensorRatio = 0;
	else if (wt > 0)
		SensorRatio = (uint16_t)(((uint64_t)wb * 1000) / wt);
	// else: no cycle completed this window (slow pulse rate) — hold the last
	// value; SensorOK zeroes it if the signal actually stops.
}
