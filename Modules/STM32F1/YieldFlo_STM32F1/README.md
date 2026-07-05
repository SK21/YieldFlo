# YieldFlo_STM32F1

CAN-only port of the YieldFlo module firmware for the STM32F103 ("Blue Pill").
Sends the same CAN frames as the ESP32 module in CAN mode — the PC app works
unchanged (enable CAN mode in the app; SLCAN / PCAN / InnoMaker adapters all work).

WiFi, the web portal, and OTA updates are **not** included. All settings are
compile-time constants at the top of `YieldFlo_STM32F1.ino` — edit and reflash.

## Settings (top of YieldFlo_STM32F1.ino)

| Constant | Default | Meaning |
|---|---|---|
| `ModuleID` | 0 | Module ID (informational, printed at boot) |
| `UseCompSignal` | true | Main + Comp noise rejection. Set false when Comp is not wired (e.g. FarmTrx tap — Main only) |
| `RPMEnabled` | true | RPM sensor wired to `RPMPin`. When false the module reports the fixed reference 200 |
| `ADS1115Enabled` | true | Moisture1 daughter board present |
| `UseAnalogFallback` | false | Read moisture from the native ADC (`AnalogPin`) when there is no ADS1115 |

## Hardware

- STM32F103C8T6 board (Blue Pill) — beware of counterfeit chips; CAN is one of
  the peripherals that misbehaves on some clones.
- CAN transceiver (MCP2562, TJA1050, SN65HVD230, …). With an MCP2562 run VIO at
  3.3 V. Remember the 120 Ω termination if the module is at the end of the bus.
- ST-Link V2 for flashing (SWD pins on the board end).

### Wiring

| Signal | Pin | Notes |
|---|---|---|
| Optical sensor Main | PB0 | **3.3 V logic only** — PB0 is not 5 V tolerant. Use the same conditioning/divider as the YF1 board |
| Optical sensor Comp | PB1 | Same — 3.3 V only. Leave unconnected in Main-only mode |
| RPM sensor | PB10 | 5 V tolerant |
| ADS1115 SCL | PB6 | 5 V tolerant, open-drain — OK with a 5 V ADS1115 through the PCA9306, or direct if the bus is pulled to 3.3 V |
| ADS1115 SDA | PB7 | Same |
| ADS1115 ALERT/RDY | PB5 | Open-drain active-low, internal pull-up to 3.3 V — safe with a 5 V ADS1115 |
| CAN RX | PB8 | To transceiver RXD |
| CAN TX | PB9 | To transceiver TXD |
| Debug UART TX | PA9 | 38400 baud, boot messages + bus-off warnings |
| Debug UART RX | PA10 | Unused (no commands) |

The interrupt pins (PB0, PB1, PB10, PB5) were chosen with distinct pin
ordinals on purpose: EXTI lines are shared across ports by pin number
(PA0 and PB0 both use EXTI0), so no two interrupt pins may share an ordinal.

## Building (Arduino IDE)

1. Install the **official STM32 core**: File → Preferences → Additional boards
   manager URLs →
   `https://github.com/stm32duino/BoardManagerFiles/raw/main/package_stmicroelectronics_index.json`
   then Boards Manager → install "STM32 MCU based boards".
2. Library Manager → install **STM32_CAN** (pazi88).
   The sketch folder's `hal_conf_extra.h` enables the HAL CAN module that the
   core ships disabled — the library won't compile without it, so keep that
   file with the sketch.
3. Tools menu:
   - Board: *Generic STM32F1 series* → Board part number: *Generic F103C8Tx*
     (or *…CBTx* for 128 KB parts)
   - **USB support (if available): "None"** — required. USB and CAN share SRAM
     on the F103 and cannot run together; PA11/PA12 are also the CAN default
     pins. This sketch remaps CAN to PB8/PB9 and uses USART1 for debug.
   - U(S)ART support: *Enabled (generic 'Serial')* — maps `Serial` to USART1
     on PA9/PA10.
   - Upload method: *STM32CubeProgrammer (SWD)* with an ST-Link.

## CAN protocol (identical to the ESP32 module)

250 kbps, extended IDs.

**Data frame — 0x18FF00F8, DLC 8, 5 Hz**

| Bytes | Field |
|---|---|
| 0 | status_flags: bit0=SensorOK, bit1=RPMPresent, bit2=MoistureOK |
| 1-2 | sensor_ratio uint16 LE (ratio × 1000) |
| 3-4 | moisture_raw uint16 LE (raw ADS1115 AIN0-AIN1 differential) |
| 5-6 | module_rpm uint16 LE (fixed 200 when `RPMEnabled` is false) |
| 7 | noise_count (ISR-rejected edges per 200 ms window, capped 255) |

**Temperature frame — 0x18FF01F8, DLC 8, 1 Hz**

| Bytes | Field |
|---|---|
| 0 | flags: bit0=TempOK |
| 1-2 | temp_raw int16 LE (raw ADS1115 AIN2) |
| 3-7 | 0 |

## Differences from the ESP32 module

- No WiFi / web portal / OTA — settings are compile-time constants.
- Bus Off recovery is done in hardware (bxCAN ABOM) instead of the TWAI
  recovery state machine; there is no WiFi fallback.
- TX is single-shot: a frame that gets no ACK (nothing else on the bus) is
  dropped rather than retried, so a lone module never floods the bus with
  error frames. Data resumes automatically when a listener appears.
- No EEPROM use — nothing is stored on the chip.
