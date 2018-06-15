using System.Net;

namespace Microex.All.MicroService
{
    public class DnsEndpoint
    {
        public string Address { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 8600;

        public static explicit operator IPEndPoint(DnsEndpoint dnsEndpoint)
        {
            return new IPEndPoint(IPAddress.Parse(dnsEndpoint.Address), dnsEndpoint.Port);
        }
    }
}