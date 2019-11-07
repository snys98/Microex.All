using System.Collections.Generic;
using Microex.All.IdentityServer.Identity;
using Microsoft.AspNetCore.Identity;

namespace Microex.All.IdentityServer.PredefinedConfigurations
{
    public static class IdentityPredefinedConfiguration<TUser> where TUser : GeexUser, new()
    {
        public const string AdministrationPolicy = "RequireAdministratorRole";
        public const string AdministrationRoleName = "Administrator";
        public static TUser Administrator
        {
            get
            {
                var admin = new TUser()
                {
                    UserName = "admin",
                    NormalizedUserName = "admin",
                    SecurityStamp = "0",
                    ConcurrencyStamp = "0"
                };
                var password = new PasswordHasher<TUser>().HashPassword(admin, "admin");
                admin.PasswordHash = password;
                return admin;
            }
        }

        public static Role AdministratorRole =>
            new Role()
            {
                ConcurrencyStamp = "0",
                Name = AdministrationRoleName,
                NormalizedName = AdministrationRoleName.ToUpper()
            };
        public static IEnumerable<TUser> Users => new[] { Administrator };
        public static IEnumerable<Role> Roles => new[] { AdministratorRole };
        public static IEnumerable<(TUser user, Role role)> UserRoles => new[] { (Administrator, AdministratorRole), };
    }
}
