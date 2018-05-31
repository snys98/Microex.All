using System;
using System.Collections.Generic;
using System.Text;

namespace Microex.All.Consul
{
    public class ServiceDiscoveryOptions
    {
        public string ServiceName { get; set; }

        public ConsulOptions ConsulOptions { get; set; }
    }
}
