using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microex.All.IdentityServer.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microex.All.IdentityServer
{
    public class MicroexSignInManager<TUser> : SignInManager<TUser> where TUser : class
    {
        public override ILogger Logger { get => base.Logger; set => base.Logger = value; }

        public override Task<bool> CanSignInAsync(TUser user)
        {
            return base.CanSignInAsync(user);
        }

        public override Task<SignInResult> CheckPasswordSignInAsync(TUser user, string password, bool lockoutOnFailure)
        {
            return base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        }

        public override AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            return base.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId);
        }

        public override Task<ClaimsPrincipal> CreateUserPrincipalAsync(TUser user)
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

        public override Task<TUser> GetTwoFactorAuthenticationUserAsync()
        {
            return base.GetTwoFactorAuthenticationUserAsync();
        }

        public override bool IsSignedIn(ClaimsPrincipal principal)
        {
            return base.IsSignedIn(principal);
        }

        public override Task<bool> IsTwoFactorClientRememberedAsync(TUser user)
        {
            return base.IsTwoFactorClientRememberedAsync(user);
        }

        public override Task<SignInResult> PasswordSignInAsync(TUser user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }

        public override Task RefreshSignInAsync(TUser user)
        {
            return base.RefreshSignInAsync(user);
        }

        public override Task RememberTwoFactorClientAsync(TUser user)
        {
            return base.RememberTwoFactorClientAsync(user);
        }

        public override Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null)
        {
            return base.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public override Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
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

        public override Task<TUser> ValidateSecurityStampAsync(ClaimsPrincipal principal)
        {
            return base.ValidateSecurityStampAsync(principal);
        }

        public override Task<bool> ValidateSecurityStampAsync(TUser user, string securityStamp)
        {
            return base.ValidateSecurityStampAsync(user, securityStamp);
        }

        public override Task<TUser> ValidateTwoFactorSecurityStampAsync(ClaimsPrincipal principal)
        {
            return base.ValidateTwoFactorSecurityStampAsync(principal);
        }

        protected override Task<bool> IsLockedOut(TUser user)
        {
            return base.IsLockedOut(user);
        }

        protected override Task<SignInResult> LockedOut(TUser user)
        {
            return base.LockedOut(user);
        }

        protected override Task<SignInResult> PreSignInCheck(TUser user)
        {
            return base.PreSignInCheck(user);
        }

        protected override Task ResetLockout(TUser user)
        {
            return base.ResetLockout(user);
        }

        protected override Task<SignInResult> SignInOrTwoFactorAsync(TUser user, bool isPersistent, string loginProvider = null, bool bypassTwoFactor = false)
        {
            return base.SignInOrTwoFactorAsync(user, isPersistent, loginProvider, bypassTwoFactor);
        }

        public MicroexSignInManager(UserManager<TUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<TUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<TUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<TUser> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }
    }
}
