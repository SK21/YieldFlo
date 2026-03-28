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

                // Resolve crop test weight and header width from the job record.
                double testWeightKgPerBu = Props.TestWeightKgPerBu;
                double headerWidthM      = 9.144;
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
                    if (j.headerId > 0)
                    {
                        foreach (var h in Core.Database.Headers.GetAll())
                        {
                            if (h.id == j.headerId) { headerWidthM = h.widthM; break; }
                        }
                    }
                    break;
                }

                string safeName = string.Concat(jobName.Split(Path.GetInvalidFileNameChars()));
                string folder = Props.ExportFolder;
                string path = Path.Combine(folder, $"{safeName}_{jobId}.csv");

                var sb = new StringBuilder();
                // Columns 0-5 match RC's YieldOverlayCreator expected format (parsed by position).
                // Extra columns follow for YieldFlo-specific data.
                sb.AppendLine("Timestamp,Latitude,Longitude,WidthMeters,YieldKg,ElevationMeters,Speed_kmh,Heading,Moisture_pct,HaAccumulated,Sensor1Raw");

                foreach (var p in points)
                {
                    double yieldTha  = p.YieldRate * testWeightKgPerBu / 1000.0 / 0.404686;
                    double yieldKgHa = yieldTha * 1000.0;
                    double haAcc     = p.AcresAccumulated * 0.404686;

                    sb.AppendLine(
                        $"{p.Timestamp:yyyy-MM-dd HH:mm:ss}," +
                        $"{p.Latitude:F7},{p.Longitude:F7}," +
                        $"{headerWidthM:F3},{yieldKgHa:F1},{p.Elevation:F1}," +
                        $"{p.Speed:F2},{p.Heading:F1}," +
                        $"{p.Moisture:F1}," +
                        $"{haAcc:F4}," +
                        $"{p.Sensor1Raw:F4}");
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
