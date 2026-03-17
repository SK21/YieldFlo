using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ModuleSimulator
{
    /// <summary>
    /// Simulates a YieldFlo hardware module by sending sensor UDP packets to
    /// the YieldFlo PC app on port 30100 (loopback).
    /// Use sliders to set simulated yield flow and moisture.
    /// </summary>
    public partial class frmSimulator : Form
    {
        // YieldFlo module ports (recv 30100 / send 30200 on PC side)
        // Simulator sends TO port 30100, FROM port 30200
        private const int PC_RECV_PORT = 30100;
        private const int SIM_SEND_PORT = 30201;   // unique from-port for simulator

        private System.Windows.Forms.Timer _sendTimer;
        private UdpClient _udp;
        private IPEndPoint _target;
        private double _simAngle = 0;   // for sine-wave yield simulation

        public frmSimulator()
        {
            InitializeComponent();
        }

        private void frmSimulator_Load(object sender, EventArgs e)
        {
            try
            {
                _udp = new UdpClient(SIM_SEND_PORT);
                _target = new IPEndPoint(IPAddress.Loopback, PC_RECV_PORT);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "UDP Error: " + ex.Message;
                return;
            }

            _sendTimer = new System.Windows.Forms.Timer { Interval = 100 }; // 10 Hz
            _sendTimer.Tick += SendTimer_Tick;
            _sendTimer.Start();
            lblStatus.Text = "Sending to 127.0.0.1:" + PC_RECV_PORT;
        }

        private void SendTimer_Tick(object sender, EventArgs e)
        {
            _simAngle += 0.05;

            double yieldSlider = trkYield.Value / 100.0;        // 0.0 – 1.0
            double moistureSlider = trkMoisture.Value / 10.0;   // 0.0 – 30.0 %
            bool useWave = chkSineWave.Checked;

            // Sensor obstruction ratio (0.0–1.0)
            double ratio = useWave
                ? yieldSlider * (0.5 + 0.5 * Math.Sin(_simAngle))
                : yieldSlider;

            // Convert to uint16 counts (0–1000)
            ushort s1 = (ushort)(ratio * 1000);
            ushort s2 = (ushort)(ratio * 950 + 10);  // slight asymmetry
            ushort moistRaw = (ushort)(moistureSlider * 10);
            ushort rpm = 200;

            byte flags = 0x07;  // all sensors OK

            byte[] packet = BuildPacket(s1, s2, moistRaw, rpm, flags);
            try
            {
                _udp.Send(packet, packet.Length, _target);
            }
            catch { }

            // Update UI labels
            lblSensor1.Text = $"S1: {ratio:F3}";
            lblSensor2.Text = $"S2: {(ratio * 0.95):F3}";
            lblMoistureVal.Text = $"Mst: {moistureSlider:F1}%";
        }

        private byte[] BuildPacket(ushort s1, ushort s2, ushort moisture, ushort rpm, byte flags)
        {
            // YieldFlo module → PC packet (13 bytes)
            // [0-1] PGN 40001 little-endian
            // [2]   packet type 0x01
            // [3-4] sensor1_count
            // [5-6] sensor2_count
            // [7-8] moisture_raw
            // [9-10] module_rpm
            // [11]  status_flags
            // [12]  CRC8 (sum of bytes 0-11)

            byte[] pkt = new byte[13];
            pkt[0] = 0x41;  // 40001 low byte
            pkt[1] = 0x9C;  // 40001 high byte
            pkt[2] = 0x01;
            pkt[3] = (byte)(s1 & 0xFF);
            pkt[4] = (byte)(s1 >> 8);
            pkt[5] = (byte)(s2 & 0xFF);
            pkt[6] = (byte)(s2 >> 8);
            pkt[7] = (byte)(moisture & 0xFF);
            pkt[8] = (byte)(moisture >> 8);
            pkt[9] = (byte)(rpm & 0xFF);
            pkt[10] = (byte)(rpm >> 8);
            pkt[11] = flags;

            // CRC: sum of bytes 0-11
            int ck = 0;
            for (int i = 0; i < 12; i++) ck += pkt[i];
            pkt[12] = (byte)ck;

            return pkt;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _sendTimer?.Stop();
            _udp?.Close();
            base.OnFormClosed(e);
        }
    }
}
