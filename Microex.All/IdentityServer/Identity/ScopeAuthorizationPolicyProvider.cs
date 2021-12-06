using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4.Extensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;

namespace Microex.All.IdentityServer.Identity
{
    public class ScopeAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        /// <summary>
        /// Creates a new instance of <see cref="T:Microsoft.AspNetCore.Authorization.DefaultAuthorizationPolicyProvider" />.
        /// </summary>
        /// <param name="options">The options used to configure this instance.</param>
        public ScopeAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        /// <summary>
        /// Gets a <see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationPolicy" /> from the given <paramref name="policyName" />
        /// </summary>
        /// <param name="policyName">The policy name to retrieve.</param>
        /// <returns>The named <see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationPolicy" />.</returns>
        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            return new AuthorizationPolicy(new[] { new AssertionRequirement(x =>
            {
                return x.User.IsAuthenticated()&& x.User.Claims.FirstOrDefault(x=>x.Type == JwtClaimTypes.Scope).Value.Split(' ').Contains(policyName);
            }) }, new[] { JwtBearerDefaults.AuthenticationScheme });
        }
    }
}
