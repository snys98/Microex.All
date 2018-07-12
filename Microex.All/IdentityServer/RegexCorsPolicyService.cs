using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Microex.All.IdentityServer
{
    public class RegexCorsPolicyService: DefaultCorsPolicyService
    {
        private readonly IHostingEnvironment _env;

        public RegexCorsPolicyService(ILogger<DefaultCorsPolicyService> logger,IHostingEnvironment env) : base(logger)
        {
            _env = env;
        }

        public override async Task<bool> IsOriginAllowedAsync(string origin)
        {
            if (_env.IsDevelopment())
            {
                return true;
            }
            var regexs = base.AllowedOrigins.Select(x => new Regex(x));
            return regexs.All(x => x.IsMatch(origin)) || await base.IsOriginAllowedAsync(origin);
        }
    }
}
