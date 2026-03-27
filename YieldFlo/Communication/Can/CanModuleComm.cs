using System;
using System.Timers;
using YieldFlo.Classes;

namespace YieldFlo.Communication.Can
{
    /// <summary>
    /// Manages CAN communication with the YieldFlo sensor module.
    /// Receive-only — filters frame ID 0x18FF00F8, parses 8-byte body, writes Core fields.
    /// CAN frame ID: Extended 0x18FF00F8 (Priority=6, PF=0xFF ProprietaryB, PS=0x00, SA=0xF8)
    /// </summary>
    public class CanModuleComm : IDisposable
    {
        private const uint ModuleFrameId   = 0x18FF00F8u;
        private const int  AdapterTimeoutMs = 4000;
        private const int  ModuleTimeoutMs  = 2000;

        private ICanInterface _driver;
        private Timer _timeoutTimer;
        private DateTime _lastFrameAny    = DateTime.MinValue;
        private DateTime _lastModuleFrame = DateTime.MinValue;

        /// <summary>True if the CAN adapter is open and any frame was received within 4 s.</summary>
        public bool AdapterConnected =>
            _driver != null && _driver.IsOpen &&
            (DateTime.UtcNow - _lastFrameAny).TotalMilliseconds < AdapterTimeoutMs;

        /// <summary>True if a module frame (0x18FF00F8) was received within 2 s.</summary>
        public bool ModuleReceiving =>
            (DateTime.UtcNow - _lastModuleFrame).TotalMilliseconds < ModuleTimeoutMs;

        public bool Start(CanDriver driver, string port)
        {
            Stop();

            switch (driver)
            {
                case CanDriver.InnoMaker: _driver = new InnoMakerInterface(); break;
                case CanDriver.PCAN:      _driver = new PcanInterface();      break;
                default:                  _driver = new SlcanInterface();     break;
            }

            _driver.FrameReceived += OnFrameReceived;

            if (!_driver.Open(port, 250000))
            {
                _driver.FrameReceived -= OnFrameReceived;
                _driver.Dispose();
                _driver = null;
                return false;
            }

            _timeoutTimer = new Timer(500) { AutoReset = true };
            _timeoutTimer.Elapsed += OnTimerElapsed;
            _timeoutTimer.Start();
            return true;
        }

        public void Stop()
        {
            _timeoutTimer?.Stop();
            _timeoutTimer?.Dispose();
            _timeoutTimer = null;

            if (_driver != null)
            {
                _driver.FrameReceived -= OnFrameReceived;
                _driver.Dispose();
                _driver = null;
            }
        }

        private void OnFrameReceived(object sender, CanFrameEventArgs e)
        {
            _lastFrameAny = DateTime.UtcNow;

            if (e.Frame.Id != ModuleFrameId || e.Frame.Dlc != 8 || e.Frame.Data == null)
                return;

            _lastModuleFrame = DateTime.UtcNow;

            // Marshal parse to UI thread (same pattern as UDPComm)
            byte[] data = e.Frame.Data;
            var mf = Core.MainForm;
            if (mf != null && mf.IsHandleCreated && !mf.IsDisposed && !Core.IsShuttingDown)
            {
                try { mf.BeginInvoke((Action)(() => ParseModuleData(data))); }
                catch (InvalidOperationException) { }
            }
        }

        private void ParseModuleData(byte[] d)
        {
            // 8-byte data body (identical layout to bytes [3-10] of the UDP packet):
            // [0]   status_flags  bit0=SensorOK, bit1=RPMPresent, bit2=MoistureOK
            // [1-2] sensor_ratio  uint16 LE  (ratio × 1000, 0–1000 = 0.0–100.0%)
            // [3-4] moisture_raw  uint16 LE  (value × 10 = tenths of percent)
            // [5-6] module_rpm    uint16 LE
            // [7]   noise_count   uint8  (ISR-rejected edges per 200 ms window)
            byte   flags    = d[0];
            ushort ratio    = (ushort)(d[1] | (d[2] << 8));
            ushort moisture = (ushort)(d[3] | (d[4] << 8));
            byte   noise    = d[7];

            bool s1Ok       = (flags & 0x01) != 0;
            bool moistureOk = (flags & 0x04) != 0;

            Core.LastSensor1       = s1Ok       ? ratio    / 1000.0 : 0;
            Core.LastMoisture      = moistureOk ? moisture / 10.0   : 0;
            Core.LastNoiseCount    = noise;
            Core.ModuleConnected   = true;
            Core.LastModuleReceive = DateTime.UtcNow;

            Core.Yield?.PushSensorReading(Core.LastSensor1);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!ModuleReceiving && Core.ModuleConnected)
                Core.ModuleConnected = false;
        }

        public void Dispose() => Stop();
    }
}
