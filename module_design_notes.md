---
name: YieldFlo module hardware design notes
description: Sensor approach, flow measurement method, RPM sensor options, FarmTRX comparison
type: project
---

## Packet format (current — PGN 40001, 13 bytes)

| Bytes | Field | Type | Notes |
|---|---|---|---|
| 0–1 | PGN 40001 | uint16 LE | 0x41, 0x9C |
| 2 | Packet type | byte | 0x01 |
| 3–4 | Sensor 1 count | uint16 | obstruction ratio × 1000 |
| 5–6 | Sensor 2 count | uint16 | second optical sensor |
| 7–8 | Moisture raw | uint16 | ADC/capacitance reading |
| 9–10 | Module RPM | uint16 | elevator shaft speed |
| 11 | Status flags | byte | bits: S1 OK, S2 OK, moisture OK |
| 12 | CRC8 | byte | sum of bytes 0–11 |

Potential additions: module temperature (moisture compensation), supply voltage, fault code byte.

---

## Flow measurement — optical sensor

An IR TX/RX pair mounts across the clean-grain elevator housing. The beam passes through the elevator throat:

- Elevator paddles interrupt the beam periodically at a rate proportional to elevator RPM
- With no grain, only the paddles block the beam — this is the **baseline** obstruction ratio (~0.2)
- Grain on the paddles increases blocked duration → higher ratio
- Teensy counts blocked ticks vs total ticks per window, reports ratio × 1000

**App calculation:**
```
netRatio = clamp(sensorRatio - SensorBaseline, 0)
InstantYield = netRatio × YieldFactor × speed / HeaderWidthM
```

---

## Similarity to RC flow meter pulse counting

A flow meter (RC module) and the optical elevator sensor are structurally identical:

- **Flow meter:** pulses_per_second × volume_per_pulse = flow rate
- **Optical sensor:** (blocked_fraction − baseline) × YieldFactor = flow rate

Both are a rate-proportional signal multiplied by a calibration constant. The ratio approach is actually **better suited to an elevator** than pure pulse counting because it naturally handles variable elevator RPM — if the elevator slows, the ratio stays proportional to grain load without requiring RPM compensation.

The existing `clsYieldCalculator` code is already structured identically to an RC flow accumulator. Only `Calculate()` and the packet parser would need changes to switch to a pure pulse-count approach.

---

## RPM sensor — optional vs required

**Without RPM sensor:**
- The ratio approach still works
- The YieldFactor calibration implicitly includes "normal" elevator speed
- Calibration remains valid as long as elevator speed doesn't vary significantly
- Suitable when elevator is driven directly from combine (speed tracks engine RPM, varies little)
- Simpler hardware — one fewer sensor

**With RPM sensor:**
- Allows normalizing sensor ratio by actual elevator speed:
  `normalizedFlow = netRatio / (moduleRpm / referenceRpm)`
- Makes YieldFactor independent of elevator speed variation
- Important if elevator can slip, slow under heavy load, or run at different speeds
- `module_rpm` field already exists in the packet and is sent by the module
- **Currently unused in clsYieldCalculator.Calculate()** — field is received but not applied

**Recommendation:** Design firmware to support both modes. If an RPM sensor is connected, the module reports actual RPM. If not, it reports a fixed reference value (e.g. 200) so the app can detect the difference and enable/disable RPM normalization via a settings flag.

---

## Moisture measurement — capacitance sensor

Grain flowing past two electrodes changes the dielectric constant → changes capacitance → ADC reading proportional to moisture. The Teensy converts ADC to moisture % using a crop-specific lookup or polynomial.

**Limitations:**
- Temperature dependent — module temperature should be logged for compensation
- Different crops have different dielectric curves — hence moisture offset is per-crop in YieldFlo
- Needs to be in the grain stream, not measuring air gaps between paddles

**Alternative:** NIR spectroscopy — more accurate but much more expensive. Capacitance is standard in entry-level monitors (FarmTRX, Ag Leader Insight).

**FarmTRX:** Whether FarmTRX uses an elevator RPM sensor is unconfirmed — verify against their installation documentation.

---

## Selected optical sensor — Banner T18-2

**Datasheet:** p/n 201875 Rev. E (D:\Sync\YieldFlo\PhotoSensor\201875.pdf)

The Banner T18-2 Epoxy Encapsulated Right-Angle sensor family is the selected sensor for the elevator optical measurement. Key specs relevant to YieldFlo:

- **Supply:** 10–30V DC — powered directly from 12V combine rail
- **Output response:** 1.5 ms ON / 1 ms OFF (opposed mode); repeatability 187 µs — adequate for elevator paddle rates
- **Environmental:** IP67/IP68/IP69K, –40 °C to +70 °C, ECOLAB certified — suitable for combine environment
- **Connection:** 4-pin M12 quick disconnect

**Model selection:**
- Emitter: **T18-2NAEL-Q8** (visible red, Brown/Blue power only — no signal wire)
- Receiver NPN: **T18-2VNRL-Q8** — complementary sinking outputs
- Receiver PNP: **T18-2VPRL-Q8-809578** — complementary sourcing outputs — **selected variant**

FarmTrx uses the **T18-2VPRL-Q8-809578** (PNP). Adopting the same part ensures alignment with a proven field installation and simplifies sourcing.

**M12 pinout:**

| Pin | Wire | Function |
|---|---|---|
| 1 | Brown | +12V supply |
| 2 | White | Complementary output (opposite of Pin 4) |
| 3 | Blue | GND |
| 4 | Black | Main output |

---

## Complementary dual-output noise rejection

The T18-2 receiver provides two complementary outputs (Pin 4 and Pin 2) that are always opposite states. Using both in firmware gives noise rejection without additional hardware:

- A valid beam transition flips **both** signals simultaneously
- A noise glitch on one wire leaves the other unchanged → reject if both pins read the same logic level
- A `noiseCount` metric per reporting window flags marginal alignment or EMI during installation

**ESP32 ISR logic:**
```
On CHANGE interrupt from either pin:
  read both pins
  if (pinA == pinB)         → noise, discard
  if (newState == currentState) → duplicate interrupt, discard
  else                      → valid edge, accumulate blocked/clear time
```

The ratio reported each window is `blockedTime / (blockedTime + clearTime)`, accounting for the partial in-progress segment at snapshot time.

---

## RC16 PCB as module base

The RC16 rate controller PCB (`D:\Sync\RATE CONTROL\PCB Design\RC16 v9\RC16.kicad_sch`) is a strong base for the YieldFlo hardware module.

**Key components:**

| Component | Status | Notes |
|---|---|---|
| ESP32-WROOM-32U | Keep | Identical MCU |
| MCP2562 CAN transceiver + 120Ω + ESD | Keep | Identical to requirements |
| XL2596 buck + AZ1117 3.3V LDO | Keep | 12V in → 3.3V, correct rails |
| CH340C USB-UART | Keep | Firmware programming |
| 74HC14 Schmitt trigger (×6 gates) | Keep | Sensor signal conditioning |
| BSS138 level shifters (Q1, Q2) | Keep/repurpose | |
| PC817 optocouplers (U5, U6, U7) | Keep | U5=RPM, U6=MainSignal, U7=CompSignal |
| VNH5200 H-bridge motor driver | **Remove** | Not needed — biggest saving |

**Changes required:**
- Remove VNH5200 and associated motor drive passives
- Swap output connector pinout for M12 sensor connector
- Rework PC817 U6/U7 input circuits for PNP sourcing sensor (see sensor interface section below)
- Add 10kΩ pull-downs on MainSignal and CompSignal nets — new components, replaces NPN pull-ups
- Add moisture sensor header and RPM sensor header

**Sensor interface — T18-2VPRL (PNP) requires PC817 input circuit revision:**

Verified from YF1 v2 schematic (`D:\Sync\YieldFlo\PCB design\YF1 v2\YF1.kicad_sch`). The two optical sensor channels are:

- **U6** (MainSignal / T18-2 Pin 4 Black) — series resistor **R8** (270Ω), output pull-up **R11** (10K)
- **U7** (CompSignal / T18-2 Pin 2 White) — series resistor **R9** (270Ω), output pull-up **R12** (10K)
- **U5** (RPM) — series resistor **R7** (270Ω), output pull-up **R10** (10K) — RPM sensor type TBD

**Current NPN circuit (U6 as confirmed in schematic):**
```
+5V ──── R8 (270Ω) ──── U6 Anode (pin 1)
                         U6 Cathode (pin 2) ──── MainSignal ──── [NPN sensor → GND]
U6 Collector (pin 4) ──── R11 (10K) ──── 3.3V / MainPin → GPIO
U6 Emitter   (pin 3) ──── GND
```

**Required PNP circuit (U6 after rework):**
```
MainSignal ──── R_PD (10kΩ to GND)   [pull-down, new component]
MainSignal ──── U6 Anode (pin 1)
                U6 Cathode (pin 2) ──── R8 (560Ω) ──── GND
U6 Collector (pin 4) ──── R11 (10K) ──── 3.3V / MainPin → GPIO   [unchanged]
U6 Emitter   (pin 3) ──── GND                                     [unchanged]
```

Apply identically to U7/R9/R12/CompSignal.

**LED current:** (12V − 1.2V) / 560Ω = **19 mA** — well within PC817 spec, well above ~1 mA threshold.

**No firmware changes needed.** Output polarity through PC817 → 74HC14 → GPIO is preserved.

**Schematic change summary — per optical channel (U6 and U7):**

| Item | Current (NPN) | Required (PNP) |
|---|---|---|
| R8 / R9 value | 270Ω | **560Ω** |
| R8 / R9 position | +5V side → Anode | **Cathode → GND** |
| +5V power symbol on input net | Present | **Delete** |
| GND symbol on cathode/R end | Absent | **Add** |
| Sensor signal connects to | Cathode (pin 2) | **Anode (pin 1)** |
| Pull-down on signal net | None | **Add 10kΩ to GND** (new R) |
| R11 / R12 (output pull-up) | Unchanged | No change |
| Collector / Emitter wiring | Unchanged | No change |
| Firmware | — | No change |

**RPM channel (U5/R7/R10):** Same changes required if RPM sensor is PNP. Leave unchanged until RPM sensor is selected.

---

## Signal protection — MainSignal and CompSignal (YF1 v2)

### Existing protection (confirmed in schematic)

**D6** (SMBJ15CA, SMB) — MainSignal to GND
**D8** (SMBJ15CA, SMB) — CompSignal to GND

LCSC part: C19077570. Bidirectional TVS, 15V standoff, clamps at ~16.7–24.4V, 600W peak pulse. Shunts high-energy transients from the field harness to GND before they reach the PC817 anode. Bidirectional variant handles both positive and negative spikes.

### Proposed additional protection — BAT42 diode clamps

Add a BAT42 clamp pair per channel, directly at the PC817 anode input, for tighter clamping within normal operating range:

```
MainSignal ──┬──[SMBJ15CA]──GND       (high-energy transients, clamps ~16.7V+)
             │
             ├── BAT42: anode→signal, cathode→+12V   (clamp high to ~12.2V)
             ├── BAT42: cathode→signal, anode→GND    (clamp low to ~-0.2V)
             │
             ├── series resistor (100–470Ω, anode side)   [limits clamp current]
             │
             └──── PC817 anode (U6 pin 1)
```

Apply identically to CompSignal / U7.

**Why BAT42 upper clamp (to +12V):** Normal PNP sensor output swings to ~12V — clamping at 12.2V does not affect normal operation but limits overvoltage at the PC817 LED, which has a low absolute maximum reverse voltage (6V).

**Why BAT42 lower clamp (to GND):** Clamps negative transients to ~-0.2V, protecting the PC817 LED from reverse breakdown. The SMBJ15CA handles large reverse events; the BAT42 provides a tight fast clamp for smaller negative spikes.

**Series resistor:** A small resistor (100–470Ω) on the anode side limits current through the BAT42s when clamping. The existing R8/R9 (560Ω, cathode side) partially serves this role but is on the wrong side of the LED for anode-side protection.

| Layer | Device | Clamps at | Handles |
|---|---|---|---|
| 1 | SMBJ15CA | ~16.7–24.4V | High-energy harness transients |
| 2 | BAT42 upper | ~12.2V | Overvoltage above normal 12V swing |
| 2 | BAT42 lower | ~-0.2V | Negative spikes / PC817 reverse voltage |

---

---

## YF1 v1 PCB — confirmed design

**Schematic:** D:\Sync\YieldFlo\PCB design\YF1 v1\YF1.kicad_sch

### J1 connector pinout (DTM13-12PA-R008)

| Pin | Signal | Connects to |
|---|---|---|
| 1 | +12V | T18-2 emitter Brown |
| 2 | +12V | T18-2 receiver Brown |
| 3 | GND | T18-2 emitter Blue / RPM GND / DS18B20 GND |
| 4 | GND | T18-2 receiver Blue |
| 5 | CompSignal | T18-2 receiver Pin 2 (White) → PC817 U7 |
| 6 | MainSignal | T18-2 receiver Pin 4 (Black) → PC817 U6 |
| 7 | CAN_L | CAN bus |
| 8 | CAN_H | CAN bus |
| 9 | RPM | RPM sensor signal → PC817 (3rd opto, same circuit) |
| 10 | Moisture A | Electrode A → ADS1115 AIN0 |
| 11 | Temperature | DS18B20 DQ → ESP32 IO17 (1-Wire) |
| 12 | Moisture B | Electrode B → ADS1115 AIN1 (differential pair with AIN0) |

**Moisture measurement is differential AIN0 − AIN1.** Both electrode wires run in the same harness — common-mode noise cancels in the subtraction. RPM and DS18B20 GND returns share pins 3/4.

### Key design decisions confirmed

- **ADS1115 powered at +5V** — better analog range; PCA9306 I2C level shifter bridges ESP32 3.3V I2C to 5V ADS1115
- **DS18B20 on ESP32 IO17** — 1-Wire digital, frees all ADS1115 channels for analog; requires 4.7kΩ pull-up to 3.3V on PCB
- **RPM through third PC817** — consistent isolated input, same circuit as MainSignal and CompSignal
- **Moisture on ADS1115 AIN0** — single-ended; AIN1–AIN3 available for expansion
- **Dual 12V pins** — supplies T18-2 emitter and receiver independently
- **Triple GND pins** — return paths for three isolated sensor circuits

### Firmware notes

- DS18B20 parasitic vs normal power mode must match PCB wiring — normal mode preferred for reliability
- ADS1115 ADDR pin sets I2C address — confirm net connection (GND=0x48, VDD=0x49, SDA=0x4A, SCL=0x4B)
- ADS1115 AIN1–AIN3 unallocated — available for supply voltage monitoring or second analog sensor

---

## Moisture sensor — signal conditioning front end (REQUIRED)

The ADS1115 is a DC/low-frequency ADC and **cannot directly measure a 30 kHz capacitive signal**. A signal conditioning stage must sit between the electrode and the ADS1115.

### Excitation and detection circuit

```
ESP32 LEDC PWM (30 kHz)
        │
        ├──── 74HC4053D analog switch (synchronous demodulation)
        │              │
        │         Electrode A (P1) ──── grain ──── Electrode B (P2)
        │                                                │
        └──── AD8604ARUZ op-amp stage ◄─────────────────┘
                        │
                   DC voltage (1.5–2V span across moisture range)
                        │
                  ADS1115 AIN0/AIN1 differential
```

**Optimal excitation frequency: 30 kHz** — confirmed by MDPI 2024 research via frequency sweep 1–100 kHz. Above 40 kHz the output saturates and resolution is lost.

**Key components to add to YF1 PCB:**

| Component | Value/Model | Purpose |
|---|---|---|
| 74HC4053D | NXP, SOIC-16 | 3-channel analog switch — synchronous detection |
| AD8604ARUZ | Analog Devices, TSSOP-14 | Quad op-amp — amplify and differential output |
| LM4040 2.5V | SOT-23 | Precision mid-rail reference for op-amp |
| C0G reference cap | Adjustable 6–10 pF | Nulls no-load capacitance, maximises ADC range |

Output voltage span: approximately **2268–3793 mV** across 12–26% moisture (from research data). Well within ADS1115's 5V-supplied input range.

---

## Electrode design — coplanar PCB sensor

The moisture sensor electrode is a small PCB that mounts flush with the elevator wall at the cleanout hatch, with grain sliding across its face.

### Why coplanar, not parallel plate

A parallel plate capacitor sandwiches grain between two opposing plates — impractical to mount in an elevator. A coplanar design places **both electrodes on the same surface**. The electric field lines arc from P1, out through the grain above, and back to P2. This fringing field is sensitive to the dielectric constant of grain flowing over the face — no opposing plate needed.

```
        Grain flow over surface
    ────────────────────────────────
    |  P1 (excitation) | gap | P2 (guard/return) |
    └─────────────────────────────────────────────┘
              FR4 substrate (1.6mm)

    Field lines: arc up through grain from P1 → P2
```

### Dimensions (from MDPI 2024)

| Feature | Dimension |
|---|---|
| Substrate | FR4, 1.6 mm, 2 oz copper |
| P1 (excitation plate) | 24 mm × 56 mm |
| P2 (outer guard plate) | 30 mm × 60 mm |
| Guard strip gap | 0.254 mm (10 mil — standard PCB process) |
| No-load capacitance | ~8 pF |

### How the guard electrode works

P2 surrounds P1 and is held at a defined potential (ground or driven). This eliminates edge fringing effects at the boundary of P1 — the field at P1's edges terminates on P2, not on surrounding metal. Without the guard, nearby elevator metalwork would cause readings to vary with installation geometry.

### Sensitivity and penetration depth

The fringing field penetrates to a depth approximately equal to the electrode pitch. The 24 mm P1 width gives sensitivity to grain a similar distance above the surface — sufficient for elevator flow depths. Grain with higher moisture content has higher dielectric constant (water ε ≈ 80, dry grain ε ≈ 3–5), so even small moisture changes produce measurable capacitance shifts.

### Reference capacitor

A 6–10 pF C0G (NP0) adjustable capacitor in the signal conditioning circuit subtracts the no-load (~8 pF) baseline. This means the op-amp stage amplifies only the **change** due to grain, using the full ADC range for the measurement window rather than wasting range on the fixed baseline.

### Calibration

Output is non-linear. MDPI example calibration curve (soybeans):
```
M = −0.000009U² + 0.064U − 92.665    (U in mV, M in % moisture)
```
Crop-specific curves required — fits naturally with YieldFlo's existing per-crop settings.

### Temperature compensation

```
correctedMoisture = rawMoisture + k × (sensorTemp − 15.5°C)
```
`k` is crop-specific (~−0.18 %/°C for corn). DS18B20 on IO17 provides `sensorTemp`. 15.5°C (60°F) is the industry reference temperature.

---

## CAN tapping — OEM moisture and yield as supplementary data

The YF1 already has an MCP2562 CAN transceiver. By passively listening on the combine's CAN bus, OEM moisture and yield data can be decoded as a supplementary source — cross-checking or replacing the YF1's own sensors where OEM data is available and reliable.

### ISOBUS DDI — standardised data identifiers

ISO 11783-11 defines standard Data Dictionary Identifiers (DDIs) for harvest data. A Task Controller (TC) node broadcasts these in real time:

| DDI | Description |
|---|---|
| 271 | Moisture content (%) × 100 |
| 272 | Dry mass per time (yield) |
| 273 | Dry mass per area |
| 274 | Dry mass flow (kg/s) |
| 275 | Wet mass flow |
| 276 | Volume per area |

These are accessible on the ISOBUS (implement bus, 9-pin Deutsch connector) if the combine's terminal implements TC client. Modern John Deere (GS3/GS4), Case IH AFS Pro 700, and New Holland IntelliView 12 all expose some harvest DDIs.

### OEM-specific CAN buses

| OEM | Bus | Notes |
|---|---|---|
| John Deere | Internal machine CAN + ISOBUS | ActiveYield data on internal bus (address 117); some data mirrored to ISOBUS TC |
| Case IH / NH | AFS CAN + ISOBUS | AFS partly ISOBUS-compliant; moisture and yield on AFS bus |
| AGCO | Internal + ISOBUS | Ag Leader integration exposes data on ISOBUS |

### Implementation approach

1. Connect YF1 CAN to combine's ISOBUS via 9-pin Deutsch connector (already standard on most combines)
2. ESP32 passively listens — does not transmit, no risk of interfering with machine bus
3. Parse incoming CAN frames for known DDIs (271 moisture, 274 mass flow)
4. Where OEM data is present and valid, use as `CANMoisture` and `CANYield` in the packet
5. Fall back to YF1 sensor data when OEM data is absent or zero

### Value

- OEM moisture sensors are pre-calibrated and crop-aware — better accuracy than a new custom sensor during initial deployment
- Cross-checking OEM vs YF1 moisture readings identifies calibration drift
- On John Deere machines with ActiveYield, the OEM mass flow sensor provides an independent yield channel
- Zero additional hardware cost — MCP2562 is already present

### Caution

OEM internal CAN messages use proprietary PGN/CAN ID ranges and require per-model decoding. ISOBUS DDIs are standardised but only exposed if the combine's terminal implements TC. Passive listening is safe; do not transmit on OEM internal buses.

---

**How to apply:** Reference these notes when designing firmware, PCB modifications, or expanding clsYieldCalculator to support RPM normalization.

---

## YF1 v4 — J7 connector pinout (DT13-12PA, confirmed 2026-03-24)

**Schematic:** D:\Sync\YieldFlo\PCB design\YF1 v4\YF1.kicad_sch

J7 is the main vehicle harness connector (TE Connectivity DEUTSCH DT13-12PA, 12-pin, 90° PCB header).

| Pin | Signal | Notes |
|---|---|---|
| 1 | 12V Out | Switched 12V to sensors (T18-2 emitter/receiver) |
| 2 | RPM | RPM sensor signal |
| 3 | MainSignal | T18-2 primary output (Pin 4 / Black wire) |
| 4 | CAN H | J1939 CAN High |
| 5 | Moisture B | OEM sensor Moisture Reference (differential pair) |
| 6 | Moisture A | OEM sensor Moisture Signal |
| 7 | Chassis Ground | Shield/drain reference (isolated CHASSIS_GND net) |
| 8 | Chassis Ground | Shield/drain reference (isolated CHASSIS_GND net) |
| 9 | MoistureTemp | OEM sensor temperature output |
| 10 | CAN L | J1939 CAN Low |
| 11 | CompSignal | T18-2 complementary output (Pin 2 / White wire) — noise rejection pair with MainSignal |
| 12 | 12V In | Supply from combine (12V nominal) |

Dual Chassis Ground pins (7 & 8) provide adequate current capacity for shield returns. CompSignal (pin 11) and MainSignal (pin 3) are always complementary — firmware discards edges where both read the same logic level.

---

## YF1 v2 — Moisture daughter board (confirmed approach)

**Schematic:** D:\Sync\YieldFlo\PCB design\YF1 v2\YF1.kicad_sch

> **Note:** Connector/header numbering below reflects v2. In v4 the signals are routed via J7 (DEUTSCH) and internal headers J5/J6. Update this section when v4 daughter board PCB is designed.

The moisture signal conditioning circuit is **not integrated on the main YF1 PCB**. Instead, the main board exposes a daughter board header. This isolates the unproven moisture front-end from the stable optical/CAN/RPM circuitry and allows the moisture approach to be iterated independently.

### Main PCB provides (2×8 header, 2.54mm or Molex KK)

| Pin | Signal | Direction | Purpose |
|---|---|---|---|
| 1 | +12V | → DB | OEM sensor supply |
| 2 | +5V | → DB | ADS1115 supply |
| 3 | +3.3V | → DB | Logic/op-amp supply |
| 4 | GND | — | Signal return |
| 5 | MoistureA | ↔ | J1-10 passthrough |
| 6 | MoistureB | ↔ | J1-12 passthrough |
| 7 | MoistureTemp | ↔ | J1-11 passthrough |
| 8 | SDA | ↔ | ESP32 I2C bus |
| 9 | SCL | ↔ | ESP32 I2C bus |
| 10 | PWM_EXCITE | → DB | ESP32 LEDC 30 kHz (custom electrode only) |
| 11 | DB_INT | → ESP32 | ADS1115 ALRT/RDY interrupt |
| 12 | CHASSIS_GND | — | Connector shell / shield drain (isolated) |

### Daughter board variants

**Variant A — OEM sensor (simple):** ADS1115 + PCA9306 only. OEM sensor outputs conditioned DC — no excitation circuit needed. Pins MoistureA/B route directly to ADS1115 AIN0/AIN1 differential; MoistureTemp to AIN2.

**Variant B — Custom coplanar electrode:** Full front-end: 74HC4053D + AD8604ARUZ + LM4040 + C0G trim cap + ADS1115 + PCA9306. PWM_EXCITE drives the synchronous demodulator.

### ADS1115 channel assignment (both variants)

| Channel | Custom electrode | OEM sensor |
|---|---|---|
| AIN0 | Electrode A (demodulated) | Moisture Signal (OEM Pin 3) |
| AIN1 | Electrode B (reference) | Moisture Reference (OEM Pin 4) |
| AIN2 | unallocated | Temperature (OEM Pin 5) |
| AIN3 | unallocated | unallocated |

Read mode is differential AIN0−AIN1 in both variants — firmware read logic unchanged, only calibration curve differs.

---

## OEM moisture sensor — Case IH 2388 pinout and J1 mapping

The 2388 uses a 6-pin connector (Deutsch DT06-6S on harness side). Sensor is self-conditioned — outputs DC analog, no external excitation required.

| OEM Pin | Function | J1 Pin | Notes |
|---|---|---|---|
| 1 | +12V | J1-1 or J1-2 | |
| 2 | Signal Ground | J1-3 | Analog return |
| 3 | Moisture Signal | J1-10 → ADS1115 AIN0 | DC analog voltage |
| 4 | Moisture Reference | J1-12 → ADS1115 AIN1 | Differential pair |
| 5 | Moisture Temperature | J1-11 → ADS1115 AIN2 | Processed analog voltage |
| 6 | Shield / Drain | Connector shell | See shield ground section below |

J1-11 is freed from DS18B20 for this use — DS18B20 is PCB-mounted on IO17 and does not need to exit via J1.

---

## Shield ground — KiCad net and PCB topology (YF1 v2)

The shield/drain wire (OEM Pin 6, or any shielded cable shield) connects to the **connector shell / backshell**, not to a signal pin and not directly to the GND net.

### KiCad implementation

- Define a separate net: `CHASSIS_GND`
- J1 shell mounting pads connect to `CHASSIS_GND` copper pour (isolated island)
- `CHASSIS_GND` ties to `GND` at **one point only** — at the main power input connector, via a ferrite bead:

```
J1 shell  →  CHASSIS_GND pour  →  [ferrite bead 300Ω@100MHz, 0603]  →  GND plane
                                    (e.g. Murata BLM21PG300SN1L)
```

Single tie-point at power entry is the lowest-impedance, most stable reference point. Tying anywhere else risks a ground loop through the combine chassis.

### Why not a signal pin

Crimping the shield into a pin cavity and routing it to GND creates a loop: combine chassis → shield → PCB GND → sensor return wire → sensor → combine chassis. This loop couples chassis noise directly into the moisture signal. The backshell/shell approach avoids the loop entirely.

### Backshell

Use Deutsch DTM strain relief backshell (DTHD-24-00 or equivalent) — it has an internal provision for a shield drain wire saddle clamp.

---

## FarmTrx signal tap — shared optical sensor (9070 combine, planned 2026-07-04)

The 9070 has a FarmTrx yield monitor installed: module **AG-YMK-2500** (Yield Monitor Plus+, legacy ECU, **DB15 connector** at head unit in cab). Its optical pair is **T18-2NAEL-Q8-809577** emitter + **T18-2VPRL-Q8-809578** receiver — the same PNP complementary Banner sensor selected for YieldFlo. Plan: run both monitors from the one sensor via a Y-tap at the DB15 in the cab, using the FarmTrx install as a calibrated reference while proving out YieldFlo.

### Why parallel sharing is safe

- YF1 input per channel = PC817 LED + 560Ω ≈ **19 mA**, plus 10k pull-down ≈ 1.2 mA — small fraction of the T18-2 output rating (~150 mA, verify datasheet 201875)
- PC817 opto isolation: only load is LED between signal and sensor GND — no back-path from ESP32 logic into FarmTrx wiring
- 10k pull-down improves PNP off-state definition for both monitors
- Both modules in cab share the Y → common ground automatic

### DB15 pinout — NOT public

Installation PDFs (media.farmtrx.com) are image-based, no pinout; confirmed unhelpful. Option: email support@farmtrx.com with part no. Otherwise identify pins by measurement:

**Continuity trace (chosen method, unpowered both ends):**
1. Unplug M12 at receiver and DB15 at head unit
2. Jumper M12 harness-side pin 4 (black) → pin 3 (blue/GND); at DB15 find ~0 Ω pin to known GND = **Main**
3. Repeat with M12 pin 2 (white) = **Comp**
4. Jumper pin 1 (brown) to find sensor 12V feed — if ECU-switched output (not battery feed), YieldFlo must tap the battery side

**Cross-check (powered, after Y wired):** cardboard beam-block test — Main ≈12V beam clear / ≈0V blocked, Comp always opposite. Firmware reads HIGH on Main = beam clear (`BeamBlocked = (digitalRead(MainPin) == LOW)`).

**Toggle test variant:** with everything connected and key on, block beam — exactly two DB15 pins swap ~12V↔~0V; HIGH-when-clear = Main → J7 pin 3, other = Comp → J7 pin 11. Probe with head unit connected (its load pins the PNP off-state low); unloaded pins float on a 10MΩ DMM.

### Key risk — complementary wire may not reach the cab

If FarmTrx only wired black through the harness, white/Comp never reaches the DB15. Firmware **requires both** signals (Begin.ino attaches interrupts only when MainPin AND CompPin valid; ISR noise rejection needs the pair). Fallback: tap white at sensor M12 with a splitter, or pull one extra wire cab-to-elevator.

### Hardware for the tap

- DB15 male-female **breakout board with screw terminals** — probe point during identification, then permanent Y-tap (check FarmTrx shell: 2-row DA-15 vs 3-row HD15 before ordering)
- M12 5-pin female flying-lead cable (A-coded, mates with 4-pin sensor) for the receiver-end breakout during the trace

### Reference measurement

Empty-elevator duty cycle on the OEM install = baseline obstruction ratio. **Measured 2026-07-12 via the Y-tap: ~6%, identical at idle and full rpm** (rpm-independence is expected — the ratio is pure paddle geometry, width ÷ spacing). The earlier ~20% planning estimate was wrong. FarmTrx exposes its own tare for comparison: app → Device Calibration → Sensor Calibration card, "Calibration Results: X% @ Y Hz" (blocked % + paddle frequency; operator's manual p. 20 example shows 5% @ 18.53 Hz — same ballpark). Cross-check: FarmTrx % vs YieldFlo empty flow % from the same run, FarmTrx Hz vs sprocket × chain-speed math. Then compare FarmTrx yield trace vs YieldFlo SensorRatio/NoiseCount during a harvest pass. NoiseCount near zero = clean shared tap.

---

## References

### Sensor hardware
- [Banner T18-2 Datasheet p/n 201875 Rev. E](https://www.bannerengineering.com/us/en/products/part.806862.html)

### OEM moisture sensors
- [John Deere AXE21466 Moisture Sensor Module](https://shop.deere.com/us/product/AXE21466:-Moisture-Sensor-Module/p/AXE21466)
- [John Deere AXE16724 Moisture Sensor Module](https://shop.deere.com/us/product/AXE16724:-Moisture-Sensor-Module/p/AXE16724)
- [John Deere ActiveYield Operation and Adjustment Guide](https://www.deere.com/assets/pdfs/common/qrg/active-yield.pdf)
- [Case IH 47855244 Grain Moisture Sensor](https://www.mycnhstore.com/us/en/caseih/category/electrical/sensors/sensor/p/47855244)
- [New Holland 47855244 Grain Moisture Sensor](https://www.mycnhstore.com/us/en/newhollandag/category/electrical/sensors/sensor/p/47855244)

### Aftermarket moisture sensors
- [FarmTRX Moisture Sensor — product page](https://farmtrx.com/product/moisture-sensor/)
- [FarmTRX Moisture Sensor — development notes](https://farmtrx.com/developing-the-farmtrx-moisture-sensor/)
- [FarmTRX Operator's Manual](https://media.farmtrx.com/farmtrx-operatorsmanual.pdf)
- [Precision Planting YieldSense Operator's Guide](https://docs.precisionplanting.com/2020/operators_guide/yieldsense/)
- [Loup Electronics Elite Yield Monitor](https://loupelectronics.com/products/yield_monitor.html)
- [HarvestMaster GrainGage product page](https://harvestmaster.com/products/graingage)
- [HarvestMaster EM Sensor upgrade notes](https://harvestmaster.com/support/article/14588)

### Academic research
- [MDPI Sensors 2024 — Differential Grain Moisture Detection Device for Combined Harvester](https://www.mdpi.com/1424-8220/24/14/4551) — primary reference for electrode design and signal conditioning circuit
- [PMC11281324 — full text of above](https://pmc.ncbi.nlm.nih.gov/articles/PMC11281324/)
- [PMC8234241 — RF Differential Moisture Sensor (100 MHz)](https://pmc.ncbi.nlm.nih.gov/articles/PMC8234241)

### Community
- [The Combine Forum — aftermarket moisture sensor discussion](https://www.thecombineforum.com/threads/is-there-an-aftermarket-moisture-sensor-for-sts-combines.190242/)
- [NewAgTalk — YieldSense moisture sensor discussion](https://talk.newagtalk.com/forums/thread-view.asp?tid=739453&DisplayType=flat&setCookie=1)
