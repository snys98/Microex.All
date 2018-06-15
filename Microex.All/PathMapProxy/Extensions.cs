using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microex.All.Common;
using Microex.All.RestHttpClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microex.All.PathMappedProxy
{
    public static class Extensions
    {
        /// <summary>
        /// 添加更便于使用的resthttpclient
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddPathMapProxy(this IServiceCollection services, Action<PathMapProxyOptions> optionsAction)
        {
            PathMapProxyOptions options = new PathMapProxyOptions();
            optionsAction.Invoke(options);
            services.AddSingleton(options);
            services.AddSingleton<PathMapProxyMiddleware>();
            return services;
        }
    }
}
