
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
#define InoID 8076          // firmware version — update with every build (DDMMY format)

// ── User settings (compile-time) ─────────────────────────────────────────
const uint8_t ModuleID       = 0;     // module ID (informational)
const bool    UseCompSignal  = true;  // true = Main + Comp noise rejection, false = Main only (e.g. FarmTrx tap, Comp not wired)
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
volatile uint32_t LastEdgeUs = 0;		// micros() at last valid edge — set ONLY in the ISR (sensor health)
volatile uint32_t SegStartUs = 0;		// micros() at start of the current blocked/clear segment
volatile bool BeamBlocked = false;		// current beam state
volatile uint16_t NoiseCount = 0;		// rejected noise edges this window
bool SensorOK = false;
uint16_t SensorRatio = 0;		// ratio × 1000, updated by ReadFlow()

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
