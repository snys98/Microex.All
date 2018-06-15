using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microex.All.UEditor
{
    public static class Extensions
    {
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
        public static IApplicationBuilder UseUEditor(this IApplicationBuilder builder)
        {
            var options = builder.ApplicationServices.GetRequiredService<UEditorOptions>();
            builder.Map($@"/{options.EndPoint.TrimStart('/')}", (x) =>
            {
                x.UseMiddleware<UEditorMiddleware>();
            });
            return builder;
        }
    }
}
