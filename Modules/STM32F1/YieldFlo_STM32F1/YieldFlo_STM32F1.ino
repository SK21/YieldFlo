
#include <Wire.h>
#include "STM32_CAN.h"

// YieldFlo module — STM32F1 port, CAN bus only.
// Board: STM32F103C8 "Blue Pill" (or any F103 with CAN)
// Core:  Official STMicroelectronics Arduino core (stm32duino)
// Lib:   STM32_CAN by pazi88 (Library Manager)
//
// Ported from YieldFlo_ESP32. WiFi, web portal and OTA are removed.
// All settings are compile-time constants below — edit and reflash.
//
// IMPORTANT: USB and CAN share SRAM on the F103 and cannot be used together.
// Build with USB support set to "None"; debug output is on USART1 (PA9/PA10).

#define InoDescription "YieldFlo_STM32F1"
#define InoID 5076          // firmware version — update with every build (DDMMY format)

// ── User settings (compile-time) ─────────────────────────────────────────
const uint8_t ModuleID       = 0;     // module ID (informational)
const bool    UseCompSignal  = true;  // true = Main + Comp noise rejection, false = Main only (e.g. FarmTrx tap, Comp not wired)
const bool    InvertSensor   = false; // false = PNP (HIGH = beam clear, FarmTrx), true = NPN (inverted logic)
const bool    RPMEnabled     = true;  // RPM sensor wired to RPMPin
const bool    ADS1115Enabled = true;  // Moisture1 daughter board present

// ── Pin map ──────────────────────────────────────────────────────────────
// All interrupt pins must have distinct pin ordinals: EXTI lines are shared
// across ports by pin number (PA0 and PB0 both use EXTI0).
// If MainPin or CompPin change, update the matching PinName constants in
// Flow.ino (MainPinName / CompPinName) — they must refer to the same pins.
const uint32_t MainPin   = PB0;   // main PNP signal from light sensor   (EXTI0)
const uint32_t CompPin   = PB1;   // complementary PNP signal            (EXTI1)
const uint32_t RPMPin    = PB10;  // RPM sensor                          (EXTI10)
const uint32_t AlertPin  = PB5;   // ADS1115 ALERT/RDY, open-drain       (EXTI5)
const bool     UseAnalogFallback = false;  // native ADC moisture when no ADS1115
const uint32_t AnalogPin = PA0;   // fallback ADC input (only if enabled above)
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

// optical sensor ISR state
volatile uint32_t BlockedAccum = 0;		// µs beam blocked this window
volatile uint32_t ClearAccum = 0;		// µs beam clear this window
volatile uint32_t LastEdgeUs = 0;		// micros() at last valid edge — set ONLY in the ISR (sensor health)
volatile uint32_t SegStartUs = 0;		// micros() at start of the current blocked/clear segment
volatile bool BeamBlocked = false;		// current beam state
volatile uint16_t NoiseCount = 0;		// rejected noise edges this window
bool SensorOK = false;
uint16_t SensorRatio = 0;		// ratio × 1000, updated by ReadFlow()

// RPM ISR state
volatile uint32_t RPMpulseCount = 0;
volatile uint32_t LastRPMedgeUs = 0;

// CAN — CAN1 on PB8 (RX) / PB9 (TX)
STM32_CAN Can(CAN1, ALT);

const uint16_t LoopTime = 50;   // ms = 20 Hz
uint32_t       LoopLast = LoopTime;
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
		ReadFlow();
	}
	CheckCanBus();
	SendCAN();
}
