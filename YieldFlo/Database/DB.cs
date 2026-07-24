using System.Data.SQLite;
using System;
using YieldFlo.Classes;

namespace YieldFlo.Database
{
    /// <summary>
    /// SQLite database wrapper. Creates and migrates all tables on first run.
    /// </summary>
    public class DB
    {
        private readonly string _connectionString;

        public JobRepo Jobs { get; private set; }
        public YieldDataRepo YieldData { get; private set; }
        public ProfileRepo Profiles { get; private set; }
        public CropRepo Crops { get; private set; }
        public HeaderRepo Headers { get; private set; }
        public FieldRepo Fields { get; private set; }
        public CalibrationRepo Calibrations { get; private set; }

        public DB(string dbPath)
        {
            _connectionString = $"Data Source={dbPath};Foreign Keys=True;";
        }

        public void Initialize()
        {
            try
            {
                using var conn = OpenConnection();
                CreateTables(conn);
                MigrateSchema(conn);

                Jobs = new JobRepo(_connectionString);
                YieldData = new YieldDataRepo(_connectionString);
                Profiles = new ProfileRepo(_connectionString);
                Crops = new CropRepo(_connectionString);
                Headers = new HeaderRepo(_connectionString);
                Fields = new FieldRepo(_connectionString);
                Calibrations = new CalibrationRepo(_connectionString);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("DB/Initialize: " + ex.Message);
                throw;
            }
        }

        public SQLiteConnection OpenConnection()
        {
            var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public void Close()
        {
            SQLiteConnection.ClearAllPools();
        }

        private void CreateTables(SQLiteConnection conn)
        {
            string sql = @"
PRAGMA journal_mode=WAL;
PRAGMA foreign_keys=ON;

CREATE TABLE IF NOT EXISTS profiles (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT    NOT NULL,
    combine_id  TEXT    NOT NULL DEFAULT '',
    created_at  TEXT    NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS crops (
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    name             TEXT    NOT NULL,
    category         TEXT    NOT NULL DEFAULT 'Cereal',
    test_weight      REAL    NOT NULL DEFAULT 60.0,
    market_moisture  REAL    NOT NULL DEFAULT 14.0,
    dry_moisture     REAL    NOT NULL DEFAULT 14.0
);

CREATE TABLE IF NOT EXISTS headers (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT    NOT NULL,
    header_type TEXT    NOT NULL DEFAULT 'Draper',
    cut_width   REAL    NOT NULL DEFAULT 9.144,
    fwd_offset  REAL    NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS fields (
    id       INTEGER PRIMARY KEY AUTOINCREMENT,
    name     TEXT NOT NULL,
    boundary TEXT NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS jobs (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    profile_id   INTEGER REFERENCES profiles(id),
    field_id     INTEGER REFERENCES fields(id),
    crop_id      INTEGER REFERENCES crops(id),
    header_id    INTEGER REFERENCES headers(id),
    name         TEXT    NOT NULL,
    started_at   TEXT    NOT NULL DEFAULT (datetime('now')),
    ended_at     TEXT,
    status       TEXT    NOT NULL DEFAULT 'Active',
    total_acres  REAL    NOT NULL DEFAULT 0,
    total_volume REAL    NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS calibrations (
    id                   INTEGER PRIMARY KEY AUTOINCREMENT,
    profile_id           INTEGER REFERENCES profiles(id),
    crop_id              INTEGER REFERENCES crops(id),
    sensor_baseline      REAL    NOT NULL DEFAULT 0,
    yield_factor         REAL    NOT NULL DEFAULT 1,
    processing_delay_sec INTEGER NOT NULL DEFAULT 10,
    calibrated_at        TEXT    NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS yield_data (
    id                 INTEGER PRIMARY KEY AUTOINCREMENT,
    job_id             INTEGER NOT NULL REFERENCES jobs(id),
    timestamp          TEXT    NOT NULL,
    latitude           REAL    NOT NULL DEFAULT 0,
    longitude          REAL    NOT NULL DEFAULT 0,
    elevation          REAL    NOT NULL DEFAULT 0,
    speed              REAL    NOT NULL DEFAULT 0,
    heading            REAL    NOT NULL DEFAULT 0,
    yield_rate         REAL    NOT NULL DEFAULT 0,
    moisture           REAL    NOT NULL DEFAULT 0,
    acres_accumulated  REAL    NOT NULL DEFAULT 0,
    sensor1_raw        REAL    NOT NULL DEFAULT 0,
    sensor2_raw        REAL    NOT NULL DEFAULT 0
);

CREATE INDEX IF NOT EXISTS idx_yield_data_job ON yield_data(job_id);
";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private void MigrateSchema(SQLiteConnection conn)
        {
            // Add columns introduced after initial release — harmless if they already exist.
            try
            {
                using var cmd = new SQLiteCommand(
                    "ALTER TABLE crops ADD COLUMN moisture_offset REAL NOT NULL DEFAULT 0;", conn);
                cmd.ExecuteNonQuery();
            }
            catch { }
            try
            {
                using var cmd = new SQLiteCommand(
                    "ALTER TABLE profiles ADD COLUMN temp_offset REAL NOT NULL DEFAULT 0;", conn);
                cmd.ExecuteNonQuery();
            }
            catch { }
            try
            {
                using var cmd = new SQLiteCommand(
                    "ALTER TABLE profiles ADD COLUMN temp_scale REAL NOT NULL DEFAULT 0.0125;", conn);
                cmd.ExecuteNonQuery();
            }
            catch { }
            try
            {
                using var cmd = new SQLiteCommand(
                    "ALTER TABLE profiles ADD COLUMN moist_scale REAL NOT NULL DEFAULT 0.001;", conn);
                cmd.ExecuteNonQuery();
            }
            catch { }
            try
            {
                using var cmd = new SQLiteCommand(
                    "ALTER TABLE headers ADD COLUMN fwd_offset REAL NOT NULL DEFAULT 0;", conn);
                cmd.ExecuteNonQuery();
            }
            catch { }
            try
            {
                using var cmd = new SQLiteCommand(
                    "ALTER TABLE jobs ADD COLUMN notes TEXT NOT NULL DEFAULT '';", conn);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }
    }
}
