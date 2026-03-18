using System;
using System.Net;
using System.Net.Sockets;
using YieldFlo.Classes;
using YieldFlo.Forms;

namespace YieldFlo.Communication
{
    // YieldFlo UDP port assignments (different from RC to allow both apps to run simultaneously)
    //   AOG GPS:       recv 17777  send 15555   (same as RC — both apps share via SO_REUSEADDR)
    //   Module data:   recv 30100  send 30200   (unique to YieldFlo)

    public class UDPComm
    {
        private readonly frmMain mf;
        private byte[] buffer = new byte[1024];
        private string cConnectionName;
        private IPAddress cNetworkEP;
        private int cReceivePort;
        private int cSendFromPort;
        private int cSendToPort;
        private HandleDataDelegateObj HandleDataDelegate = null;
        private Socket recvSocket;
        private volatile bool Running = false;
        private Socket sendSocket;

        public UDPComm(frmMain CallingForm, int ReceivePort, int SendToPort, int SendFromPort,
                       string ConnectionName, string DestinationEndPoint = "")
        {
            mf = CallingForm;
            cReceivePort = ReceivePort;
            cSendToPort = SendToPort;
            cSendFromPort = SendFromPort;
            cConnectionName = ConnectionName;
            SetEP(DestinationEndPoint);
        }

        private delegate void HandleDataDelegateObj(int port, byte[] msg);

        public bool IsRunning { get { return Running; } }

        public string NetworkEP
        {
            get { return cNetworkEP?.ToString() ?? ""; }
            set
            {
                if (IPAddress.TryParse(value, out _))
                {
                    string[] parts = value.Split('.');
                    cNetworkEP = IPAddress.Parse(parts[0] + "." + parts[1] + "." + parts[2] + ".255");
                }
            }
        }

        public void Send(byte[] byteData)
        {
            if (!Running || sendSocket == null || byteData == null || byteData.Length == 0) return;
            try
            {
                IPEndPoint endPt = new IPEndPoint(cNetworkEP, cSendToPort);
                sendSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None,
                    endPt, new AsyncCallback(HandleSend), null);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("UDPComm/Send " + ex.Message);
            }
        }

        public void Start()
        {
            try
            {
                HandleDataDelegate = HandleData;

                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                recvSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                recvSocket.Bind(new IPEndPoint(IPAddress.Any, cReceivePort));

                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sendSocket.Bind(new IPEndPoint(IPAddress.Any, cSendFromPort));

                EndPoint client = new IPEndPoint(IPAddress.Any, 0);
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                    ref client, new AsyncCallback(Receive), recvSocket);

                Running = true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("UDPComm/Start " + ex.Message);
            }
        }

        public void Stop()
        {
            if (!Running) return;
            Running = false;
            try { recvSocket?.Close(); } catch { }
            try { sendSocket?.Close(); } catch { }
            recvSocket = null;
            sendSocket = null;
        }

        // ── Packet dispatch ───────────────────────────────────────────────────
        private void HandleData(int port, byte[] data)
        {
            try
            {
                if (data.Length < 2 || Core.IsShuttingDown) return;

                int pgn = data[1] << 8 | data[0];

                switch (pgn)
                {
                    // ── AOG GPS packets (PGN 33152 = 0x8180) ─────────────────
                    case 33152:
                        if (data.Length > 4)
                        {
                            switch (data[3])
                            {
                                case 100:   // Corrected position (lat/lon doubles; speed not present)
                                case 208:   // Dual GPS / TwoL (lat/lon/speed/elevation doubles)
                                    Core.GPS.ParseByteData(data, data[3]);
                                    Core.RaiseGpsUpdated();

                                    // Feed GPS update to data collector
                                    // moisture is supplied by the module (stored in Core later)
                                    Core.Collector?.OnGpsUpdate(Core.LastMoisture);
                                    break;

                                case 229:   // Section control — bytes 5-12 are 64 section bits
                                    if (data.Length >= 13)
                                    {
                                        bool any = false;
                                        for (int i = 5; i <= 12; i++)
                                            if (data[i] != 0) { any = true; break; }
                                        Core.GPS.SectionsActive = any;
                                    }
                                    break;

                                case 254:   // AutoSteer data — carries speed when not using TwoL
                                    // Bytes 5-6: speed * 10 (uint16, km/h)
                                    if (data.Length >= 7)
                                        Core.GPS.ParseSpeedPgn254(data);
                                    break;
                            }
                        }
                        break;

                    // ── YieldFlo module sensor data (PGN 40001) ───────────────
                    case 40001:
                        ParseModulePacket(data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("UDPComm/HandleData " + ex.Message);
            }
        }

        private void ParseModulePacket(byte[] data)
        {
            // Module → PC packet (13 bytes):
            // [0-1] PGN 40001 little-endian
            // [2]   packet type = 0x01
            // [3-4] sensor1_count uint16
            // [5-6] sensor2_count uint16
            // [7-8] moisture_raw  uint16 (value * 10 = tenths of percent)
            // [9-10] module_rpm   uint16
            // [11]  status_flags
            // [12]  CRC8
            if (data.Length < 13) return;
            if (!Core.Tls.GoodCRC(data)) return;

            ushort s1 = BitConverter.ToUInt16(data, 3);
            ushort s2 = BitConverter.ToUInt16(data, 5);
            ushort moistureRaw = BitConverter.ToUInt16(data, 7);
            byte flags = data[11];

            bool s1Ok = (flags & 0x01) != 0;
            bool s2Ok = (flags & 0x02) != 0;
            bool moistureOk = (flags & 0x04) != 0;

            double sensor1 = s1Ok ? s1 / 1000.0 : 0;
            double sensor2 = s2Ok ? s2 / 1000.0 : 0;
            double moisture = moistureOk ? moistureRaw / 10.0 : 0;

            Core.LastMoisture = moisture;
            Core.LastSensor1 = sensor1;
            Core.LastSensor2 = sensor2;
            Core.ModuleConnected = true;
            Core.LastModuleReceive = DateTime.UtcNow;

            Core.Yield?.PushSensorReading(sensor1, sensor2);
        }

        // ── Socket callbacks ──────────────────────────────────────────────────
        private void HandleSend(IAsyncResult asyncResult)
        {
            try { sendSocket?.EndSend(asyncResult); }
            catch (Exception ex) { Props.WriteErrorLog("UDPComm/HandleSend " + ex.Message); }
        }

        private void Receive(IAsyncResult asyncResult)
        {
            if (!Running) return;
            try
            {
                EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);
                int msgLen = recvSocket.EndReceiveFrom(asyncResult, ref epSender);
                byte[] localMsg = null;
                int port = 0;

                if (msgLen > 0)
                {
                    localMsg = new byte[msgLen];
                    Array.Copy(buffer, localMsg, msgLen);
                    port = ((IPEndPoint)epSender).Port;
                }

                // Re-arm listener
                try
                {
                    if (Running && recvSocket != null)
                    {
                        EndPoint nextSender = new IPEndPoint(IPAddress.Any, 0);
                        recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                            ref nextSender, new AsyncCallback(Receive), recvSocket);
                    }
                }
                catch (ObjectDisposedException) { }

                // Marshal to UI thread
                if (Running && !Core.IsShuttingDown && msgLen > 0 && HandleDataDelegate != null
                    && mf != null && mf.IsHandleCreated && !mf.IsDisposed)
                {
                    try { mf.BeginInvoke(HandleDataDelegate, new object[] { port, localMsg }); }
                    catch (InvalidOperationException) { }
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex) { Props.WriteErrorLog("UDPComm/Receive " + ex.Message); }
        }

        private void SetEP(string dest)
        {
            try
            {
                if (IPAddress.TryParse(dest, out _))
                    NetworkEP = dest;
                else
                    NetworkEP = Properties.Settings.Default.NetworkEndPoint;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("UDPComm/SetEP " + ex.Message);
                NetworkEP = "192.168.1.255";
            }
        }
    }
}
