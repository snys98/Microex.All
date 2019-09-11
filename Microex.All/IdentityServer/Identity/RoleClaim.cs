using System;
using Microsoft.AspNetCore.Identity;

namespace Microex.All.IdentityServer.Identity
{
    public class RoleClaim : IdentityRoleClaim<string>
    {
        public new string Id { get; set; }
    }
}