using System;
using Microsoft.AspNetCore.Identity;

namespace Microex.All.IdentityServer.Identity
{
    public class UserClaim : IdentityUserClaim<string>
    {
        public new string Id { get; set; }
    }
}
