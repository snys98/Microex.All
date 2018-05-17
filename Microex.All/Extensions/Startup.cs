using System;
using System.IO;
using System.Reflection;
using Microex.All.AliyunOss;
using Microex.All.AliyunSls;
using Microex.All.UEditor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microex.All.Extensions
{
    public static class Startup
    {
        public static IWebHostBuilder UseMicroexHostConfig(this IWebHostBuilder builder)
        {
            return builder.CaptureStartupErrors(true);
        }

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
                    logger.LogError(ex, $"执行DBContext {typeof(TContext).Name} seed方法失败");
                }
            }

            return host;
        }

        /// <summary>
        /// add microex middlewares to the pipe line
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IApplicationBuilder AddMicroexAll(this IApplicationBuilder builder)
        {
            var options = MicroexOptions.Instance;
            if (options.AliyunOssOptions != null)
            {
                var ossOptions = options.AliyunOssOptions;
                builder.Map($@"/{ossOptions.LocalEndPoint.TrimStart('/')}", (x) =>
                {
                    x.UseMiddleware<AliyunOssFileUploadMiddleware>();
                });
            }
            if (options.UEditorOptions != null)
            {
                var ueditorOptions = options.UEditorOptions;
                builder.Map($@"/{ueditorOptions.EndPoint.TrimStart('/')}", (x) =>
                {
                    x.UseMiddleware<UEditorMiddleware>();
                });
            }
            return builder;
        }

        /// <summary>
        /// add microex middlewares to the pipe line
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddMicroexAll(this IServiceCollection builder, Action<MicroexOptions> optionsAction)
        {
            var options = MicroexOptions.Instance;
            optionsAction.Invoke(options);
            builder.AddSingleton<MicroexOptions>((provider => options));
            builder.AddSingleton<AliyunOssClient>((provider => new AliyunOssClient(options.AliyunOssOptions)));
            builder.AddSingleton<UEditorOptions>((provider => options.UEditorOptions));
            if (options.AliyunSlsOptions != null)
            {
                var slsOptions = options.AliyunSlsOptions;
                builder.AddLogging((loggingBuilder) => { loggingBuilder.AddProvider(new AliyunSlsLoggingProvider(options.AliyunSlsOptions,slsOptions.MinLevel)); });
            }
            return builder;
        }

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
    }
}
