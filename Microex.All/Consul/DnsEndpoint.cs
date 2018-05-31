using System.Net;

namespace Microex.All.Consul
{
    public class DnsEndpoint
    {
        public string Address { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 8600;

        public IPEndPoint ToIPEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(Address), Port);
        }
    }
}