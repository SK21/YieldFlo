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
        public bool IsConnected { get; set; }

        public void ParseByteData(byte[] data, byte subType)
        {
            if (subType == 208)
                ParsePgn208(data);
            else
                ParsePgn100(data);
        }

        private void ParsePgn100(byte[] data)
        {
            // AOG PGN 33152, sub-type 100 (roll-corrected lat/lon)
            // Bytes 5-8   : latitude  * 10,000,000  (int32, little-endian)
            // Bytes 9-12  : longitude * 10,000,000  (int32, little-endian)
            // Bytes 13-14 : heading   * 10          (uint16)
            // Bytes 15-16 : speed     * 10          (uint16, km/h)
            // Bytes 17-18 : altitude  * 10          (int16, meters)
            // Byte  19    : fix quality
            if (data.Length < 20) return;

            int latRaw = System.BitConverter.ToInt32(data, 5);
            int lonRaw = System.BitConverter.ToInt32(data, 9);
            ushort hdgRaw = System.BitConverter.ToUInt16(data, 13);
            ushort spdRaw = System.BitConverter.ToUInt16(data, 15);
            short altRaw = System.BitConverter.ToInt16(data, 17);

            Latitude = latRaw / 10_000_000.0;
            Longitude = lonRaw / 10_000_000.0;
            Heading = hdgRaw / 10f;
            Speed = spdRaw / 10f;
            Altitude = altRaw / 10f;
            FixQuality = data[19];
            IsConnected = FixQuality > 0;
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

            IsConnected = true;
        }
    }
}
