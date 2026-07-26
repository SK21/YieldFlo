
#include <Wire.h>
#include "Can.h"

// YieldFlo module — STM32F1 port, CAN bus only.
// Board: STM32F103C8 "Blue Pill" (or any F103 with CAN)
// Core:  Official STMicroelectronics Arduino core (stm32duino)
// Lib:   Can.h — bare-metal bxCAN register driver (bundled in this sketch)
//
// Ported from YieldFlo_ESP32. WiFi, web portal and OTA are removed.
// All settings are compile-time constants below — edit and reflash.
//
// IMPORTANT: USB and CAN share SRAM on the F103 and cannot be used together.
// Build with USB support set to "None"; debug output is on USART1 (PA9/PA10).

#define InoDescription "YieldFlo_STM32F1"
#define InoID 26076          // firmware version — update with every build (DDMMY format)

// ── User settings (compile-time) ─────────────────────────────────────────
const uint8_t ModuleID       = 0;     // module ID (informational)
const bool    UseCompSignal  = true;  // true = Main + Comp noise rejection, false = Main only (e.g. FarmTrx tap, Comp not wired)
const bool    InvertSensor   = true;  // true = NPN (inverted logic — default for this port), false = PNP (HIGH = beam clear, FarmTrx)
const bool    RPMEnabled     = true;  // RPM sensor wired to RPMPin
const bool    ADS1115Enabled = true;  // Moisture1 daughter board present
const bool    DebugLED       = true;  // PC13 onboard LED mirrors beam-blocked state (lit = blocked)

// Paddle pitch of the clean-grain elevator chain, mm. NOT used in any
// calculation — the CAN payload is deliberately pitch-free (see Flow.ino) so a
// wrong value here can never bias the yield. It is printed at boot as the
// conversion factor an operator needs to read the paddle channel as a grain
// column speed:  mm/s = flow_sum/1000 / window_s × PaddlePitchMm.
const uint16_t PaddlePitchMm = 190;

// ── Pin map ──────────────────────────────────────────────────────────────
// All interrupt pins must have distinct pin ordinals: EXTI lines are shared
// across ports by pin number (PA0 and PB0 both use EXTI0) — boot prints a
// warning if enabled interrupt pins clash. Flow.ino derives its fast-read
// PinNames from these constants automatically, so nothing else needs editing
// when a pin changes.
const uint32_t MainPin   = PB0;   // main signal from light sensor       (EXTI0)
const uint32_t CompPin   = PB1;   // complementary signal                (EXTI1)
const uint32_t RPMPin    = PB10;  // RPM sensor                          (EXTI10)
const uint32_t AlertPin  = PB5;   // ADS1115 ALERT/RDY, open-drain       (EXTI5)
const bool     UseAnalogFallback = false;  // native ADC moisture when no ADS1115
const uint32_t AnalogPin = PA0;   // fallback ADC input (only if enabled above)
const uint32_t LedPin    = PC13;  // onboard LED, active-low — lit when beam blocked (debug)
// I2C1:  PB6 = SCL, PB7 = SDA (Wire defaults)
// CAN1:  PB8 = RX,  PB9 = TX  (remapped — PA11/PA12 are the USB pins)
// USART1: PA9 = TX, PA10 = RX (debug "Serial", 38400 baud)

const int16_t ADS1115_Address = 0x48;

// analog
int16_t MoistureReading = 0;
int16_t TemperatureReading = 0;
bool ADSfound = false;
volatile bool ADSconversionReady = false;
uint32_t LastADSReadMs = 0;   // set on every successful conversion-register read

void onADSReady()
{
	ADSconversionReady = true;
}

// optical sensor ISR state — ratio is accounted per signal CYCLE (closed at each
// clear→blocked edge), so the reported value is exact at any pulse rate and a
// fixed read window can never alias against the signal period.
volatile uint32_t CycStartUs = 0;		// micros() at the clear→blocked edge that started this cycle
volatile uint32_t CycBlockedUs = 0;		// µs blocked within the current cycle
volatile uint32_t WinBlockedUs = 0;		// blocked µs of completed cycles since last ReadFlow
volatile uint32_t WinTotalUs = 0;		// total µs of completed cycles since last ReadFlow
volatile uint32_t LastEdgeUs = 0;		// micros() at last committed edge — set ONLY by CommitEdge (sensor health)
volatile uint32_t SegStartUs = 0;		// micros() at start of the current blocked/clear segment
volatile bool BeamBlocked = false;		// current committed beam state
volatile uint16_t NoiseCount = 0;		// rejected noise edges this window
volatile uint16_t PaddleCycles = 0;		// completed paddle cycles since last TakePaddleHz()
volatile uint32_t MinCycleUs = 0xFFFFFFFF;	// shortest completed paddle cycle since last TakeMinCycleMs()
bool SensorOK = false;
uint16_t SensorRatio = 0;		// ratio × 1000, updated by ReadFlow()

// ── Paddle-event channel ─────────────────────────────────────────────────
// Second, independent reduction of the SAME committed edge stream. The duty
// channel above answers "what fraction of the time is the beam blocked"; this
// one answers "how much beam obstruction went past, per paddle". They differ
// exactly where the duty channel is weakest — a paddle cycle split in two by a
// kernel bridging the inter-paddle gap, a missed edge merging two paddles into
// one, or the elevator running off its nominal speed — so disagreement between
// them is a usable fault signal rather than redundancy. Nothing here feeds back
// into the duty channel: its numbers are bit-identical to the previous firmware.
volatile uint32_t PeriodEmaUs = 0;			// running mean accepted paddle period (µs)
volatile uint32_t CarryCycUs = 0;			// duration of a too-short cycle held for merging
volatile uint32_t CarryBlockedUs = 0;		// its blocked µs, carried with it
volatile uint32_t WinFlowSumX1000 = 0;		// Σ(per-paddle duty × pitches spanned) × 1000
volatile uint32_t WinAccountedUs = 0;		// Σ duration of accepted paddle cycles
volatile uint16_t WinPaddles = 0;			// Σ pitches spanned by accepted cycles
volatile uint16_t WinRejects = 0;			// cycles merged, resynced or scaled this window
volatile uint16_t WinSatPaddles = 0;		// paddles at/above SatDutyX1000

// Per-paddle duty at or above this is saturated: the beam is blocked for
// essentially the whole pitch, so extra grain adds no signal and the channel
// under-reads. Reported so the app can flag it instead of mapping a false low.
const uint16_t SatDutyX1000 = 900;

// Cycle length limits, as a fraction of the running period estimate. Below
// MinCycNum/MinCycDen the cycle is a fragment and is carried into the next one.
// MaxPitchSpan caps how many paddles one cycle may be credited with, so a beam
// parked blocked cannot inject a large phantom flow.
const uint32_t MinCycNum = 1;
const uint32_t MinCycDen = 2;		// < 0.5 × period estimate = fragment
const uint32_t MaxPitchSpan = 8;

// Snapshots taken by ReadFlow() for the 5 Hz paddle packet
bool     FlowValid = false;		// at least one paddle accounted this window
uint16_t FlowSumX1000 = 0;		// Σ(duty × pitches) × 1000
uint16_t FlowAccountedMs = 0;	// ms of window time inside accepted cycles
uint8_t  FlowPaddles = 0;		// pitches accounted
uint8_t  FlowRejects = 0;
uint8_t  FlowSatPaddles = 0;
bool     FlowUnaccounted = false;	// >25% of the window fell outside accepted cycles

// Glitch filter: an edge is committed only after the state it starts survives
// GlitchMinUs. EMI pulses are ~0.1 ms wide; the shortest real paddle segment
// is >10 ms on any elevator, so 2 ms separates them with wide margin on both
// sides. A pending edge cancelled by a return edge inside the window is a
// glitch pair — both edges are discarded and counted in NoiseCount (which
// therefore works in Main-only mode too, where there is no Comp cross-check).
const uint32_t GlitchMinUs = 2000;
volatile bool     PendingValid = false;		// an uncommitted transition is held
volatile bool     PendingBlocked = false;	// the state that transition switches to
volatile uint32_t PendingTimeUs = 0;		// micros() of the pending edge

// RPM ISR state
volatile uint32_t RPMpulseCount = 0;
volatile uint32_t LastRPMedgeUs = 0;

// CAN — CAN1 on PB8 (RX) / PB9 (TX). The Can.h driver is stateless (no object);
// it is initialised in DoSetup() via CANInit(CAN_250KBPS, 2) — remap 2 = PB8/PB9.

const uint16_t LoopTime = 50;   // ms = 20 Hz (analog reads)
uint32_t       LoopLast = LoopTime;
const uint16_t FlowTime = 200;  // ms — flow window matches the send period, so every
uint32_t       FlowLast = FlowTime;   // blocked/clear µs since the last packet is counted
const uint16_t SendTimePK1 = 200;  // ms = 5 Hz  (main data packet)
uint32_t       SendLastPK1 = SendTimePK1;
const uint16_t SendTimePK2 = 1000; // ms = 1 Hz  (temperature packet)
uint32_t       SendLastPK2 = SendTimePK2;
const uint16_t SendTimePK3 = 200;  // ms = 5 Hz  (paddle-event flow packet — same
uint32_t       SendLastPK3 = SendTimePK3;   // window as PK1 so both channels pair up)

void setup()
{
	DoSetup();
}

void loop()
{
	DrainCanRx();
	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();
		ReadAnalog();
	}
	if (millis() - FlowLast >= FlowTime)
	{
		FlowLast = millis();
		ReadFlow();
	}
	CheckCanBus();
	SendCAN();
}
