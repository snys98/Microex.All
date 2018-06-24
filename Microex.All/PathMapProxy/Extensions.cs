using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microex.All.PathMapProxy
{
    public static class Extensions
    {
        /// <summary>
        /// 注入端口代理到容器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddPortProxy(this IServiceCollection services, Action<PathMapProxyOptions> optionsAction)
        {
            PathMapProxyOptions options = new PathMapProxyOptions();
            optionsAction.Invoke(options);
            services.AddSingleton(options);
            services.AddSingleton<PortProxyMiddleware>();
            return services;
        }

        /// <summary>
        /// 添加端口代理到pipeline
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IApplicationBuilder UsePortProxy(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<PortProxyMiddleware>();
            return appBuilder;
        }
    }
}
