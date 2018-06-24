using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Microex.All.Common
{
    public static class Extensions
    {
        public static PagedList<TTarget> Cast<T, TTarget>(this PagedList<T> @this, Func<T, TTarget> changeExpression) where TTarget : class where T : class
        {
            var result = new PagedList<TTarget>()
            {
                PageSize = @this.PageSize,
                TotalCount = @this.TotalCount
            };
            result.Data.AddRange(@this.Data.Select(changeExpression).ToArray());
            return result;
        }

        public static IPAddress GetLocalIPv4(this IEnumerable<IPAddress> addressList)
        {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == NetworkInterfaceType.Ethernet && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address;
                        }
                    }
                }
            }
            return null;
        }
    }
}
