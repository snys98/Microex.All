using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Microex.All.MicroService
{
    internal class HealthCheckMiddleware:IMiddleware
    {
        private readonly ILogger<HealthCheckMiddleware> _logger;

        public HealthCheckMiddleware(ILogger<HealthCheckMiddleware> logger)
        {
            _logger = logger;
        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _logger.LogInformation("HealthCheck OK!");
            return context.Response.WriteAsync("OK");
        }
    }
}
