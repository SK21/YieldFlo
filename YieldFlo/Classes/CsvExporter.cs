using System;
using System.IO;
using System.Text;
using YieldFlo.Database;

namespace YieldFlo.Classes
{
    public static class CsvExporter
    {
        /// <summary>
        /// Exports all YieldDataPoints for a job to a CSV file.
        /// Returns the output file path, or null on error.
        /// </summary>
        public static string ExportJob(int jobId, string jobName)
        {
            try
            {
                var points = Core.Database.YieldData.GetByJob(jobId);
                if (points == null || points.Count == 0) return null;

                // Resolve the crop's test weight so we can convert bu/ac → t/ha correctly.
                double testWeightKgPerBu = Props.TestWeightKgPerBu;
                foreach (var j in Core.Database.Jobs.GetAll())
                {
                    if (j.id != jobId) continue;
                    if (j.cropId > 0)
                    {
                        foreach (var c in Core.Database.Crops.GetAll())
                        {
                            if (c.id == j.cropId) { testWeightKgPerBu = c.testWeight * 0.453592; break; }
                        }
                    }
                    break;
                }

                string safeName = string.Concat(jobName.Split(Path.GetInvalidFileNameChars()));
                string folder = Props.ExportFolder;
                string path = Path.Combine(folder, $"{safeName}_{jobId}.csv");

                var sb = new StringBuilder();
                sb.AppendLine("Timestamp,Latitude,Longitude,Elevation_m,Speed_kmh,Heading,YieldRate_tha,Moisture_pct,HaAccumulated,Sensor1Raw,Sensor2Raw");

                foreach (var p in points)
                {
                    double yieldTha = p.YieldRate * testWeightKgPerBu / 1000.0 / 0.404686;
                    double haAcc    = p.AcresAccumulated * 0.404686;

                    sb.AppendLine(
                        $"{p.Timestamp:yyyy-MM-dd HH:mm:ss}," +
                        $"{p.Latitude:F7},{p.Longitude:F7}," +
                        $"{p.Elevation:F1},{p.Speed:F2},{p.Heading:F1}," +
                        $"{yieldTha:F2},{p.Moisture:F1}," +
                        $"{haAcc:F4}," +
                        $"{p.Sensor1Raw:F4},{p.Sensor2Raw:F4}");
                }

                File.WriteAllText(path, sb.ToString());
                return path;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("CsvExporter: " + ex.Message);
                return null;
            }
        }
    }
}
