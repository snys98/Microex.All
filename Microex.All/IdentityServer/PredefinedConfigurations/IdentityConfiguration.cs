using System.Collections.Generic;
using Microex.All.IdentityServer.Identity;
using Microsoft.AspNetCore.Identity;

namespace Microex.All.IdentityServer.PredefinedConfigurations
{
    public static class IdentityConfiguration
    {
        public const string AdministrationPolicy = "RequireAdministratorRole";
        public const string AdministrationRoleName = "Administrator";
        public static User Administrator
        {
            get
            {
                var admin = new User
                {
                    UserName = "admin",
                    NormalizedUserName = "admin",
                    SecurityStamp = "0",
                    ConcurrencyStamp = "0"
                };
                var password = new PasswordHasher<User>().HashPassword(admin, "admin");
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
        public static IEnumerable<User> Users => new[] { Administrator };
        public static IEnumerable<Role> Roles => new[] { AdministratorRole };
        public static IEnumerable<(User user, Role role)> UserRoles => new [] { (Administrator, AdministratorRole),  };
    }
}
