using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microex.All.IdentityServer
{
    public class MicroexSignInManager<T> : SignInManager<T> where T: class 
    {
        public override ILogger Logger { get => base.Logger; set => base.Logger = value; }

        public override Task<bool> CanSignInAsync(T user)
        {
            return base.CanSignInAsync(user);
        }

        public override Task<SignInResult> CheckPasswordSignInAsync(T user, string password, bool lockoutOnFailure)
        {
            return base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        }

        public override AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            return base.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId);
        }

        public override Task<ClaimsPrincipal> CreateUserPrincipalAsync(T user)
        {
            return base.CreateUserPrincipalAsync(user);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            return base.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);
        }

        public override Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
        {
            return base.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);
        }

        public override Task ForgetTwoFactorClientAsync()
        {
            return base.ForgetTwoFactorClientAsync();
        }

        public override Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            return base.GetExternalAuthenticationSchemesAsync();
        }

        public override Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            return base.GetExternalLoginInfoAsync(expectedXsrf);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override Task<T> GetTwoFactorAuthenticationUserAsync()
        {
            return base.GetTwoFactorAuthenticationUserAsync();
        }

        public override bool IsSignedIn(ClaimsPrincipal principal)
        {
            return base.IsSignedIn(principal);
        }

        public override Task<bool> IsTwoFactorClientRememberedAsync(T user)
        {
            return base.IsTwoFactorClientRememberedAsync(user);
        }

        public override Task<SignInResult> PasswordSignInAsync(T user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }

        public override Task RefreshSignInAsync(T user)
        {
            return base.RefreshSignInAsync(user);
        }

        public override Task RememberTwoFactorClientAsync(T user)
        {
            return base.RememberTwoFactorClientAsync(user);
        }

        public override Task SignInAsync(T user, bool isPersistent, string authenticationMethod = null)
        {
            return base.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public override Task SignInAsync(T user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            return base.SignInAsync(user, authenticationProperties, authenticationMethod);
        }

        public override Task SignOutAsync()
        {
            return base.SignOutAsync();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient)
        {
            return base.TwoFactorAuthenticatorSignInAsync(code, isPersistent, rememberClient);
        }

        public override Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
        {
            return base.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
        }

        public override Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient)
        {
            return base.TwoFactorSignInAsync(provider, code, isPersistent, rememberClient);
        }

        public override Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo externalLogin)
        {
            return base.UpdateExternalAuthenticationTokensAsync(externalLogin);
        }

        public override Task<T> ValidateSecurityStampAsync(ClaimsPrincipal principal)
        {
            return base.ValidateSecurityStampAsync(principal);
        }

        public override Task<bool> ValidateSecurityStampAsync(T user, string securityStamp)
        {
            return base.ValidateSecurityStampAsync(user, securityStamp);
        }

        public override Task<T> ValidateTwoFactorSecurityStampAsync(ClaimsPrincipal principal)
        {
            return base.ValidateTwoFactorSecurityStampAsync(principal);
        }

        protected override Task<bool> IsLockedOut(T user)
        {
            return base.IsLockedOut(user);
        }

        protected override Task<SignInResult> LockedOut(T user)
        {
            return base.LockedOut(user);
        }

        protected override Task<SignInResult> PreSignInCheck(T user)
        {
            return base.PreSignInCheck(user);
        }

        protected override Task ResetLockout(T user)
        {
            return base.ResetLockout(user);
        }

        protected override Task<SignInResult> SignInOrTwoFactorAsync(T user, bool isPersistent, string loginProvider = null, bool bypassTwoFactor = false)
        {
            return base.SignInOrTwoFactorAsync(user, isPersistent, loginProvider, bypassTwoFactor);
        }

        public MicroexSignInManager(UserManager<T> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<T> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<T>> logger, IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }
    }
}
