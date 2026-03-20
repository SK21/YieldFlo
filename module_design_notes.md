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

**How to apply:** Reference these notes when designing firmware or expanding clsYieldCalculator to support RPM normalization.
