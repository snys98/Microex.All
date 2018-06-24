using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Consul;
using DnsClient;
using IdentityServer4.Extensions;
using Microex.All.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Microex.All.MicroService
{
    public static class Extensions
    {
        /// <summary>
        /// 添加Consul的服务发现相关配置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsulServiceDiscoveryAndHealthCheck(this IServiceCollection services, Action<ServiceDiscoveryAndHealthCheckOptions> optionsAction = null)
        {
            var options = new ServiceDiscoveryAndHealthCheckOptions();
            optionsAction?.Invoke(options);
            services.AddTransient(_ => options);
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                if (!string.IsNullOrEmpty(options.ConsulOptions.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(options.ConsulOptions.HttpEndpoint);
                }
            }));
            services.AddSingleton<IDnsQuery>(p => new LookupClient((IPEndPoint)options.ConsulOptions.DnsEndpoint));
            services.AddScoped<HealthCheckMiddleware>();
            return services;
        }

        /// <summary>
        /// 添加Consul的注册以及健康检查到pipeline
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <param name="consulClient"></param>
        /// <param name="applicationLifetime"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsulServiceDiscoveryAndHealthCheck(this IApplicationBuilder builder,
            ServiceDiscoveryAndHealthCheckOptions options,
            IConsulClient consulClient,
            IApplicationLifetime applicationLifetime,
            ILogger<IStartup> logger,
            IHostingEnvironment env,
            string iisExternalPort)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            var address = ipHost.AddressList.GetLocalIPv4().MapToIPv4().ToString();

            if (options.ServiceName.IsNullOrEmpty())
            {
                options.ServiceName = env.ApplicationName;
            }

            AgentServiceRegistration registration = null;
            try
            {
                var serviceId = $"{options.ServiceName}_{address}:{iisExternalPort}";

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(options.HealthCheckOptions.DeregisterTimeout),
                    Interval = TimeSpan.FromSeconds(options.HealthCheckOptions.Interval),
                    HTTP = new Uri(new Uri($"{options.Schema}://{address}:{iisExternalPort}"), "HealthCheck").OriginalString
                };

                registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    Address = address,
                    ID = serviceId,
                    Name = options.ServiceName,
                    Port = int.Parse(iisExternalPort),
                };

                consulClient.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                applicationLifetime.ApplicationStopping.Register(() =>
                {
                    consulClient.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                });
            }
            catch (Exception e)
            {
                if (!env.IsDevelopment())
                {
                    logger.LogCritical(e, "服务发现注册失败,请检查当前服务发现配置{registration}", registration);
                }
                else
                {
                    logger.LogWarning("服务注册失败");
                }
            }

            builder.Map("/HealthCheck", (x) => { x.UseMiddleware<HealthCheckMiddleware>(); });

            return builder;
        }

        /// <summary>
        /// 添加Ocelot,默认读取项目目录下的ocelot.{Environment}.json
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOcelotWithConsul(this IServiceCollection services)
        {
            services
                .AddOcelot();
            //.AddStoreOcelotConfigurationInConsul();
            return services;
        }

        /// <summary>
        /// 添加Ocelot,默认设置Consul的配置保存
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseOcelotWithConsul(this IApplicationBuilder builder)
        {
            builder.UseOcelot().Wait();
            return builder;
        }

        /// <summary>
        /// 添加ocelot默认的配置文件到依赖注入,默认为根目录下ocelot.{Environment}.json
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseOcelotConfigFiles(this IWebHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration(
                (Action<WebHostBuilderContext, IConfigurationBuilder>)((hostingContext, config) =>
                {
                    IHostingEnvironment hostingEnvironment = hostingContext.HostingEnvironment;
                    //ocelot相关配置
                    config.AddJsonFile("ocelot.json", true, true)
                        .AddJsonFile(
                            string.Format("ocelot.{0}.json", (object)hostingEnvironment.EnvironmentName), true, true);
                }));
        }
    }
}
