
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
#define InoID 19086          // firmware version — update with every build (DDMMY format)

// ── User settings (compile-time) ─────────────────────────────────────────
const uint8_t ModuleID       = 0;     // module ID (informational)
// Defaults to the SAFE side: Main-only with Comp wired loses an optimization and
// nothing else (the glitch filter still rejects noise), while Main+Comp with no
// Comp wire discards one edge of every paddle and records nothing at all,
// silently. Turn it on only after confirming Comp is actually connected.
const bool    UseCompSignal  = false; // true = Main + Comp noise rejection, false = Main only (e.g. FarmTrx tap, Comp not wired)
const bool    InvertSensor   = true;  // true = NPN (inverted logic — default for this port), false = PNP (HIGH = beam clear, FarmTrx)
const bool    RPMEnabled     = true;  // RPM sensor wired to RPMPin
const bool    ADS1115Enabled = true;  // Moisture1 daughter board present
const bool    DebugLED       = true;  // PC13 onboard LED mirrors beam-blocked state (lit = blocked)

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

// Comp-wire fault detection. Counts the edges the comp cross-check has discarded
// since the last edge that actually committed; CommitEdge zeroes it, so a single
// real paddle getting through clears it. That is what makes a high value mean
// "every edge is being thrown away" rather than "some noise was rejected" — the
// state is impossible with working wiring, because if edges are arriving often
// enough to be rejected then some of them are paddles and some must commit.
//
// Counts ONLY comp cross-check rejects, not the glitch filter's. That keeps the
// meaning exact: Comp is enabled and is eating the signal, which is a fault with
// a specific remedy the app can name. A blocked pulse narrower than GlitchMinUs
// produces the same "nothing ever commits" outcome through the glitch filter,
// but the fix there is mechanical, so it does not belong behind the same flag.
volatile uint16_t CompRejectsSinceCommit = 0;
bool CompFault = false;			// set by ReadFlow, sent as status_flags bit 3

// Rejects since the last commit before the wiring is called faulty — about 3 s at
// the 18 Hz paddle rate this was measured on. Any committed edge resets the count,
// so ordinary rejected noise on a healthy machine never approaches it.
const uint16_t CompFaultRejects = 50;

// Glitch filter: an edge is committed only after the state it starts survives
// GlitchMinUs. A pending edge cancelled by a return edge inside the window is a
// glitch pair — both edges are discarded and counted in NoiseCount (which
// therefore works in Main-only mode too, where there is no Comp cross-check).
//
// Sizing this needs the BLOCKED segment, not the paddle period:
//     blocked_ms = duty × 1000 / paddle_hz
// Duty is geometric (paddle width ÷ pitch on the chain), so it barely moves with
// elevator speed while the blocked time shrinks as speed rises. A blocked
// segment shorter than the window does not merely degrade the reading — the
// pending transition is dropped and the whole paddle vanishes, so paddle_hz
// reads low and the ratio collapses. Measured machine: 18 Hz at 6% empty duty
// = 3.33 ms, i.e. 1.7× this window. The cliff is at 2 ms, which that machine
// would reach at 30 Hz, or if a worn paddle ran ~40% under nominal width.
//
// An earlier note here claimed "the shortest real paddle segment is >10 ms on
// any elevator" — false. At 18 Hz that needs 18% duty; real baselines measured
// on this machine are 5–10%.
//
// 2 ms is kept deliberately rather than lowered for margin: it was bench-verified
// as the fix for a real EMI complaint, and even at 2 ms it is not fully airtight
// (0.24 rejects/s survived a 20 glitch/s injection). EMI pulses are ~0.1 ms, so
// the window sits 20× above the noise and 1.7× below the signal — the tight side
// is the signal, and the Paddles readout is the check: it must equal the true
// paddle rate.
//
// Note this filter cannot separate a bridging grain kernel from a paddle at this
// geometry: a kernel is a few ms, WIDER than a 3.33 ms paddle. Only the period
// gate below, which tests arrival time rather than width, distinguishes them.
const uint32_t GlitchMinUs = 2000;
volatile bool     PendingValid = false;		// an uncommitted transition is held
volatile bool     PendingBlocked = false;	// the state that transition switches to
volatile uint32_t PendingTimeUs = 0;		// micros() of the pending edge

// ── Period gate ──────────────────────────────────────────────────────────
// A grain kernel crossing the beam between paddles blocks it for several ms —
// long enough to outlive the glitch filter above, so it arrives here as a real
// edge. CommitEdge would treat it as a cycle boundary, splitting one paddle
// pitch into two short cycles and corrupting the ratio. The gate rejects a
// leading edge arriving too early to be a paddle: the cycle simply stays open,
// and the kernel's obstruction counts as blocked time within it, which is what
// it physically is. Nothing is discarded and no repair is needed afterwards.
//
// The threshold is a fraction of the median of recent raw leading-edge
// intervals, recorded BEFORE the gate sees them. A median over raw intervals,
// rather than a mean over accepted ones, for two reasons. A split can only ever
// make an interval shorter, never longer, so the true period sits above the
// corrupted tail and a median walks straight past it — it holds until short
// cycles become the majority, which needs about a third of all pitches to be
// splitting. And recording every interval regardless of the verdict means
// rejections cannot starve the estimator, so the gate has no way to latch
// itself shut the way a feedback-driven threshold would.
const uint8_t  GateRingSize   = 32;		// ~4 s of history at 7.4 paddles/s
const uint8_t  GateMinSamples = 12;		// ring fill required before gating starts
// Reject a leading edge arriving below this percentage of the median period.
// Higher catches a kernel landing further into the inter-paddle gap; lower is
// safer against a genuine fast cycle. A sustained speed change moves the median
// with it, so only an implausibly abrupt one could be clipped. This is the value
// to tune from field min_cycle_ms data — everything else here is
// self-determining. Integer percent rather than a float: it keeps both
// platforms' Flow.ino identical, and on the STM32F1 (Cortex-M3, no FPU) a single
// float multiply links in ~1 KB of soft-float — measured — for arithmetic that
// runs five times a second outside the ISR.
const uint32_t GatePercent = 75;

volatile uint32_t GateRing[GateRingSize];	// raw leading-edge intervals, µs
volatile uint8_t  GateRingCount = 0;
volatile uint8_t  GateRingIndex = 0;
volatile uint32_t LastLeadingUs = 0;	// micros() of the last leading edge, gated or not
volatile uint32_t GateMinCycUs = 0;		// live threshold; 0 = too little history, gate open
volatile uint16_t GateRejects = 0;		// leading edges rejected since last TakeGateRejects()

// The median the threshold above was derived from, cached for the 1 Hz packet.
// Not ISR state and not volatile: ReadFlow writes it and Comm reads it, both
// from loop(). Reporting the median rather than the threshold costs the same
// byte but says more — the threshold is just 75% of it, whereas 0 additionally
// means the estimator is unarmed, and comparing it against 1000/paddle_hz
// measures how contaminated the raw interval stream is (paddle_hz counts gated
// cycles, this median is over raw intervals, so they diverge in proportion to
// the spurious edges arriving).
uint32_t GateMedianUsCache = 0;

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
