using System;
using Microsoft.AspNetCore.Identity;

namespace Microex.All.IdentityServer.Identity
{
    public class User : IdentityUser
    {
        public User()
        {
            Id = SequentialGuid.SequentialGuidGenerator.Instance.NewGuid(DateTime.Now).ToString();
        }
    }
}