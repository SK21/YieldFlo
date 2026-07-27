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
        public int ModuleRpm { get; set; }      // elevator RPM from the module packet; fixed reference 200 when no RPM sensor fitted
        public int PaddleHz { get; set; } = -1;      // paddles/s from the 1 Hz packet; -1 = not reported
        public int MinCycleMs { get; set; } = -1;    // shortest completed paddle cycle in the 1 Hz packet's window, ms; -1 = not reported
        public int GateRejects { get; set; } = -1;   // edges the module's period gate rejected in that window; -1 = not reported (old firmware or UDP)
    }
}
