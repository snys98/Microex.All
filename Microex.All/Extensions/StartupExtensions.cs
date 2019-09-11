using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Consul;
using Microex.All.ElasticSearch.Zero.Logging.Elasticsearch;
using Microex.All.MicroService;
using Microex.All.RestHttpClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microex.All.Extensions
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Config angular4 spa pipe line in one line of code, default serve path is \wwwroot\index.html
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configServeStatic">true for excute <code>builder.UseStaticFiles();</code></param>
        /// <param name="wwwrootPath"></param>
        /// <param name="angularIndexName"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAngularRoute(this IApplicationBuilder builder, bool configServeStatic = true, string wwwrootPath = @"\wwwroot", string angularIndexName = "index.html")
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
        public static IMvcBuilder UsePrefferedJsonSettings(this IMvcBuilder builder)
        {
            builder.AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            return builder;
        }
        
        /// <summary>
        /// 添加更便于使用的resthttpclient
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddRestHttpClient(this IServiceCollection services,Action<ResilientPolicyOptions> optionsAction = null)
        {
            if (optionsAction != null)
            {
                var option = new ResilientPolicyOptions();
                optionsAction.Invoke(option);
                services.AddSingleton<IRestHttpClient, ResilientRestHttpClient>(_services=> new ResilientRestHttpClient(_services.GetRequiredService<ILogger<ResilientRestHttpClient>>(),option));
            }
            else
            {
                services.AddSingleton<IRestHttpClient, StandardRestHttpClient>();
            }
            return services;
        }
        public static IServiceCollection AddMicroService(this IServiceCollection serviceCollection,string serviceName = null)
        {
            serviceCollection.AddConsulServiceDiscoveryAndHealthCheck();

            return serviceCollection;
        }

        public static IApplicationBuilder UseMicroService(this IApplicationBuilder appBuilder,string iisExternalPort = "80")
        {
            
            appBuilder.UseConsulServiceDiscoveryAndHealthCheck(
                appBuilder.ApplicationServices.GetService<ServiceDiscoveryAndHealthCheckOptions>(),
                appBuilder.ApplicationServices.GetService<IConsulClient>(),
                appBuilder.ApplicationServices.GetService<IHostApplicationLifetime>(),
                appBuilder.ApplicationServices.GetService<ILogger<IStartup>>(),
                appBuilder.ApplicationServices.GetService<IHostEnvironment>(),
                iisExternalPort);
            return appBuilder;
        }

        /// <summary>
        /// Configures <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> to use Application Insights services.
        /// </summary>
        /// <param name="webHostBuilder">The <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> instance.</param>
        /// <returns>The <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" />.</returns>
        public static IWebHostBuilder ConfigEsLogging(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureLogging((context, builder) =>
            {
                if (!context.HostingEnvironment.IsDevelopment())
                {
                    builder.AddElasticsearch(); 
                }
            });

            return webHostBuilder;
        }

    }
}
