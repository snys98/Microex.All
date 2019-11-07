using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microex.All.IdentityServer.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microex.All.IdentityServer
{
    public class MicroexUserManager<TUser> : UserManager<TUser> where TUser : GeexUser
    {
        private IServiceProvider _services;

        public MicroexUserManager(IUserStore<TUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<MicroexUserManager<TUser>> logger) : base(store,
            optionsAccessor,
            passwordHasher,
            userValidators,
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger)
        {
            this._services = services;
        }
        public async Task<GeexUser> FindByPhoneNumberAsync(string phone)
        {
            this.ThrowIfDisposed();
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));
            GeexUser byEmailAsync = await (this.Users).FirstOrDefaultAsync((Expression<Func<GeexUser, bool>>)(u => u.PhoneNumber == phone), CancellationToken.None);
            if ((object)byEmailAsync == null && this.Options.Stores.ProtectPersonalData)
            {
                ILookupProtectorKeyRing service = this._services.GetService<ILookupProtectorKeyRing>();
                ILookupProtector protector = this._services.GetService<ILookupProtector>();
                if (service != null && protector != null)
                {
                    foreach (string allKeyId in service.GetAllKeyIds())
                    {
                        
                        byEmailAsync = await (this.Users).FirstOrDefaultAsync((Expression<Func<GeexUser, bool>>)(u => u.PhoneNumber == protector.Protect(allKeyId, phone)), CancellationToken.None);
                        if ((object)byEmailAsync != null)
                            return byEmailAsync;
                    }
                }
                protector = (ILookupProtector)null;
            }
            return byEmailAsync;
        }

        private IUserPhoneNumberStore<GeexUser> GetPhoneNumberStore()
        {
            IUserPhoneNumberStore<GeexUser> store = this.Store as IUserPhoneNumberStore<GeexUser>;
            if (store != null)
                return store;
            throw new NotSupportedException("StoreNotIUserPhoneNumberStore");
        }
    }
}
