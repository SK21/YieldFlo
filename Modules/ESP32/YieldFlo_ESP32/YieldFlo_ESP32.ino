
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
#define InoID 13076         // firmware version — update with every build (DDMMY format)
#define StructVersion 4     // EEPROM layout version — increment ONLY when ModuleData fields change

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
bool SensorOK = false;
uint16_t SensorRatio = 0;		// ratio × 1000, updated by ReadFlow()

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

uint8_t DisconnectCount = 0;
void WiFiStationConnected(WiFiEvent_t event, WiFiEventInfo_t info)
{
	Serial.print("Connected to '");
	Serial.print(MDL.SSID);
	Serial.println("'");
}

void WiFiGotIP(WiFiEvent_t event, WiFiEventInfo_t info)
{
	Serial.print("Network IP: ");
	Serial.println(WiFi.localIP());
	IPAddress Wifi_LocalIP = WiFi.localIP();
	Wifi_DestinationIP = IPAddress(Wifi_LocalIP[0], Wifi_LocalIP[1], Wifi_LocalIP[2], 255);
}

void WiFiStationDisconnected(WiFiEvent_t event, WiFiEventInfo_t info)
{
	Serial.println("Disconnected from WiFi access point");
	Serial.print("WiFi lost connection. Reason: ");
	Serial.println(info.wifi_sta_disconnected.reason);
	Serial.print("Trying to Reconnect: ");
	DisconnectCount++;
	Serial.println(DisconnectCount);
	WiFi.begin(MDL.SSID, MDL.Password);

	if (DisconnectCount > 5)
	{
		// use AP mode only
		MDL.WifiModeUseStation = false;
		SaveData();
		ESP.restart();
	}
}

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
	dnsServer.processNextRequest();
	server.handleClient();
	ReceiveComm();
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

