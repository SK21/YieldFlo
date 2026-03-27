using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ModuleSimulator
{
    /// <summary>
    /// Simulates a YieldFlo hardware module — sends PK1 (sensor, 5 Hz) and
    /// PK2 (temperature, 1 Hz) UDP packets to the YieldFlo PC app on port 30100.
    ///
    /// Moisture and temperature sliders represent calibrated values.
    /// Raw counts sent = value / default_scale so the PC app reads correctly
    /// at default scale settings (moist_scale=0.001, temp_scale=0.0125).
    /// </summary>
    public partial class frmSimulator : Form
    {
        private const int PC_RECV_PORT  = 30100;
        private const int SIM_SEND_PORT = 30201;

        // Default scales — must match Core.cs defaults so slider values display correctly
        private const double DefaultMoistScale = 0.001;   // %/count
        private const double DefaultTempScale  = 0.0125;  // °C/count

        private System.Windows.Forms.Timer _sendTimer;
        private UdpClient  _udp;
        private IPEndPoint _target;
        private double _simAngle  = 0;
        private int    _pk2Ticks  = 0;   // PK2 sent every 10 ticks (1 Hz at 100 ms timer)

        public frmSimulator()
        {
            InitializeComponent();
        }

        private static readonly string _posFile =
            Path.Combine(Application.LocalUserAppDataPath, "simpos.txt");

        private void frmSimulator_Load(object sender, EventArgs e)
        {
            RestorePosition();

            try
            {
                _udp    = new UdpClient(SIM_SEND_PORT);
                _target = new IPEndPoint(IPAddress.Loopback, PC_RECV_PORT);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "UDP Error: " + ex.Message;
                return;
            }

            _sendTimer = new System.Windows.Forms.Timer { Interval = 100 };  // 10 Hz
            _sendTimer.Tick += SendTimer_Tick;
            _sendTimer.Start();
            lblStatus.Text = "Sending to 127.0.0.1:" + PC_RECV_PORT;
        }

        private void RestorePosition()
        {
            try
            {
                if (!File.Exists(_posFile)) return;
                var parts = File.ReadAllText(_posFile).Split(',');
                if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                {
                    var pt = new System.Drawing.Point(x, y);
                    foreach (Screen s in Screen.AllScreens)
                        if (s.WorkingArea.Contains(pt)) { Location = pt; return; }
                }
            }
            catch { }
        }

        private void SavePosition()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_posFile));
                File.WriteAllText(_posFile, $"{Location.X},{Location.Y}");
            }
            catch { }
        }

        private void chkSections_CheckedChanged(object sender, EventArgs e)
        {
            chkSections.ForeColor = chkSections.Checked
                ? System.Drawing.Color.DarkGreen
                : System.Drawing.Color.Red;
        }

        private void SendTimer_Tick(object sender, EventArgs e)
        {
            _simAngle += 0.05;
            _pk2Ticks++;

            double yieldSlider    = trkYield.Value    / 100.0;   // 0.0 – 1.0
            double moistureSlider = trkMoisture.Value / 10.0;    // 0.0 – 30.0 %
            double tempSlider     = trkTemperature.Value / 10.0; // -10.0 – 50.0 °C
            double variation      = trkVariation.Value / 100.0;

            lblVariationSlider.Text = $"Variation: {trkVariation.Value}%";

            // Sensor obstruction ratio
            const double SimBaseline = 0.2;
            double ratio = 0;
            if (chkSections.Checked)
            {
                double flow = chkSineWave.Checked
                    ? yieldSlider * (1.0 + variation * Math.Sin(_simAngle))
                    : yieldSlider;
                ratio = SimBaseline + flow * (1.0 - SimBaseline);
            }

            // sensor_ratio: uint16, 0–1000 (ratio × 1000)
            ushort sensorRatio = (ushort)Math.Max(0, Math.Min(1000, ratio * 1000));

            // moisture_raw: raw ADS1115 count — back-calculated from % using default scale
            ushort moistRaw = (ushort)Math.Max(0, Math.Min(65535, moistureSlider / DefaultMoistScale));

            byte flags = 0x05;  // bit0=SensorOK, bit2=MoistureOK (no RPM sensor in sim)
            ushort rpm = 200;   // fixed RPM-absent sentinel

            SendPK1(sensorRatio, moistRaw, rpm, flags);

            if (_pk2Ticks >= 10)
            {
                _pk2Ticks = 0;
                // temp_raw: raw ADS1115 count — back-calculated from °C using default scale
                short tempRaw = (short)Math.Max(-32768, Math.Min(32767, tempSlider / DefaultTempScale));
                SendPK2(tempRaw);
            }

            // Update UI labels
            lblSensor1.Text   = $"S1: {ratio:F3}";
            lblMoistureVal.Text = $"Mst: {moistureSlider:F1}%";
            lblTempVal.Text   = $"Tmp: {tempSlider:F1}°C";
        }

        private void SendPK1(ushort sensorRatio, ushort moistureRaw, ushort rpm, byte flags)
        {
            // PK1 — 11 bytes:
            // [0-1]  PGN 40001 LE
            // [2]    flags  bit0=SensorOK, bit1=RPMPresent, bit2=MoistureOK
            // [3-4]  sensor_ratio  uint16 LE  (ratio × 1000)
            // [5-6]  moisture_raw  uint16 LE  (raw ADS1115 AIN0-AIN1 count)
            // [7-8]  module_rpm    uint16 LE
            // [9]    noise_count   uint8
            // [10]   CRC8
            byte[] pkt = new byte[11];
            pkt[0] = 0x41;
            pkt[1] = 0x9C;
            pkt[2] = flags;
            pkt[3] = (byte)(sensorRatio & 0xFF);
            pkt[4] = (byte)(sensorRatio >> 8);
            pkt[5] = (byte)(moistureRaw & 0xFF);
            pkt[6] = (byte)(moistureRaw >> 8);
            pkt[7] = (byte)(rpm & 0xFF);
            pkt[8] = (byte)(rpm >> 8);
            pkt[9] = 0;  // noise_count

            int ck = 0;
            for (int i = 0; i < 10; i++) ck += pkt[i];
            pkt[10] = (byte)ck;

            try { _udp.Send(pkt, pkt.Length, _target); } catch { }
        }

        private void SendPK2(short tempRaw)
        {
            // PK2 — 6 bytes:
            // [0-1] PGN 40002 LE
            // [2]   flags  bit0=TempOK
            // [3-4] temp_raw  int16 LE  (raw ADS1115 AIN2 count)
            // [5]   CRC8
            byte[] pkt = new byte[6];
            pkt[0] = 0x42;
            pkt[1] = 0x9C;
            pkt[2] = 0x01;  // TempOK
            pkt[3] = (byte)(tempRaw & 0xFF);
            pkt[4] = (byte)((tempRaw >> 8) & 0xFF);

            int ck = 0;
            for (int i = 0; i < 5; i++) ck += pkt[i];
            pkt[5] = (byte)ck;

            try { _udp.Send(pkt, pkt.Length, _target); } catch { }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SavePosition();
            _sendTimer?.Stop();
            _udp?.Close();
            base.OnFormClosed(e);
        }
    }
}
