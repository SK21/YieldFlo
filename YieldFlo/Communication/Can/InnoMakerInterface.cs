using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
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
    /// Binding mirrors the vendor's own demo app (usb2can repo, C#-V1.3.0):
    ///   class InnoMakerUsb2CanLib.UsbCan
    ///   scanInnoMakerDevices() → getInnoMakerDeviceCount() → getInnoMakerDevice(i)
    ///   UrbSetupDevice(device, UsbCanMode.UsbCanModeNormal, innomaker_device_bittming)
    ///   syncSendInnoMakerDeviceBuf(device, bytes, length, timeout, transferOut)
    ///   syncGetInnoMakerDeviceBuf(device, bytes, size, transferIn, timeout)
    ///   UrbResetDevice(device) + closeInnoMakerDevice(device) on close
    /// There is no explicit open call — scan opens the USB handles and
    /// UrbSetupDevice starts the CAN channel.
    ///
    /// Host frame is the marshaled innomaker_host_frame (24 bytes):
    ///   [0-3]   echo_id   uint32 LE  (0xFFFFFFFF = received frame, else TX echo)
    ///   [4-7]   can_id    uint32 LE  (bit31 = extended, bit30 = RTR, id in low 29)
    ///   [8]     can_dlc
    ///   [9]     channel
    ///   [10]    flags
    ///   [11]    reserved
    ///   [12-19] data
    ///   [20-23] timestamp_us (0 on TX)
    /// </summary>
    public class InnoMakerInterface : ICanInterface
    {
        private const int HostFrameSize = 24;
        private const uint EchoIdRxFrame = 0xFFFFFFFFu;
        private const uint CanIdExtendedFlag = 0x80000000u;
        private const uint CanIdRtrFlag = 0x40000000u;
        private const int RecvTimeoutMs = 100;
        private const int SendTimeoutMs = 10;

        private object _lib;             // UsbCan instance
        private object _device;          // InnoMakerDevice instance
        private Type _libType;
        private MethodInfo _sendMethod;
        private MethodInfo _recvMethod;

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

                Invoke("scanInnoMakerDevices");

                object count = Invoke("getInnoMakerDeviceCount");
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

                if (!SetupDevice(bitrate)) return false;

                _sendMethod = FindMethod("syncSendInnoMakerDeviceBuf") ?? FindMethod("sendInnoMakerDeviceBuf");
                _recvMethod = FindMethod("syncGetInnoMakerDeviceBuf") ?? FindMethod("recvInnoMakerDeviceBuf");
                if (_sendMethod == null || _recvMethod == null)
                {
                    Props.WriteErrorLog("InnoMakerInterface/Open: send/receive buf methods not found in SDK. " +
                        "Methods available: " + ListMethods());
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

                // The SDK class is UsbCan (namespace InnoMakerUsb2CanLib). Match by
                // name first, then fall back to whichever class carries the setup
                // method so an SDK rename doesn't break us.
                var classNames = new List<string>();
                foreach (Type t in asm.GetTypes())
                {
                    if (!t.IsClass) continue;
                    classNames.Add(t.Name);
                    if (string.Equals(t.Name, "UsbCan", StringComparison.OrdinalIgnoreCase))
                    {
                        _libType = t;
                        break;
                    }
                }
                if (_libType == null)
                {
                    foreach (Type t in asm.GetTypes())
                    {
                        if (t.IsClass && HasMethod(t, "UrbSetupDevice")) { _libType = t; break; }
                    }
                }

                if (_libType == null)
                {
                    Props.WriteErrorLog("InnoMakerInterface/Open: no UsbCan class found in DLL — wrong or " +
                        "incompatible SDK version. Classes found: " + string.Join(", ", classNames));
                    return false;
                }

                _lib = Activator.CreateInstance(_libType);
                SetNoOpDeviceDelegates();
                return true;
            }
            catch (BadImageFormatException)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: InnoMakerUsb2CanLib.dll could not be loaded " +
                    "(BadImageFormat in a " + (Environment.Is64BitProcess ? "64" : "32") + "-bit app). " +
                    "The file may be a 64-bit or corrupt download — use the 32-bit DLL from the SDK's " +
                    "Lib folder, downloaded as a raw binary (not a GitHub web page).");
                return false;
            }
        }

        /// <summary>
        /// The SDK invokes addDeviceDelegate/removeDeviceDelegate on USB hot-plug.
        /// The demo always assigns them; install no-ops so a null delegate can't
        /// crash the SDK's notification thread when an adapter is unplugged.
        /// </summary>
        private void SetNoOpDeviceDelegates()
        {
            foreach (string name in new[] { "addDeviceDelegate", "removeDeviceDelegate" })
            {
                try
                {
                    FieldInfo f = _libType.GetField(name, BindingFlags.Public | BindingFlags.Instance |
                                                          BindingFlags.IgnoreCase);
                    if (f == null || !typeof(Delegate).IsAssignableFrom(f.FieldType)) continue;

                    MethodInfo sig = f.FieldType.GetMethod("Invoke");
                    if (sig == null || sig.ReturnType != typeof(void)) continue;

                    ParameterExpression[] pars = Array.ConvertAll(sig.GetParameters(),
                        p => Expression.Parameter(p.ParameterType, p.Name));
                    f.SetValue(_lib, Expression.Lambda(f.FieldType, Expression.Empty(), pars).Compile());
                }
                catch { }
            }
        }

        /// <summary>
        /// Call UrbSetupDevice(device, UsbCanModeNormal, bittiming). The mode and
        /// bit-timing types are taken from the method's own parameter list so the
        /// exact SDK type names don't matter.
        /// </summary>
        private bool SetupDevice(int bitrate)
        {
            MethodInfo setup = FindMethod("UrbSetupDevice");
            if (setup == null)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: SDK method UrbSetupDevice not found. " +
                    "Methods available: " + ListMethods());
                return false;
            }

            ParameterInfo[] pars = setup.GetParameters();
            if (pars.Length != 3)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: unexpected UrbSetupDevice signature (" +
                    pars.Length + " parameters).");
                return false;
            }

            // Bit-timing values straight from the vendor demo's bitrate table
            // (48 MHz clock, 16 tq per bit). sync(1) + prop + seg1 + seg2 = 16.
            int propSeg, phaseSeg1, phaseSeg2, brp;
            switch (bitrate)
            {
                case 250000:  propSeg = 6; phaseSeg1 = 7; phaseSeg2 = 2; brp = 12; break;
                case 1000000: propSeg = 5; phaseSeg1 = 6; phaseSeg2 = 4; brp = 3;  break;
                case 500000:  propSeg = 5; phaseSeg1 = 6; phaseSeg2 = 4; brp = 6;  break;
                case 125000:  propSeg = 5; phaseSeg1 = 6; phaseSeg2 = 4; brp = 24; break;
                case 100000:  propSeg = 5; phaseSeg1 = 6; phaseSeg2 = 4; brp = 30; break;
                default:
                    propSeg = 6; phaseSeg1 = 7; phaseSeg2 = 2;
                    brp = 48000000 / (bitrate * 16);
                    if (brp < 1 || brp * bitrate * 16 != 48000000)
                    {
                        Props.WriteErrorLog("InnoMakerInterface/Open: unsupported bitrate " + bitrate + ".");
                        return false;
                    }
                    break;
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
            if (!SetNumericField(bt, "prop_seg", propSeg) |
                !SetNumericField(bt, "phase_seg1", phaseSeg1) |
                !SetNumericField(bt, "phase_seg2", phaseSeg2) |
                !SetNumericField(bt, "sjw", 1) |
                !SetNumericField(bt, "brp", brp))
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: bit-timing fields not found on " + btType.Name + ".");
                return false;
            }

            object result = setup.Invoke(_lib, new object[] { _device, mode, bt });
            if (result is bool ok && !ok)
            {
                Props.WriteErrorLog("InnoMakerInterface/Open: UrbSetupDevice failed (bitrate " + bitrate + "). " +
                    "Rescan/replug the adapter, or it may be in use by another program.");
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
                    InvokeOptional("UrbResetDevice", _device);
                    InvokeOptional("closeInnoMakerDevice", _device);
                    _device = null;
                }
            }
            catch { }
        }

        public bool Send(CanFrame frame)
        {
            if (!_open || _sendMethod == null) return false;
            try
            {
                byte[] buf = new byte[HostFrameSize];
                // echo_id 0 — the device echoes TX frames back with this id; the
                // receive loop drops anything whose echo_id is not 0xFFFFFFFF.
                uint canId = frame.Id & 0x1FFFFFFFu;
                if (frame.IsExtended) canId |= CanIdExtendedFlag;
                buf[4] = (byte)(canId & 0xFF);
                buf[5] = (byte)((canId >> 8) & 0xFF);
                buf[6] = (byte)((canId >> 16) & 0xFF);
                buf[7] = (byte)((canId >> 24) & 0xFF);
                buf[8] = frame.Dlc;
                if (frame.Data != null)
                    Array.Copy(frame.Data, 0, buf, 12, Math.Min(frame.Dlc, frame.Data.Length));
                // timestamp_us [20-23] stays 0 on TX

                object result = InvokeBufMethod(_sendMethod, buf, SendTimeoutMs);
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
                    object result = InvokeBufMethod(_recvMethod, buf, RecvTimeoutMs);
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

        /// <summary>
        /// Invoke a send/get buf method, adapting to its arity:
        ///   5 params: (device, buf, length, x, y) where the demo passes
        ///     send: (device, buf, len, timeout, transferOut)
        ///     recv: (device, buf, size, transferIn, timeout)
        ///   4 params: (device, buf, length, timeout)
        ///   3 params: (device, buf, length)
        /// Trailing int/byref parameters beyond the known ones get 0.
        /// </summary>
        private object InvokeBufMethod(MethodInfo m, byte[] buf, int timeoutMs)
        {
            ParameterInfo[] pars = m.GetParameters();
            object[] args = new object[pars.Length];
            for (int i = 0; i < pars.Length; i++)
            {
                Type pt = pars[i].ParameterType;
                if (pt.IsByRef) pt = pt.GetElementType();

                object v;
                if (i == 0) v = _device;
                else if (i == 1) v = buf;
                else if (i == 2) v = buf.Length;
                else v = 0;

                // Demo signatures: on send the timeout is the 4th parameter, on
                // recv it is the 5th (last). Put the timeout in the last int slot;
                // for the 5-param send that slot is transferOut, so timeout goes
                // in slot 3 instead — matching the demo's (…, 10, transferOut).
                if (pars.Length == 4 && i == 3) v = timeoutMs;
                if (pars.Length == 5 && i == (IsRecv(m) ? 4 : 3)) v = timeoutMs;

                args[i] = ConvertArg(v, pt);
            }
            return m.Invoke(_lib, args);
        }

        private static bool IsRecv(MethodInfo m)
        {
            string n = m.Name.ToLowerInvariant();
            return n.Contains("get") || n.Contains("recv");
        }

        private static object ConvertArg(object v, Type pt)
        {
            if (v != null && pt.IsPrimitive && v.GetType() != pt)
                return Convert.ChangeType(v, pt);
            return v;
        }

        private bool HasMethod(Type t, string name)
        {
            foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                if (string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private MethodInfo FindMethod(string name)
        {
            if (_libType == null) return null;
            foreach (MethodInfo m in _libType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                if (string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase))
                    return m;
            return null;
        }

        private string ListMethods()
        {
            if (_libType == null) return "(no type)";
            var names = new List<string>();
            foreach (MethodInfo m in _libType.GetMethods(BindingFlags.Public | BindingFlags.Instance |
                                                         BindingFlags.DeclaredOnly))
                names.Add(m.Name);
            return string.Join(", ", names);
        }

        /// <summary>Call a required SDK method; throws if it does not exist.</summary>
        private object Invoke(string name, params object[] args)
        {
            MethodInfo m = FindMethod(name);
            if (m == null)
                throw new MissingMethodException("SDK method " + name + " not found. " +
                    "Methods available: " + ListMethods());
            ParameterInfo[] pars = m.GetParameters();
            object[] converted = new object[args.Length];
            for (int i = 0; i < args.Length && i < pars.Length; i++)
                converted[i] = ConvertArg(args[i], pars[i].ParameterType);
            return m.Invoke(_lib, converted);
        }

        /// <summary>Call an SDK method if it exists; returns null otherwise.</summary>
        private object InvokeOptional(string name, params object[] args)
        {
            return FindMethod(name) == null ? null : Invoke(name, args);
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
