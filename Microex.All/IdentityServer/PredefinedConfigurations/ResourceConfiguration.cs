using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace Microex.All.IdentityServer.PredefinedConfigurations
{
    public static class ResourceConfiguration
    {
        public static IEnumerable<IdentityResource> IdentityResources=> new[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Phone(),
            new IdentityResource()
            {
                Name = JwtClaimTypes.Role,
                DisplayName = "Your role",
                Emphasize = true,
                UserClaims = {JwtClaimTypes.Role},
            }
        };

        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]{};
    }
}