using System;
using System.Runtime.InteropServices;
using System.Threading;
using YieldFlo.Classes;

namespace YieldFlo.Communication.Can
{
    /// <summary>
    /// CAN interface for Peak PCAN-USB adapters via the PCAN-Basic API.
    /// PCANBasic.dll is installed system-wide by the PEAK device driver package
    /// (32-bit copy in SysWOW64), so no files need to be copied to the app folder.
    /// The first responding USB channel (PCAN_USBBUS1–8) is used.
    /// </summary>
    public class PcanInterface : ICanInterface
    {
        // ── PCAN-Basic P/Invoke ──────────────────────────────────────────────

        private const ushort PCAN_USBBUS1 = 0x51;   // ..PCAN_USBBUS8 = 0x58

        // BTR0BTR1 codes
        private const ushort PCAN_BAUD_1M = 0x0014;
        private const ushort PCAN_BAUD_500K = 0x001C;
        private const ushort PCAN_BAUD_250K = 0x011C;
        private const ushort PCAN_BAUD_125K = 0x031C;
        private const ushort PCAN_BAUD_100K = 0x432F;

        private const uint PCAN_ERROR_OK = 0;
        private const uint PCAN_ERROR_QRCVEMPTY = 0x20;
        private const uint PCAN_ERROR_INITIALIZE = 0x40000;

        private const byte PCAN_MESSAGE_RTR = 0x01;
        private const byte PCAN_MESSAGE_EXTENDED = 0x02;
        private const byte PCAN_MESSAGE_STATUS = 0x80;

        private const byte PCAN_BUSOFF_AUTORESET = 0x07;   // TPCANParameter
        private const uint PCAN_PARAMETER_ON = 1;

        [StructLayout(LayoutKind.Sequential)]
        private struct TPCANMsg
        {
            public uint ID;
            public byte MSGTYPE;
            public byte LEN;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] DATA;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TPCANTimestamp
        {
            public uint millis;
            public ushort millis_overflow;
            public ushort micros;
        }

        [DllImport("PCANBasic.dll", EntryPoint = "CAN_Initialize")]
        private static extern uint CAN_Initialize(ushort channel, ushort btr0Btr1, byte hwType, uint ioPort, ushort interrupt);

        [DllImport("PCANBasic.dll", EntryPoint = "CAN_Uninitialize")]
        private static extern uint CAN_Uninitialize(ushort channel);

        [DllImport("PCANBasic.dll", EntryPoint = "CAN_Read")]
        private static extern uint CAN_Read(ushort channel, out TPCANMsg msg, out TPCANTimestamp timestamp);

        [DllImport("PCANBasic.dll", EntryPoint = "CAN_Write")]
        private static extern uint CAN_Write(ushort channel, ref TPCANMsg msg);

        [DllImport("PCANBasic.dll", EntryPoint = "CAN_SetValue")]
        private static extern uint CAN_SetValue(ushort channel, byte parameter, ref uint value, uint length);

        // ── State ────────────────────────────────────────────────────────────

        private ushort _channel;
        private Thread _receiveThread;
        private volatile bool _running = false;
        private bool _open = false;

        public event EventHandler<CanFrameEventArgs> FrameReceived;

        public bool IsOpen => _open;

        public bool Open(string port, int bitrate)
        {
            ushort baud;
            switch (bitrate)
            {
                case 1000000: baud = PCAN_BAUD_1M; break;
                case 500000: baud = PCAN_BAUD_500K; break;
                case 250000: baud = PCAN_BAUD_250K; break;
                case 125000: baud = PCAN_BAUD_125K; break;
                case 100000: baud = PCAN_BAUD_100K; break;
                default:
                    Props.WriteErrorLog("PcanInterface/Open: unsupported bitrate " + bitrate + ".");
                    return false;
            }

            try
            {
                // First USB channel that initializes wins (adapter can enumerate
                // on any bus number depending on which port it was plugged into).
                uint lastStatus = PCAN_ERROR_OK;
                for (ushort ch = PCAN_USBBUS1; ch <= PCAN_USBBUS1 + 7; ch++)
                {
                    uint status = CAN_Initialize(ch, baud, 0, 0, 0);
                    if (status == PCAN_ERROR_OK)
                    {
                        _channel = ch;
                        break;
                    }
                    lastStatus = status;
                }

                if (_channel == 0)
                {
                    Props.WriteErrorLog("PcanInterface/Open: no PCAN-USB adapter responded on USBBUS1-8 " +
                        "(last status 0x" + lastStatus.ToString("X") + "). Check the USB connection and " +
                        "that the PEAK device driver is installed, and that no other program has the " +
                        "adapter open at a different bitrate.");
                    return false;
                }

                // Recover from Bus Off automatically (e.g. module power-cycled mid-frame)
                uint on = PCAN_PARAMETER_ON;
                CAN_SetValue(_channel, PCAN_BUSOFF_AUTORESET, ref on, sizeof(uint));

                _open = true;
                _running = true;
                _receiveThread = new Thread(ReceiveLoop) { IsBackground = true, Name = "PcanRx" };
                _receiveThread.Start();
                return true;
            }
            catch (DllNotFoundException)
            {
                Props.WriteErrorLog("PcanInterface/Open: PCANBasic.dll not found. Install the PEAK " +
                    "device driver package (includes PCAN-Basic) from peak-system.com.");
                return false;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("PcanInterface/Open: " + ex.GetType().Name + " - " + ex.Message);
                return false;
            }
        }

        public void Close()
        {
            _running = false;
            _open = false;
            try
            {
                _receiveThread?.Join(1000);
                _receiveThread = null;
                if (_channel != 0)
                {
                    Thread.Sleep(50);   // let pending TX frames leave the queue
                    CAN_Uninitialize(_channel);
                    _channel = 0;
                }
            }
            catch { }
        }

        public bool Send(CanFrame frame)
        {
            if (!_open) return false;
            try
            {
                var msg = new TPCANMsg
                {
                    ID = frame.Id,
                    MSGTYPE = frame.IsExtended ? PCAN_MESSAGE_EXTENDED : (byte)0,
                    LEN = frame.Dlc > 8 ? (byte)8 : frame.Dlc,
                    DATA = new byte[8]
                };
                if (frame.Data != null)
                    Array.Copy(frame.Data, msg.DATA, Math.Min(msg.LEN, frame.Data.Length));

                return CAN_Write(_channel, ref msg) == PCAN_ERROR_OK;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("PcanInterface/Send: " + ex.GetType().Name + " - " + ex.Message);
                return false;
            }
        }

        private void ReceiveLoop()
        {
            while (_running)
            {
                try
                {
                    TPCANMsg msg;
                    TPCANTimestamp ts;
                    uint status = CAN_Read(_channel, out msg, out ts);

                    if (status == PCAN_ERROR_QRCVEMPTY)
                    {
                        Thread.Sleep(2);
                        continue;
                    }
                    if (status != PCAN_ERROR_OK) { Thread.Sleep(2); continue; }
                    if ((msg.MSGTYPE & (PCAN_MESSAGE_STATUS | PCAN_MESSAGE_RTR)) != 0) continue;

                    byte dlc = msg.LEN > 8 ? (byte)8 : msg.LEN;
                    byte[] data = new byte[dlc];
                    if (msg.DATA != null) Array.Copy(msg.DATA, data, dlc);

                    var frame = new CanFrame
                    {
                        Id = msg.ID,
                        Dlc = dlc,
                        Data = data,
                        IsExtended = (msg.MSGTYPE & PCAN_MESSAGE_EXTENDED) != 0
                    };
                    FrameReceived?.Invoke(this, new CanFrameEventArgs(frame));
                }
                catch (Exception ex)
                {
                    if (_running)
                    {
                        Props.WriteErrorLog("PcanInterface/ReceiveLoop: " + ex.GetType().Name + " - " + ex.Message);
                        Thread.Sleep(500);
                    }
                }
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
