namespace Microex.All.MicroService
{
    public class ConsulOptions
    {
        /// <summary>
        /// 默认值为localhost:8500
        /// </summary>
        public string HttpEndpoint { get; set; } = "http://localhost:8500";
        /// <summary>
        /// 默认值为localhost:8600
        /// </summary>
        public DnsEndpoint DnsEndpoint { get; set; } = new DnsEndpoint()
        {
            Address = "127.0.0.1",
            Port = 8600
        };
    }
}