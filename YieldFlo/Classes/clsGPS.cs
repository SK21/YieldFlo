namespace YieldFlo.Classes
{
    /// <summary>
    /// Holds the latest GPS fix received from AgOpenGPS via UDP.
    /// Populated by UDPComm.HandleData when PGN 33152 / sub-type 100 or 208 arrives.
    /// </summary>
    public class clsGPS
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float Heading { get; set; }      // degrees
        public float Speed { get; set; }        // km/h
        public float Altitude { get; set; }     // meters
        public byte FixQuality { get; set; }    // 0=none 1=GPS 2=DGPS 4=RTK

        // Connected = a position packet within the last 4 s. Same staleness
        // window RateController uses (PGN208.TWOLconnected) — a plain latch
        // would stay "connected" forever after the first fix even if AOG
        // closes or the link drops.
        private System.DateTime _lastFixUtc;
        public bool IsConnected => (System.DateTime.UtcNow - _lastFixUtc).TotalSeconds < 4.0;

        // True when ≥1 section is on (AOG PGN 229). AOG sends 229 every GPS tick
        // while a job is open, so a value older than 4 s means AOG closed or the
        // link dropped — treat that as sections off instead of latching the last
        // state forever (same staleness window RateController uses).
        private bool _sectionsActive;
        private System.DateTime _sectionsAtUtc;
        public bool SectionsActive
        {
            get => _sectionsActive && (System.DateTime.UtcNow - _sectionsAtUtc).TotalSeconds < 4.0;
            set { _sectionsActive = value; _sectionsAtUtc = System.DateTime.UtcNow; }
        }

        public void ParseByteData(byte[] data, byte subType)
        {
            if (subType == 208)
                ParsePgn208(data);
            else
                ParsePgn100(data);
        }

        private void ParsePgn100(byte[] data)
        {
            // AOG PGN 33152, sub-type 100 (corrected position)
            // Bytes 5-12  : longitude (double)
            // Bytes 13-20 : latitude  (double)
            // Bytes 21-28 : Fix2Fix heading (double) — present only in extended variant (length=24)
            // Speed is NOT in this packet; it arrives via sub-type 254 (AutoSteer)
            if (data.Length < 21) return;

            Longitude = System.BitConverter.ToDouble(data, 5);
            Latitude  = System.BitConverter.ToDouble(data, 13);

            if (data.Length >= 29)
                Heading = (float)System.BitConverter.ToDouble(data, 21);

            FixQuality = 1;
            _lastFixUtc = System.DateTime.UtcNow;
        }

        public void ParseSpeedPgn254(byte[] data)
        {
            // AOG PGN 33152, sub-type 254 (AutoSteer data)
            // Bytes 5-6: speed * 10 (uint16, km/h) — only update speed; lat/lon come from sub-type 100
            Speed = (float)((System.BitConverter.ToUInt16(data, 5)) / 10.0);
        }

        private void ParsePgn208(byte[] data)
        {
            // AOG PGN 33152, sub-type 208 (TwoL dual-GPS)
            // Bytes 5-12  : longitude (double)
            // Bytes 13-20 : latitude  (double)
            // Bytes 21-28 : speed KMH (double) — smoothed 0.1 new + 0.9 old
            // Bytes 29-36 : elevation (double)
            if (data.Length < 29) return;

            Longitude = System.BitConverter.ToDouble(data, 5);
            Latitude  = System.BitConverter.ToDouble(data, 13);
            double newSpeed = System.BitConverter.ToDouble(data, 21);
            Speed = (float)((newSpeed * 0.1) + (Speed * 0.9));

            if (data.Length > 36)
                Altitude = (float)System.BitConverter.ToDouble(data, 29);

            _lastFixUtc = System.DateTime.UtcNow;
        }
    }
}
