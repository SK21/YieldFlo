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
        public bool IsAutoPaused { get; private set; }  // true only when paused by AOG condition (not manually)
        public int ActiveJobId { get; private set; } = -1;
        public string ActiveJobName { get; private set; } = "";

        public double TotalAcres { get; private set; }
        public double TotalBushels { get; private set; }
        public double AverageYield => TotalAcres > 0.01 ? TotalBushels / TotalAcres : 0;
        public double AverageMoisture { get; private set; }

        private DateTime _lastWriteTime = DateTime.MinValue;
        private double _lastLat = 0, _lastLon = 0;
        private double _moistureSum = 0;
        private int _moistureCount = 0;

        public void StartJob(int jobId, string jobName = "")
        {
            // Close any currently active job before starting a new one
            if (ActiveJobId > 0 && Core.Database != null)
            {
                Core.Database.Jobs.UpdateTotals(ActiveJobId, TotalAcres, TotalBushels);
                Core.Database.Jobs.Close(ActiveJobId);
            }

            ActiveJobId = jobId;
            ActiveJobName = jobName;
            TotalAcres = 0;
            TotalBushels = 0;
            AverageMoisture = 0;
            _moistureSum = 0;
            _moistureCount = 0;
            _lastLat = 0;
            _lastLon = 0;
            _lastWriteTime = DateTime.MinValue;
            IsRecording  = false;
            IsAutoPaused = true;   // starts recording when sections come on
        }

        public void RenameActiveJob(string name) => ActiveJobName = name;

        public void StopJob()
        {
            IsRecording = false;
            if (ActiveJobId > 0 && Core.Database != null)
            {
                Core.Database.Jobs.UpdateTotals(ActiveJobId, TotalAcres, TotalBushels);
                Core.Database.Jobs.Close(ActiveJobId);
            }
            ActiveJobId = -1;
            ActiveJobName = "";
        }

        public void PauseJob() { IsRecording = false; IsAutoPaused = false; }  // manual pause

        private void AutoPause() { IsRecording = false; IsAutoPaused = true; }

        private void AutoResume() { IsRecording = true; IsAutoPaused = false; }

        /// <summary>
        /// Saves accumulated totals to the DB but leaves the job status as Active
        /// so it can be auto-resumed on the next app start.
        /// </summary>
        public void SuspendJob()
        {
            IsRecording = false;
            if (ActiveJobId > 0 && Core.Database != null)
                Core.Database.Jobs.UpdateTotals(ActiveJobId, TotalAcres, TotalBushels);
            ActiveJobId   = -1;
            ActiveJobName = "";
        }

        public void ResumeJob() { IsRecording = false; IsAutoPaused = true; }  // re-arms auto-resume; recording starts when sections come on

        /// <summary>
        /// Loads a previously created job, restoring its accumulated totals.
        /// Used when resuming a job from a prior session.
        /// </summary>
        public void LoadJob(int jobId, string jobName, double existingAcres, double existingBushels)
        {
            ActiveJobId = jobId;
            ActiveJobName = jobName;
            TotalAcres = existingAcres;
            TotalBushels = existingBushels;
            AverageMoisture = 0;
            _moistureSum = 0;
            _moistureCount = 0;
            _lastLat = 0;
            _lastLon = 0;
            _lastWriteTime = DateTime.MinValue;
            IsRecording  = false;
            IsAutoPaused = true;   // auto-resumes once AOG connects and harvesting starts
        }

        /// <summary>
        /// Called every GPS update (~10 Hz). Writes to DB once per second.
        /// </summary>
        public void OnGpsUpdate(double moisture)
        {
            if (ActiveJobId < 0) return;

            // Allow auto-resume check even when paused — but skip entirely if manually paused.
            if (!IsRecording && !IsAutoPaused) return;

            var gps = Core.GPS;
            if (!gps.IsConnected) return;

            var yield = Core.Yield;

            // Calculate yield for this GPS position
            yield.Calculate(gps.Speed);

            // Auto-pause/resume based on AOG state: speed > 0 AND at least one section on.
            // Sections turn off over already-harvested ground even when moving.
            bool harvestActive = gps.SectionsActive;

            if (!harvestActive)
            {
                _lastLat = 0;
                _lastLon = 0;
                if (!IsAutoPaused)
                {
                    AutoPause();
                    Core.RaiseJobStateChanged();
                }
                return;
            }

            if (IsAutoPaused)
            {
                AutoResume();
                Core.RaiseJobStateChanged();
            }

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
