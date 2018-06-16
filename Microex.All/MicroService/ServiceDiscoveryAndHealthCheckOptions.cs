using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace Microex.All.MicroService
{
    public class ServiceDiscoveryAndHealthCheckOptions
    {
        public string ServiceName { get; set; }
        public string Port { get; set; } = "80";
        public HttpScheme Schema { get; set; } = HttpScheme.Http;
        public ConsulOptions ConsulOptions { get; set; } = new ConsulOptions();

        public HealthCheckOptions HealthCheckOptions { get; set; } = new HealthCheckOptions();
        
    }
}
