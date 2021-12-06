using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;

using Microex.All.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Microex.All.IdentityServer
{
    public class GeexProfileService<TUser> : ProfileService<TUser> where TUser : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:IdentityServer4.AspNetIdentity.ProfileService`1" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        public GeexProfileService(UserManager<TUser> userManager, IUserClaimsPrincipalFactory<TUser> claimsFactory) : base(userManager, claimsFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:IdentityServer4.AspNetIdentity.ProfileService`1" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        /// <param name="logger">The logger.</param>
        public GeexProfileService(UserManager<TUser> userManager, IUserClaimsPrincipalFactory<TUser> claimsFactory, ILogger<ProfileService<TUser>> logger) : base(userManager, claimsFactory, logger)
        {
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await base.GetProfileDataAsync(context);
            var claims = new List<Claim>
            {
                new Claim("tenants", "1 2"),
                new Claim("current_tenant", "1"),
                new Claim("roles", "admin user"),
            };
            context.IssuedClaims.AddRange(claims);
        }

        ///// <summary>Gets the claims for a user.</summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //protected override async Task<ClaimsPrincipal> GetUserClaimsAsync(TUser user)
        //{
        //    var claimsPrincipal = await base.GetUserClaimsAsync(user);

        //    return new ClaimsPrincipal(new ClaimsIdentity(claimsPrincipal.Claims.Concat(new[]
        //    {
        //        new Claim("tenants", "1 2"),
        //        new Claim("current_tenant", "1"),
        //        new Claim("roles", "admin user"),
        //    })));
        //}
    }
}
