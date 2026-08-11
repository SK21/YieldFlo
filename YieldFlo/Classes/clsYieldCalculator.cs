using System;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Converts raw optical sensor readings into instantaneous yield (bu/ac).
    /// The transport delay between the header and the clean-grain elevator
    /// sensor is handled by the position pipeline in clsDataCollector, which
    /// pairs the current sensor flow with the position harvested
    /// ProcessingDelaySec earlier.
    ///
    /// Two flow channels are maintained from the module's single sensor:
    ///
    ///   Duty channel   — blocked time / total time. A fill fraction: it is
    ///                    speed-invariant, so it reads the same when the
    ///                    elevator changes speed carrying the same fill.
    ///   Paddle channel — per-paddle obstruction summed over paddles, divided
    ///                    by the window. Scales with elevator speed, so it
    ///                    tracks volume flow rather than fill.
    ///
    /// The paddle channel is the better yield input and is used whenever the
    /// module reports it; the duty channel is kept live alongside it as an
    /// independent cross-check (ChannelsDisagree) and as the fallback for ESP32
    /// modules and older firmware, which send no paddle frame.
    ///
    /// Both are normalised to the same scale — at the nominal paddle rate the
    /// paddle index equals the corrected duty ratio — so a YieldFactor
    /// calibrated on the duty channel carries over to the paddle channel
    /// unchanged, and SensorBaseline means the same thing to both.
    /// </summary>
    public class clsYieldCalculator
    {
        // Calibration values — set from the active Calibration record
        public double SensorBaseline { get; set; } = 0.0;      // paddle-only obstruction ratio
        public double YieldFactor { get; set; } = 1.0;         // crop calibration multiplier
        public int ProcessingDelaySec { get; set; } = 10;

        /// <summary>
        /// Paddle rate the elevator runs at with the separator engaged, Hz.
        /// Normalises the paddle channel onto the duty channel's 0–1 scale so
        /// one YieldFactor serves both. Captured by Set Baseline (the elevator
        /// is at working speed then by definition). 0 = not captured yet: the
        /// live rate is used instead, which makes the paddle index collapse to
        /// the corrected duty ratio — safe, but it gives up the speed tracking
        /// that is the whole point of the channel.
        /// </summary>
        public double RefPaddleHz { get; set; } = 0.0;

        /// <summary>Use the paddle channel when the module provides it. False forces the duty channel.</summary>
        public bool PreferPaddleChannel { get; set; } = true;

        // Crop / header
        public double TestWeightLbsBu { get; set; } = 60.0;    // lbs per bushel (wheat default)
        public double HeaderWidthM { get; set; } = 9.144;      // metres (30 ft default)
        public double HeaderFwdOffsetM { get; set; } = 0;      // metres the header sits AHEAD of the GPS antenna

        // Latest calculated values (read by DataCollector / UI)
        public double InstantYield { get; private set; }        // bu/ac
        public double SmoothedYield { get; private set; }       // simple rolling average
        public double InstantWorkRate { get; private set; }     // bu/hr — grain throughput
        public double SmoothedWorkRate { get; private set; }    // bu/hr, rolling average
        public bool IsFlowing { get; private set; }
        public double CurrentRatio { get; private set; }        // duty channel, baseline-corrected
        public double CurrentPaddleRatio { get; private set; }  // paddle channel, baseline-corrected and normalised
        public double CurrentFlowIndex { get; private set; }    // whichever channel Calculate() used
        public bool PaddleReadingValid { get; private set; }    // module sent a usable paddle frame
        public bool UsingPaddleChannel { get; private set; }    // paddle channel fed the last Calculate()

        // Below this the elevator is considered empty. Shared with the collector's
        // tail drain so "still flowing" means the same thing in both places.
        public const double FlowStopRatio = 0.01;

        /// <summary>
        /// The channel Calculate() would use right now, resolved independently of it.
        /// The tail drain runs while the machine is emptying on a headland, where
        /// Calculate() has bailed out on the speed gate and CurrentFlowIndex is stale —
        /// so it cannot be read from there.
        /// </summary>
        public double ActiveFlowRatio =>
            (PreferPaddleChannel && PaddleReadingValid && Core.PaddleChannelLive)
                ? CurrentPaddleRatio : CurrentRatio;

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
            // ActiveFlowRatio * YieldFactor is the calibrated flow in lbs/s
            return ActiveFlowRatio * YieldFactor / TestWeightLbsBu;
        }

        /// <summary>
        /// Signed difference paddle − duty, in flow-index units. Zero while the
        /// elevator runs at the rate the baseline was captured at, and grows in
        /// proportion to how far it has drifted from it — that drift is real
        /// information the duty channel structurally cannot carry, which is why
        /// the paddle channel is the one that feeds the map.
        /// </summary>
        public double ChannelDelta { get; private set; }

        /// <summary>
        /// Either the elevator is running far enough off its calibration speed
        /// that the duty channel would be materially wrong (roughly ±50%), or
        /// the module reported it lost the paddle phase for the window — edges
        /// being missed, the beam parked, chatter on the sensor wire. The yield
        /// figure should be treated as suspect rather than mapped silently.
        ///
        /// Note this is not a per-paddle mechanical fault detector: a bent or
        /// missing paddle changes neither channel's totals much, because the
        /// grain it should have carried still goes past the beam. That shows up
        /// as the module's cycle repair rate instead (Core.LastFlowRejects,
        /// displayed as "R:n/s" on the calibration screen).
        /// </summary>
        public bool ChannelsDisagree { get; private set; }

        // Tolerance band: a flat floor for low flow, widening with flow so a
        // constant percentage error does not trip it at high rates. Sized so a
        // ±25% elevator speed swing — normal engine load variation — stays
        // quiet and only a genuine mismatch trips it.
        private const double DisagreeFloor = 0.05;
        private const double DisagreeSlope = 0.20;

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

        private double _smoothAccum = 0;
        private double _workAccum = 0;
        private int _smoothCount = 0;
        private const int SmoothWindow = 5;

        /// <summary>
        /// Duty channel. Called each time a module packet arrives (5 Hz).
        /// Stores the latest baseline-corrected reading.
        /// </summary>
        public void PushSensorReading(double sensor1Raw)
        {
            CurrentRatio = Math.Max(0.0, Math.Min(1.0, sensor1Raw - SensorBaseline));
        }

        /// <summary>
        /// Paddle channel. Called each time a paddle-event frame arrives (5 Hz).
        /// flowRate is per-paddle obstruction per second, uncorrected;
        /// paddlesPerS is the paddle rate over the same window.
        /// </summary>
        public void PushPaddleReading(double flowRate, double paddlesPerS, bool valid)
        {
            PaddleReadingValid = valid && paddlesPerS > 0;
            if (!PaddleReadingValid)
            {
                CurrentPaddleRatio = 0;
                return;
            }

            CurrentPaddleRatio = PaddleIndex(flowRate, paddlesPerS, SensorBaseline, RefPaddleHz);
        }

        /// <summary>
        /// Paddle channel flow index. The baseline is subtracted once per
        /// paddle rather than once per window — that is the substantive
        /// difference from the duty channel, because the empty-paddle
        /// obstruction is a per-paddle constant, and yield is an integral, so
        /// getting it wrong biases the season total rather than adding noise.
        /// Dividing by the nominal rate puts the result on the duty channel's
        /// 0–1 scale; it is allowed to exceed 1 when the elevator runs faster
        /// than nominal, which is exactly the information the duty channel
        /// cannot carry.
        /// </summary>
        private static double PaddleIndex(double flowRate, double paddlesPerS,
                                          double baseline, double refPaddleHz)
        {
            double grainRate = flowRate - paddlesPerS * baseline;
            if (grainRate <= 0) return 0;

            double refHz = refPaddleHz > 0 ? refPaddleHz : paddlesPerS;
            if (refHz <= 0) return 0;

            return Math.Min(2.0, grainRate / refHz);
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
        ///
        /// Channel-agnostic: the width only reaches areaRateM2s below, which sits
        /// under the duty/paddle selection — whichever channel supplied the ratio,
        /// it is divided by the same area.
        /// </summary>
        public double Calculate(double speedKmh, double effectiveWidthM)
        {
            // Channel selection and the cross-check run even when the machine is
            // stopped — a disagreement that only shows up while harvesting is
            // one the operator finds out about too late.
            UsingPaddleChannel = PreferPaddleChannel && PaddleReadingValid && Core.PaddleChannelLive;
            CurrentFlowIndex = UsingPaddleChannel ? CurrentPaddleRatio : CurrentRatio;

            if (PaddleReadingValid && Core.PaddleChannelLive)
            {
                ChannelDelta = CurrentPaddleRatio - CurrentRatio;
                double tolerance = DisagreeFloor + DisagreeSlope * Math.Max(CurrentRatio, CurrentPaddleRatio);
                ChannelsDisagree = Math.Abs(ChannelDelta) > tolerance || Core.LastFlowUnaccounted;
            }
            else
            {
                ChannelDelta = 0;
                ChannelsDisagree = false;
            }

            if (speedKmh < 0.5 || effectiveWidthM <= 0 || TestWeightLbsBu <= 0)
            {
                InstantYield = 0;
                InstantWorkRate = 0;
                IsFlowing = false;
                Smooth(0, 0);
                return 0;
            }

            double ratio = CurrentFlowIndex;

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
        /// Rolling-average smoothing. Zeros must be pushed through here when
        /// flow stops so SmoothedYield decays to 0 instead of freezing at the
        /// last flowing value.
        /// </summary>
        private void Smooth(double instantYield, double instantWork)
        {
            _smoothAccum += instantYield;
            _workAccum   += instantWork;
            _smoothCount++;
            if (_smoothCount >= SmoothWindow)
            {
                SmoothedYield    = Math.Round(_smoothAccum / _smoothCount, 1);
                SmoothedWorkRate = Math.Round(_workAccum   / _smoothCount, 1);
                _smoothAccum = 0;
                _workAccum   = 0;
                _smoothCount = 0;
            }
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
            _workAccum = 0;
            _smoothCount = 0;
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
            return ComputeYieldRate(sensor1Raw, -1, -1, speedKmh, baseline, 0,
                                    yieldFactor, headerWidthM, testWeightLbsBu);
        }

        /// <summary>
        /// Dual-channel form. flowRate/paddlesPerS come from the stored paddle
        /// frame; pass -1 for either when the point was logged by a module that
        /// sent no paddle frame, and the duty channel is used instead. Points
        /// recorded before and after a firmware upgrade therefore recalculate
        /// correctly inside the same job.
        /// </summary>
        public static double ComputeYieldRate(double sensor1Raw, double flowRate, double paddlesPerS,
            double speedKmh, double baseline, double refPaddleHz,
            double yieldFactor, double headerWidthM, double testWeightLbsBu)
        {
            if (speedKmh < 0.5 || headerWidthM <= 0 || testWeightLbsBu <= 0) return 0;

            double ratio = (flowRate >= 0 && paddlesPerS > 0)
                ? PaddleIndex(flowRate, paddlesPerS, baseline, refPaddleHz)
                : Math.Max(0.0, Math.Min(1.0, sensor1Raw - baseline));
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
