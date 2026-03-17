using System;
using System.Collections.Generic;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Converts raw optical sensor readings into instantaneous yield (bu/ac).
    /// Maintains a ring buffer to apply the processing delay between the header
    /// and the clean-grain elevator sensors.
    /// </summary>
    public class clsYieldCalculator
    {
        // Ring buffer: pairs (timestamp, averaged obstruction ratio)
        private readonly Queue<(DateTime time, double ratio)> _delayBuffer
            = new Queue<(DateTime, double)>();

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

        private const double M2_PER_ACRE = 4046.856;
        private const double KG_PER_BUSHEL_WHEAT = 27.215;     // approx — overridden by TestWeight
        private const double LBS_PER_KG = 2.20462;
        // 1 lb/ac = 1/(27.215*2.20462) bu/ac  →  use TestWeightLbsBu directly

        private double _smoothAccum = 0;
        private int _smoothCount = 0;
        private const int SmoothWindow = 5;

        /// <summary>
        /// Called each time a sensor packet arrives (~10 Hz).
        /// Pushes the raw reading into the delay buffer.
        /// </summary>
        public void PushSensorReading(double sensor1Raw, double sensor2Raw)
        {
            // Average the two sensors, subtract baseline, clamp to [0,1]
            double avg = (sensor1Raw + sensor2Raw) / 2.0;
            double ratio = Math.Max(0.0, Math.Min(1.0, avg - SensorBaseline));
            _delayBuffer.Enqueue((DateTime.UtcNow, ratio));

            // Trim old entries beyond 2× the delay window
            while (_delayBuffer.Count > 0 &&
                   (DateTime.UtcNow - _delayBuffer.Peek().time).TotalSeconds > ProcessingDelaySec * 2)
            {
                _delayBuffer.Dequeue();
            }
        }

        /// <summary>
        /// Called each time a GPS position arrives.
        /// Returns the yield value for this position point.
        /// Speed in km/h.
        /// </summary>
        public double Calculate(double speedKmh)
        {
            if (speedKmh < 0.5 || HeaderWidthM <= 0 || TestWeightLbsBu <= 0)
            {
                InstantYield = 0;
                IsFlowing = false;
                return 0;
            }

            // Pull the delayed sensor reading
            double delayedRatio = GetDelayedRatio();

            IsFlowing = delayedRatio > 0.01;

            if (!IsFlowing)
            {
                InstantYield = 0;
                return 0;
            }

            // Area rate: m²/s
            double speedMs = speedKmh / 3.6;
            double areaRateM2s = speedMs * HeaderWidthM;

            // Grain flow index (arbitrary volume/s) — calibrated via YieldFactor
            double grainFlowIndex = delayedRatio * YieldFactor;

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

        private double GetDelayedRatio()
        {
            if (_delayBuffer.Count == 0) return 0;

            DateTime target = DateTime.UtcNow.AddSeconds(-ProcessingDelaySec);
            double best = 0;
            DateTime bestTime = DateTime.MinValue;

            foreach (var entry in _delayBuffer)
            {
                if (entry.time <= target && entry.time > bestTime)
                {
                    best = entry.ratio;
                    bestTime = entry.time;
                }
            }
            return best;
        }
    }
}
