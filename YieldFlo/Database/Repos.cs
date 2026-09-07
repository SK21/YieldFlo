using System.Data.SQLite;
using System;
using System.Collections.Generic;
using YieldFlo.Classes;

namespace YieldFlo.Database
{
    // Thrown when a delete is blocked by a foreign-key constraint (the row is
    // still referenced by a job or calibration) so callers can show a friendly
    // message instead of letting the raw SQLiteException surface.
    public class ItemInUseException : Exception
    {
        public ItemInUseException() : base("Item is referenced by one or more jobs.") { }
    }

    // ── JobRepo ───────────────────────────────────────────────────────────────
    public class JobRepo
    {
        private readonly string _cs;
        public JobRepo(string connectionString) { _cs = connectionString; }

        public int Create(string name, int profileId, int cropId, int headerId, int fieldId = -1)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            // A newly created job is not yet recording — it becomes 'Active' only
            // when Loaded (JobRepo.Reopen / Collector.LoadJob), which closes any
            // previously active job. Creating it 'Active' here produced a second
            // active row alongside the job actually recording, so the DB could
            // hold two 'Active' jobs and resume the wrong one on next start.
            using var cmd = new SQLiteCommand(
                "INSERT INTO jobs (name, profile_id, crop_id, header_id, field_id, status) " +
                "VALUES (@n, @p, @c, @h, @f, 'New'); SELECT last_insert_rowid();", conn);
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

        public List<(int id, string name, string status, string startedAt, double acres, double volume, int profileId, int cropId, int headerId, int fieldId, string notes)> GetAll()
        {
            var result = new List<(int, string, string, string, double, double, int, int, int, int, string)>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT id, name, status, started_at, total_acres, total_volume, profile_id, crop_id, header_id, field_id, notes FROM jobs ORDER BY id DESC", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
                            reader.GetString(3), reader.GetDouble(4), reader.GetDouble(5),
                            reader.IsDBNull(6) ? -1 : reader.GetInt32(6),
                            reader.IsDBNull(7) ? -1 : reader.GetInt32(7),
                            reader.IsDBNull(8) ? -1 : reader.GetInt32(8),
                            reader.IsDBNull(9) ? -1 : reader.GetInt32(9),
                            reader.IsDBNull(10) ? "" : reader.GetString(10)));
            return result;
        }

        public void Update(int jobId, string name, int cropId, int headerId, int profileId, int fieldId = -1, string notes = "")
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE jobs SET name=@n, crop_id=@c, header_id=@h, profile_id=@p, field_id=@f, notes=@nt WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@n",  name);
            cmd.Parameters.AddWithValue("@c",  cropId);
            cmd.Parameters.AddWithValue("@h",  headerId);
            cmd.Parameters.AddWithValue("@p",  profileId);
            cmd.Parameters.AddWithValue("@f",  fieldId > 0 ? (object)fieldId : DBNull.Value);
            cmd.Parameters.AddWithValue("@nt", notes ?? "");
            cmd.Parameters.AddWithValue("@id", jobId);
            cmd.ExecuteNonQuery();
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
            using var tx = conn.BeginTransaction();
            // Deleting a job takes its recorded readings with it — with FK
            // enforcement on, yield_data.job_id would otherwise block this delete.
            using (var cmd = new SQLiteCommand("DELETE FROM yield_data WHERE job_id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", jobId);
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new SQLiteCommand("DELETE FROM jobs WHERE id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", jobId);
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }
    }

    // ── YieldDataRepo ─────────────────────────────────────────────────────────
    public class YieldDataRepo
    {
        private readonly string _cs;
        public YieldDataRepo(string connectionString) { _cs = connectionString; }

        public bool Insert(YieldDataPoint pt)
        {
            try
            {
                using var conn = new SQLiteConnection(_cs);
                conn.Open();
                using var cmd = new SQLiteCommand(@"
INSERT INTO yield_data
    (job_id, timestamp, latitude, longitude, elevation, speed, heading,
     yield_rate, moisture, acres_accumulated, sensor1_raw, sensor2_raw,
     rpm, paddle_hz, min_cycle_ms, gate_rejects)
VALUES
    (@jid, @ts, @lat, @lon, @elev, @spd, @hdg,
     @yr, @mst, @ac, @s1, @s2,
     @rpm, @phz, @mcm, @gr)", conn);
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
                cmd.Parameters.AddWithValue("@rpm", pt.ModuleRpm);
                cmd.Parameters.AddWithValue("@phz", pt.PaddleHz);
                cmd.Parameters.AddWithValue("@mcm", pt.MinCycleMs);
                cmd.Parameters.AddWithValue("@gr", pt.GateRejects);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("YieldDataRepo/Insert: " + ex.Message);
                return false;
            }
        }

        public List<YieldDataPoint> GetByJob(int jobId)
        {
            var result = new List<YieldDataPoint>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            // Name every column: the reader below is positional, and SELECT * makes
            // those positions depend on the schema the database happens to carry.
            // Method2 adds flow_rate/paddles_per_s/flow_flags, which land at 16-18
            // and push gate_rejects to 19 — so SELECT * on a database that has ever
            // run a Method2 build fed a REAL into GetInt32(16) and threw
            // InvalidCastException on the first row of every job.
            using var cmd = new SQLiteCommand(
                "SELECT id, job_id, timestamp, latitude, longitude, elevation, speed, heading, " +
                "yield_rate, moisture, acres_accumulated, sensor1_raw, sensor2_raw, " +
                "rpm, paddle_hz, min_cycle_ms, gate_rejects " +
                "FROM yield_data WHERE job_id=@jid ORDER BY timestamp", conn);
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
                    Sensor2Raw = reader.GetDouble(12),
                    ModuleRpm = reader.GetInt32(13),
                    PaddleHz = reader.GetInt32(14),
                    MinCycleMs = reader.GetInt32(15),
                    GateRejects = reader.GetInt32(16)
                });
            }
            return result;
        }

        // Re-derives YieldRate for every point in a job from its stored raw sensor
        // reading and speed, using the given (typically just-updated) calibration,
        // and brings jobs.total_volume along with it. Returns the rows rewritten and
        // the job's new total. Caller repaints the map and, if this is the recording
        // job, syncs the collector's in-memory total (see clsDataCollector).
        //
        // The total is RESCALED by the ratio of area-weighted sums, not recomputed
        // from the rows. The live total accumulates InstantYield * AcresInc at GPS
        // rate, whereas a row is a 1 Hz mean — so a row-by-row integration loses the
        // within-second covariance between yield and distance and would land on a
        // different number even when nothing changed. Taking a ratio cancels that
        // bias: an unchanged calibration leaves the total bit-identical, and a pure
        // YieldFactor change scales it exactly.
        public (int rows, double totalVolume) RecalculateJob(int jobId, double baseline, double yieldFactor,
                                   double headerWidthM, double testWeightLbsBu)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var tx = conn.BeginTransaction();

            double oldTotal = 0;
            using (var totCmd = new SQLiteCommand("SELECT total_volume FROM jobs WHERE id=@jid", conn, tx))
            {
                totCmd.Parameters.AddWithValue("@jid", jobId);
                object o = totCmd.ExecuteScalar();
                if (o != null && o != DBNull.Value) oldTotal = Convert.ToDouble(o);
            }

            var updates = new List<(int id, double rate)>();
            double oldSum = 0, newSum = 0, prevAcres = 0;

            // ORDER BY timestamp is load-bearing here, not cosmetic: acres_accumulated
            // is cumulative, so the per-row area weight is a difference against the
            // previous row and is meaningless in arbitrary order.
            using (var selCmd = new SQLiteCommand(
                "SELECT id, speed, sensor1_raw, yield_rate, acres_accumulated " +
                "FROM yield_data WHERE job_id=@jid ORDER BY timestamp", conn, tx))
            {
                selCmd.Parameters.AddWithValue("@jid", jobId);
                using var reader = selCmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    double speed = reader.GetFloat(1);
                    double sensor1Raw = reader.GetDouble(2);
                    double oldRate = reader.GetDouble(3);
                    double acres = reader.GetDouble(4);

                    double rate = clsYieldCalculator.ComputeYieldRate(
                        sensor1Raw, speed, baseline, yieldFactor, headerWidthM, testWeightLbsBu);
                    updates.Add((id, rate));

                    // Guard the delta: a resumed job restarts its accumulator, which
                    // would otherwise contribute a large negative area.
                    double dAcres = acres - prevAcres;
                    if (dAcres > 0)
                    {
                        oldSum += oldRate * dAcres;
                        newSum += rate * dAcres;
                    }
                    prevAcres = acres;
                }
            }

            using (var updCmd = new SQLiteCommand("UPDATE yield_data SET yield_rate=@yr WHERE id=@id", conn, tx))
            {
                var pYr = updCmd.Parameters.Add("@yr", System.Data.DbType.Double);
                var pId = updCmd.Parameters.Add("@id", System.Data.DbType.Int32);
                foreach (var (id, rate) in updates)
                {
                    pYr.Value = rate;
                    pId.Value = id;
                    updCmd.ExecuteNonQuery();
                }
            }

            double newTotal = oldTotal;
            if (updates.Count > 0)
            {
                // Ratio when there is a usable baseline to scale; otherwise fall back
                // to the direct integration. The fallback covers a job whose total was
                // never written (0 with rows present) — an approximate total beats
                // leaving Recalculate visibly doing nothing. Where the job genuinely
                // harvested nothing, newSum is ~0 anyway, so the fallback is safe.
                newTotal = (oldSum > 0 && oldTotal > 0) ? oldTotal * (newSum / oldSum) : newSum;

                using var jobCmd = new SQLiteCommand("UPDATE jobs SET total_volume=@v WHERE id=@jid", conn, tx);
                jobCmd.Parameters.AddWithValue("@v", newTotal);
                jobCmd.Parameters.AddWithValue("@jid", jobId);
                jobCmd.ExecuteNonQuery();
            }
            // else: no rows — leave the stored total untouched rather than zeroing a
            // job whose points are missing for some other reason.

            tx.Commit();
            return (updates.Count, newTotal);
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

        public List<(int id, string name, string combineId, double tempOffset, double tempScale, double moistScale, double sensorBaseline)> GetAll()
        {
            var result = new List<(int, string, string, double, double, double, double)>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT id, name, combine_id, temp_offset, temp_scale, moist_scale, sensor_baseline FROM profiles ORDER BY name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
                            reader.IsDBNull(3) ? 0.0    : reader.GetDouble(3),
                            reader.IsDBNull(4) ? 0.0125 : reader.GetDouble(4),
                            reader.IsDBNull(5) ? 0.001  : reader.GetDouble(5),
                            reader.IsDBNull(6) ? 0.0    : reader.GetDouble(6)));
            return result;
        }

        // The sensor's zero point belongs to the machine, not the crop — see the
        // migration note in DB.cs. The date is stamped only when the value actually
        // moves, so "Baseline last saved" answers "when was this sensor last zeroed"
        // rather than "when was anything on the Yield Cal form last saved": a
        // factor-only save writes the same baseline back and must not bump it.
        // 0.0005 is half a step of numBaseline's 3 decimal places.
        public void UpdateSensorBaseline(int id, double baseline)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE profiles SET sensor_baseline=@b, baseline_set_at=datetime('now') " +
                "WHERE id=@id AND ABS(sensor_baseline - @b) > 0.0005", conn);
            cmd.Parameters.AddWithValue("@b",  baseline);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // When this profile's baseline was last changed (local time), or null if it
        // has never been set. Stored UTC by datetime('now'), converted here.
        public DateTime? GetBaselineSetDate(int id)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT datetime(baseline_set_at, 'localtime') FROM profiles WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            if (cmd.ExecuteScalar() is string s && DateTime.TryParse(s, out var dt))
                return dt;
            return null;
        }

        public void UpdateTempOffset(int id, double offset)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE profiles SET temp_offset=@o WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@o",  offset);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateTempScale(int id, double scale)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE profiles SET temp_scale=@s WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@s",  scale);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateMoistScale(int id, double scale)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE profiles SET moist_scale=@s WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@s",  scale);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
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
            using var tx = conn.BeginTransaction();
            // Deleting a profile takes its calibration history with it — with FK
            // enforcement on, calibrations.profile_id would otherwise block this
            // delete on any profile that has ever been calibrated (the normal
            // Save workflow), which is every profile in practice.
            using (var cmd = new SQLiteCommand("DELETE FROM calibrations WHERE profile_id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new SQLiteCommand("DELETE FROM profiles WHERE id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", id);
                try { cmd.ExecuteNonQuery(); }
                catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint)
                { tx.Rollback(); throw new ItemInUseException(); }
            }
            tx.Commit();
        }
    }

    // ── CropRepo ──────────────────────────────────────────────────────────────
    public class CropRepo
    {
        private readonly string _cs;
        public CropRepo(string connectionString) { _cs = connectionString; }

        public int Create(string name, string category, double testWeight,
                          double marketMoisture, double dryMoisture, double moistureOffset = 0)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "INSERT INTO crops (name, category, test_weight, market_moisture, dry_moisture, moisture_offset) " +
                "VALUES (@n, @cat, @tw, @mm, @dm, @mo); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@n",   name);
            cmd.Parameters.AddWithValue("@cat", category);
            cmd.Parameters.AddWithValue("@tw",  testWeight);
            cmd.Parameters.AddWithValue("@mm",  marketMoisture);
            cmd.Parameters.AddWithValue("@dm",  dryMoisture);
            cmd.Parameters.AddWithValue("@mo",  moistureOffset);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<(int id, string name, string category, double testWeight, double marketMoisture, double dryMoisture, double moistureOffset)> GetAll()
        {
            var result = new List<(int, string, string, double, double, double, double)>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT id, name, category, test_weight, market_moisture, dry_moisture, moisture_offset FROM crops ORDER BY name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
                            reader.GetDouble(3), reader.GetDouble(4), reader.GetDouble(5),
                            reader.IsDBNull(6) ? 0.0 : reader.GetDouble(6)));
            return result;
        }

        public void Update(int id, string name, string category, double testWeight,
                           double marketMoisture, double dryMoisture, double moistureOffset = 0)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE crops SET name=@n, category=@cat, test_weight=@tw, " +
                "market_moisture=@mm, dry_moisture=@dm, moisture_offset=@mo WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@n",   name);
            cmd.Parameters.AddWithValue("@cat", category);
            cmd.Parameters.AddWithValue("@tw",  testWeight);
            cmd.Parameters.AddWithValue("@mm",  marketMoisture);
            cmd.Parameters.AddWithValue("@dm",  dryMoisture);
            cmd.Parameters.AddWithValue("@mo",  moistureOffset);
            cmd.Parameters.AddWithValue("@id",  id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateMoistureOffset(int id, double offset)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE crops SET moisture_offset=@o WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@o",  offset);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var tx = conn.BeginTransaction();
            // Deleting a crop takes its calibration history with it — same reason
            // as ProfileRepo.Delete (calibrations.crop_id would otherwise block
            // this delete on any crop that has ever been calibrated).
            using (var cmd = new SQLiteCommand("DELETE FROM calibrations WHERE crop_id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new SQLiteCommand("DELETE FROM crops WHERE id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", id);
                try { cmd.ExecuteNonQuery(); }
                catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint)
                { tx.Rollback(); throw new ItemInUseException(); }
            }
            tx.Commit();
        }
    }

    // ── HeaderRepo ────────────────────────────────────────────────────────────
    public class HeaderRepo
    {
        private readonly string _cs;
        public HeaderRepo(string connectionString) { _cs = connectionString; }

        public int Create(string name, string headerType, double cutWidthM, double fwdOffsetM = 0)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "INSERT INTO headers (name, header_type, cut_width, fwd_offset) VALUES (@n, @t, @w, @o); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@t", headerType);
            cmd.Parameters.AddWithValue("@w", cutWidthM);
            cmd.Parameters.AddWithValue("@o", fwdOffsetM);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<(int id, string name, string type, double widthM, double fwdOffsetM)> GetAll()
        {
            var result = new List<(int, string, string, double, double)>();
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT id, name, header_type, cut_width, fwd_offset FROM headers ORDER BY name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add((reader.GetInt32(0), reader.GetString(1),
                            reader.GetString(2), reader.GetDouble(3), reader.GetDouble(4)));
            return result;
        }

        public void Update(int id, string name, string headerType, double cutWidthM, double fwdOffsetM = 0)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "UPDATE headers SET name=@n, header_type=@t, cut_width=@w, fwd_offset=@o WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@n",  name);
            cmd.Parameters.AddWithValue("@t",  headerType);
            cmd.Parameters.AddWithValue("@w",  cutWidthM);
            cmd.Parameters.AddWithValue("@o",  fwdOffsetM);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM headers WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            try { cmd.ExecuteNonQuery(); }
            catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint)
            { throw new ItemInUseException(); }
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

        public void Update(int id, string name)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE fields SET name=@n WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@n",  name);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM fields WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            try { cmd.ExecuteNonQuery(); }
            catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint)
            { throw new ItemInUseException(); }
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

        // When the latest calibration for this profile/crop was saved (local time),
        // or null if none has been saved yet. calibrated_at is written by SQLite
        // as UTC (datetime('now')) — converted here so callers only see local.
        public DateTime? GetLatestDate(int profileId, int cropId)
        {
            using var conn = new SQLiteConnection(_cs);
            conn.Open();
            using var cmd = new SQLiteCommand(
                "SELECT datetime(calibrated_at, 'localtime') FROM calibrations " +
                "WHERE profile_id=@p AND crop_id=@c ORDER BY id DESC LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@p", profileId);
            cmd.Parameters.AddWithValue("@c", cropId);
            if (cmd.ExecuteScalar() is string s && DateTime.TryParse(s, out var dt))
                return dt;
            return null;
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
