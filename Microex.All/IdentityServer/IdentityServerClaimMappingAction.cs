using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;

namespace Microex.All.IdentityServer
{
    public class IdentityServerClaimMappingAction: ClaimAction
    {
        public IdentityServerClaimMappingAction() : base("All", "http://www.w3.org/2001/XMLSchema#string")
        {
            
        }
        public IdentityServerClaimMappingAction(string claimType, string valueType) : base(claimType, valueType)
        {
        }

        public override void Run(JObject userData, ClaimsIdentity identity, string issuer)
        {
            if (userData == null)
                return;
            foreach (KeyValuePair<string, JToken> keyValuePair in userData)
            {
                
                KeyValuePair<string, JToken> pair = keyValuePair;
                JToken jtoken;
                string claimValue = userData.TryGetValue(pair.Key, out jtoken) ? jtoken.ToString() : (string)null;
                if (JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.TryGetValue(keyValuePair.Key, out var indentityClaimKey) &&
                    !identity.HasClaim(indentityClaimKey, claimValue))
                {
                    identity.AddClaim(new Claim(indentityClaimKey, claimValue, "http://www.w3.org/2001/XMLSchema#string", issuer));
                }
                if (identity.FindFirst((Predicate<Claim>)(c =>
                {
                    if (string.Equals(c.Type, pair.Key, StringComparison.OrdinalIgnoreCase))
                        return string.Equals(c.Value, claimValue, StringComparison.Ordinal);
                    return false;
                })) == null)
                    identity.AddClaim(new Claim(pair.Key, claimValue, "http://www.w3.org/2001/XMLSchema#string", issuer));
            }
        }
    }
}
