using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microex.All.Consul
{
    internal class HealthCheckMiddleware:IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            return context.Response.WriteAsync("OK");
        }
    }
}
