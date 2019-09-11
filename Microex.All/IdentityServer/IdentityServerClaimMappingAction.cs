using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;

namespace Microex.All.IdentityServer
{
    public class IdentityServerClaimMappingAction : ClaimAction
    {
        public IdentityServerClaimMappingAction() : base("All", "http://www.w3.org/2001/XMLSchema#string")
        {

        }
        public IdentityServerClaimMappingAction(string claimType, string valueType) : base(claimType, valueType)
        {
        }

        public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
        {
            if (Equals(userData, default))
                return;
            foreach (var pair in userData.EnumerateObject())
            {

                string claimValue = userData.TryGetProperty(pair.Name, out var jtoken) ? jtoken.ToString() : (string)null;
                if (JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.TryGetValue(pair.Name, out var indentityClaimKey) &&
                    !identity.HasClaim(indentityClaimKey, claimValue))
                {
                    identity.AddClaim(new Claim(indentityClaimKey, claimValue, "http://www.w3.org/2001/XMLSchema#string", issuer));
                }
                if (identity.FindFirst((Predicate<Claim>)(c =>
                {
                    if (string.Equals(c.Type, pair.Name, StringComparison.OrdinalIgnoreCase))
                        return string.Equals(c.Value, claimValue, StringComparison.Ordinal);
                    return false;
                })) == null)
                    identity.AddClaim(new Claim(pair.Name, claimValue, "http://www.w3.org/2001/XMLSchema#string", issuer));
            }
        }
    }
}
