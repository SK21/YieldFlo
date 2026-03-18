using System.Data.SQLite;
using System;
using System.Collections.Generic;
using YieldFlo.Classes;

namespace YieldFlo.Database
{
    // ── JobRepo ───────────────────────────────────────────────────────────────
    public class JobRepo
    {
        private readonly string _cs;
        public JobRepo(string connectionString) { _cs = connectionString; }

        public int Create(string name, int profileId, int cropId, int headerId, int fieldId = -1)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "INSERT INTO jobs (name, profile_id, crop_id, header_id, field_id, status) " +
                "VALUES (@n, @p, @c, @h, @f, 'Active'); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@p", profileId);
            cmd.Parameters.AddWithValue("@c", cropId);
            cmd.Parameters.AddWithValue("@h", headerId);
            cmd.Parameters.AddWithValue("@f", fieldId > 0 ? (object)fieldId : DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void UpdateTotals(int jobId, double acres, double bushels)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE jobs SET total_acres=@a, total_volume=@b WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@a", acres);
            cmd.Parameters.AddWithValue("@b", bushels);
            cmd.Parameters.AddWithValue("@id", jobId);
            cmd.ExecuteNonQuery();
        }

        public void Close(int jobId)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE jobs SET status='Complete', ended_at=datetime('now') WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", jobId);
            cmd.ExecuteNonQuery();
        }

        public List<(int id, string name, string status, string startedAt, double acres, double volume, int profileId, int cropId, int headerId)> GetAll()
        {
            var result = new List<(int, string, string, string, double, double, int, int, int)>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT id, name, status, started_at, total_acres, total_volume, profile_id, crop_id, header_id FROM jobs ORDER BY id DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
                            reader.GetString(3), reader.GetDouble(4), reader.GetDouble(5),
                            reader.IsDBNull(6) ? -1 : reader.GetInt32(6),
                            reader.IsDBNull(7) ? -1 : reader.GetInt32(7),
                            reader.IsDBNull(8) ? -1 : reader.GetInt32(8)));
            return result;
        }

        public void Reopen(int jobId)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE jobs SET status='Active', ended_at=NULL WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", jobId);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int jobId)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM jobs WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", jobId);
            cmd.ExecuteNonQuery();
        }
    }

    // ── YieldDataRepo ─────────────────────────────────────────────────────────
    public class YieldDataRepo
    {
        private readonly string _cs;
        public YieldDataRepo(string connectionString) { _cs = connectionString; }

        public void Insert(YieldDataPoint pt)
        {
            try
            {
                using var conn = new SQLiteConnection(_cs);
                conn.Open();
                using var cmd = new SQLiteCommand(@"
INSERT INTO yield_data
    (job_id, timestamp, latitude, longitude, elevation, speed, heading,
     yield_rate, moisture, acres_accumulated, sensor1_raw, sensor2_raw)
VALUES
    (@jid, @ts, @lat, @lon, @elev, @spd, @hdg,
     @yr, @mst, @ac, @s1, @s2)", conn);
                cmd.Parameters.AddWithValue("@jid", pt.JobId);
                cmd.Parameters.AddWithValue("@ts", pt.Timestamp.ToString("o"));
                cmd.Parameters.AddWithValue("@lat", pt.Latitude);
                cmd.Parameters.AddWithValue("@lon", pt.Longitude);
                cmd.Parameters.AddWithValue("@elev", pt.Elevation);
                cmd.Parameters.AddWithValue("@spd", pt.Speed);
                cmd.Parameters.AddWithValue("@hdg", pt.Heading);
                cmd.Parameters.AddWithValue("@yr", pt.YieldRate);
                cmd.Parameters.AddWithValue("@mst", pt.Moisture);
                cmd.Parameters.AddWithValue("@ac", pt.AcresAccumulated);
                cmd.Parameters.AddWithValue("@s1", pt.Sensor1Raw);
                cmd.Parameters.AddWithValue("@s2", pt.Sensor2Raw);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("YieldDataRepo/Insert: " + ex.Message);
            }
        }

        public List<YieldDataPoint> GetByJob(int jobId)
        {
            var result = new List<YieldDataPoint>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT * FROM yield_data WHERE job_id=@jid ORDER BY timestamp", conn);
            cmd.Parameters.AddWithValue("@jid", jobId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new YieldDataPoint
                {
                    Id = reader.GetInt32(0),
                    JobId = reader.GetInt32(1),
                    Timestamp = DateTime.Parse(reader.GetString(2)),
                    Latitude = reader.GetDouble(3),
                    Longitude = reader.GetDouble(4),
                    Elevation = reader.GetDouble(5),
                    Speed = reader.GetFloat(6),
                    Heading = reader.GetFloat(7),
                    YieldRate = reader.GetDouble(8),
                    Moisture = reader.GetDouble(9),
                    AcresAccumulated = reader.GetDouble(10),
                    Sensor1Raw = reader.GetDouble(11),
                    Sensor2Raw = reader.GetDouble(12)
                });
            }
            return result;
        }
    }

    // ── ProfileRepo ───────────────────────────────────────────────────────────
    public class ProfileRepo
    {
        private readonly string _cs;
        public ProfileRepo(string connectionString) { _cs = connectionString; }

        public int Create(string name, string combineId = "")
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "INSERT INTO profiles (name, combine_id) VALUES (@n, @c); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@c", combineId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<(int id, string name, string combineId)> GetAll()
        {
            var result = new List<(int, string, string)>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT id, name, combine_id FROM profiles ORDER BY name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            return result;
        }

        public void Update(int id, string name, string combineId)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE profiles SET name=@n, combine_id=@c WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@n",  name);
            cmd.Parameters.AddWithValue("@c",  combineId);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM profiles WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }

    // ── CropRepo ──────────────────────────────────────────────────────────────
    public class CropRepo
    {
        private readonly string _cs;
        public CropRepo(string connectionString) { _cs = connectionString; }

        public int Create(string name, string category, double testWeight,
                          double marketMoisture, double dryMoisture)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "INSERT INTO crops (name, category, test_weight, market_moisture, dry_moisture) " +
                "VALUES (@n, @cat, @tw, @mm, @dm); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@cat", category);
            cmd.Parameters.AddWithValue("@tw", testWeight);
            cmd.Parameters.AddWithValue("@mm", marketMoisture);
            cmd.Parameters.AddWithValue("@dm", dryMoisture);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<(int id, string name, string category, double testWeight, double marketMoisture, double dryMoisture)> GetAll()
        {
            var result = new List<(int, string, string, double, double, double)>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT id, name, category, test_weight, market_moisture, dry_moisture FROM crops ORDER BY name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
                            reader.GetDouble(3), reader.GetDouble(4), reader.GetDouble(5)));
            return result;
        }

        public void Update(int id, string name, string category, double testWeight,
                           double marketMoisture, double dryMoisture)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE crops SET name=@n, category=@cat, test_weight=@tw, " +
                "market_moisture=@mm, dry_moisture=@dm WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@n",   name);
            cmd.Parameters.AddWithValue("@cat", category);
            cmd.Parameters.AddWithValue("@tw",  testWeight);
            cmd.Parameters.AddWithValue("@mm",  marketMoisture);
            cmd.Parameters.AddWithValue("@dm",  dryMoisture);
            cmd.Parameters.AddWithValue("@id",  id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM crops WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }

    // ── HeaderRepo ────────────────────────────────────────────────────────────
    public class HeaderRepo
    {
        private readonly string _cs;
        public HeaderRepo(string connectionString) { _cs = connectionString; }

        public int Create(string name, string headerType, double cutWidthM)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "INSERT INTO headers (name, header_type, cut_width) VALUES (@n, @t, @w); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@t", headerType);
            cmd.Parameters.AddWithValue("@w", cutWidthM);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<(int id, string name, string type, double widthM)> GetAll()
        {
            var result = new List<(int, string, string, double)>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT id, name, header_type, cut_width FROM headers ORDER BY name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add((reader.GetInt32(0), reader.GetString(1),
                            reader.GetString(2), reader.GetDouble(3)));
            return result;
        }

        public void Update(int id, string name, string headerType, double cutWidthM)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE headers SET name=@n, header_type=@t, cut_width=@w WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@n",  name);
            cmd.Parameters.AddWithValue("@t",  headerType);
            cmd.Parameters.AddWithValue("@w",  cutWidthM);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM headers WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }

    // ── FieldRepo ─────────────────────────────────────────────────────────────
    public class FieldRepo
    {
        private readonly string _cs;
        public FieldRepo(string connectionString) { _cs = connectionString; }

        public int Create(string name, string boundaryGeoJson = "")
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "INSERT INTO fields (name, boundary) VALUES (@n, @b); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@b", boundaryGeoJson);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<(int id, string name)> GetAll()
        {
            var result = new List<(int, string)>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT id, name FROM fields ORDER BY name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add((reader.GetInt32(0), reader.GetString(1)));
            return result;
        }
    }

    // ── CalibrationRepo ───────────────────────────────────────────────────────
    public class CalibrationRepo
    {
        private readonly string _cs;
        public CalibrationRepo(string connectionString) { _cs = connectionString; }

        public int Save(int profileId, int cropId, double baseline,
                        double yieldFactor, int delaySec)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(@"
INSERT INTO calibrations
    (profile_id, crop_id, sensor_baseline, yield_factor, processing_delay_sec)
VALUES (@p, @c, @b, @f, @d);
SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@p", profileId);
            cmd.Parameters.AddWithValue("@c", cropId);
            cmd.Parameters.AddWithValue("@b", baseline);
            cmd.Parameters.AddWithValue("@f", yieldFactor);
            cmd.Parameters.AddWithValue("@d", delaySec);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public (double baseline, double yieldFactor, int delaySec) GetLatest(int profileId, int cropId)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT sensor_baseline, yield_factor, processing_delay_sec " +
                "FROM calibrations WHERE profile_id=@p AND crop_id=@c " +
                "ORDER BY id DESC LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@p", profileId);
            cmd.Parameters.AddWithValue("@c", cropId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return (reader.GetDouble(0), reader.GetDouble(1), reader.GetInt32(2));
            return (0, 1, 10);  // defaults
        }
    }
}
