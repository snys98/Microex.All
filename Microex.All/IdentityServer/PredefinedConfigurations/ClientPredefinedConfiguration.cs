using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Microex.All.IdentityServer.PredefinedConfigurations

{
    public static class ClientPredefinedConfiguration
    {
        public static Client AdminManageClient => new Client
        {
            ClientId = nameof(AdminManageClient),
            ClientName = nameof(AdminManageClient),
            AllowedGrantTypes = GrantTypes.Hybrid,

            AllowAccessTokensViaBrowser = true,
            RequireConsent = false,
            ClientSecrets =
                {
                    new Secret(nameof(AdminManageClient).Sha256())
                },

            RedirectUris = { "*" },
            PostLogoutRedirectUris = { "*" },
            AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    JwtClaimTypes.Role
                },
        };
    }
}