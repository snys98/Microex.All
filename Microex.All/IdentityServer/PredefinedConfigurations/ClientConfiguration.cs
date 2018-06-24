using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Microex.All.IdentityServer.PredefinedConfigurations

{
    public static class ClientConfiguration
    {
        public static Client LocalServer => new Client
        {
            ClientId = nameof(LocalServer),
            ClientName = nameof(LocalServer),
            AllowedGrantTypes = GrantTypes.Hybrid,

            AllowAccessTokensViaBrowser = true,
            RequireConsent = false,
            ClientSecrets =
                {
                    new Secret(nameof(LocalServer).Sha256())
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