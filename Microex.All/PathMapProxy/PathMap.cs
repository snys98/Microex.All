using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microex.All.PathMapProxy
{
    public class PathMap
    {
        public string Path { get; set; }

        public ProxyAddress ProxyAddress { get; set; }
    }

    public class ProxyAddress
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
    }
}
