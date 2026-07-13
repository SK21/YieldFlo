# YieldFlo_ESP32

YieldFlo module firmware for the ESP32 (DOIT ESP32 DEVKIT V1). Reads the
optical grain-flow sensor, the Moisture1 daughter board (ADS1115) and an
optional RPM sensor, and sends the data to the PC app over **WiFi UDP,
wired Ethernet UDP (W5500) or CAN bus** — selectable at runtime in the
web portal.

Settings are stored in EEPROM and edited through the module's web portal;
nothing needs to be recompiled to reconfigure. (For the CAN-only STM32 port
with compile-time settings, see `Modules/STM32F1`.)

## Web portal

The module always runs a WiFi access point, even in CAN mode:

- AP name: `YieldFlo_ESP32_<8 hex digits of the MAC>`; open network unless an
  AP password of 8+ characters is set in the portal.
- Portal address: `192.168.<200 + module ID>.1` (default ID 0 → `192.168.200.1`).
  A captive-portal DNS redirects any address, and mDNS answers at `yieldflo`.
- Firmware update page: `/update` (ESP2SOTA). Prebuilt images live one folder
  up (`YieldFlo_ESP32.ino.bin`).

Portal settings:

| Setting | Meaning |
|---|---|
| Comm mode | WiFi UDP, CAN bus, or Ethernet UDP (W5500) |
| Ethernet subnet | First three octets of the wired network (default `192.168.1`). Module IP is `subnet.(50 + ID)`, /24, broadcast to `subnet.255`. In Ethernet mode the portal shows W5500/link status |
| Signals | *Main + Comp* noise rejection, or *Main only* (e.g. FarmTrx tap — Comp not wired) |
| Network / Password + Connect | Optional station mode: also join an existing WiFi network (default `Tractor`). After repeated failures the module reverts to AP-only and restarts |
| AP password | Password for the module's own access point |

Settings are validated against an EEPROM layout version (`StructVersion`) —
bumping it in the source wipes stored settings back to defaults.

## Hardware / pins (defaults)

| Signal | GPIO | Notes |
|---|---|---|
| Optical sensor Main | 33 | 3.3 V logic — condition/divide on the board (YF1) |
| Optical sensor Comp | 32 | Same. Unused in Main-only mode |
| RPM sensor | 35 | Input-only pin, no internal pull-up |
| ADS1115 SDA | 21 | Through PCA9306 level shifter on the YF1 board |
| ADS1115 SCL | 22 | Same |
| ADS1115 ALERT/RDY | 16 | Open-drain active-low, conversion-ready interrupt |
| CAN TX | 14 | → MCP2562 TXD |
| CAN RX | 27 | ← MCP2562 RXD |
| W5500 SS | 5 | Ethernet board chip select (same wiring as AOG_RC) |
| W5500 SCK / MISO / MOSI | 18 / 19 / 23 | VSPI defaults; W5500 also needs 3.3 V + GND |
| Debug UART | USB serial | 38400 baud, boot messages + CAN warnings |

Pin assignments live in `ModuleConfig` (EEPROM) but are not exposed on the
portal — change the defaults in the source if a board revision moves them.

## Building

Arduino IDE (or arduino-cli) with the **esp32 core** (tested with 3.3.7),
board **DOIT ESP32 DEVKIT V1**. One external library: **Ethernet_Generic**
(install via Library Manager — the same library AOG_RC uses for the W5500).
The modified ESP2SOTA OTA library is bundled in `src/ESP2SOTA_RC/` (it must
stay under `src/` so Arduino builds compile it). The `.vcxproj` / `__vm`
files are a Visual Micro project for building from Visual Studio; plain
Arduino IDE users can ignore them.

## Protocol

Both transports carry the same 8-byte data body:

| Bytes | Field |
|---|---|
| 0 | status_flags: bit0=SensorOK, bit1=RPMPresent, bit2=MoistureOK |
| 1-2 | sensor_ratio uint16 LE (ratio × 1000) |
| 3-4 | moisture_raw uint16 LE (raw ADS1115 AIN0-AIN1 differential) |
| 5-6 | module_rpm uint16 LE (fixed 200 when no RPM sensor) |
| 7 | noise_count (ISR-rejected edges per 200 ms window, capped 255) |

**WiFi / Ethernet UDP** — module listens on port 28001, sends broadcast to
port 30100 (Ethernet mode broadcasts to `subnet.255` on the wired network;
packet format is identical):

- Data packet, 5 Hz: 11 bytes — PGN 40001 LE, 8-byte body, CRC8 (byte sum)
- Temperature packet, 1 Hz: 7 bytes — PGN 40002 LE, flags (bit0=TempOK,
  bit1=PaddleHzPresent), temp_raw int16 LE (raw ADS1115 AIN2),
  paddle_hz uint8 (paddles/s), CRC8

**CAN bus** — 250 kbps, extended IDs, DLC 8:

- Data frame, 5 Hz: `0x18FF00F8`, 8-byte body (no CRC byte — CAN has its own)
- Temperature frame, 1 Hz: `0x18FF01F8`, [0]=flags, [1-2]=temp_raw,
  [3]=paddle_hz (paddles/s), [4-7]=0

CAN Bus Off is retried every 3 s; after 5 failed recoveries the module falls
back to WiFi for the session (EEPROM unchanged — CAN returns on restart).

## Firmware version

`InoID` encodes the build date as DDMMY (e.g. 4076 → 2026-07-04). Update it
with every build; the boot banner prints it decoded.
