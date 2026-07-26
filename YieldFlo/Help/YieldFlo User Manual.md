# YieldFlo User Manual

YieldFlo is a yield monitoring application that works alongside AgOpenGPS (AOG). It records yield rate, moisture, and GPS position continuously while harvesting, and produces spatial yield maps and job reports.

YieldFlo receives GPS and section control data from AOG over a local network (UDP). A YieldFlo hardware module connects to the combine's clean-grain elevator and moisture sensor and sends live readings to the PC application.

---

## Table of Contents

1. [Getting Started](#1-getting-started)
2. [Main Screen](#2-main-screen)
3. [Jobs](#3-jobs)
4. [Crops](#4-crops)
5. [Headers](#5-headers)
6. [Profiles](#6-profiles)
7. [Fields](#7-fields)
8. [Yield Calibration](#8-yield-calibration)
9. [Moisture Calibration](#9-moisture-calibration)
10. [Yield Map](#10-yield-map)
11. [Job Report](#11-job-report)
12. [Settings](#12-settings)
13. [Language](#13-language)
14. [Module Web Portal](#14-module-web-portal)
15. [Troubleshooting](#15-troubleshooting)

---

## 1. Getting Started

### Requirements

| Item | Requirement |
|------|-------------|
| Operating system | Windows 10 or Windows 11 |
| .NET Framework | 4.8 (pre-installed on Windows 10 May 2019 Update and later) |
| AgOpenGPS | Running on the same PC or local network |

**No installer is required.** Extract the YieldFlo folder to any location and run `YieldFlo.exe`.

### Hardware module

- YieldFlo module (based on ESP32) mounted on the combine
- Connected to the clean-grain elevator optical sensor
- Connected to the capacitance moisture sensor (optional)
- Communicates with the PC via WiFi or CAN bus

### Network

- **WiFi mode:** connect the PC to the same WiFi network as the YieldFlo module, or directly to the module's access point
- AOG must be running and broadcasting GPS/section data on the local network

### Recommended first-run setup

On first launch YieldFlo creates a default crop, header, and profile. Before starting your first job:

1. Open **Menu → Settings** and select your preferred units (Imperial or Metric)
2. Open **Menu → Profiles** and enter your combine ID
3. Open **Menu → Crops** and add or edit crops for your operation
4. Open **Menu → Headers** and enter the correct cutting width
5. Open **Menu → Fields** and add field names if desired

---

## 2. Main Screen

The main screen displays live harvest data and job status.

### Toolbar buttons

Left to right, as they appear on screen:

| Button | Action |
|--------|--------|
| **Exit** | Close YieldFlo |
| **─** | Switch to compact (mini) view — see below |
| **Menu** | Open the main menu |
| **Start** | Start a new job (opens Jobs menu) or resume the paused active job |
| **Pause** | Manually pause recording |
| **Stop** | Stop and close the current job (requires confirmation) |

### Data panels

| Panel | Description |
|-------|-------------|
| **YIELD** | Instantaneous yield rate (bu/ac or t/ha), smoothed over recent readings |
| **MOISTURE** | Live grain moisture percentage from the capacitance sensor |
| Totals area | Total area harvested, total mass, and average yield for the current job |

### Sensor bars

Two horizontal bar gauges below the data panels:

- **Elev Flow:** Grain obstruction percentage in the clean-grain elevator. Higher = more grain flowing.
- **Moisture:** Relative moisture sensor reading scaled to a 0–30% range.

### Status bar

| Indicator | Green | Orange | Red / Silver |
|-----------|-------|--------|---------------|
| **GPS** | AOG connected and sending position | — | No AOG data |
| **Module** | Module data received within 5 s | — | No module data |
| **Job name** | Job recording (active) | Job paused | No active job (silver) |

When YieldFlo displays a notification (e.g. export complete, error), a full-width message overlays the status bar for 10 seconds then clears automatically. Yellow text = informational; red text = error.

### Compact (mini) view

Press **─** in the top-right corner of the main screen to switch to a small floating overlay that shows only the live yield reading. This is useful when running YieldFlo alongside AgOpenGPS on a single monitor.

The compact overlay can be dragged anywhere on screen. Press the restore button on the overlay to return to the full main screen. Its position is remembered between sessions.

---

## 3. Jobs

**Menu: Menu → Jobs**

A job records yield, moisture, GPS position, and area for a single harvest session. Each job is linked to a crop, header, and profile.

### Starting a job

1. Press **Menu** on the main screen, then **Jobs**
2. Press **New** — a draft job is prepared with a default name and the current Crop/Header/Profile, and the **Field** drop-down is focused
3. Select a **Field** (optional) — this replaces the default job name with the field name; the **Job Name** can still be edited afterward
4. Adjust **Crop**, **Header**, and **Profile** if needed
5. Enter any **Notes** for the job (optional)
6. Press **Save** — this creates the job and starts it immediately (prompts for confirmation if another job is currently active)

Recording begins automatically when harvesting conditions are met (speed > 0.5 km/h and AOG sections are on).

### Managing jobs

The jobs list shows all saved jobs with their status, date, area, and linked field. Click a column header to sort by that column.

| Action | Description |
|--------|-------------|
| **Load** | Make a saved job the active job (prompts if another job is already active) |
| **Delete** | Permanently remove a job and all its recorded data — cannot be undone |

Only one job can be active at a time.

### Auto-pause

The job automatically pauses when harvesting stops:

- Machine speed falls below 0.5 km/h, **or**
- AOG turns all header sections off (e.g. turning on headland, travelling over harvested ground)

When auto-paused the job name turns orange in the status bar. Recording resumes automatically when harvesting conditions return.

### Manual pause and resume

Press **Pause** on the main screen to manually pause recording. Press **Start** to resume.

### Stopping a job

Press **Stop** → confirm. The job is saved and closed. All totals are written to the database. The job remains in the list and can be viewed in the Job Report.

### Resume on start

If **Resume Job on Start** is enabled in Settings, the active job is automatically reloaded when YieldFlo restarts. Useful if the PC is rebooted during a harvest day.

---

## 4. Crops

**Menu: Menu → Crops**

Crops store the grain-specific parameters used in yield calculations and reporting.

### Crop settings

| Field | Description |
|-------|-------------|
| **Name** | Crop name (e.g. Wheat, Corn, Canola) |
| **Category** | Grain type category |
| **Test Weight** | Standard bushel weight (lbs/bu) — used to convert mass to bushels |
| **Market Moisture** | Standard moisture for yield reporting (e.g. 14.5% for wheat) |

### Adding a crop

1. Press **New** and enter a crop name
2. Set the test weight and market moisture
3. Press **Save**

Select a crop from the list to edit it. At least one crop must exist at all times.

---

## 5. Headers

**Menu: Menu → Headers**

Headers define the cutting width of the front attachment. Width is used to calculate the area harvested per data point and to draw swath polygons on the yield map.

### Header settings

| Field | Description |
|-------|-------------|
| **Name** | Header name (e.g. 30ft Draper, 8-row Corn Head) |
| **Type** | Header type category |
| **Width** | Cutting width in feet (Imperial) or metres (Metric) |
| **Ahead of Pivot** | Distance the header sits ahead of the position AOG broadcasts. Enter AOG's **pivot-to-header** distance (from the AOG implement setup). This shifts the recorded coverage to the header, so pass boundaries on the yield map land where the header actually crossed them. |

### Adding a header

1. Press **New** and enter a header name
2. Enter the cutting width
3. Enter the pivot-to-header distance from your AOG implement setup
4. Press **Save**

At least one header must exist at all times. Changes to a header take effect when a job is started or loaded.

> **Note — coverage painted slightly before AOG's:** AOG turns its section bits on *early* by its turn-on look-ahead (anticipating valve opening time) but paints its own coverage only where product actually applies. YieldFlo records from the section bits, so each pass start on the yield map can begin a metre or two before AOG's painted coverage. Pass *ends* match exactly. This is normal and harmless for harvest — there is no valve delay on a header — and can be removed in testing by setting AOG's section look-ahead to zero.

---

## 6. Profiles

**Menu: Menu → Profiles**

Profiles store combine-specific and calibration settings. Use a separate profile for each combine if running YieldFlo on multiple machines.

### Profile settings

| Field | Description |
|-------|-------------|
| **Name** | Profile name (e.g. JD 9600, Case 2388) |
| **Combine ID** | Identifier for the module — must match the module configuration |
| **Moisture offset** | Fixed offset applied to all moisture readings for this profile |
| **Moisture scale** | %/count scale factor for the moisture sensor |
| **Temperature offset** | Temperature offset (°C) |
| **Temperature scale** | °C/count scale factor for the temperature sensor |

> **Note:** Moisture and temperature calibration values are set in **Menu → Moisture Cal** and saved automatically to the active profile.

### Adding a profile

1. Press **New** and enter a profile name
2. Enter the Combine ID to match your module
3. Press **Save**

At least one profile must exist at all times.

---

## 7. Fields

**Menu: Menu → Fields**

Fields are optional location labels that can be assigned to jobs for organisation and reporting.

### Adding a field

1. Press **New** and enter a field name
2. Press **Save**

Fields are not required. A job can be created without a field selected.

### Importing fields from AgOpenGPS

Press **Import** to open a checklist of field names found in AgOpenGPS's and TWOL's field folders (`Documents\AgOpenGPS\Fields` and `Documents\TWOL\Fields`). Fields already present in YieldFlo are shown but cannot be re-checked. Tick the fields to bring in (or press **Select All**), then press **Import** to add them as new field records — only the field name is imported, not boundary data.

---

## 8. Yield Calibration

**Menu: Menu → Yield Cal**

Yield calibration corrects the elevator sensor reading to match the actual mass of grain harvested. Two parameters are set here:

- **Sensor Baseline** — the obstruction reading with paddles running but no grain (set once at installation)
- **Yield Factor** — a multiplier that scales the sensor reading to match a weighed reference

### Setting the baseline

1. Start the clean-grain elevator with no grain
2. Open **Menu → Yield Cal**
3. Press **Set Baseline** — it samples for 5 seconds and enters the result in the **Sensor Baseline** field
4. Press **Save & Apply**

Nothing takes effect, and nothing is saved, until **Save & Apply** is pressed — this applies to both Set Baseline and Apply Cal.

> **Tip:** Run the empty elevator for at least 10 seconds before setting the baseline so the reading stabilises.

### Noise readout

Next to the **Set Baseline** button, the **Noise** readout shows how many electrical glitches per second the module is rejecting on the optical sensor signal, averaged over the last 5 seconds. On a healthy installation it reads **0**; the value turns orange when glitches are being rejected, and shows **--** when no module is connected.

A rising noise value — especially one that climbs with elevator speed — indicates electrical interference on the sensor wire (static discharge from the elevator, a chafed or loose wire shaken by vibration, or pickup from nearby wiring). The module filters these glitches out of the yield reading automatically, but persistent noise is worth fixing at the source: check the sensor wire's routing and shielding, connector condition, and the ground path from the sensor bracket and elevator housing to the chassis. The readout works in both sensor signal modes, including Main only.

### Paddle rate readout

Below the **Set Baseline** button, the **Paddles** readout shows how many elevator paddles per second the sensor is seeing, averaged over the last 5 seconds. It shows **--** when no module is connected or the module firmware predates the feature.

Use it to verify the sensor is catching every paddle: the value should match the paddle rate calculated from elevator speed and paddle spacing, and should scale directly with elevator rpm. A reading at half the expected rate means the sensor is missing paddles (alignment or sensing-range problem). On combines with another optical yield monitor sharing the sensor, the value can be compared directly against that monitor's sensor calibration result (for FarmTrx, the "X% @ Y Hz" line on its Sensor Calibration page).

### Processing delay

The processing delay accounts for the travel time of grain from the header to the elevator sensor. Set this value (in seconds) to match your combine. Typical range is 4–12 seconds.

### Running a calibration pass

A calibration pass measures the actual mass of grain harvested during a known run, then adjusts the Yield Factor to match.

**Preparation:** Position a weigh wagon or grain cart to catch grain from the unloading auger and record start and end weights.

1. Open **Menu → Yield Cal**
2. Press **Start Run** — the app begins accumulating sensor data
3. Harvest a suitable area (at least one full tank is recommended)
4. Press **Stop Run**
5. Weigh the harvested grain
6. Enter the actual weight in the **Actual weight** field
7. Press **Apply Cal**

YieldFlo calculates a new Yield Factor and enters it in the **Yield Factor** field. Press **Save & Apply** to save it to the active profile.

Below the **Save & Apply** button, a **Last saved** line shows the date and time the calibration for the active profile and crop was last saved with **Save & Apply**. It is blank until a calibration has been saved.

### Manual factor adjustment

The Yield Factor can also be adjusted manually in the **Yield Factor** field. Increase to raise yield readings; decrease to lower them.

---

## 9. Moisture Calibration

**Menu: Menu → Moisture Cal**

Moisture and temperature readings are both calculated the same way:

```
Value = raw_count × scale  +  offset
```

Both support single-point calibration. With offset set to 0, adjust scale until the displayed reading matches a reference instrument:

```
scale = known_value / raw_count
```

For example: if the raw count is 420 and a certified meter reads 18.5%, set scale to 18.5 ÷ 420 ≈ 0.044. The offset field can then be used for fine trimming against a second reference point if needed.

### Moisture calibration procedure

1. Take a grain sample and measure moisture with a certified grain moisture meter
2. Open **Menu → Moisture Cal**
3. With the module connected and grain flowing, observe the live raw count
4. Set offset to 0 and adjust **%/count** scale until the displayed reading matches the meter
5. Press **Save**

### Moisture calibration fields

| Field | Saved to | Description |
|-------|----------|--------------|
| **%** | Active crop | Moisture offset — set to 0 for single-point scale calibration |
| **%/count** | Active profile | Scale factor converting raw ADS1115 counts to moisture percent |

### Temperature calibration

Same principle as moisture. The default scale of 0.0125 assumes an LM35 sensor (10 mV/°C at PGA = 4.096 V). With offset at 0, adjust **°C/count** until the reading matches a thermometer.

| Field | Saved to | Description |
|-------|----------|--------------|
| **°C** | Active profile | Temperature offset — set to 0 for single-point scale calibration |
| **°C/count** | Active profile | Scale factor converting raw ADS1115 counts to degrees Celsius |

> **Note:** Temperature calibration is optional. Temperature does not appear directly on the main screen.

---

## 10. Yield Map

**Menu: Menu → Yield Map**

The yield map displays a colour-coded spatial plot of yield rate across the harvested area. Each data point is drawn as a filled swath polygon sized to the header width and oriented to the GPS heading at that point.

### Colour scale

Yield is mapped to a five-colour gradient:

| Colour | Yield |
|--------|-------|
| Blue | Lowest |
| Cyan | Low |
| Green | Mid |
| Yellow | High |
| Red | Highest |

The legend at the bottom of the full-screen map shows the low and high yield values for the current data set.

### Mini mode

When opened from the menu, the yield map starts as a small floating 300×300 overlay (mini mode). The title bar shows the active job name and zoom controls.

| Control | Action |
|---------|--------|
| **─** / **+** | Zoom out / in |
| **×** | Close the map overlay |
| Click the map | Expand to full-screen mode |
| Drag the title bar | Move the overlay |

The mini map position is remembered between sessions.

### Full-screen mode

Click the map or press **×** to expand to full screen. The toolbar at the top provides:

| Control | Action |
|---------|--------|
| Job selector (drop-down) | Choose which job to display |
| **─** / **+** | Zoom out / in |
| **Recalculate** | Re-derives every point's yield for the selected job from its stored raw sensor reading using the crop's *current* calibration, then repaints the map — see below |
| **Print** | Export the map as a PNG image — see below |
| **Close** | Return to mini mode |

The map can be dragged in both mini and full-screen modes.

### Live update during harvest

When the yield map is open and a job is actively recording, the map updates automatically every 2 seconds. New swath polygons are added without re-centering the view.

### Recalculating yield

If you change a crop's calibration (Sensor Baseline or Yield Factor) *after* a job was recorded, the job's existing points still reflect the old calibration. Press **Recalculate** in the full-screen toolbar, then confirm the prompt, to re-derive every point's yield from its stored raw sensor reading using the crop's current calibration and repaint the map with the corrected values. This only affects the selected job and does not change the raw sensor data.

### Exporting the map as a PNG

Press **Print** in the full-screen toolbar to export the current map view (including the legend) as a PNG image.

1. A save dialog opens with a default filename of `JobName_Date.png`
2. Choose a folder and confirm
3. The image is saved and a confirmation appears in the status bar

The export folder is remembered for subsequent exports.

---

## 11. Job Report

**Menu: Menu → Reports**

The job report shows a summary of a completed or active job. Select a job from the list on the left to view its details.

### Report contents

| Field | Description |
|-------|-------------|
| **Job name** | Name entered when the job was created |
| **Field** | Field name assigned to the job (if set) |
| **Crop** | Crop assigned to the job (if set) |
| **Area** | Total area harvested (ac or ha) |
| **Total** | Total mass harvested (bu or tonnes) |
| **Avg Yield** | Average yield rate across all data points |
| **Avg Moisture** | Average grain moisture across all data points |
| **Data Points** | Number of recorded GPS data points |

### Printing a report

Press **Print** to send the job summary to the system print dialog. The report is formatted as plain text suitable for any printer.

### CSV export

Press **Export CSV** to save all raw data points for the selected job.

1. A save dialog opens with a default filename of `JobName_Date.csv`
2. Choose a folder and confirm
3. The file is saved and a confirmation appears in the status bar

The export folder is remembered for subsequent exports. The CSV format is compatible with the AgOpenGPS Rate Controller yield overlay.

| Column | Description |
|--------|-------------|
| Timestamp | Date and time of the reading |
| Latitude | GPS latitude (decimal degrees) |
| Longitude | GPS longitude (decimal degrees) |
| WidthMeters | Header cutting width (metres) |
| Yield_kgha | Yield rate in kg/ha |
| ElevationMeters | GPS elevation (metres) |
| Speed_kmh | Ground speed (km/h) |
| Heading | GPS heading (degrees) |
| Moisture_pct | Grain moisture (%) |
| HaAccumulated | Cumulative area at this point (ha) |
| Sensor1Raw | Raw elevator sensor ratio |

---

## 12. Settings

**Menu: Menu → Settings**

### Units

| Setting | Options |
|---------|---------|
| **Units** | Imperial (bu/ac, mph, acres, lbs) or Metric (t/ha, km/h, ha, tonnes) |

Changing units takes effect immediately. Historical data is stored in metric units internally and converted for display.

### Module communication

| Setting | Description |
|---------|-------------|
| **WiFi / Ethernet** | Module communicates over UDP on port 30100 — via the module's WiFi access point / a shared WiFi network, or via wired Ethernet (W5500 board on the module). |
| **CAN** | Module communicates over CAN bus via a USB CAN interface. |
| **CAN Driver** | Select the CAN interface driver (SLCAN, InnoMaker, or PCAN). |
| **COM Port** | Serial/USB port for the CAN interface (SLCAN driver only). |

When **CAN** mode is selected, an **Adapter: Connected** / **Not Connected** indicator appears below the port settings, showing whether the CAN adapter that is actually running (from previously saved settings) is open and seeing bus traffic. It updates live and does not reflect the driver/port currently selected in the drop-downs until they are saved and applied.

The module can also send the same UDP packets over **wired Ethernet** (W5500 board attached to the module). No app setting is needed — select Ethernet mode in the module's web portal and give the PC's wired adapter a static IP on the module's subnet (default `192.168.1.x`). The app receives WiFi and Ethernet packets on the same port.

> **Module setup page:** the module's own settings (communication mode, sensor signals, WiFi credentials, firmware update) are configured on its built-in web page at `http://192.168.200.1`, reached while connected to the module's WiFi hotspot — see [Module Web Portal](#14-module-web-portal).

#### Supported CAN adapters

| Driver | Adapters | Notes |
|--------|----------|-------|
| **SLCAN** | CANable (slcan firmware), SH-C30A, other SLCAN adapters | Appears as a COM port — select it under **COM Port**. |
| **InnoMaker** | InnoMaker USB2CAN | Requires the vendor's driver and SDK files: copy `InnoMakerUsb2CanLib.dll` and `LibUsbDotNet.dll` from the InnoMaker USB2CAN C# SDK (`Lib` folder) into the YieldFlo application folder. Download them as raw binaries, not from a GitHub web page. The COM Port setting is ignored — the first adapter found is used. |
| **PCAN** | Peak PCAN-USB | Install the PEAK device driver package (includes PCAN-Basic) from peak-system.com — no files need to be copied. The COM Port setting is ignored — the first adapter found on PCAN USB channels 1–8 is used. |

If CAN fails to start, the reason is written to the error log in `Documents\YieldFlo\Logs`.

### Resume Job on Start

When enabled, YieldFlo automatically reloads the active job when the app starts. Useful if the PC is rebooted during a harvest day.

### Saving settings

Press **Save & Apply** to save and apply all settings immediately.

---

## 13. Language

**Menu: Menu → Language**

YieldFlo supports the following languages:

| Code | Language |
|------|----------|
| en | English |
| de | German |
| fr | French |
| hu | Hungarian |
| nl | Dutch |
| pl | Polish |
| ru | Russian |
| lt | Lithuanian |

### Changing language

1. Press **Menu → Language**
2. Select the desired language from the list
3. Press **Save & Restart**

YieldFlo will restart automatically with the new language applied.

> **Note:** If AgOpenGPS is already installed, YieldFlo will automatically use the same language on first launch.

---

## 14. Module Web Portal

The YieldFlo module has a built-in settings page served from its own WiFi hotspot. Use it to configure the communication mode, the optical sensor signals, and WiFi credentials, and to update the module firmware.

### Opening the portal

1. Power the module. It broadcasts a WiFi hotspot named `YieldFlo_ESP32_XXXXXXXX` (the suffix is unique to each module).
2. Connect the PC, tablet, or phone to that hotspot.
3. Browse to `http://192.168.200.1` (with the default module ID 0 — the address is 192.168.(200 + module ID).1).

### Settings

| Setting | Description |
|---------|-------------|
| **Communication — Mode** | WiFi (UDP), CAN bus, or Ethernet (UDP over a wired W5500 board). WiFi/Ethernet and CAN must match the Module communication setting in the PC app — the app treats WiFi and Ethernet identically. |
| **Communication — Ethernet subnet** | First three octets of the wired network (default `192.168.1`). The module takes IP `subnet.(50 + module ID)`; give the PC's wired adapter a static IP on the same subnet (e.g. `192.168.1.10`). In Ethernet mode the portal shows whether the W5500 board and cable link are detected. |
| **Optical Sensor — Signals** | **Main + Comp** (default): both receiver outputs are wired to the module. The module compares them on every edge and rejects electrical noise glitches. **Main only**: only the main signal wire is connected — for example, when sharing the elevator sensor with another yield monitor through a harness that does not carry the complementary wire. Noise rejection is disabled in this mode. |
| **Optical Sensor — Polarity** | **PNP** (default — FarmTrx-style, output HIGH with beam clear) or **NPN** (inverted logic). Select NPN if flow reads high with no grain and low with grain. |
| **WiFi Network** | Name and password of an external WiFi network. Tick **Use this Network** to have the module join it in addition to its own hotspot. If the connection fails repeatedly the module reverts to hotspot-only. |
| **Hotspot — Password** | Password for the module's own hotspot. Use 8–10 characters, or leave empty for an open hotspot. |

Press **Save / Restart** to store the settings in the module and restart it.

### Firmware update

The **Update Firmware** link at the bottom of the portal opens the over-the-air update page. Select the compiled firmware file (`.bin`) and upload it — the module flashes itself and restarts. The installed firmware version is shown at the top of the portal page.

> **Note:** A firmware update that changes the stored settings layout resets the module to factory defaults. Reopen the portal afterwards and re-enter your settings.

---

## 15. Troubleshooting

### GPS indicator stays red

- Confirm AgOpenGPS is running on the same PC or local network
- Check that both apps are on the same subnet
- Verify the network configuration in YieldFlo Settings

### Module indicator stays off

- Check the module is powered and the WiFi or CAN connection is active
- In WiFi mode, confirm the PC is connected to the correct network
- Check the module LED for status indication
- Restart the module and wait 10–15 seconds for it to connect

### Yield reads zero or very low

- Check the elevator optical sensor alignment and wiring
- Verify the Sensor Baseline is set correctly (set with elevator running empty)
- Confirm the processing delay is appropriate for your combine
- If the sensor's complementary wire is not connected, set **Signals** to **Main only** in the module web portal — in Main + Comp mode a missing complementary signal causes erratic, low readings

### Moisture reads incorrectly

- Verify the moisture sensor is installed and wired correctly
- Check that the moisture calibration (offset and scale) is set in the active profile via **Menu → Moisture Cal**
- Compare against a certified grain moisture meter and adjust the calibration

### App crashes on start

- Check `YieldFlo_Crash.log` in the application folder for details

### Job does not resume after restart

- Ensure **Resume Job on Start** is enabled in Settings
- The job must have been active (not stopped) when YieldFlo was closed

### AOG UDP failed to start

- If AgOpenGPS Rate Controller (RC) is running, both RC and YieldFlo must be built from current source for UDP port sharing to work
- YieldFlo will still function for module data — only GPS reception will be affected

---

*YieldFlo is designed for use with AgOpenGPS. It is not a certified weighing system and should not be used as the sole basis for grain settlement.*
