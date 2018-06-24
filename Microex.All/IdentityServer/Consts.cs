using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microex.All.Common;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using ClientModel = IdentityServer4.Models.Client;
using SecretModel = IdentityServer4.Models.Secret;
using Secret = IdentityServer4.EntityFramework.Entities.Secret;
using IdentityServer4.EntityFramework.Entities;

namespace Microex.All.IdentityServer
{
    public class IdentityServerConsts : ConstValue<string>
    {
        protected IdentityServerConsts()
        {
        }

        protected IdentityServerConsts(string value, string name) : base(value, name)
        {
        }

        public class IdentityRoleConsts : ConstValue<string>
        {
            protected IdentityRoleConsts()
            {
            }

            protected IdentityRoleConsts(string value, string name) : base(value, name)
            {
            }

            public static IdentityRoleConsts Admin { get; set; } = new IdentityRoleConsts(nameof(Admin), nameof(Admin));
        }
    }
}
