namespace Microex.All.Consul
{
    public class ConsulOptions
    {
        public string HttpEndpoint { get; set; } = "http://127.0.0.1:8500";

        public DnsEndpoint DnsEndpoint { get; set; } = new DnsEndpoint()
        {
            Address = "127.0.0.1",
            Port = 8600
        };
    }
}