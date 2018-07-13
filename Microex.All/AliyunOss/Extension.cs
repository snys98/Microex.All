using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microex.All.AliyunOss
{
    public static class Extension
    {
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
            builder.AddTransient<AliyunOssOptions>((_)=> options);
            builder.AddSingleton<AliyunOssClient>((provider => new AliyunOssClient(options)));
            return builder;
        }
        /// <summary>
        /// add aliyun oss middlewares to the pipe line
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAliyunOss(this IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetRequiredService<AliyunOssOptions>();
            builder.Map($@"/{options.LocalEndPoint.TrimStart('/')}", (x) =>
            {
                x.UseMiddleware<AliyunOssFileUploadMiddleware>();
            });
            return builder;
        }
    }
}
