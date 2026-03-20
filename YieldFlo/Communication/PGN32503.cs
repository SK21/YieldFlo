using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using YieldFlo.Classes;

namespace YieldFlo.Communication
{
    /// <summary>
    /// PGN32503 — Subnet change broadcast.
    /// Sends the new subnet (first 3 IP octets) to all interfaces
    /// so the module can reconfigure its network endpoint.
    /// Byte layout:
    ///   [0] Header Lo  = 247
    ///   [1] Header Hi  = 126
    ///   [2] IP octet 0
    ///   [3] IP octet 1
    ///   [4] IP octet 2
    ///   [5] CRC
    /// </summary>
    public class PGN32503
    {
        private readonly byte[] cData = new byte[6];

        public PGN32503()
        {
            cData[0] = 247;
            cData[1] = 126;
        }

        /// <summary>
        /// Broadcast the new subnet to 255.255.255.255:28888 on every active interface.
        /// Returns true if the IP parsed successfully and the send was attempted.
        /// </summary>
        public bool Send(string ep)
        {
            if (!IPAddress.TryParse(ep, out _)) return false;

            string[] parts = ep.Split('.');
            cData[2] = byte.Parse(parts[0]);
            cData[3] = byte.Parse(parts[1]);
            cData[4] = byte.Parse(parts[2]);
            cData[5] = Core.Tls.CRC(cData, 5);

            IPEndPoint dest = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 28888);

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (!nic.Supports(NetworkInterfaceComponent.IPv4)) continue;
                if (nic.OperationalStatus != OperationalStatus.Up) continue;

                foreach (UnicastIPAddressInformation info in nic.GetIPProperties().UnicastAddresses)
                {
                    if (info.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                    if (IPAddress.IsLoopback(info.Address)) continue;
                    if (info.IPv4Mask == null) continue;

                    try
                    {
                        using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                        {
                            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast,   true);
                            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute,    true);
                            sock.Bind(new IPEndPoint(info.Address, 9578));
                            sock.SendTo(cData, 0, cData.Length, SocketFlags.None, dest);
                        }
                    }
                    catch (Exception ex)
                    {
                        Props.WriteErrorLog("PGN32503/Send: " + ex.Message);
                    }
                }
            }

            return true;
        }
    }
}
