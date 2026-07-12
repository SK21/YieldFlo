// SensorSim — optical grain-flow sensor simulator for YieldFlo bench testing.
//
// Drives the module's Main + Comp inputs with a complementary square wave,
// like the elevator optical sensor: blocked fraction of each period = the
// sensor ratio the module should report.
//
// Board: any 3.3 V Arduino-compatible (ESP32 devkit, ESP8266 D1 mini/pro,
// STM32 Blue Pill, ...).
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
//   j 10      jitter — randomize each cycle's blocked time by ±% (0–50, 0 = off).
//             Emulates paddle-to-paddle transit variation (worn/curled paddles,
//             chain whip). Cycle period stays fixed; only the blocked segment moves.
//   x 5       glitches per second (0–100, 0 = off) — brief ~100 µs opposite-polarity
//             pulses at random times. Emulates EMI edge noise; in Main-only mode
//             the module has no complementary check, so these pass straight through.
//   p         toggle polarity: PNP (default, HIGH = clear) / NPN (inverted)
//   s         stop — freeze outputs at the current level (no edges; tests
//             the module's no-pulse handling: SensorOK must drop in 500 ms)
//   g         go — resume pulsing
//   b         freeze in the BLOCKED state (reproduces "beam latched blocked")
//   c         freeze in the CLEAR state
//   ?         print current settings
//
// Defaults: 100 Hz, 30 % blocked, PNP, no jitter, no glitches, running.

#if defined(ESP8266)
#include <ESP8266WiFi.h>
// Board pins are labeled D0-D8, NOT GPIO numbers: wire the pins marked D1/D2.
const uint8_t MAIN_PIN = 5;   // pin labeled D1 on a WeMos/LOLIN D1 mini (pro)
const uint8_t COMP_PIN = 4;   // pin labeled D2
#else
const uint8_t MAIN_PIN = 25;
const uint8_t COMP_PIN = 26;
#endif

uint16_t dutyPct  = 30;     // % of period blocked
uint16_t freqHz   = 100;
bool     npn      = false;  // false = PNP (HIGH = clear), true = inverted
bool     running  = true;

uint16_t jitterPct    = 0;  // ± % applied to each cycle's blocked time
uint16_t glitchPerSec = 0;  // brief opposite-polarity pulses per second

uint32_t periodUs  = 10000;
uint32_t blockedUs = 3000;

// Current phase: true = blocked segment
bool     phaseBlocked = false;
uint32_t phaseStartUs = 0;

uint32_t cycleBlockedUs = 3000;  // this cycle's (possibly jittered) blocked time
uint32_t nextGlitchUs   = 0;

void applySettings()
{
	periodUs  = 1000000UL / freqHz;
	blockedUs = (uint32_t)((uint64_t)periodUs * dutyPct / 100);
	cycleBlockedUs = blockedUs;
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
	Serial.print("Hz  jitter=+/-");
	Serial.print(jitterPct);
	Serial.print("%  glitches=");
	Serial.print(glitchPerSec);
	Serial.print("/s  polarity=");
	Serial.print(npn ? "NPN" : "PNP");
	Serial.print("  ");
	Serial.println(running ? "RUNNING" : (phaseBlocked ? "FROZEN (blocked)" : "FROZEN (clear)"));
}

void setup()
{
#if defined(ESP8266)
	// The ESP8266 core starts WiFi by default (and retries any saved SSID),
	// stalling loop() for tens of ms — which stretches our software-timed
	// segments and corrupts the duty cycle. This sketch never needs WiFi.
	WiFi.mode(WIFI_OFF);
	WiFi.forceSleepBegin();
#endif
	pinMode(MAIN_PIN, OUTPUT);
	pinMode(COMP_PIN, OUTPUT);
	Serial.begin(115200);
	randomSeed(micros());
	applySettings();
	writeBeam(false);
	phaseStartUs = micros();
	Serial.println();
	Serial.println("SensorSim ready. Commands: d <pct>, f <hz>, j <pct>, x <n>, p, s, g, b, c, ?");
	printStatus();
}

// Non-blocking command reader. Accepts '\n', '\r', or a 100 ms pause as
// end-of-command, so it works with any serial monitor regardless of its
// line-ending setting (Visual Micro, Arduino IDE, PuTTY, raw port writes).
// readStringUntil() is deliberately avoided: it blocks loop() for up to 1 s,
// which freezes the output waveform while a command is being typed.
char     cmdBuf[16];
uint8_t  cmdLen = 0;
uint32_t lastCharMs = 0;

void handleSerial()
{
	while (Serial.available())
	{
		char ch = (char)Serial.read();
		lastCharMs = millis();
		if (ch == '\n' || ch == '\r') { processCommand(); continue; }
		if (cmdLen < sizeof(cmdBuf) - 1) cmdBuf[cmdLen++] = ch;
	}
	if (cmdLen > 0 && millis() - lastCharMs > 100) processCommand();
}

void processCommand()
{
	if (cmdLen == 0) return;
	cmdBuf[cmdLen] = 0;
	String line = String(cmdBuf);
	cmdLen = 0;
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
		case 'j':
			if (val >= 0 && val <= 50) jitterPct = (uint16_t)val;
			break;
		case 'x':
			if (val >= 0 && val <= 100) { glitchPerSec = (uint16_t)val; nextGlitchUs = micros(); }
			break;
		case 'p': npn = !npn; break;
		case 's': running = false; break;
		case 'g': running = true; phaseStartUs = micros(); break;
		case 'b': running = false; phaseBlocked = true;  writeBeam(true);  break;
		case 'c': running = false; phaseBlocked = false; writeBeam(false); break;
		case '?': break;
		default:
			Serial.println("unknown — d <pct>, f <hz>, j <pct>, x <n>, p, s, g, b, c, ?");
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

	// EMI glitch: brief opposite-polarity pulse at a random moment,
	// averaging glitchPerSec per second
	if (glitchPerSec > 0 && (int32_t)(now - nextGlitchUs) >= 0)
	{
		writeBeam(!phaseBlocked);
		delayMicroseconds(100);
		writeBeam(phaseBlocked);
		uint32_t meanUs = 1000000UL / glitchPerSec;
		nextGlitchUs = now + (uint32_t)random(meanUs / 2, meanUs + meanUs / 2);
	}

	uint32_t segLen = phaseBlocked ? cycleBlockedUs : (periodUs - cycleBlockedUs);
	if (now - phaseStartUs >= segLen)
	{
		phaseBlocked = !phaseBlocked;
		if (phaseBlocked)
		{
			// new cycle — roll this cycle's blocked time; the period is
			// untouched, matching a paddle whose transit past the beam
			// varies while the chain spacing stays fixed
			cycleBlockedUs = blockedUs;
			if (jitterPct > 0)
			{
				int32_t dev = (int32_t)random(-(int32_t)jitterPct, (int32_t)jitterPct + 1);
				cycleBlockedUs = blockedUs + (int32_t)(((int64_t)blockedUs * dev) / 100);
			}
		}
		phaseStartUs = now;
		writeBeam(phaseBlocked);
	}
}
