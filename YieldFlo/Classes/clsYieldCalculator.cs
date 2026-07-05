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

        // Latest calculated values (read by DataCollector / UI)
        public double InstantYield { get; private set; }        // bu/ac
        public double SmoothedYield { get; private set; }       // simple rolling average
        public bool IsFlowing { get; private set; }
        public double CurrentRatio { get; private set; }        // latest baseline-corrected reading

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
        /// Sets YieldFactor in place and returns the new value.
        /// </summary>
        public double ComputeNewFactor(double actualBushels)
        {
            if (CalRunBushels <= 0) return YieldFactor;
            YieldFactor = YieldFactor * (actualBushels / CalRunBushels);
            return YieldFactor;
        }

        private double _smoothAccum = 0;
        private int _smoothCount = 0;
        private const int SmoothWindow = 5;

        /// <summary>
        /// Called each time a sensor packet arrives (~10 Hz).
        /// Stores the latest baseline-corrected reading.
        /// </summary>
        public void PushSensorReading(double sensor1Raw)
        {
            CurrentRatio = Math.Max(0.0, Math.Min(1.0, sensor1Raw - SensorBaseline));
        }

        /// <summary>
        /// Pairs the current sensor flow with a buffered position point.
        /// speedKmh is the ground speed recorded at that position.
        /// Returns the yield value for that position.
        /// </summary>
        public double Calculate(double speedKmh)
        {
            if (speedKmh < 0.5 || HeaderWidthM <= 0 || TestWeightLbsBu <= 0)
            {
                InstantYield = 0;
                IsFlowing = false;
                return 0;
            }

            double ratio = CurrentRatio;

            IsFlowing = ratio > 0.01;

            if (!IsFlowing)
            {
                InstantYield = 0;
                return 0;
            }

            // Area rate: m²/s
            double speedMs = speedKmh / 3.6;
            double areaRateM2s = speedMs * HeaderWidthM;

            // Grain flow index (arbitrary volume/s) — calibrated via YieldFactor
            double grainFlowIndex = ratio * YieldFactor;

            // Yield in lbs/m²·s / (area m²/s) → lbs/m²
            // Then convert lbs/m² → bu/ac
            // bu/ac = (lbs/m²) * M2_PER_ACRE / TestWeightLbsBu
            double yieldLbsPerM2 = grainFlowIndex / areaRateM2s;
            double yieldBuAc = yieldLbsPerM2 * M2_PER_ACRE / TestWeightLbsBu;

            InstantYield = Math.Round(yieldBuAc, 1);

            // Smoothing
            _smoothAccum += InstantYield;
            _smoothCount++;
            if (_smoothCount >= SmoothWindow)
            {
                SmoothedYield = Math.Round(_smoothAccum / _smoothCount, 1);
                _smoothAccum = 0;
                _smoothCount = 0;
            }

            return InstantYield;
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
            _smoothAccum = 0;
            _smoothCount = 0;
            SmoothedYield = 0;
        }
    }
}
