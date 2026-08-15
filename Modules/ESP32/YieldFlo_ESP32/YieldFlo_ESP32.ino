
#include "src/ESP2SOTA_RC/index_html.h"
#include "src/ESP2SOTA_RC/ESP2SOTA_RC.h"	// modified from https://github.com/pangodream/ESP2SOTA

#include <WiFi.h>
#include <ESPmDNS.h>
#include <WebServer.h>
#include <DNSServer.h>

#include <WiFiUdp.h>
#include <WiFiClient.h>
#include <EEPROM.h>
#include <Wire.h>

#include <SPI.h>
#include <Ethernet_Generic.h>	// W5500 wired ethernet (same library as AOG_RC)
#include <EthernetUdp.h>

#include "driver/twai.h"

// YieldFlo module, board: DOIT ESP32 DEVKIT V1
#define InoDescription "YieldFlo_ESP32"
#define InoID 15086         // firmware version — update with every build (DDMMY format)
#define StructVersion 5     // EEPROM layout version — increment ONLY when ModuleData fields change

// Comm modes
const uint8_t CommModeWifi = 0;
const uint8_t CommModeCan = 1;
const uint8_t CommModeEth = 2;

const uint8_t NC = 0xFF;		// Pin not connected
const uint8_t ModStringLengths = 15;
const uint16_t EEPROM_SIZE = 512;
const int16_t ADS1115_Address = 0x48;

// analog
int16_t MoistureReading = 0;
int16_t TemperatureReading = 0;
bool ADSfound = false;
volatile bool ADSconversionReady = false;
uint32_t LastADSReadMs = 0;   // set on every successful conversion-register read

void IRAM_ATTR onADSReady()
{
	ADSconversionReady = true;
}

// ISR forward declarations — the Arduino sketch preprocessor does not
// auto-generate prototypes for functions with IRAM_ATTR.
void IRAM_ATTR onSensorEdge();
void IRAM_ATTR onRPMedge();
void IRAM_ATTR CommitEdge(bool blocked, uint32_t at);

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

struct ModuleConfig
{
	uint8_t ID = 0;
	char APname[ModStringLengths] = "YieldFlo_ESP32";
	char APpassword[ModStringLengths] = "";
	bool WifiModeUseStation = false;				// false - AP mode, true - AP + Station
	char SSID[ModStringLengths] = "Tractor";		// name of network ESP32 connects to
	char Password[ModStringLengths] = "111222333";
	bool ADS1115Enabled = true;
	uint8_t RPMpin = 35;
	uint8_t CompPin = 32;	// complementary PNP signal from light sensor
	uint8_t MainPin = 33;	// main PNP signal from light sensor
	bool UseCompSignal = true;	// true = Main + Comp noise rejection, false = Main only (e.g. FarmTrx tap, Comp not wired)
	bool InvertSensor = false;	// false = PNP (HIGH = beam clear, FarmTrx), true = NPN (inverted logic)
	uint8_t AlertPin = 16;
	uint8_t AnalogPin = NC;
	uint8_t CommMode = CommModeWifi;	// 0 = WiFi UDP, 1 = CAN bus, 2 = Ethernet UDP
	uint8_t CanTxPin = 14;			// TWAI TX → MCP2562 TXD
	uint8_t CanRxPin = 27;			// TWAI RX ← MCP2562 RXD
	uint8_t EthIP0 = 192;			// Ethernet subnet — module IP is EthIP0.EthIP1.EthIP2.(50+ID)
	uint8_t EthIP1 = 168;
	uint8_t EthIP2 = 1;
	uint8_t StaChannelCache = 0;	// channel the station network was last found on; 0 = unknown
};
ModuleConfig MDL;

// ethernet (W5500 on VSPI: SCK 18, MISO 19, MOSI 23, SS 5 — same wiring as AOG_RC ESP32)
const uint8_t W5500_SS = 5;
EthernetUDP UDP_Ethernet;
bool EthChipFound = false;
IPAddress Ethernet_DestinationIP;

// wifi
WiFiUDP UDP_Wifi;
IPAddress Wifi_DestinationIP(192, 168, 100, 255);
WiFiClient client;
WebServer server(80);
DNSServer dnsServer;
const byte AP_DNS_PORT = 53;
const uint16_t ListeningPort = 28001;
const uint16_t ModuleSendPort = 30100;	// PC receive port

// WiFi station connection management lives in Wifi.ino — the event handlers,
// the paced retry, and the policy behind both.

const uint16_t LoopTime = 50;   // ms = 20 Hz (analog reads)
uint32_t       LoopLast = LoopTime;
const uint16_t FlowTime = 200;  // ms — flow window matches the send period, so every
uint32_t       FlowLast = FlowTime;   // blocked/clear µs since the last packet is counted
const uint16_t SendTimePK1 = 200;  // ms = 5 Hz  (main data packet)
uint32_t       SendLastPK1 = SendTimePK1;
const uint16_t SendTimePK2 = 1000; // ms = 1 Hz  (temperature packet)
uint32_t       SendLastPK2 = SendTimePK2;
// Identity/health packet. Nothing in it changes fast except uptime, so it is
// sent slowly — it exists so a field log can answer "which firmware, what
// configuration, did it restart", not to be plotted.
const uint16_t SendTimePK3 = 5000; // ms = 0.2 Hz (identity/health packet)
uint32_t       SendLastPK3 = SendTimePK3;

// Why the module last restarted, normalised to the same codes on both
// platforms so the app does not need to know which one it is talking to:
//   0 unknown  1 power-on  2 reset pin  3 software  4 watchdog
//   5 brownout  6 panic/fault  7 other
// Captured once at boot because the hardware flags are cleared after reading.
uint8_t ResetReasonCode = 0;

void setup()
{
	DoSetup();
}

void loop()
{
	dnsServer.processNextRequest();
	server.handleClient();
	ReceiveComm();
	ServiceWifiStation();   // paced station reconnect — see Wifi.ino
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
	if (MDL.CommMode == CommModeCan)
	{
		CheckCanBus();
		SendCAN();
	}
	else
	{
		SendUdp();   // WiFi or Ethernet, per MDL.CommMode
	}
	//Blink();
}

bool GoodCRC(byte Data[], byte Length)
{
	byte ck = CRC(Data, Length - 1, 0);
	bool Result = (ck == Data[Length - 1]);
	return Result;
}

byte CRC(byte Chk[], byte Length, byte Start)
{
	byte Result = 0;
	for (int i = Start; i < Length; i++)
	{
		Result += Chk[i];
	}
	return Result;
}

//bool State = false;
//uint32_t LastBlink;
//uint32_t LastLoop;
//byte ReadReset;
//uint32_t MaxLoopTime;
//double debug1;
//double debug2;
//
//// max loop about 2500, 18/Sep/2025
//void Blink()
//{
//	if (millis() - LastBlink > 1000)
//	{
//		LastBlink = millis();
//		State = !State;
//
//		Serial.print(MaxLoopTime);
//
//		Serial.print(", ");
//		Serial.print(debug1);
//
//		Serial.print(", ");
//		Serial.print(debug2);
//
//		//Serial.print(", ");
//		//Serial.print(WifiMasterOn);
//
//		Serial.print(", ");
//		Serial.print(Sensor[0].TotalPulses);
//
//		Serial.println("");
//
//		if (ReadReset++ > 5)
//		{
//			ReadReset = 0;
//			MaxLoopTime = 0;
//		}
//	}
//	if (micros() - LastLoop > MaxLoopTime) MaxLoopTime = micros() - LastLoop;
//	LastLoop = micros();
//}

