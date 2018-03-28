using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microex.AngularSpa.Extensions
{
    public static class Startup
    {
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
