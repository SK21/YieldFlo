using System;
using YieldFlo.Database;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Accumulates yield data points during an active job.
    /// Writes one record per second to the database.
    /// </summary>
    public class clsDataCollector
    {
        public bool IsRecording { get; private set; }
        public int ActiveJobId { get; private set; } = -1;

        public double TotalAcres { get; private set; }
        public double TotalBushels { get; private set; }
        public double AverageYield => TotalAcres > 0.01 ? TotalBushels / TotalAcres : 0;
        public double AverageMoisture { get; private set; }

        private DateTime _lastWriteTime = DateTime.MinValue;
        private double _lastLat = 0, _lastLon = 0;
        private double _moistureSum = 0;
        private int _moistureCount = 0;

        public void StartJob(int jobId)
        {
            ActiveJobId = jobId;
            TotalAcres = 0;
            TotalBushels = 0;
            AverageMoisture = 0;
            _moistureSum = 0;
            _moistureCount = 0;
            _lastLat = 0;
            _lastLon = 0;
            _lastWriteTime = DateTime.MinValue;
            IsRecording = true;
        }

        public void StopJob()
        {
            IsRecording = false;
            if (ActiveJobId > 0 && Core.Database != null)
            {
                Core.Database.Jobs.UpdateTotals(ActiveJobId, TotalAcres, TotalBushels);
                Core.Database.Jobs.Close(ActiveJobId);
            }
            ActiveJobId = -1;
        }

        public void PauseJob() { IsRecording = false; }

        public void ResumeJob() { IsRecording = true; }

        /// <summary>
        /// Called every GPS update (~10 Hz). Writes to DB once per second.
        /// </summary>
        public void OnGpsUpdate(double moisture)
        {
            if (!IsRecording || ActiveJobId < 0) return;

            var gps = Core.GPS;
            if (!gps.IsConnected) return;

            var yield = Core.Yield;

            // Calculate yield for this GPS position
            yield.Calculate(gps.Speed);

            // Accumulate acres
            if (_lastLat != 0 && _lastLon != 0)
            {
                double distM = HaversineMetres(_lastLat, _lastLon, gps.Latitude, gps.Longitude);
                double acresInc = clsYieldCalculator.MetresToAcres(distM, yield.HeaderWidthM);
                TotalAcres += acresInc;
                double bushelsInc = yield.InstantYield * acresInc;
                TotalBushels += bushelsInc;
                yield.AccumulateCalRun(bushelsInc);
            }
            _lastLat = gps.Latitude;
            _lastLon = gps.Longitude;

            // Moisture average
            if (moisture > 0)
            {
                _moistureSum += moisture;
                _moistureCount++;
                AverageMoisture = _moistureSum / _moistureCount;
            }

            // Write to DB once per second
            if ((DateTime.UtcNow - _lastWriteTime).TotalSeconds >= 1.0)
            {
                _lastWriteTime = DateTime.UtcNow;

                var point = new YieldDataPoint
                {
                    JobId = ActiveJobId,
                    Timestamp = DateTime.UtcNow,
                    Latitude = gps.Latitude,
                    Longitude = gps.Longitude,
                    Elevation = gps.Altitude,
                    Speed = gps.Speed,
                    Heading = gps.Heading,
                    YieldRate = yield.InstantYield,
                    Moisture = moisture,
                    AcresAccumulated = TotalAcres,
                    Sensor1Raw = Core.LastSensor1,
                    Sensor2Raw = Core.LastSensor2
                };

                Core.Database?.YieldData.Insert(point);
            }
        }

        private static double HaversineMetres(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6_371_000; // earth radius metres
            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                     + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                     * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRad(double deg) => deg * Math.PI / 180.0;
    }
}
