
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
				PaddleCycles++;
				if (cyc < MinCycleUs) MinCycleUs = cyc;
				AccountPaddle(cyc, CycBlockedUs);
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

// Reduce one completed signal cycle to a paddle event. Runs in ISR context,
// called from CommitEdge() after the cycle has already been folded into the
// duty-channel sums — it only reads its arguments, so the duty channel is
// unaffected by anything decided here.
//
// The quantity accumulated is Σ(duty_i × pitches_i): per-paddle obstruction,
// summed over paddles rather than over time. Divided by the window it becomes
// obstruction per second, which is what actually tracks grain volume flow —
// duty alone is a fill fraction and stays put when the elevator changes speed.
// Multiplying by the paddle pitch would give mm/s, but the pitch is a constant
// scale factor that cancels against the yield factor, so it is left out of the
// payload entirely: the app cannot be miscalibrated by a wrong pitch setting.
void IRAM_ATTR AccountPaddle(uint32_t cyc, uint32_t blockedUs)
{
	// Fold in a fragment held back from the previous cycle.
	cyc       += CarryCycUs;
	blockedUs += CarryBlockedUs;
	CarryCycUs = 0;
	CarryBlockedUs = 0;

	// Same absurd-cycle limit CommitEdge applies to a raw cycle, re-applied to
	// the merged length. Nothing that slow is a paddle, and it also keeps the
	// ×1000 below from having to trust a bound that the carry can widen.
	if (cyc >= 2000000)
	{
		if (WinRejects < 0xFFFF) WinRejects++;
		return;
	}

	if (PeriodEmaUs == 0) PeriodEmaUs = cyc;	// first cycle after boot or a stall

	// A cycle shorter than half the running period estimate is a fragment: a
	// kernel bridging the inter-paddle gap briefly cleared the beam and split
	// one paddle into two cycles. Neither half is a paddle, and scoring them
	// as two would double the count and halve the duty — so hold this one and
	// merge it into the next. (The duty channel has no equivalent repair; this
	// is one of the cases where the two channels legitimately diverge.)
	if (cyc * MinCycDen < PeriodEmaUs * MinCycNum)
	{
		CarryCycUs = cyc;
		CarryBlockedUs = blockedUs;
		if (WinRejects < 0xFFFF) WinRejects++;
		return;
	}

	// How many paddle pitches this cycle spans. A missed edge merges two
	// paddles into one long cycle; counting it as a single paddle would halve
	// the reported flow, so the pitch count is scaled instead and the duty is
	// applied over all of them. Near saturation this is the normal case, not a
	// fault: the inter-paddle gaps close up, the beam stays blocked across
	// several paddles, and the span keeps the flow integral roughly right even
	// though the channel has stopped being sensitive to extra grain.
	uint32_t pitches = (cyc + PeriodEmaUs / 2) / PeriodEmaUs;
	if (pitches == 0) pitches = 1;
	if (pitches > MaxPitchSpan) pitches = MaxPitchSpan;

	// blockedUs <= cyc < 2e6, so ×1000 stays inside uint32 — no 64-bit divide
	// in the ISR.
	uint32_t dutyX1000 = (blockedUs * 1000) / cyc;
	if (dutyX1000 > 1000) dutyX1000 = 1000;

	WinFlowSumX1000 += dutyX1000 * pitches;
	WinAccountedUs  += cyc;
	WinPaddles      += pitches;
	if (dutyX1000 >= SatDutyX1000) WinSatPaddles += pitches;

	if (pitches == 1)
	{
		// Track the per-pitch period. Slow EMA (1/8) so one bad cycle cannot
		// move the estimate far enough to reclassify the next one.
		int32_t err = (int32_t)cyc - (int32_t)PeriodEmaUs;
		PeriodEmaUs = (uint32_t)((int32_t)PeriodEmaUs + err / 8);
	}
	else
	{
		// A multi-pitch cycle only tells us its total length — dividing it by a
		// guessed pitch count and feeding that back would let one bad estimate
		// justify itself. The estimate is left on the last cycle that was
		// unambiguously one paddle; a real speed change is picked up from those,
		// and a genuine restart resets it via the stall check in ReadFlow().
		if (WinRejects < 0xFFFF) WinRejects++;
	}
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

void ReadFlow()
{
	static uint32_t lastReadUs = 0;

	// Snapshot both channels under one critical section so the duty ratio and
	// the paddle sums describe exactly the same set of cycles — the app's
	// cross-check between them is meaningless if they cover different windows.
	noInterrupts();
	uint32_t wb = WinBlockedUs;  WinBlockedUs = 0;
	uint32_t wt = WinTotalUs;    WinTotalUs = 0;
	uint32_t lastEdge = LastEdgeUs;
	uint32_t fSum = WinFlowSumX1000;  WinFlowSumX1000 = 0;
	uint32_t fAcc = WinAccountedUs;   WinAccountedUs = 0;
	uint16_t fPad = WinPaddles;       WinPaddles = 0;
	uint16_t fRej = WinRejects;       WinRejects = 0;
	uint16_t fSat = WinSatPaddles;    WinSatPaddles = 0;
	interrupts();

	uint32_t nowUs = micros();
	uint32_t elapsedUs = (lastReadUs == 0) ? 0 : nowUs - lastReadUs;   // 0 on the first call — no window to compare against
	lastReadUs = nowUs;

	FlowValid = (fPad > 0);
	FlowSumX1000    = (fSum > 0xFFFF) ? 0xFFFF : (uint16_t)fSum;
	FlowAccountedMs = (fAcc / 1000 > 0xFFFF) ? 0xFFFF : (uint16_t)(fAcc / 1000);
	FlowPaddles     = (fPad > 255) ? 255 : (uint8_t)fPad;
	FlowRejects     = (fRej > 255) ? 255 : (uint8_t)fRej;
	FlowSatPaddles  = (fSat > 255) ? 255 : (uint8_t)fSat;

	// Window time the paddle model failed to account for. A healthy signal
	// lands almost entirely inside completed cycles; a large shortfall means
	// edges are being missed, the beam is parked, or the elevator stopped
	// mid-window — the paddle channel's numbers cannot be trusted there even
	// though it produced some. This is the one genuinely independent check on
	// the paddle model, so it is reported rather than silently corrected.
	FlowUnaccounted = FlowValid && elapsedUs > 0
		&& (fAcc < (elapsedUs - elapsedUs / 4) || fAcc > elapsedUs + elapsedUs / 4);

	// Sensor health: no valid edge for 500ms — elevator stopped, beam stuck or
	// sensor missing. LastEdgeUs must only be written by the ISR: this function
	// once reused it as a window marker, which reset it every read and made
	// this check always pass, so a beam latched blocked (paddle or grain
	// parked in front of the sensor) reported 100% flow with no pulses.
	SensorOK = ((micros() - lastEdge) < 500000);

	if (!SensorOK)
	{
		SensorRatio = 0;
		// Drop the paddle model too, and discard the period estimate and any
		// held fragment: whatever the elevator does next (restart, different
		// speed) must be measured fresh, not classified against a stale period.
		FlowValid = false;
		FlowSumX1000 = 0;
		FlowPaddles = 0;
		noInterrupts();
		PeriodEmaUs = 0;
		CarryCycUs = 0;
		CarryBlockedUs = 0;
		interrupts();
	}
	else if (wt > 0)
		SensorRatio = (uint16_t)(((uint64_t)wb * 1000) / wt);
	// else: no cycle completed this window (slow pulse rate) — hold the last
	// value; SensorOK zeroes it if the signal actually stops.
}
