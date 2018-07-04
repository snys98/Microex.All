using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Microex.All.IdentityServer.PredefinedConfigurations
{
    public static class ResourcePredefinedConfiguration
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

        public static ApiResource AdminManageResource => new ApiResource
        {
            Name = nameof(AdminManageResource),
            UserClaims = new List<string>(){ JwtClaimTypes.Subject,JwtClaimTypes.Role },
            DisplayName = nameof(AdminManageResource),
            Scopes = new List<Scope>()
            {
                new Scope(nameof(AdminManageResource))
            },
            ApiSecrets =
            {
                new Secret(nameof(AdminManageResource).Sha256())
            },
        };
    }
}