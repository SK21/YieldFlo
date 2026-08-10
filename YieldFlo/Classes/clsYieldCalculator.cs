using System;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Converts raw optical sensor readings into instantaneous yield (bu/ac).
    /// The transport delay between the header and the clean-grain elevator
    /// sensor is handled by the position pipeline in clsDataCollector, which
    /// pairs the current sensor flow with the position harvested
    /// ProcessingDelaySec earlier.
    /// </summary>
    public class clsYieldCalculator
    {
        // Calibration values — set from the active Calibration record
        public double SensorBaseline { get; set; } = 0.0;      // paddle-only obstruction ratio
        public double YieldFactor { get; set; } = 1.0;         // crop calibration multiplier
        public int ProcessingDelaySec { get; set; } = 10;

        // Crop / header
        public double TestWeightLbsBu { get; set; } = 60.0;    // lbs per bushel (wheat default)
        public double HeaderWidthM { get; set; } = 9.144;      // metres (30 ft default)
        public double HeaderFwdOffsetM { get; set; } = 0;      // metres the header sits AHEAD of the GPS antenna

        // Latest calculated values (read by DataCollector / UI)
        public double InstantYield { get; private set; }        // bu/ac
        public double SmoothedYield { get; private set; }       // exponentially smoothed, display only
        public double InstantWorkRate { get; private set; }     // bu/hr — grain throughput
        public double SmoothedWorkRate { get; private set; }    // bu/hr, exponentially smoothed
        public bool IsFlowing { get; private set; }
        public double CurrentRatio { get; private set; }        // latest baseline-corrected reading

        // Below this the elevator is considered empty. Shared with the collector's
        // tail drain so "still flowing" means the same thing in both places.
        public const double FlowStopRatio = 0.01;

        private const double M2_PER_ACRE = 4046.856;
        private const double KG_PER_BUSHEL_WHEAT = 27.215;     // approx — overridden by TestWeight
        private const double LBS_PER_KG = 2.20462;
        // 1 lb/ac = 1/(27.215*2.20462) bu/ac  →  use TestWeightLbsBu directly

        // Calibration run accumulators
        public bool IsCalRunActive { get; private set; }
        public double CalRunBushels { get; private set; }

        public void StartCalRun()
        {
            CalRunBushels = 0;
            IsCalRunActive = true;
        }

        public void StopCalRun()
        {
            IsCalRunActive = false;
        }

        /// <summary>Called by DataCollector each GPS tick to accumulate cal-run bushels.</summary>
        public void AccumulateCalRun(double bushelsInc)
        {
            if (IsCalRunActive)
                CalRunBushels += bushelsInc;
        }

        /// <summary>
        /// Computes a corrected YieldFactor from the actual weighed mass.
        /// actualBushels must be in internal bushels (already converted from display unit).
        /// Pure calculation — does not change YieldFactor; the caller decides
        /// whether/when to apply the result (Save & Apply).
        /// </summary>
        public double ComputeNewFactor(double actualBushels)
        {
            if (CalRunBushels <= 0) return YieldFactor;
            return YieldFactor * (actualBushels / CalRunBushels);
        }

        // Display damping. 0.2 is the loosest coefficient that beats the block
        // average it replaced on both spread and step: measured on the bench at
        // 5 bridging kernels/s, sd 3.6%→3.2% and the jump between shown values
        // 1.19→0.64 bu/ac. At 0.3 the readout chases excursions and sd gets
        // worse than the block average. Drop to 0.1 if the field turns out as
        // noisy as that bench case — it costs ~1 s more lag for sd 1.9%.
        private const double SmoothAlpha = 0.1;
        private bool _smoothSeeded = false;
        // Full-precision EMA state. The exposed properties are rounded for
        // display, but the state must not be: feeding a rounded value back in
        // latches it. At 0.1 bu/ac with zeros arriving, 0.1*0.8 = 0.08 rounds
        // straight back to 0.1 and the readout never reaches zero.
        private double _emaYield = 0;
        private double _emaWork = 0;

        /// <summary>
        /// Called each time a sensor packet arrives (~10 Hz).
        /// Stores the latest baseline-corrected reading.
        /// </summary>
        public void PushSensorReading(double sensor1Raw)
        {
            CurrentRatio = Math.Max(0.0, Math.Min(1.0, sensor1Raw - SensorBaseline));
        }

        /// <summary>
        /// Grain mass rate in bushels/second from the sensor reading alone.
        ///
        /// Deliberately independent of ground speed: this is what the elevator is
        /// delivering, not what the ground is yielding. Calculate() cannot be used
        /// for this — it returns 0 below 0.5 km/h, which is exactly the situation
        /// it is needed in, a combine crawling round a headland while the machine
        /// finishes emptying. With no new ground being cut there is no bu/ac to
        /// compute, only mass.
        /// </summary>
        public double CurrentBushelsPerSec()
        {
            if (TestWeightLbsBu <= 0) return 0;
            // CurrentRatio * YieldFactor is the calibrated flow in lbs/s
            return CurrentRatio * YieldFactor / TestWeightLbsBu;
        }

        /// <summary>
        /// Pairs the current sensor flow with a buffered position point.
        /// speedKmh is the ground speed recorded at that position.
        /// Returns the yield value for that position.
        /// </summary>
        public double Calculate(double speedKmh)
        {
            return Calculate(speedKmh, HeaderWidthM);
        }

        /// <summary>
        /// As Calculate(speed), but divides by the width actually cutting new crop
        /// rather than the full header.
        ///
        /// On a half-overlapped pass only half the header meets standing crop, so
        /// the flow arriving is half — dividing that by the full width returns half
        /// the true yield and paints a cold streak on ground that yielded normally.
        /// Dividing by the width that did the cutting returns the field's actual
        /// yield. Mass is unaffected either way: the caller's acres carry the same
        /// factor, so effective width cancels out of bushels entirely.
        /// </summary>
        public double Calculate(double speedKmh, double effectiveWidthM)
        {
            if (speedKmh < 0.5 || effectiveWidthM <= 0 || TestWeightLbsBu <= 0)
            {
                InstantYield = 0;
                InstantWorkRate = 0;
                IsFlowing = false;
                Smooth(0, 0);
                return 0;
            }

            double ratio = CurrentRatio;

            IsFlowing = ratio > FlowStopRatio;

            if (!IsFlowing)
            {
                InstantYield = 0;
                InstantWorkRate = 0;
                Smooth(0, 0);
                return 0;
            }

            // Area rate: m²/s
            double speedMs = speedKmh / 3.6;
            double areaRateM2s = speedMs * effectiveWidthM;

            // Grain flow index (arbitrary volume/s) — calibrated via YieldFactor
            double grainFlowIndex = ratio * YieldFactor;

            // Yield in lbs/m²·s / (area m²/s) → lbs/m²
            // Then convert lbs/m² → bu/ac
            // bu/ac = (lbs/m²) * M2_PER_ACRE / TestWeightLbsBu
            double yieldLbsPerM2 = grainFlowIndex / areaRateM2s;
            double yieldBuAc = yieldLbsPerM2 * M2_PER_ACRE / TestWeightLbsBu;

            InstantYield = Math.Round(yieldBuAc, 1);

            // Throughput (bu/hr). grainFlowIndex is the calibrated grain flow in
            // lbs/s, so bu/hr = flow * 3600 / testweight — independent of ground
            // speed and consistent with the accumulated bushel total.
            InstantWorkRate = Math.Round(grainFlowIndex * 3600.0 / TestWeightLbsBu, 1);

            Smooth(InstantYield, InstantWorkRate);

            return InstantYield;
        }

        /// <summary>
        /// Exponential smoothing for the displayed figures: new = previous*(1-a)
        /// + current*a. Zeros must be pushed through here when flow stops so
        /// SmoothedYield decays to 0 instead of freezing at the last flowing
        /// value — the decay is now a ~6 s fade rather than a step, which is no
        /// worse than honest given grain keeps arriving for the transport delay.
        ///
        /// This replaced a block average that summed five samples, emitted, and
        /// reset. Consecutive shown values there shared no data, so the readout
        /// sat still and then snapped to an independent estimate — that stepping
        /// was most of what read as a jumpy display. Updating every sample makes
        /// the number drift instead.
        ///
        /// Display only. Totals, the map and yield_data all record InstantYield,
        /// so damping here cannot affect a measurement.
        /// </summary>
        private void Smooth(double instantYield, double instantWork)
        {
            // Seed on the first sample after a reset. Starting from zero would
            // make the readout crawl up to the true value over a couple of
            // seconds every time a job starts, which looks like a fault.
            if (!_smoothSeeded)
            {
                _emaYield = instantYield;
                _emaWork = instantWork;
                _smoothSeeded = true;
            }
            else
            {
                _emaYield = _emaYield * (1 - SmoothAlpha) + instantYield * SmoothAlpha;
                _emaWork = _emaWork * (1 - SmoothAlpha) + instantWork * SmoothAlpha;
            }

            SmoothedYield = Math.Round(_emaYield, 1);
            SmoothedWorkRate = Math.Round(_emaWork, 1);
        }

        /// <summary>
        /// Calculate incremental acres from a distance travelled.
        /// distanceM: metres travelled since last call.
        /// </summary>
        public static double MetresToAcres(double distanceM, double headerWidthM)
        {
            return (distanceM * headerWidthM) / M2_PER_ACRE;
        }

        public void ResetSmoothing()
        {
            _emaYield = 0;
            _emaWork = 0;
            _smoothSeeded = false;
            SmoothedYield = 0;
            SmoothedWorkRate = 0;
        }

        /// <summary>
        /// Recomputes a single yield value from a stored raw sensor reading, using
        /// the same math as Calculate() but with no instance state (no smoothing,
        /// no side effects). Used to re-derive already-logged YieldDataPoints under
        /// a new calibration (see YieldDataRepo.RecalculateJob) without disturbing
        /// the live calculator mid-job.
        /// </summary>
        public static double ComputeYieldRate(double sensor1Raw, double speedKmh, double baseline,
            double yieldFactor, double headerWidthM, double testWeightLbsBu)
        {
            if (speedKmh < 0.5 || headerWidthM <= 0 || testWeightLbsBu <= 0) return 0;

            double ratio = Math.Max(0.0, Math.Min(1.0, sensor1Raw - baseline));
            if (ratio <= 0.01) return 0;

            double speedMs = speedKmh / 3.6;
            double areaRateM2s = speedMs * headerWidthM;
            double grainFlowIndex = ratio * yieldFactor;
            double yieldLbsPerM2 = grainFlowIndex / areaRateM2s;
            double yieldBuAc = yieldLbsPerM2 * M2_PER_ACRE / testWeightLbsBu;

            return Math.Round(yieldBuAc, 1);
        }
    }
}
