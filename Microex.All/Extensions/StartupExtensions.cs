using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Consul;
using Microex.All.AliyunOss;
using Microex.All.Consul;
using Microex.All.UEditor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microex.All.Extensions
{
    public static class StartupExtensions
    {
        #region Consul
        /// <summary>
        /// 添加Consul的服务发现相关配置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsulServiceDiscovery(this IServiceCollection services, Action<ServiceDiscoveryOptions> optionsAction)
        {
            var options = new ServiceDiscoveryOptions();
            optionsAction.Invoke(options);
            services.AddTransient(_=> options);
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                if (!string.IsNullOrEmpty(options.ConsulOptions.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(options.ConsulOptions.HttpEndpoint);
                }
            }));
            return services;
        }

        /// <summary>
        /// 添加Consul的注册以及健康检查到pipeline
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="discoveryOptions"></param>
        /// <param name="consulClient"></param>
        /// <param name="applicationLifetime"></param>
        /// <returns></returns>
        public static IApplicationBuilder AddConsulServiceDiscoveryAndHealthCheck(this IApplicationBuilder builder,
            ServiceDiscoveryOptions discoveryOptions,
            IConsulClient consulClient,
            IApplicationLifetime applicationLifetime)
        {
            var features = builder.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{discoveryOptions.ServiceName}_{address.Host}:{address.Port}";

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "HealthCheck").OriginalString
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    Address = address.Host,
                    ID = serviceId,
                    Name = discoveryOptions.ServiceName,
                    Port = address.Port
                };

                consulClient.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                applicationLifetime.ApplicationStopping.Register(() =>
                {
                    consulClient.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                });
            }

            builder.Map("/HealthCheck", (x) => { x.UseMiddleware<HealthCheckMiddleware>(); });

            return builder;
        }
        #endregion

        #region 阿里云oss
        /// <summary>
        /// config aliyun oss
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddAliyunOss(this IServiceCollection builder, Action<AliyunOssOptions> optionsAction)
        {
            var options = new AliyunOssOptions();
            optionsAction.Invoke(options);
            builder.AddSingleton<AliyunOssClient>((provider => new AliyunOssClient(options)));
            return builder;
        }
        /// <summary>
        /// add aliyun oss middlewares to the pipe line
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder AddAliyunOss(this IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetRequiredService<AliyunOssOptions>();
            builder.Map($@"/{options.LocalEndPoint.TrimStart('/')}", (x) =>
            {
                x.UseMiddleware<AliyunOssFileUploadMiddleware>();
            });
            return builder;
        }
        #endregion

        #region UEditor
        /// <summary>
        /// config ueditor
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddUEditor(this IServiceCollection builder, Action<UEditorOptions> optionsAction)
        {
            var options = new UEditorOptions();
            optionsAction.Invoke(options);
            builder.AddSingleton(options);
            return builder;
        }
        /// <summary>
        /// add ueditor middlewares to the pipe line
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder AddUEditor(this IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetRequiredService<UEditorOptions>();
            builder.Map($@"/{options.EndPoint.TrimStart('/')}", (x) =>
            {
                x.UseMiddleware<UEditorMiddleware>();
            });
            return builder;
        }
        #endregion

        /// <summary>
        /// Config angular4 spa pipe line in one line of code, default serve path is \wwwroot\index.html
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configServeStatic">true for excute <code>builder.UseStaticFiles();</code></param>
        /// <param name="wwwrootPath"></param>
        /// <param name="angularIndexName"></param>
        /// <returns></returns>
        public static IApplicationBuilder AddAngularRoute(this IApplicationBuilder builder, bool configServeStatic = true, string wwwrootPath = @"\wwwroot", string angularIndexName = "index.html")
        {
            if (configServeStatic)
            {
                builder.UseStaticFiles();
            }

            builder.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine(wwwrootPath, angularIndexName));
            });
            return builder;
        }

        /// <summary>
        /// Use a preffered json format setting(eg: format date at yyyy-MM-dd HH:mm:ss & ignore loop reference & so on)
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddPrefferedJsonSettings(this IMvcBuilder builder)
        {
            builder.AddJsonOptions(jsonOptions =>
            {
                //jsonOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //jsonOptions.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                //jsonOptions.SerializerSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
                //jsonOptions.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //jsonOptions.SerializerSettings.Error = (sender, args) =>
                //{
                //    args.ErrorContext.Handled = true;
                //};
                //jsonOptions.SerializerSettings.Converters = (IList<JsonConverter>)new List<JsonConverter>()
                //{
                //    (JsonConverter) new StringEnumConverter()
                //};
                foreach (var propertyInfo in jsonOptions.SerializerSettings.GetType().GetProperties(BindingFlags.Public | BindingFlags.SetProperty))
                {
                    var value = propertyInfo.GetValue(Json.DefaultSerializeSettings);
                    if (value != null)
                    {
                        propertyInfo.SetValue(jsonOptions, value);
                    }
                }
            });
            return builder;
        }
        /// <summary>
        /// 用于自动迁移dbcontext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="host"></param>
        /// <param name="seedAction"></param>
        /// <returns></returns>
        public static IWebHost MigrateDbContext<TContext>(this IWebHost host, Action<TContext, IServiceProvider> seedAction) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {//只在本区间内有效
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    context.Database.Migrate();
                    seedAction(context, services);

                    logger.LogInformation($"执行DBContext {typeof(TContext).Name} seed执行成功");
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, $"执行DBContext {typeof(TContext).Name} seed方法失败");
                }
            }

            return host;
        }
    }
}
