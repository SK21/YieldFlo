# YieldFlo — Design Plan
*Combine Yield & Moisture Monitor — Initial Architecture*

---

## 1. Overview

YieldFlo is a Windows desktop application for real-time yield and moisture monitoring on a combine harvester. It receives GPS data from AgOpenGPS (AOG) over UDP and sensor data from a hardware module (similar to RC's Teensy modules) via **UDP (Ethernet) or CAN** — the same two communication paths used by RC. Data is stored in a local SQLite database, displayed on a live yield map, and exported to CSV.

**Reference design:** AOG_RC (`F:\Documents\GitHub\RateControl\AOG_RC`)
**UI framework:** Windows Forms, .NET 8
**Storage:** SQLite (replaces JSON/flat-file approach used in RC)
**Target hardware:** Combine with clean-grain elevator optical sensors + optional capacitance moisture sensor

---

## 2. System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                       YieldFlo PC App                        │
│                                                              │
│  ┌──────────┐  ┌──────────────┐  ┌────────────────────────┐ │
│  │ frmMain  │  │ frmMenu      │  │ frmYieldMap            │ │
│  │ (status, │  │ (profiles,   │  │ (GMap.NET live map     │ │
│  │  gauges) │  │  jobs, crop) │  │  with yield coloring)  │ │
│  └────┬─────┘  └──────┬───────┘  └────────────┬───────────┘ │
│       │               │                        │             │
│  ┌────▼───────────────▼────────────────────────▼───────────┐ │
│  │                        Core (static hub)                 │ │
│  │  UDPcomm | ModuleComm | YieldCalculator | DataCollector  │ │
│  └────┬──────────────────────────┬────────────────────────-─┘ │
│       │                          │                             │
│  ┌────▼────┐               ┌─────▼──────┐                     │
│  │ SQLite  │               │ CSV Export │                      │
│  │  DB     │               │ Engine     │                      │
│  └─────────┘               └────────────┘                     │
└──────────────┬──────────────────────┬───────────────────────--┘
               │ UDP :17777           │ UDP :30100 or CAN (J1939)
               ▼                      ▼
         AgOpenGPS               YieldFlo Module
         (GPS source)            (Teensy/Arduino)
```

---

## 3. Solution Structure

```
YieldFlo.sln
├── YieldFlo/                        ← Main WinForms application
│   ├── Core.cs                      ← Static hub (same pattern as RC)
│   ├── Program.cs
│   ├── Forms/
│   │   ├── frmMain.cs               ← Main window: live gauges, status bar
│   │   ├── frmMenu.cs               ← Menu hierarchy (same as RC pattern)
│   │   ├── frmMenuProfiles.cs       ← Profile management
│   │   ├── frmMenuJobs.cs           ← Job management
│   │   ├── frmMenuCrops.cs          ← Crop/header configuration
│   │   ├── frmMenuCalibrate.cs      ← Sensor & crop calibration
│   │   ├── frmYieldMap.cs           ← Live GMap.NET yield map
│   │   ├── frmJobReport.cs          ← Post-job summary
│   │   └── frmMsgBox.cs             ← Custom message box (port from RC)
│   ├── Classes/
│   │   ├── clsYieldCalculator.cs    ← Core yield math
│   │   ├── clsMoistureSensor.cs     ← Moisture data handling
│   │   ├── clsDataCollector.cs      ← Real-time data point accumulation
│   │   ├── clsProfile.cs            ← Profile model
│   │   ├── clsJob.cs                ← Job model
│   │   ├── clsCrop.cs               ← Crop model (test weight, moisture)
│   │   ├── clsHeader.cs             ← Header model (type, cut width)
│   │   ├── clsCalibration.cs        ← Calibration data model
│   │   ├── clsYieldDataPoint.cs     ← Single recorded data point
│   │   └── clsTools.cs              ← Utilities (port from RC)
│   ├── Database/
│   │   ├── DB.cs                    ← SQLite connection wrapper, migrations
│   │   ├── ProfileRepo.cs           ← Profile CRUD
│   │   ├── JobRepo.cs               ← Job CRUD
│   │   ├── CropRepo.cs              ← Crop CRUD
│   │   ├── YieldDataRepo.cs         ← Insert & query yield data points
│   │   └── CalibrationRepo.cs       ← Calibration CRUD
│   ├── Communication/
│   │   ├── UDPcomm.cs               ← AOG GPS receiver (port from RC, port 17777)
│   │   ├── UDPmodule.cs             ← UDP comm with YieldFlo Module (recv 30100 / send 30200)
│   │   ├── CanBridgeComm.cs         ← CAN bridge comm (port from RC)
│   │   ├── IModuleInterface.cs      ← Interface: UDP or CAN (same strategy as RC)
│   │   └── PacketParser.cs          ← Decode module data packets (PGN-style)
│   ├── Map/
│   │   ├── YieldMapController.cs    ← GMap.NET integration, yield overlay
│   │   └── YieldColorScale.cs       ← bu/ac → color gradient mapping
│   ├── Export/
│   │   └── CsvExporter.cs           ← Export job data to CSV
│   ├── Language/
│   │   └── Lang.resx                ← Localization (English base)
│   └── Properties/
│       └── Settings.settings        ← Theme, last profile, last job, etc.
│
├── YieldFloModule/                  ← Arduino/Teensy firmware (C++)
│   ├── YieldFloModule.ino
│   ├── OpticalSensor.h/.cpp
│   ├── MoistureSensor.h/.cpp
│   └── Comms.h/.cpp
│
└── ModuleSimulator/                 ← WinForms simulator (for dev without hardware)
    ├── frmSimulator.cs              ← Sliders for yield/moisture simulation
    └── SimUDPSender.cs              ← Sends simulated packets via UDP (loopback)
```

---

## 4. Database Schema (SQLite)

### `profiles`
| Column | Type | Notes |
|---|---|---|
| id | INTEGER PK | |
| name | TEXT | Display name |
| combine_id | TEXT | Machine identifier |
| created_at | TEXT | ISO 8601 |

### `crops`
| Column | Type | Notes |
|---|---|---|
| id | INTEGER PK | |
| name | TEXT | "Wheat", "Canola", etc. |
| category | TEXT | Cereal / OilSeed / Pulse / Corn / Other |
| test_weight | REAL | lbs/bushel or kg/L |
| market_moisture | REAL | % — elevator's dry threshold |
| dry_moisture | REAL | % — field average for dry yield calc |

### `headers`
| Column | Type | Notes |
|---|---|---|
| id | INTEGER PK | |
| name | TEXT | |
| header_type | TEXT | Draper / Auger / Corn |
| cut_width | REAL | meters |

### `fields`
| Column | Type | Notes |
|---|---|---|
| id | INTEGER PK | |
| name | TEXT | |
| boundary | TEXT | GeoJSON polygon |

### `jobs`
| Column | Type | Notes |
|---|---|---|
| id | INTEGER PK | |
| profile_id | INTEGER FK | |
| field_id | INTEGER FK | nullable |
| crop_id | INTEGER FK | |
| header_id | INTEGER FK | |
| name | TEXT | |
| started_at | TEXT | ISO 8601 |
| ended_at | TEXT | ISO 8601, null if active |
| status | TEXT | Active / Paused / Complete |
| total_acres | REAL | |
| total_volume | REAL | bushels |

### `calibrations`
| Column | Type | Notes |
|---|---|---|
| id | INTEGER PK | |
| profile_id | INTEGER FK | |
| crop_id | INTEGER FK | |
| sensor_baseline | REAL | paddle-only obstruction time (ms) |
| yield_factor | REAL | calibration multiplier |
| processing_delay_sec | INTEGER | header-to-sensor transit time |
| calibrated_at | TEXT | ISO 8601 |

### `yield_data`
| Column | Type | Notes |
|---|---|---|
| id | INTEGER PK | |
| job_id | INTEGER FK | |
| timestamp | TEXT | ISO 8601 |
| latitude | REAL | |
| longitude | REAL | |
| elevation | REAL | meters |
| speed | REAL | km/h |
| heading | REAL | degrees |
| yield_rate | REAL | bu/ac instantaneous |
| moisture | REAL | % |
| acres_accumulated | REAL | running total for job |
| raw_sensor_1 | REAL | raw obstruction ratio sensor 1 |
| raw_sensor_2 | REAL | raw obstruction ratio sensor 2 |

---

## 5. Key Classes

### `Core.cs` (static hub — same pattern as RC)
```csharp
public static class Core
{
    public static UDPcomm UDPaog;           // GPS from AOG (port 17777)
    public static UDPcomm UDPmodule;        // Module UDP comm (recv 30100 / send 30200)
    public static CanBridgeComm CanBridge;  // CAN bridge (if CAN mode selected)
    public static IModuleInterface Module;  // Active interface: UDP or CAN
    public static clsYieldCalculator Yield; // Yield math
    public static clsDataCollector Collector;
    public static clsProfile ActiveProfile;
    public static clsJob ActiveJob;
    public static DB Database;

    public static event EventHandler UpdateDisplay;
    public static event EventHandler GpsUpdated;
    public static event EventHandler JobStateChanged;
    public static event EventHandler AppExit;
}
```

### `clsYieldCalculator.cs`
```csharp
// Inputs: raw sensor obstruction ratio, speed, header width, crop test weight
// Processing delay buffer: ring buffer of timestamped sensor readings
// Output: instantaneous yield (bu/ac), smoothed yield, acres/hr

public double CalculateYield(double obstructionRatio, double speedKmh,
                              double headerWidthM, double testWeightLbsBu)
// Algorithm:
//  1. Apply processing delay (ring buffer lookup)
//  2. Subtract baseline (paddle-only reading from calibration)
//  3. grain_flow_volume = (obstruction - baseline) * calibration_factor
//  4. area_rate = speed * header_width (ha/hr)
//  5. yield = grain_flow_volume / area_rate, converted to bu/ac
```

### `clsDataCollector.cs`
```csharp
// Triggered by timer (10 Hz GPS update rate from AOG)
// - Reads GPS position, speed, heading, elevation
// - Applies processing delay to sensor readings
// - Calculates instantaneous yield
// - Accumulates acres (trapezoidal area integration)
// - Writes clsYieldDataPoint to database every 1 second
// - Fires UpdateDisplay event for UI refresh
```

### `CsvExporter.cs`
```csharp
// Exports all yield_data rows for a job to CSV
// Columns: Timestamp, Latitude, Longitude, Elevation_m,
//          Speed_kmh, YieldRate_buAc, Moisture_pct,
//          AcresAccumulated, Sensor1_Raw, Sensor2_Raw
public void Export(int jobId, string filePath)
```

### `YieldMapController.cs`
```csharp
// GMap.NET overlay of yield data
// Color scale: Red (low) → Yellow → Green (high) — configurable range
// Renders polygon or point markers per data point
// Updates live during harvest
// Supports post-job static replay
```

---

## 6. Hardware Module Design

Similar to RC's Teensy Rate module. Reads sensors, sends data to PC, receives configuration.

### Hardware
- **MCU:** Teensy 4.0 or Arduino Mega (same family as RC modules)
- **Sensor inputs:**
  - 2× optical sensor channels (interrupt-driven pulse counting, 3.3V/5V TTL)
  - 1× moisture sensor input (analog ADC or I2C — e.g., Capacitec or similar)
- **Communication:** **Ethernet (W5500/ENC28J60) or CAN (MCP2515)** — same two options as RC modules
  - **Ethernet mode:** Module sends UDP packets to PC on port **30100**; PC sends config to module on port **30200** (distinct from RC's 29999/28888 so both apps can run simultaneously)
  - **CAN mode:** Module connects via CAN bus through a CAN bridge adapter (SLCAN, InnoMaker, PCAN — same drivers as RC)

### Packet Protocol (binary, same style as RC PGNs — works over both UDP and CAN)

**Module → PC** (sent at 10 Hz):
```
Byte  0    : 0x7B  (start byte)
Byte  1    : 0x7C  (start byte)
Byte  2    : packet_type = 0x01
Byte  3-4  : sensor1_count  (uint16, paddle obstructions per 100ms)
Byte  5-6  : sensor2_count  (uint16)
Byte  7-8  : moisture_raw   (uint16, ADC counts or fixed-point %)
Byte  9-10 : module_rpm     (uint16, elevator chain speed if encoder fitted)
Byte  11   : status_flags   (bit 0 = sensor1_ok, bit 1 = sensor2_ok, bit 2 = moisture_ok)
Byte  12   : CRC8
```

**PC → Module** (configuration, sent on change):
```
Byte  0    : 0x7B
Byte  1    : 0x7C
Byte  2    : packet_type = 0x10
Byte  3    : processing_delay_sec
Byte  4-5  : baseline_sensor1 (uint16)
Byte  6-7  : baseline_sensor2 (uint16)
Byte  8    : CRC8
```

### Calibration Procedure (in-app)
1. User selects **Calibrate Sensors** from menu
2. App sends calibration command to module
3. Module runs elevator at full speed with no crop for 10 seconds
4. Records average obstruction count per window → stored as `sensor_baseline`
5. Result written to `calibrations` table

---

## 7. GPS Integration (AOG UDP)

Same as RC — listens on UDP port **9999** for AOG PANDA sentences or port **17777** for the standard AOG position packet.

### Port Reference (all YieldFlo UDP ports)

| Direction | Port | Purpose | RC equivalent |
|---|---|---|---|
| PC receives | 17777 | AOG GPS broadcast | 17777 (shared, SO_REUSEADDR) |
| PC sends from | **1461** | AOG send-from port | 1460 ← RC uses this, must differ |
| PC receives | 30100 | Data from YieldFlo Module | 29999 |
| PC sends | 30200 | Config/commands to Module | 28888 |
| PC sends from | 1500 | Module send-from port | 1480 |

> **Known issue:** RC does not set `SO_REUSEADDR` on its port 17777 socket. Until RC is updated, only one app can bind port 17777 at a time. Fix required in RC: add `recvSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true)` before `Bind()` in UDPcomm.cs. YieldFlo already has this set.

**AOG Position Packet (parsed from UDP):**
- Latitude (double)
- Longitude (double)
- Heading (float, degrees)
- Speed (float, km/h)
- Altitude/Elevation (float, meters)
- Fix quality

The `processing_delay_sec` setting means a ring buffer holds the last N seconds of sensor readings. When a GPS position arrives, the sensor reading from `now - delay` is paired with the current position.

---

## 8. UI Design (matching RC style)

### frmMain (Main Window)
```
┌────────────────────────────────────────────────────┐
│ [Menu] [Start Job] [Pause] [Stop]   YieldFlo v1.0  │
├────────────────────────────────────────────────────┤
│  YIELD          MOISTURE       SPEED               │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐         │
│  │  87.3    │  │  14.2%   │  │  6.8     │         │
│  │  bu/ac   │  │          │  │  km/h    │         │
│  └──────────┘  └──────────┘  └──────────┘         │
│                                                    │
│  ACRES  12.4      TOTAL  1,082 bu    AVG  87.3     │
│                                                    │
│  [Sensor 1 ████████░░] [Sensor 2 ███████░░░]       │
│  [GPS ●] [Module ●] [Job: Field 3 - Wheat]        │
└────────────────────────────────────────────────────┘
```

- Same dark theme, color-coded status indicators (green/yellow/red dots)
- VerticalBar custom controls for sensor flow visualization
- Touchscreen-friendly large tap targets
- Light/dark theme toggle stored in Settings

### frmYieldMap
- GMap.NET embedded map
- Live color-coded trail (configurable bu/ac color scale)
- Toggle: yield / moisture / elevation overlays
- Field boundary overlay (drawn or imported shapefile)
- Legend showing min/max/avg

### frmMenu hierarchy
```
Menu
├── Profiles       → List, New, Edit, Delete
├── Jobs           → Current, New, History, Report
├── Crops          → List, New, Edit
├── Headers        → List, New, Edit
├── Calibrate      → Sensor Cal, Crop Cal, Delay Setting
├── Module         → Port select, baud rate, test
├── Map            → Basemap source, yield scale config
├── Export         → CSV export (select job)
└── Settings       → Theme, Units, Language
```

---

## 9. Profiles, Jobs & Workflow

### Profiles
- Represent a combine/operator combination
- Store: combine name, operator, default units (bu/ac vs t/ha)
- Multiple profiles supported (different operators or machines)

### Jobs
- One job = one field pass session
- A job links: Profile + Field + Crop + Header
- Jobs can be paused and resumed
- Job history stored indefinitely in DB
- Exportable individually or in batch

### Typical Workflow
1. **Setup:** Create/select Profile → select Crop → select Header
2. **Calibrate:** Run sensor calibration (no grain) → set processing delay
3. **Start Job:** Select or create Field → press Start
4. **Harvest:** Live display updates, map fills with color-coded yield
5. **Pause/Resume:** Supported mid-field (gap in map trail)
6. **Stop Job:** Summary shown (total acres, avg yield, avg moisture)
7. **Export:** Menu → Export → select Job → choose CSV path

---

## 10. CSV Export Format

**Filename:** `YieldFlo_[JobName]_[Date].csv`

```csv
Timestamp,Latitude,Longitude,Elevation_m,Speed_kmh,Heading_deg,YieldRate_buAc,Moisture_pct,AcresAccumulated,Sensor1_Raw,Sensor2_Raw
2025-07-15T14:32:01,49.123456,-104.654321,632.4,6.8,270.0,87.3,14.2,12.4,0.72,0.69
2025-07-15T14:32:02,49.123491,-104.654389,632.6,6.9,270.2,88.1,14.1,12.43,0.73,0.71
...
```

**Units note:** App stores in metric internally; CSV output respects user's unit preference setting (bu/ac or t/ha, mph or km/h, ft or m for elevation).

---

## 11. Yield Calculation Algorithm

```
ObstructionRatio = (SensorCount - BaselineCount) / WindowSize
                   clamped to [0.0, 1.0]

AveragedRatio = (ObstructionRatio_S1 + ObstructionRatio_S2) / 2

GrainFlowIndex = AveragedRatio * CalibrationFactor   [volume / time]

AreaRate = Speed_ms * HeaderWidth_m                  [m²/s]

YieldRate_m3_per_m2 = GrainFlowIndex / AreaRate

YieldRate_buAc = YieldRate_m3_per_m2
                 * (1.0 / TestWeight_kg_m3)
                 * ConversionFactor_to_buAc

AcresIncrement = AreaRate * DeltaTime * M2_TO_ACRES
```

**Processing delay:** A `Queue<(DateTime time, double ratio)>` holds sensor readings. Each GPS update dequeues readings older than `processing_delay_sec` and uses the most recent dequeued value.

---

## 12. Dependencies (NuGet)

| Package | Purpose |
|---|---|
| Microsoft.Data.Sqlite | SQLite database |
| GMap.NET.WinForms | Mapping (same as RC) |
| NetTopologySuite | Spatial math, field boundary polygon area |
| Newtonsoft.Json | Settings, GeoJSON field boundaries |
| CsvHelper | CSV export (or manual — simple enough) |

---

## 13. Module Simulator

`ModuleSimulator` project provides a WinForms window that:
- Generates synthetic sensor readings (sinusoidal yield pattern)
- Allows slider control of simulated yield rate and moisture
- Sends same binary packets as real module (loopback COM port or named pipe)
- Useful for full UI testing without hardware

---

## 14. Settings (Properties/Settings.settings)

| Key | Type | Default |
|---|---|---|
| CurrentProfile | string | "" |
| CurrentJob | string | "" |
| ModuleCommType | string | "UDP" |  ← "UDP" or "CAN"
| CanDriver | string | "SLCAN" |   ← SLCAN / InnoMaker / PCAN (same as RC)
| CanPort | string | "" |
| Theme | string | "Dark" |
| Units | string | "Imperial" |
| YieldMapSource | string | "Satellite" |
| YieldScaleMin | double | 0 |
| YieldScaleMax | double | 150 |
| ProcessingDelaySec | int | 10 |
| LastExportFolder | string | "" |

---

## 15. Phase Plan

### Phase 1 — Foundation
- [ ] Solution setup, WinForms project, RC-style UI skeleton
- [ ] SQLite DB with all tables, migrations, repos
- [ ] Core.cs static hub, Settings, theme system
- [ ] frmMain skeleton with placeholder gauges

### Phase 2 — Data Input
- [ ] UDP GPS receiver (AOG integration)
- [ ] ModuleComm serial receiver + PacketParser
- [ ] Module firmware (Teensy) — optical + moisture sensors
- [ ] ModuleSimulator for dev testing

### Phase 3 — Yield Engine
- [ ] clsYieldCalculator with processing delay ring buffer
- [ ] clsDataCollector (timer-driven, 1-second DB writes)
- [ ] Profile / Job / Crop / Header CRUD (menus + repos)

### Phase 4 — Visualization
- [ ] frmYieldMap with GMap.NET + live color trail
- [ ] YieldColorScale (Red-Yellow-Green gradient)
- [ ] Field boundary import/draw

### Phase 5 — Export & Reporting
- [ ] CSV exporter
- [ ] frmJobReport (summary stats, printable)
- [ ] Calibration workflow (sensor cal + crop cal)

### Phase 6 — Polish
- [ ] Module simulator completions
- [ ] Light/Dark theme (port from RC)
- [ ] Input validation, error handling
- [ ] Help documentation

---

## 16. Notes & Open Questions

1. **Elevator encoder** — An optional rotary encoder on the elevator drive shaft would allow speed compensation (elevator RPM affects sensor readings). Add as future enhancement.
2. **Multi-combine** — The DB schema supports multiple profiles/machines but UI will be single-instance per PC initially.
3. **Test weight source** — User-entered per crop. Future: integrate with USDA standard test weights lookup.
4. **Moisture calibration offset** — User can apply a ±% offset per crop to correct sensor drift (same as FarmTRX).
5. **Ethernet vs CAN** — Both supported from the start, same as RC. `IModuleInterface` abstracts the choice. Module firmware has conditional compile targets for each. The simulator always uses UDP loopback.
6. **AOG compatibility** — Parsing AOG's PANDA UDP packet gives GPS. If user doesn't run AOG, a fallback NMEA serial GPS parse should be supported.
7. **Shapefile import** — NetTopologySuite supports this; add in Phase 4.
