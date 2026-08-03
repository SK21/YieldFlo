# Diagnostic Log Guide

How to read the `Diag_*.csv` files the app writes, and what each column tells you
when a reading looks wrong.

---

## First look

Work down this until something answers. Most problems stop here; the rest of the
document explains the steps in full.

Anything mentioning `MedianCycleMs` or the gate applies to **Method3 logs only** —
Method2 has no period gate and no such column. Check the file's header row if you
are not sure which you have.

```
Noise > 0 sustained?
    Yes  -> Electrical: wiring, shield, or shared supply. Stop here —
            every number below is built on those edges.

Rpm steady?
    No   -> Elevator speed changed. PaddleHz and MedianCycleMs move with it;
            that is not a fault.

PaddleHz steady and matching the elevator?
    No   -> Roughly halved, MinCycleMs roughly doubled -> gate over-tight,
            rejecting real paddles.
         -> Erratic in any other pattern -> mechanical or sensor.

MedianCycleMs = 0?                              [Method3 logs only]
    Yes  -> Gate not armed, passing everything. Normal for a few seconds
            after each restart. Never leaving 0 means the signal is
            dropping out — back to the top.

MedianCycleMs below 1000 / PaddleHz and falling as flow rises?
    Yes  -> Gate widening under heavy grain.     [Method3 logs only]

MinCycleMs well below 75% of MedianCycleMs?
    Yes  -> A false cycle got through; check the threshold setting.

Map reads low at the ends of every pass?
         -> Not a sensor fault and not calibration. Timing. Go to
            "Measuring transport delay and clean-out".

All clean and the yield is still wrong?
         -> Sensor1, SpeedKmh, and calibration. Go to step 4.
```

---

## Where the files are

```
Documents\YieldFlo\DiagLogs\Diag_yyyyMMdd_HHmmss.csv
```

One file per app session, named for the time the app started. Logging is always
on — it does not wait for a job to be recording, so bench work and faults that
happen between jobs are captured too. About 1.5 MB per hour. The newest 20 files
are kept and older ones are deleted automatically.

This is separate from `yield_data` in the database, which stores one row per
second and only while a job is running.

---

## The columns

**Two builds are in the field and their logs are not the same shape. Read the
header row of the file before anything else.**

Method3 — the period-gate build:

```
PCTime,Sensor1,Noise,Rpm,Moisture,PaddleHz,MinCycleMs,GateRejects,MedianCycleMs,SpeedKmh,YieldRate,JobId,Sections,PipelineCount,Tail
```

Method2 — the paddle-channel build:

```
PCTime,Sensor1,Noise,Rpm,Moisture,PaddleHz,MinCycleMs,FlowRate,PaddlesPerS,FlowRejects,SpeedKmh,YieldRate,JobId,Sections,PipelineCount,Tail
```

Columns 1–7 and the last four are the same on both. Between them they differ,
and Method2 has one extra column — so **from column 8 onward, position 8 means a
different thing on each build and everything after column 9 is shifted by one.**
Never parse these files by index without checking the header.

Common to both builds:

| Column | From | Meaning |
|---|---|---|
| `PCTime` | PC clock | When the packet was parsed, `HH:mm:ss.fff` |
| `Sensor1` | 5 Hz packet | Obstruction ratio, 0–1. **Raw — not baseline-corrected** |
| `Noise` | 5 Hz packet | Glitch edges the module rejected in that 200 ms window |
| `Rpm` | 5 Hz packet | Elevator RPM. A flat **200** means no RPM sensor is fitted |
| `Moisture` | 5 Hz packet | Scaled moisture reading |
| `PaddleHz` | 1 Hz packet | Completed paddle cycles per second |
| `MinCycleMs` | 1 Hz packet | Shortest completed cycle that second. 255 = none completed |
| `SpeedKmh` | GPS | Ground speed at that moment |
| `YieldRate` | app | Instantaneous yield computed from this packet |
| `JobId` | app | Active job, or −1 when nothing is recording |
| `Sections` | app | 1 = crop entering the machine, 0 = header out |
| `PipelineCount` | app | Positions buffered waiting for their grain to reach the sensor |
| `Tail` | app | 1 while grain leaving the machine after a pass end is still being counted |

Method3 only:

| Column | From | Meaning |
|---|---|---|
| `GateRejects` | 1 Hz packet | Leading edges the period gate rejected that second |
| `MedianCycleMs` | 1 Hz packet | The gate's period estimate. **0 = gate not armed** |

Method2 only:

| Column | From | Meaning |
|---|---|---|
| `FlowRate` | 1 Hz packet | Paddle channel: per-paddle obstruction per second, uncorrected |
| `PaddlesPerS` | 1 Hz packet | Paddle rate over the same window |
| `FlowRejects` | 1 Hz packet | Cycles the module merged, scaled or reseeded that window |

**−1 in any 1 Hz column** means the module firmware predates that field. Update
the module firmware before drawing conclusions from a log full of −1s.

---

## Read this before you calculate anything

Rows arrive at **5 Hz**, but every column marked *1 Hz packet* in the tables
above comes from a slower frame. Each of their values is therefore repeated on
about five consecutive rows.

Any per-second figure from those columns has to de-duplicate first. **Summing
`GateRejects` (or `FlowRejects`) down the rows overcounts five-fold.** Take one
value per distinct 1 Hz update instead, and drop the −1 rows rather than treating
them as zero.

`Sensor1`, `Noise`, `Rpm`, `Moisture`, `Sections`, `PipelineCount` and `Tail` are
genuinely per-row and can be averaged or plotted directly.

---

## Reading order

Work down this list. Each step rules out a whole class of cause, so a problem
found at step 1 makes steps 2–4 meaningless.

### 1. Is the sensor healthy?

Look at **Noise**, **Rpm**, **PaddleHz**.

- `Noise` should be at or near zero. A steady nonzero count is an electrical
  problem — wiring, shielding, or a supply shared with something noisy. Fix that
  before touching anything else; every number downstream is built on those edges.
- `Rpm` tells you whether the elevator speed was steady. If it moved, expect
  `PaddleHz` and `MedianCycleMs` to move with it.
- `PaddleHz` should be steady at constant elevator speed. It is the single best
  indicator that real paddles are being counted.

### 2. Is the gate doing its job? — **Method3 only**

Skip this whole step on a Method2 log: there is no period gate on that build and
the columns it refers to do not exist in the file.

Look at **PaddleHz**, **MedianCycleMs**, **MinCycleMs**, **GateRejects**
together. These four completely describe why each cycle was accepted or
rejected.

Work out the nominal period first:

```
nominal period (ms) = 1000 / PaddleHz
```

`MedianCycleMs` should sit close to that, and the gate's threshold is 75% of
`MedianCycleMs`.

| PaddleHz | MedianCycleMs | MinCycleMs | GateRejects | Reading |
|---|---|---|---|---|
| steady 18 | 56 | 55 | 0 | Clean signal, nothing to reject |
| steady 18 | 56 | 54 | 2–5 | **Normal in heavy grain.** Gate working |
| steady 18 | 56 | 25 | 0 | Short cycle got through with the gate armed — should not happen; check the threshold setting |
| steady 18 | **0** | 25 | 0 | **Gate not armed** — passing everything. See below |
| **9** | 56 | **110** | high | **Gate rejecting real paddles.** Two paddles merging into one cycle |

Three things worth committing to memory:

- **A high reject count is not a fault.** Rejections are the gate catching grain
  that would otherwise have split a cycle. Rejects rising while `MinCycleMs`
  stays near the nominal period is exactly what success looks like.
- **The symptom of an over-tight gate is `PaddleHz`, not `GateRejects`.** When a
  real paddle is rejected, the cycle stays open and merges with the next one — so
  `PaddleHz` roughly halves and `MinCycleMs` roughly doubles. If `PaddleHz` holds
  steady and matches the elevator, the gate is not over-tight no matter how high
  the reject count goes.
- **`MedianCycleMs` = 0 means the gate is switched off.** It needs about a dozen
  paddles of history before it arms, and it clears that history whenever the
  sensor signal drops out. A stretch of zeros after every stop and restart is
  normal; a log that never leaves zero means the signal is dropping out
  repeatedly — go back to step 1.

There is one situation the gate cannot handle, and `MedianCycleMs` is how you
spot it. The estimate is a median over *all* leading-edge intervals, including
spurious ones — each one splits a real interval in two, and where it lands
decides whether one half or both come out short. The estimate therefore holds
while real paddles keep the majority, roughly until **one leading edge in three**
is spurious. Past that the estimate falls, the threshold falls with it, and the
gate widens. It does so **gradually** — bench runs well beyond that point still
rejected kernels and still tracked the true mean — so do not expect a clean
failure point. The signature is `MedianCycleMs` drifting **below**
`1000 / PaddleHz` as flow increases. Comparing those two numbers across a heavy
pass is the check.

### 3. Look at Sensor1

Only once steps 1 and 2 are clean.

Does it move smoothly, jump, or drift? If `Sensor1` jumps while `PaddleHz`,
`MinCycleMs` and `MedianCycleMs` all stay steady, the sensor is measuring
correctly and you are looking at **real grain flow variation**.

Remember `Sensor1` is logged raw. The zero-flow value in this column is the
baseline, which the app subtracts downstream — a `Sensor1` that never reaches
zero with an empty elevator is normal and is what the baseline is for.

### 4. Correlate with YieldRate

`YieldRate` is computed from `Sensor1`, `SpeedKmh`, the header width and the
calibration. So compare all three columns, not two:

- **Both `Sensor1` and `YieldRate` jump** → the disturbance started at the
  sensor. Go back to step 1.
- **`YieldRate` jumps, `Sensor1` flat, `SpeedKmh` jumps** → a GPS speed glitch,
  not a grain measurement problem.
- **`YieldRate` jumps, `Sensor1` and `SpeedKmh` both flat** → the cause is after
  the sensor: calibration, baseline, or header width.

One case this step will not explain: readings that are low **only at the start
and end of each pass**, with everything healthy in between. That is a timing
problem, not a calibration one, and changing the calibration to chase it will
throw out the middle of the pass as well. Use the next section instead.

---

## Measuring transport delay and clean-out

Grain does not reach the sensor where it was cut, and it does not stop arriving
when the header comes up. Both delays are measured from `Sections` — its edges
are the only marks in the file that say what the header was doing.

Take one pass with `Sections` going 1 and back to 0, and plot `Sensor1`,
`Sections`, `PipelineCount` and `Tail` on a shared time axis.

**Transport delay — from the RISING edge.** Measure from `Sections` going 1 to
`Sensor1` beginning to lift. That is the true delay for this machine and crop.
Compare it against the delay in the crop calibration.

Do this first. It has never been measured on a real machine — the value in the
setting is only a default — and a delay set too short paints the start of every
pass low in a way that looks exactly like the machine filling. Fixing a wrong
setting costs nothing; everything else here is harder.

**Clean-out — from the FALLING edge.** Measure from `Sections` going 0 to
`Sensor1` settling back to its no-flow baseline. Expect this to be far longer
than the transport delay. Measured in the field 2026-08-03: 30 % back to a 9 %
baseline in about **100 seconds**, against a delay setting of 10.

Two things to read off the same plot:

- **Where `PipelineCount` reaches 0 against where `Sensor1` is.** If the sensor
  is still well above baseline at that moment, the machine was still delivering
  grain after the app had finished placing it on the ground.
- **How `Tail` ends.** Ending as `Sensor1` reaches baseline is the machine
  emptying. Ending while it is still high means the next pass's grain became due
  first — normal, because clean-out outlasts a headland turn, so in continuous
  harvesting the machine never fully empties. `Tail` running the full 180 s
  timeout is a fault: flow never returned to baseline, which points at the sensor
  baseline having drifted rather than the machine being slow.

Do not size anything from the time to baseline alone. Most of the grain in the
tail arrives in the first 20–30 seconds and the rest is a long dribble carrying
very little — so the useful figure is where the **mass** is, not where the trace
finally goes flat.

---

## Figures worth pulling from every log

| Figure | Healthy | What a bad value means |
|---|---|---|
| Average `PaddleHz` | Near constant | Large swings = elevator speed changed, or paddles being missed |
| `Noise` per second | 0 | Electrical problem — investigate before changing any setting |
| `GateRejects` per second *(M3)* | 0 to ~5 | 40+ sustained means something is wrong upstream |
| Minimum `MinCycleMs` *(M3)* | Between 75% of `MedianCycleMs` and `MedianCycleMs` | Below 75% = a false cycle got through. Well above the median = paddles being merged |
| `MedianCycleMs` vs `1000 / PaddleHz` *(M3)* | Close, and stable | Median drifting low as flow rises = gate widening under heavy grain |
| `FlowRejects` per second *(M2)* | Low and steady | Rising = module losing track of paddle phase |
| Std deviation of `Sensor1` | As low as flow allows | This is how noisy the measurement actually is |
| `Sections` rising edge → `Sensor1` lifting | Matches the delay setting | A gap longer than the setting = delay set too short, pass starts read low |
| `Sections` falling edge → `Sensor1` at baseline | — | This is clean-out. Expect it far longer than the delay |

---

## Four plots that show almost everything

Plot these against `PCTime`, stacked so they share a time axis:

1. **`PaddleHz`** — should be nearly flat
2. **`MedianCycleMs` with `MinCycleMs`** *(Method3)* — on the same axes.
   `MinCycleMs` should stay above 75% of `MedianCycleMs`; that 25% band is the
   gate's margin, and the gap between the two traces is how much of it the
   shortest cycle used. On Method2 plot **`FlowRate` with `Sensor1`** instead —
   the two channels should track each other
3. **`Sensor1` with `Sections`, `PipelineCount` and `Tail`** — the main output,
   and the only plot that shows what the header was doing at the time. This is
   the one to draw first when the map looks wrong at the headlands
4. **`GateRejects`** *(Method3)* or **`FlowRejects`** *(Method2)* — grain activity

De-duplicate the 1 Hz columns first, or plots 1, 2 and 4 will show stair-steps
that are an artifact of the log rate rather than anything real. Plot 3 needs no
de-duplication — all four of those columns are genuinely per-row.

---

## Reporting a problem

Send the whole `Diag_*.csv` file rather than an extract, plus:

- what the machine was doing at the time (crop, rough time of day, stopped or
  moving, heavy or light flow)
- the module firmware version from the Settings screen
- which build the app is — the header row of the CSV tells you, but say it anyway
- what the display was showing that looked wrong

The time of day is what ties the file to the event — everything else can be read
out of the file itself.
