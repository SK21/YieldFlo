// SensorSim — optical grain-flow sensor simulator for YieldFlo bench testing.
//
// Drives the module's Main + Comp inputs with a complementary square wave,
// like the elevator optical sensor: blocked fraction of each period = the
// sensor ratio the module should report.
//
// Board: any 3.3 V Arduino-compatible (ESP32 devkit, STM32 Blue Pill, ...).
// Do NOT drive the module's ESP32 pins from a 5 V board (Nano/Uno) without
// a divider. Inject either at the YF1's ESP32-side test points (3.3 V) or
// through the J7 sensor input conditioning at whatever level it expects.
//
// Wiring: MAIN_PIN → module Main input, COMP_PIN → module Comp input,
// GND → module GND (required).
//
// Serial commands (115200 baud, newline-terminated):
//   d 35      duty — % of period blocked (0–100)
//   f 100     frequency in Hz (1–2000)
//   p         toggle polarity: PNP (default, HIGH = clear) / NPN (inverted)
//   s         stop — freeze outputs at the current level (no edges; tests
//             the module's no-pulse handling: SensorOK must drop in 500 ms)
//   g         go — resume pulsing
//   b         freeze in the BLOCKED state (reproduces "beam latched blocked")
//   c         freeze in the CLEAR state
//   ?         print current settings
//
// Defaults: 100 Hz, 30 % blocked, PNP, running.

const uint8_t MAIN_PIN = 25;
const uint8_t COMP_PIN = 26;

uint16_t dutyPct  = 30;     // % of period blocked
uint16_t freqHz   = 100;
bool     npn      = false;  // false = PNP (HIGH = clear), true = inverted
bool     running  = true;

uint32_t periodUs  = 10000;
uint32_t blockedUs = 3000;

// Current phase: true = blocked segment
bool     phaseBlocked = false;
uint32_t phaseStartUs = 0;

void applySettings()
{
	periodUs  = 1000000UL / freqHz;
	blockedUs = (uint32_t)((uint64_t)periodUs * dutyPct / 100);
}

// Set outputs for a beam state. PNP: clear = HIGH on main; NPN inverted.
// Comp is always the complement of Main.
void writeBeam(bool blocked)
{
	bool mainLevel = npn ? blocked : !blocked;
	digitalWrite(MAIN_PIN, mainLevel ? HIGH : LOW);
	digitalWrite(COMP_PIN, mainLevel ? LOW : HIGH);
}

void printStatus()
{
	Serial.print("duty=");
	Serial.print(dutyPct);
	Serial.print("%  freq=");
	Serial.print(freqHz);
	Serial.print("Hz  polarity=");
	Serial.print(npn ? "NPN" : "PNP");
	Serial.print("  ");
	Serial.println(running ? "RUNNING" : (phaseBlocked ? "FROZEN (blocked)" : "FROZEN (clear)"));
}

void setup()
{
	pinMode(MAIN_PIN, OUTPUT);
	pinMode(COMP_PIN, OUTPUT);
	Serial.begin(115200);
	applySettings();
	writeBeam(false);
	phaseStartUs = micros();
	Serial.println("SensorSim ready. Commands: d <pct>, f <hz>, p, s, g, b, c, ?");
	printStatus();
}

void handleSerial()
{
	if (!Serial.available()) return;
	String line = Serial.readStringUntil('\n');
	line.trim();
	if (line.length() == 0) return;

	char cmd = line.charAt(0);
	long val = line.substring(1).toInt();

	switch (cmd)
	{
		case 'd':
			if (val >= 0 && val <= 100) { dutyPct = (uint16_t)val; applySettings(); }
			break;
		case 'f':
			if (val >= 1 && val <= 2000) { freqHz = (uint16_t)val; applySettings(); }
			break;
		case 'p': npn = !npn; break;
		case 's': running = false; break;
		case 'g': running = true; phaseStartUs = micros(); break;
		case 'b': running = false; phaseBlocked = true;  writeBeam(true);  break;
		case 'c': running = false; phaseBlocked = false; writeBeam(false); break;
		case '?': break;
		default:
			Serial.println("unknown — d <pct>, f <hz>, p, s, g, b, c, ?");
			return;
	}
	printStatus();
}

void loop()
{
	handleSerial();
	if (!running) return;

	// 0% and 100% duty are steady levels — no edges by definition
	if (dutyPct == 0)   { writeBeam(false); phaseBlocked = false; return; }
	if (dutyPct == 100) { writeBeam(true);  phaseBlocked = true;  return; }

	uint32_t now = micros();
	uint32_t segLen = phaseBlocked ? blockedUs : (periodUs - blockedUs);
	if (now - phaseStartUs >= segLen)
	{
		phaseBlocked = !phaseBlocked;
		phaseStartUs = now;
		writeBeam(phaseBlocked);
	}
}
