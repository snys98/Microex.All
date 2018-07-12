using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Hosting;

namespace Microex.All.IdentityServer
{
    public class RegexRedirectUriValidator:StrictRedirectUriValidator
    {
        private readonly IHostingEnvironment _env;

        private string Wildcard2Regex(string wildcardPattern)
        {
            return wildcardPattern.Replace(".", "\\.").Replace("*", ".+");
        }

        public RegexRedirectUriValidator(IHostingEnvironment env)
        {
            _env = env;
        }

        public override Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (_env.IsDevelopment())
            {
                return Task.FromResult(true);
            }

            var regexs = client.RedirectUris.Select(x=>new Regex(x));
            return Task.FromResult(regexs.All(x=>x.IsMatch(requestedUri)));
        }

        public override Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (client.RedirectUris.Any(x=>x == "*"))
            {
                return Task.FromResult(true);
            }
            if (_env.IsDevelopment())
            {
                return Task.FromResult(true);
            }
            var host = new Uri(requestedUri).Host;
            return Task.FromResult(client.RedirectUris.Any(x => new Regex($"^{Wildcard2Regex(x)}$").IsMatch(host)));
        }
    }
}
