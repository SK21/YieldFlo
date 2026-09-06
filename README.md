# YieldFlo

YieldFlo is a combine yield and moisture monitoring system that works alongside [AgOpenGPS](https://github.com/AgOpenGPS-Official/AgOpenGPS). A Windows desktop app records yield rate, moisture, and GPS position continuously while harvesting, and produces spatial yield maps and job reports — paired with open-source hardware that mounts on the combine's clean-grain elevator.

## How it works

- **YieldFlo module** (ESP32 or STM32F1 firmware) reads an optical grain-flow sensor on the clean-grain elevator, an optional capacitance moisture sensor, and an optional RPM sensor, then streams live readings to the PC over WiFi UDP, wired Ethernet UDP, or CAN bus.
- **AgOpenGPS** supplies GPS position and section on/off state over UDP on the same network.
- **YieldFlo (PC app)** combines both streams in real time, calculates yield, stores every point to a local SQLite database, renders a live color-coded yield map, and generates per-job reports and CSV exports.

## Repository layout

| Folder | Contents |
|---|---|
| [`YieldFlo/`](YieldFlo) | Main Windows Forms application (.NET) — the PC-side app |
| [`YieldFloApp/`](YieldFloApp) | Deployed/runnable build output (exe + resources) |
| [`Modules/ESP32/YieldFlo_ESP32`](Modules/ESP32/YieldFlo_ESP32) | Module firmware for ESP32 — WiFi/Ethernet/CAN, web-portal configuration, OTA updates |
| [`Modules/STM32F1/YieldFlo_STM32F1`](Modules/STM32F1/YieldFlo_STM32F1) | CAN-only module firmware for STM32F103 ("Blue Pill") — compile-time settings, no wireless |
| [`PCBs/YF1`](PCBs/YF1) | KiCad design for the main module board (sensor conditioning, CAN transceiver, etc.) |
| [`PCBs/Moisture1`](PCBs/Moisture1) | KiCad design for the moisture daughter board (ADS1115 + OEM capacitance sensor interface) |
| [`Tools/SensorSim`](Tools/SensorSim) | Arduino-based signal generator for bench-testing module firmware without a combine |
| [`ModuleSimulator/`](ModuleSimulator) | Windows app that emulates a module over the network, for testing the PC app without hardware |

Each `Modules/` folder has its own README with build instructions, pinouts, and protocol details — see the links above.

## Getting started

1. Extract a [release](../../releases) (or build `YieldFlo.sln`) and run `YieldFlo.exe` — no installer needed. Requires Windows 10/11 and .NET Framework 4.8.
2. Flash a module (ESP32 or STM32F1 — see the module READMEs) and wire it to the elevator/moisture sensors.
3. Run AgOpenGPS on the same PC or network.
4. In YieldFlo: set units, profile, crop, and header, then start a job.

Full instructions are in the in-app user manual (`Help` button, or [`YieldFlo/Help/YieldFlo User Manual.md`](YieldFlo/Help/YieldFlo%20User%20Manual.md)).

### Rebuilding the manual

`YieldFlo/Help/` holds the manual as `.md`, `.html` and `.pdf`. All three ship, so all three have to be kept in step — the `.md` and `.html` are edited by hand, and the `.pdf` is printed from the `.html` by headless Edge:

```
"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe" ^
  --headless=new --disable-gpu --no-pdf-header-footer ^
  --print-to-pdf="YieldFlo User Manual.pdf" ^
  "file:///F:/path/to/YieldFlo/Help/YieldFlo User Manual.html"
```

`--no-pdf-header-footer` is what keeps the browser's own URL and page numbers off the pages. The HTML carries its own print CSS for page breaks, so nothing else needs setting. Chrome works identically — the PDF is produced by Chromium's Skia backend either way.

The build copies `Help/**` into `YieldFloApp/Help/`; that copy is output, not a second source to edit.

## Features

- Live main-screen gauges: yield rate, moisture, sensor status, area/total/average, work rate
- Color-coded live yield map (GMap.NET) with heading-up rotation and coverage tracking
- Profiles, crops, headers, and fields — reusable per-combine/per-crop configuration
- Two-point yield and moisture calibration workflows
- Job history with reports and CSV export (RateController-compatible)
- Imperial and metric units throughout
- Multi-language UI (English, German, French, Dutch, Polish, Hungarian, Lithuanian, Russian)
- CAN or WiFi/Ethernet module communication, selectable per install

## License

GPL-3.0 — see [`LICENSE`](LICENSE).
