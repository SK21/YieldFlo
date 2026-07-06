using System;
using System.IO;
using System.Reflection;
using System.Threading;
using YieldFlo.Classes;

namespace YieldFlo.Communication.Can
{
    /// <summary>
    /// CAN interface using the InnoMaker USB2CAN adapter via the vendor's managed
    /// SDK (InnoMakerUsb2CanLib.dll + LibUsbDotNet.dll, both copied to the app
    /// directory). The SDK is bound by reflection at runtime so the application
    /// builds and runs without the DLLs — Open() fails with a logged message if
    /// they are missing.
    ///
    /// The adapter speaks the gs_usb (candleLight) protocol: 20-byte host frames
    ///   [0-3]  echo_id   uint32 LE  (0xFFFFFFFF = received frame, else TX echo)
    ///   [4-7]  can_id    uint32 LE  (bit31 = extended, bit30 = RTR, id in low 29)
    ///   [8]    can_dlc
    ///   [9]    channel
    ///   [10]   flags
    ///   [11]   reserved
    ///   [12-19] data
    /// Bit timing: 48 MHz core clock, 16 tq/bit (sync=1, prop=6, seg1=7, seg2=2),
    /// so brp = 48000000 / (bitrate × 16) — 250 kbps → brp 12.
    /// </summary>
    public class InnoMakerInterface : ICanInterface
    {
        private const int HostFrameSize = 20;
        private const uint EchoIdRxFrame = 0xFFFFFFFFu;
        private const uint CanIdExtendedFlag = 0x80000000u;
        private const uint CanIdRtrFlag = 0x40000000u;
        private const int RecvTimeoutMs = 100;

        private object _lib;             // InnoMakerUsb2CanLib instance
        private object _device;          // InnoMakerDevice instance
        private Type _libType;
        private MethodInfo _sendMethod;  // sendInnoMakerDeviceBuf(device, byte[], int)
        private MethodInfo _recvMethod;  // recvInnoMakerDeviceBuf(device, byte[], int, timeout)

        private Thread _receiveThread;
        private volatile bool _running = false;
        private bool _open = false;

        public event EventHandler<CanFrameEventArgs> FrameReceived;

        public bool IsOpen => _open;

        public bool Open(string port, int bitrate)
        {
            try
            {
                if (!LoadSdk()) return false;

                InvokeOptional("setup");
                InvokeOptional("scanInnoMakerDevice");

                object count = InvokeOptional("getInnoMakerDeviceCount");
                if (count is int n && n < 1)
                {
                    Props.WriteErrorLog("InnoMakerInterface/Open: no USB2CAN adapter found. " +
                        "Check the USB connection and that the InnoMaker driver is installed.");
                    return false;
                }

                _device = Invoke("getInnoMakerDevice", 0);
                if (_device == null)
                {
                    Props.WriteErrorLog("InnoMakerInterface/Open: getInnoMakerDevice(0) returned null.");
                    return false;
                }

                object opened = Invoke("openInnoMakerDevice", _device);
                if (opened is bool ob && !ob)
                {
                    Props.WriteErrorLog("InnoMakerInterface/Open: openInnoMakerDevice failed. " +
                        "The adapter may be in use by another program.");
                    return false;
                }

                if (!SetupBitTiming(bitrate)) return false;

                _sendMethod = FindMethod("sendInnoMakerDeviceBuf");
                _recvMethod = FindMethod("recvInnoMakerDeviceBuf");
                if (_recvMethod == null)
                {
                    Props.WriteErrorLog("InnoMakerInterface/Open: SDK method recvInnoMakerDeviceBuf not found.");
                    CloseDevice();
                    return false;
                }

                _open = true;
                _running = true;
                _receiveThread = new Thread(ReceiveLoop) { IsBackground = true, Name = "InnoMakerRx" };
                _receiveThread.Start();
                return true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: " + Describe(ex));
                CloseDevice();
                return false;
            }
        }

        /// <summary>Load InnoMakerUsb2CanLib.dll from the application directory.</summary>
        private bool LoadSdk()
        {
            string dllPath = Path.Combine(Props.ApplicationFolder, "InnoMakerUsb2CanLib.dll");
            if (!File.Exists(dllPath))
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: InnoMakerUsb2CanLib.dll not found in " +
                    Props.ApplicationFolder + ". Copy InnoMakerUsb2CanLib.dll and LibUsbDotNet.dll " +
                    "from the InnoMaker USB2CAN C# SDK into the application folder.");
                return false;
            }

            try
            {
                Assembly asm = Assembly.LoadFrom(dllPath);
                foreach (Type t in asm.GetTypes())
                {
                    if (t.IsClass && t.Name.IndexOf("InnoMakerUsb2CanLib", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _libType = t;
                        break;
                    }
                }

                if (_libType == null)
                {
                    Props.WriteErrorLog("InnoMakerInterface/Open: class InnoMakerUsb2CanLib not found in DLL — " +
                        "wrong or incompatible SDK version.");
                    return false;
                }

                _lib = Activator.CreateInstance(_libType);
                return true;
            }
            catch (BadImageFormatException)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: InnoMakerUsb2CanLib.dll bitness does not match " +
                    "this application (" + (Environment.Is64BitProcess ? "64" : "32") + "-bit). " +
                    "Use the matching DLL from the SDK.");
                return false;
            }
        }

        /// <summary>
        /// Call urbSetupDevice(device, UsbCanModeNormal, bittiming). The mode and
        /// bit-timing types are taken from the method's own parameter list so the
        /// exact SDK type names don't matter.
        /// </summary>
        private bool SetupBitTiming(int bitrate)
        {
            MethodInfo setup = FindMethod("urbSetupDevice");
            if (setup == null)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: SDK method urbSetupDevice not found.");
                return false;
            }

            ParameterInfo[] pars = setup.GetParameters();
            if (pars.Length != 3)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: unexpected urbSetupDevice signature (" +
                    pars.Length + " parameters).");
                return false;
            }

            // 48 MHz clock, 16 tq per bit → brp must divide exactly (250k → 12)
            int brp = 48000000 / (bitrate * 16);
            if (brp * bitrate * 16 != 48000000 || brp < 1)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: unsupported bitrate " + bitrate + ".");
                return false;
            }

            // Mode parameter: enum member containing "Normal", or plain 0
            object mode;
            Type modeType = pars[1].ParameterType;
            if (modeType.IsEnum)
            {
                mode = Enum.ToObject(modeType, 0);
                foreach (string name in Enum.GetNames(modeType))
                {
                    if (name.IndexOf("Normal", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        mode = Enum.Parse(modeType, name);
                        break;
                    }
                }
            }
            else
            {
                mode = Convert.ChangeType(0, modeType);
            }

            // Bit-timing parameter: instantiate and fill by field name
            Type btType = pars[2].ParameterType;
            object bt = Activator.CreateInstance(btType);
            if (!SetNumericField(bt, "prop_seg", 6) |
                !SetNumericField(bt, "phase_seg1", 7) |
                !SetNumericField(bt, "phase_seg2", 2) |
                !SetNumericField(bt, "sjw", 1) |
                !SetNumericField(bt, "brp", brp))
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: bit-timing fields not found on " + btType.Name + ".");
                return false;
            }

            object result = setup.Invoke(_lib, new object[] { _device, mode, bt });
            if (result is bool ok && !ok)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: urbSetupDevice failed (bitrate " + bitrate + ").");
                return false;
            }
            return true;
        }

        public void Close()
        {
            _running = false;
            _open = false;
            try
            {
                _receiveThread?.Join(1000);
                _receiveThread = null;
                CloseDevice();
            }
            catch { }
        }

        private void CloseDevice()
        {
            if (_lib == null) return;
            try
            {
                if (_device != null)
                {
                    InvokeOptional("urbResetDevice", _device);   // stop CAN channel if the SDK has it
                    InvokeOptional("closeInnoMakerDevice", _device);
                    _device = null;
                }
                InvokeOptional("setdown");
            }
            catch { }
        }

        public bool Send(CanFrame frame)
        {
            if (!_open || _sendMethod == null) return false;
            try
            {
                byte[] buf = new byte[HostFrameSize];
                // echo_id 0 — buf[0-3] already zero
                uint canId = frame.Id & 0x1FFFFFFFu;
                if (frame.IsExtended) canId |= CanIdExtendedFlag;
                buf[4] = (byte)(canId & 0xFF);
                buf[5] = (byte)((canId >> 8) & 0xFF);
                buf[6] = (byte)((canId >> 16) & 0xFF);
                buf[7] = (byte)((canId >> 24) & 0xFF);
                buf[8] = frame.Dlc;
                if (frame.Data != null)
                    Array.Copy(frame.Data, 0, buf, 12, Math.Min(frame.Dlc, frame.Data.Length));

                object result = InvokeMethod(_sendMethod, _device, buf, HostFrameSize);
                return !(result is bool ok) || ok;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("InnoMakerInterface/Send: " + Describe(ex));
                return false;
            }
        }

        private void ReceiveLoop()
        {
            byte[] buf = new byte[HostFrameSize];
            while (_running)
            {
                try
                {
                    Array.Clear(buf, 0, buf.Length);
                    object result = InvokeMethod(_recvMethod, _device, buf, HostFrameSize, RecvTimeoutMs);
                    if (result is bool ok && !ok) continue;   // timeout — no frame
                    if (!_running) break;

                    uint echoId = BitConverter.ToUInt32(buf, 0);
                    if (echoId != EchoIdRxFrame) continue;    // echo of our own TX frame

                    uint rawId = BitConverter.ToUInt32(buf, 4);
                    if ((rawId & CanIdRtrFlag) != 0) continue;

                    byte dlc = buf[8] > 8 ? (byte)8 : buf[8];
                    byte[] data = new byte[dlc];
                    Array.Copy(buf, 12, data, 0, dlc);

                    var frame = new CanFrame
                    {
                        Id = rawId & 0x1FFFFFFFu,
                        Dlc = dlc,
                        Data = data,
                        IsExtended = (rawId & CanIdExtendedFlag) != 0
                    };
                    FrameReceived?.Invoke(this, new CanFrameEventArgs(frame));
                }
                catch (Exception ex)
                {
                    if (_running)
                    {
                        Props.WriteErrorLog("InnoMakerInterface/ReceiveLoop: " + Describe(ex));
                        Thread.Sleep(500);   // avoid log flooding if the adapter was unplugged
                    }
                }
            }
        }

        // ── Reflection helpers ───────────────────────────────────────────────

        private MethodInfo FindMethod(string name)
        {
            if (_libType == null) return null;
            foreach (MethodInfo m in _libType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                if (string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase))
                    return m;
            return null;
        }

        /// <summary>Call a required SDK method; throws if it does not exist.</summary>
        private object Invoke(string name, params object[] args)
        {
            MethodInfo m = FindMethod(name);
            if (m == null)
                throw new MissingMethodException("SDK method " + name + " not found in InnoMakerUsb2CanLib.dll.");
            return InvokeMethod(m, args);
        }

        /// <summary>Call an SDK method if it exists; returns null otherwise.</summary>
        private object InvokeOptional(string name, params object[] args)
        {
            MethodInfo m = FindMethod(name);
            return m == null ? null : InvokeMethod(m, args);
        }

        /// <summary>Invoke with basic numeric conversion so int args match uint/short parameters.</summary>
        private object InvokeMethod(MethodInfo m, params object[] args)
        {
            ParameterInfo[] pars = m.GetParameters();
            object[] converted = new object[args.Length];
            for (int i = 0; i < args.Length && i < pars.Length; i++)
            {
                Type pt = pars[i].ParameterType;
                if (args[i] != null && pt.IsPrimitive && args[i].GetType() != pt)
                    converted[i] = Convert.ChangeType(args[i], pt);
                else
                    converted[i] = args[i];
            }
            return m.Invoke(_lib, converted);
        }

        private static bool SetNumericField(object target, string name, int value)
        {
            Type t = target.GetType();
            FieldInfo f = t.GetField(name, BindingFlags.Public | BindingFlags.Instance |
                                           BindingFlags.IgnoreCase);
            if (f != null)
            {
                f.SetValue(target, Convert.ChangeType(value, f.FieldType));
                return true;
            }
            PropertyInfo p = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance |
                                                 BindingFlags.IgnoreCase);
            if (p != null && p.CanWrite)
            {
                p.SetValue(target, Convert.ChangeType(value, p.PropertyType), null);
                return true;
            }
            return false;
        }

        private static string Describe(Exception ex)
        {
            // Reflection wraps the real error in TargetInvocationException
            if (ex is TargetInvocationException tie && tie.InnerException != null)
                ex = tie.InnerException;
            return ex.GetType().Name + " - " + ex.Message;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
