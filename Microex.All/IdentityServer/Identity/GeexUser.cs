using System;
using Microsoft.AspNetCore.Identity;

namespace Microex.All.IdentityServer.Identity
{
    public class GeexUser : IdentityUser
    {
        public GeexUser()
        {
            Id = SequentialGuid.SequentialGuidGenerator.Instance.NewGuid(DateTime.Now).ToString();
        }
    }
}