# YieldFlo User Manual

**Version 1.0**

---

## Table of Contents

1. [Overview](#overview)
2. [Requirements and Installation](#requirements-and-installation)
3. [First Launch](#first-launch)
4. [Main Screen](#main-screen)
5. [Jobs](#jobs)
6. [Crops](#crops)
7. [Headers](#headers)
8. [Profiles](#profiles)
9. [Fields](#fields)
10. [Yield Calibration](#yield-calibration)
11. [Moisture Calibration](#moisture-calibration)
12. [Yield Map](#yield-map)
13. [Job Report](#job-report)
14. [Settings](#settings)
15. [Language](#language)
16. [Module Web Portal](#module-web-portal)
17. [Troubleshooting](#troubleshooting)

---

## Overview

YieldFlo is a yield monitoring application that works alongside AgOpenGPS (AOG). It records yield rate, moisture, and GPS position continuously while harvesting, and produces spatial yield maps and job reports.

YieldFlo receives GPS and section control data from AOG over a local network (UDP). A YieldFlo hardware module connects to the combine's clean-grain elevator and moisture sensor and sends live readings to the PC application.

---

## Requirements and Installation

**PC requirements**

- Windows 10 or Windows 11
- .NET Framework 4.8 (pre-installed on Windows 10 May 2019 Update and later)
- AgOpenGPS running on the same PC or local network

**No installer is required.** Extract the YieldFlo folder to any location and run `YieldFlo.exe`.

**Hardware module**

- YieldFlo module (based on ESP32) mounted on the combine
- Connected to the clean-grain elevator optical sensor
- Connected to the capacitance moisture sensor (optional)
- Communicates with the PC via WiFi (Phase 1) or CAN bus (Phase 2)

**Network**

- For WiFi mode: connect the PC to the same WiFi network as the YieldFlo module, or directly to the module's WiFi access point
- AOG must be running and broadcasting GPS/section data on the local network

---

## First Launch

On first launch, YieldFlo creates a default set of records:

| Item | Default |
|------|---------|
| Crop | Wheat |
| Header | 30ft Draper |
| Profile | Default |

Before starting your first job, review these defaults in the **Menu** and update them for your equipment.

**Recommended setup steps:**

1. Open **Menu → Settings** and select your preferred units (Imperial or Metric)
2. Open **Menu → Profiles** and enter your combine ID and moisture calibration values
3. Open **Menu → Crops** and add or edit crops for your operation
4. Open **Menu → Headers** and add your header with the correct width
5. Open **Menu → Fields** and add field names if desired

---

## Main Screen

The main screen displays live harvest data and job status.

### Data panels

| Panel | Description |
|-------|-------------|
| **YIELD** | Instantaneous yield rate (bu/ac or t/ha), smoothed over recent readings |
| **MOISTURE** | Live grain moisture percentage from the capacitance sensor |
| **SPEED** | Ground speed received from AOG GPS |
| Totals area | Total area harvested, total mass, and average yield for the current job |

### Sensor bars

Two vertical bar gauges on the right side of the screen:

- **Left bar — Elevator flow:** Shows the grain obstruction percentage in the clean-grain elevator. Higher = more grain flowing.
- **Right bar — Moisture:** Shows the relative moisture sensor reading.

The numeric values below each bar show the raw sensor counts from the module.

### Status bar

The status bar at the bottom shows connection and job status:

| Indicator | Green | Orange | Off/Red |
|-----------|-------|--------|---------|
| **GPS** | AOG connected and sending position | — | No AOG data |
| **Module** | Module data received within 5 s | — | No module data |
| **Job name** | Job recording (active) | Job paused | No active job |

### Buttons

| Button | Action |
|--------|--------|
| **Menu** | Open the main menu |
| **Start** | Start or resume the active job |
| **Pause** | Manually pause recording |
| **Stop** | Stop and close the current job |
| **Exit** | Close YieldFlo |

---

## Jobs

A job records yield, moisture, GPS position, and area for a single harvest session. Each job is linked to a crop, header, and profile.

### Starting a job

1. Press **Menu** on the main screen
2. Press **Jobs**
3. Press **New** and enter a job name
4. Select a **Crop**, **Header**, and **Profile** from the drop-downs
5. Optionally select a **Field**
6. Press **Start Job**

The job becomes active and recording begins when harvesting conditions are met (speed > 0.5 km/h and AOG sections are on).

### Managing jobs

The jobs list shows all saved jobs with their status, date, area, and linked field.

- **Load** — make a saved job the active job (prompts if another job is already active)
- **Delete** — permanently remove a job and all its recorded data (cannot be undone)

Only one job can be active at a time. Switching to a different job from the list will prompt for confirmation if a job is currently active.

### Auto-pause

The job automatically pauses when harvesting stops:

- Machine speed falls below 0.5 km/h, **or**
- AOG turns all header sections off (e.g. turning on headland, travelling over harvested ground)

When auto-paused the job name turns orange in the status bar. Recording resumes automatically when harvesting conditions return — no user action needed.

### Manual pause and resume

Press **Pause** on the main screen to manually pause recording. Press **Start** to resume.

### Stopping a job

Press **Stop** → confirm. The job is saved and closed. All totals are written to the database. The job remains in the list and can be viewed in the Job Report.

### Resume on start

If **Resume Job on Start** is enabled in Settings, the active job is automatically reloaded when YieldFlo restarts. This is useful if the PC is rebooted during a harvest day.

---

## Crops

Crops store the grain-specific parameters used in yield calculations and reporting.

### Crop settings

| Field | Description |
|-------|-------------|
| **Name** | Crop name (e.g. Wheat, Corn, Canola) |
| **Category** | Grain type category |
| **Test Weight** | Standard bushel weight (lbs/bu) — used to convert mass to bushels |
| **Market Moisture** | Standard moisture for yield reporting (e.g. 14.5% for wheat) |

### Adding a crop

1. Press **Menu → Crops**
2. Press **New** and enter a crop name
3. Set the test weight and market moisture
4. Press **Save**

### Editing a crop

Select the crop from the list, change the values, and press **Save**.

At least one crop must exist at all times.

---

## Headers

Headers define the cutting width of the front attachment. Width is used to calculate the area harvested.

### Header settings

| Field | Description |
|-------|-------------|
| **Name** | Header name (e.g. 30ft Draper, 8-row Corn Head) |
| **Type** | Header type category |
| **Width** | Cutting width in feet (Imperial) or metres (Metric) |
| **Ahead of Pivot** | Distance the header sits ahead of the position AOG broadcasts. Enter AOG's **pivot-to-header** distance (from the AOG implement setup). This shifts the recorded coverage to the header, so pass boundaries on the yield map land where the header actually crossed them. |

### Adding a header

1. Press **Menu → Headers**
2. Press **New** and enter a header name
3. Enter the cutting width
4. Enter the pivot-to-header distance from your AOG implement setup
5. Press **Save**

At least one header must exist at all times. Changes to a header take effect
when a job is started or loaded.

> **Note — coverage painted slightly before AOG's:** AOG turns its section
> bits on *early* by its turn-on look-ahead (anticipating valve opening time)
> but paints its own coverage only where product actually applies. YieldFlo
> records from the section bits, so each pass start on the yield map can begin
> a metre or two before AOG's painted coverage. Pass *ends* match exactly.
> This is normal and harmless for harvest — there is no valve delay on a
> header — and can be removed in testing by setting AOG's section look-ahead
> to zero.

---

## Profiles

Profiles store combine-specific and calibration settings. Use a separate profile for each combine if running YieldFlo on multiple machines.

### Profile settings

| Field | Description |
|-------|-------------|
| **Name** | Profile name (e.g. JD 9600, Case 2388) |
| **Combine ID** | Identifier for the module (must match the module configuration) |
| **Moisture offset** | Fixed offset applied to all moisture readings (°C from the temperature sensor) |
| **Moisture scale** | Counts-per-percent for the moisture sensor calibration |
| **Temperature offset** | Temperature offset (°C) |
| **Temperature scale** | Counts-per-degree for the temperature sensor |

Moisture and temperature calibration values are set in **Menu → Moisture Cal** and saved automatically to the active profile.

### Adding a profile

1. Press **Menu → Profiles**
2. Press **New** and enter a profile name
3. Enter the Combine ID to match your module
4. Press **Save**

At least one profile must exist at all times.

---

## Fields

Fields are optional location labels that can be assigned to jobs for organisation and reporting.

### Adding a field

1. Press **Menu → Fields**
2. Press **New** and enter a field name
3. Press **Save**

Fields are not required. A job can be created without a field selected.

---

## Yield Calibration

Yield calibration corrects the elevator sensor reading to match the actual mass of grain harvested. It involves two parameters:

- **Sensor Baseline** — the obstruction reading with paddles running but no grain (set once at installation)
- **Yield Factor** — a multiplier that scales the sensor reading to match a weighed reference

### Setting the baseline

The baseline must be set before the first calibration run:

1. Start the clean-grain elevator with no grain
2. Press **Menu → Yield Cal**
3. Press **Set Baseline** — it samples for 5 seconds and enters the result in the **Sensor Baseline** field
4. Press **Save && Apply**

Nothing takes effect, and nothing is saved, until **Save && Apply** is pressed — this applies to both Set Baseline and Apply Cal.

> **Tip:** Run the empty elevator for at least 10 seconds before setting the baseline so the reading stabilises.

### Noise readout

Next to the **Set Baseline** button, the **Noise** readout shows how many electrical glitches per second the module is rejecting on the optical sensor signal, averaged over the last 5 seconds. On a healthy installation it reads **0**; the value turns orange when glitches are being rejected, and shows **--** when no module is connected.

A rising noise value — especially one that climbs with elevator speed — indicates electrical interference on the sensor wire (static discharge from the elevator, a chafed or loose wire shaken by vibration, or pickup from nearby wiring). The module filters these glitches out of the yield reading automatically, but persistent noise is worth fixing at the source: check the sensor wire's routing and shielding, connector condition, and the ground path from the sensor bracket and elevator housing to the chassis. The readout works in both sensor signal modes, including Main only.

### Paddle rate readout

Below the **Set Baseline** button, the **Paddles** readout shows how many elevator paddles per second the sensor is seeing, averaged over the last 5 seconds. It shows **--** when no module is connected or the module firmware predates the feature.

Use it to verify the sensor is catching every paddle: the value should match the paddle rate calculated from elevator speed and paddle spacing, and should scale directly with elevator rpm. A reading at half the expected rate means the sensor is missing paddles (alignment or sensing-range problem). On combines with another optical yield monitor sharing the sensor, the value can be compared directly against that monitor's sensor calibration result (for FarmTrx, the "X% @ Y Hz" line on its Sensor Calibration page).

### Processing delay

The processing delay accounts for the travel time of grain from the header to the elevator sensor. Grain cut at a given GPS position arrives at the sensor several seconds later.

Set this value (in seconds) to match your combine. Typical range is 4–12 seconds depending on combine model and ground speed.

### Running a calibration pass

A calibration pass measures the actual mass of grain harvested during a known run, then adjusts the Yield Factor to match.

**Preparation:**

- Position a weigh wagon or grain cart to catch grain from the unloading auger
- Know your starting and ending tank weights, or weigh directly

**Procedure:**

1. Press **Menu → Yield Cal**
2. Press **Start Run** — the app begins accumulating sensor data
3. Harvest a suitable area (at least one full tank is recommended for accuracy)
4. Press **Stop Run**
5. Weigh the harvested grain
6. Enter the actual weight in the **Actual weight** field
7. Press **Apply Cal**

YieldFlo calculates a new Yield Factor and enters it in the **Yield Factor** field. Press **Save && Apply** to save it to the active profile.

Below the **Save && Apply** button, a **Last saved** line shows the date and time the calibration for the active profile and crop was last saved with **Save && Apply**. It is blank until a calibration has been saved.

### Manual factor adjustment

The Yield Factor can also be adjusted manually in the **Yield Factor** field. Increase the value to raise yield readings; decrease to lower them.

---

## Moisture Calibration

Moisture calibration maps the raw sensor counts from the capacitance moisture sensor to actual moisture percentage and grain temperature.

Calibration values are stored per-profile, so each combine can have its own calibration.

### Moisture calibration

The moisture reading is calculated as:

```
Moisture (%) = offset + (scale × sensor count)
```

| Field | Description |
|-------|-------------|
| **% (saved to crop)** | The reference moisture reading from an approved meter |
| **%/count** | The counts-per-percent scale factor (saved to the active profile) |

**Procedure:**

1. Take a grain sample and measure moisture with a certified grain moisture meter
2. Press **Menu → Moisture Cal**
3. With the module connected and grain flowing, note the live sensor reading
4. Enter the meter reading in the **%** field
5. Adjust the **%/count** scale until the displayed reading matches
6. Press **Save**

### Temperature calibration

The temperature sensor in the moisture probe is calibrated similarly:

| Field | Description |
|-------|-------------|
| **°C (saved to profile)** | Reference temperature from a thermometer |
| **°C/count** | Counts-per-degree scale factor |

> **Note:** Temperature calibration is optional. Temperature is used internally to compensate moisture readings; it does not appear directly on the main screen.

---

## Yield Map

The yield map displays a colour-coded spatial plot of yield rate across the harvested area.

### Viewing the map

Press **Menu → Yield Map**.

The map loads all data points for the selected job and draws them as coloured circles at each GPS position. Colour ranges from blue (low yield) to red (high yield), with the **Low** and **High** legend values shown below the map.

### Selecting a job

Use the job selector at the top of the map to choose which job to display. The map updates when a different job is selected.

### Refreshing

Press **Refresh** to reload the map data. This is useful if the map is open during an active job and you want to see the latest points.

---

## Job Report

The job report shows a summary of a completed or active job.

Press **Menu → Reports** to open the report for the most recently viewed job.

### Report contents

| Field | Description |
|-------|-------------|
| **Field** | Field name assigned to the job |
| **Area** | Total area harvested (ac or ha) |
| **Total** | Total mass harvested (bu or tonnes) |
| **Avg Yield** | Average yield rate across all data points |
| **Avg Moisture** | Average grain moisture across all data points |
| **Data Points** | Number of recorded GPS data points |

### CSV export

Press **Export CSV** to save all raw data points for the job to a CSV file.

The CSV contains one row per data point with the following columns:

| Column | Description |
|--------|-------------|
| Timestamp | Date and time of the reading |
| Latitude | GPS latitude (decimal degrees) |
| Longitude | GPS longitude (decimal degrees) |
| Yield | Instantaneous yield rate |
| Moisture | Grain moisture (%) |
| Speed | Ground speed |
| Heading | GPS heading (degrees) |
| Area | Incremental area for this point |

The file is saved to the `Jobs` folder inside the YieldFlo application folder. The file name includes the job name and date.

---

## Settings

Press **Menu → Settings** to open the settings form.

### Units

| Setting | Options |
|---------|---------|
| **Units** | Imperial (bu/ac, mph, acres, lbs) or Metric (t/ha, km/h, ha, tonnes) |

Changing units takes effect immediately. Historical data is stored in metric units internally and converted for display.

### Module communication

| Setting | Description |
|---------|-------------|
| **WiFi / Ethernet** | Module communicates over UDP — via the module's WiFi access point / a shared WiFi network, or via wired Ethernet (W5500 board on the module). |
| **CAN Driver** | Module communicates over CAN bus (Phase 2 hardware required). |
| **COM Port** | Serial port for CAN interface (SLCAN driver only). |

The module can also send the same UDP packets over **wired Ethernet** (W5500 board attached to the module). No app setting is needed — select Ethernet mode in the module's web portal and give the PC's wired adapter a static IP on the module's subnet (default `192.168.1.x`). The app receives WiFi and Ethernet packets on the same port.

> **Module setup page:** the module's own settings (communication mode, sensor signals, WiFi credentials, firmware update) are configured on its built-in web page at `http://192.168.200.1`, reached while connected to the module's WiFi hotspot — see [Module Web Portal](#module-web-portal).

#### Supported CAN adapters

| Driver | Adapters | Notes |
|--------|----------|-------|
| **SLCAN** | CANable (slcan firmware), SH-C30A, other SLCAN adapters | Appears as a COM port — select it under **COM Port**. |
| **InnoMaker** | InnoMaker USB2CAN | Requires the vendor's driver and SDK files: copy `InnoMakerUsb2CanLib.dll` and `LibUsbDotNet.dll` from the InnoMaker USB2CAN C# SDK (`Lib` folder) into the YieldFlo application folder. Download them as raw binaries, not from a GitHub web page. The COM Port setting is ignored — the first adapter found is used. |
| **PCAN** | Peak PCAN-USB | Install the PEAK device driver package (includes PCAN-Basic) from peak-system.com — no files need to be copied. The COM Port setting is ignored — the first adapter found on PCAN USB channels 1–8 is used. |

If CAN fails to start, the reason is written to the error log in `Documents\YieldFlo\Logs`.

### Resume Job on Start

When enabled, YieldFlo automatically reloads the active job when the app is started. Useful if the PC is rebooted during a harvest day.

### Saving settings

Press **Save && Apply** to save and apply all settings immediately.

---

## Language

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
3. Press **Save && Restart**

YieldFlo will restart automatically with the new language applied.

> **Note:** If AgOpenGPS is already running with a language set, YieldFlo will automatically use the same language on first launch.

---

## Module Web Portal

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

## Troubleshooting

### GPS indicator stays red

- Confirm AgOpenGPS is running on the same PC or local network
- Check that both apps are on the same subnet
- In YieldFlo Settings, verify the network configuration

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
- Check that the moisture calibration (offset and scale) is set in the active profile
- Compare against a certified grain moisture meter and adjust the calibration

### App crashes on start

- Check `YieldFlo_Crash.log` in the application folder for details

### Job does not resume after restart

- Ensure **Resume Job on Start** is enabled in Settings
- The job must have been active (not stopped) when YieldFlo was closed

---

*YieldFlo is designed for use with AgOpenGPS. It is not a certified weighing system and should not be used as the sole basis for grain settlement.*
