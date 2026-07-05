
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

	// HIGH on main = beam clear, LOW = beam blocked.
	// If state hasn't changed, this is a duplicate interrupt — discard.
	bool nowBlocked = !mainHigh;
	if (nowBlocked == BeamBlocked) return;

	// Accumulate time spent in the previous state
	uint32_t now = micros();
	uint32_t delta = now - LastEdgeUs;
	if (BeamBlocked) BlockedAccum += delta;
	else             ClearAccum += delta;

	BeamBlocked = nowBlocked;
	LastEdgeUs = now;
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
	// Sensor health: if no valid edge for 500ms the beam is stuck or sensor is missing.
	SensorOK = ((micros() - LastEdgeUs) < 500000);

	// Snapshot accumulators and close the in-progress segment.
	noInterrupts();
	uint32_t blocked = BlockedAccum;  BlockedAccum = 0;
	uint32_t clear = ClearAccum;    ClearAccum = 0;
	uint32_t now = micros();
	uint32_t delta = now - LastEdgeUs;
	LastEdgeUs = now;
	if (BeamBlocked) blocked += delta;
	else             clear += delta;
	interrupts();

	uint32_t total = blocked + clear;
	SensorRatio = (total > 0) ? (uint16_t)((uint32_t)blocked * 1000 / total) : 0;
}
