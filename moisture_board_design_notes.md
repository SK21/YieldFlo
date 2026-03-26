---
name: YieldFlo moisture daughter board design notes
description: Schematic review, input protection, RC filter, component selections, open items
type: project
---

## Overview

The moisture measurement circuit is implemented as a **separate daughter board** (Moisture1) that plugs into the main YF1 PCB via two 8-pin headers (J1 + J2, 16 pins total). This isolates the unproven moisture front-end from the stable optical/CAN/RPM circuitry and allows the moisture approach to be iterated independently.

**Schematic:** `D:\Sync\YieldFlo\PCB design\Moisture1\Moisture1.kicad_sch`

This board is **Variant A — OEM sensor interface**. It accepts a conditioned DC analog output from an OEM combine moisture sensor (e.g. Case IH 2388) and digitises it via ADS1115. No excitation circuit is present — Variant B (custom coplanar electrode) would add the 74HC4053D synchronous demodulator stage on a future revision.

---

## Connectors

Two 8-pin headers (J1 and J2) connect to the main YF1 board.

### J1 — Conn_01x08_Pin

| Pin | Signal | Notes |
|---|---|---|
| 1 | GND | |
| 2 | IO18 | ESP32 GPIO (purpose TBD) |
| 3 | IO19 | ESP32 GPIO (purpose TBD) |
| 4 | GND | |
| 5 | Moisture A | OEM sensor signal — routes to AIN0 |
| 6 | Moisture B | OEM sensor reference — routes to AIN1 |
| 7 | 3.3V | Logic supply |
| 8 | SDA / SCL | I2C bus |

### J2 — Conn_01x08_Pin

| Pin | Signal | Notes |
|---|---|---|
| 1 | GND | |
| 2 | IO17 | ESP32 GPIO (purpose TBD) |
| 3 | IO16 / Alert | ADS1115 ALERT/RDY → ESP32 interrupt pin |
| 4 | AIN0 | Moisture A after RC filter |
| 5 | AIN1 | Moisture B after RC filter |
| 6 | AIN2 | MoistureTemp |
| 7 | +5V | ADS1115 supply (pin 8) |
| 8 | PWR_FLAG | |

---

## Signal chain

```
OEM sensor output
    │
    ├── MoistureA ──── R29 (1kΩ) ──┬──── ADS1115 AIN0
    │                               ├── BAV99 D6 (clamp)
    │                               └── C51 (10nF C0G) → GND
    │
    ├── MoistureB ──── R30 (1kΩ) ──┬──── ADS1115 AIN1
    │                               ├── BAV99 D7 (clamp)
    │                               └── C50 (10nF C0G) → GND
    │
    └── MoistureTemp ── R31 (1kΩ) ──┬──── ADS1115 AIN2
                                     ├── BAV99 D8 (clamp)
                                     └── C49 (10nF C0G) → GND

ADS1115 (U16) ──── I2C (5V side) ──── PCA9306 (IC3) ──── I2C (3.3V side) ──── ESP32
                    ALERT/RDY ──────────────────────────────────────────────── ESP32 IO16
```

ADS1115 read mode: **differential AIN0 − AIN1** (moisture). AIN2 single-ended (temperature).

---

## Input protection — BAV99 dual diode clamps

**D6, D7, D8** — BAV99 SOT-23, one per channel.

Each BAV99 is wired as a dual-rail clamp at the ADS1115 input node (after series R):

- Upper diode: anode = signal, cathode = +5V → clamps high overvoltage to +5V + Vf (~5.6V)
- Lower diode: anode = GND, cathode = signal → clamps negative transients to −Vf (~−0.3V)

Both rails are protected. The series resistor (R29/R30/R31, 1kΩ) limits current through the BAV99 during a clamp event.

**ADS1115 absolute maximum on AIN:** VDD + 0.3V = **5.3V**. The BAV99 high clamp (~5.6V) marginally exceeds this — the series 1kΩ resistor is essential to limit current if a fast transient briefly exceeds the clamp voltage before it activates.

---

## RC anti-alias filter

Added in current schematic revision. One RC per channel, placed after the series resistor at the ADS1115 AIN pin:

| Component | Value | Purpose |
|---|---|---|
| R29, R30, R31 | 1kΩ | Series resistor — current limiting + RC filter |
| C49, C50, C51 | 10nF C0G | Filter cap to GND — charge reservoir + anti-alias |

**Corner frequency:** 1 / (2π × 1kΩ × 10nF) = **15.9 kHz**

The filter serves two purposes:

1. **Charge reservoir for ADS1115 switched-capacitor input.** The ADS1115 internal MUX switches a small capacitor (C_in) onto the AIN pin at the start of each conversion, drawing a brief charge pulse. Without a local cap, this glitch disturbs the input voltage and the ADC reads low until it settles. The 10nF cap supplies the charge instantly, allowing accurate conversion without additional settling delay.

2. **Anti-alias / noise filter.** Harness-coupled noise (switching regulators, ignition, CAN edges) above ~16 kHz is attenuated before reaching the ADC. The moisture signal is DC — passes through unaffected.

**Matching:** C49/C50 (the AIN0/AIN1 differential pair) must be matched. Mismatched capacitance unbalances the differential input impedance and converts common-mode noise into a differential error.

### Capacitor selection — C0G mandatory for differential pair

**Selected part:** Murata GRM1885C1H103JA01D — LCSC C85973

| Attribute | Value |
|---|---|
| Value | 10nF |
| Tolerance | ±5% |
| Voltage | 50V |
| Dielectric | **C0G (NP0)** |
| Package | 0603 |

C0G dielectric is stable across temperature (±30ppm/°C) and has no voltage coefficient drift. X7R or Y5V types drift with temperature and DC bias — this would unbalance C49/C50 in a combine cab environment and degrade differential noise rejection. Use C0G for all three channels. Source from same reel for best matching.

---

## ADS1115 configuration

**U16** — ADS1115IDGS (MSOP-10), powered at **+5V** via J2 pin 7.

| Setting | Configuration |
|---|---|
| Supply | +5V |
| ADDR pin | Pulled to GND → address **0x48** |
| Read mode | Differential AIN0 − AIN1 (moisture), AIN2 single-ended (temp) |
| ALERT/RDY | Routed to J2 pin 3 (ESP32 IO16) — conversion-ready interrupt |
| PGA | Set in firmware — full-scale ±4.096V at 5V supply |
| Data rate | Recommended 16–32 SPS for noise rejection |

**C48** (100nF) — VDD decoupling, placed close to U16 supply pin.

### ALERT/RDY — conversion-ready interrupt

Configure ALERT/RDY in **conversion-ready mode** (disable comparator). ESP32 IO16 receives a pulse on each completed conversion. Firmware starts a conversion, releases the I2C bus, and resumes reading only on the interrupt — no polling required. At 32 SPS this yields 32 samples per yield calculation window with zero CPU overhead between samples.

---

## I2C level shifting — PCA9306

**IC3** — PCA9306DCUR (SSOP-8).

Bridges ESP32 3.3V I2C to 5V ADS1115 I2C. Bidirectional, open-drain compatible.

- Low-voltage side (VREF1 = 3.3V): SCL1, SDA1 → ESP32
- High-voltage side (VREF2 = 5V): SCL2, SDA2 → ADS1115

Pull-ups:
- **R24, R25** (341Ω to +5V) — 5V side I2C pull-ups
- **R27, R28** (217Ω to +3.3V) — 3.3V side I2C pull-ups (SCL, SDA to ESP32)

ADS1115 ADDR pin is pulled to GND → address 0x48. R26 is the ADDR pull-down.

---

## Open items

1. **IO16, IO17, IO18, IO19 on J1/J2** — IO16 confirmed as ALERT/RDY interrupt. Purpose of IO17, IO18, IO19 not yet assigned — reserved for future use or Variant B PWM_EXCITE.

2. **Variant B** — custom coplanar electrode path not implemented. Would require adding 74HC4053D synchronous demodulator, AD8604ARUZ op-amp stage, LM4040 reference, and C0G trim cap on a future daughter board revision.

3. **Firmware** — ADS1115 driver needs:
   - Differential AIN0−AIN1 read for moisture
   - AIN2 single-ended read for temperature
   - Conversion-ready interrupt on IO16 (not polling)
   - Crop-specific calibration polynomial (moisture % from ADC counts)
   - Temperature compensation: `correctedMoisture = rawMoisture + k × (sensorTemp − 15.5°C)`

4. **Footprint check** — confirm C49/C50/C51 footprint in KiCad matches 0603 before switching from C5137477 to Murata C85973.

5. **OEM sensor voltage range** — confirm Case IH 2388 output range fits within ADS1115 differential input range (±4.096V with PGA=1 at 5V supply). Output is conditioned DC — expected to be within 0–5V range.
