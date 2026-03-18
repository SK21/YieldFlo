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

                string safeName = string.Concat(jobName.Split(Path.GetInvalidFileNameChars()));
                string folder = Props.ExportFolder;
                string path = Path.Combine(folder, $"{safeName}_{jobId}.csv");

                var sb = new StringBuilder();
                sb.AppendLine("Timestamp,Latitude,Longitude,Elevation,Speed,Heading,YieldRate,Moisture,AcresAccumulated,Sensor1Raw,Sensor2Raw");

                foreach (var p in points)
                {
                    sb.AppendLine(
                        $"{p.Timestamp:yyyy-MM-dd HH:mm:ss}," +
                        $"{p.Latitude:F7},{p.Longitude:F7}," +
                        $"{p.Elevation:F1},{p.Speed:F2},{p.Heading:F1}," +
                        $"{p.YieldRate:F2},{p.Moisture:F1}," +
                        $"{p.AcresAccumulated:F4}," +
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
