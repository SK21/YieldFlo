using System;

namespace YieldFlo.Database
{
    public class YieldDataPoint
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public DateTime Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }   // metres
        public float Speed { get; set; }        // km/h
        public float Heading { get; set; }      // degrees
        public double YieldRate { get; set; }   // bu/ac
        public double Moisture { get; set; }    // %
        public double AcresAccumulated { get; set; }
        public double Sensor1Raw { get; set; }
        public double Sensor2Raw { get; set; }
    }
}
