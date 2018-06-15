using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace Microex.All.IdentityServer
{
    public class WechatGrantValidator:IExtensionGrantValidator
    {
        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            throw new NotImplementedException();
        }

        public string GrantType { get; } = "wechat";
    }
}
