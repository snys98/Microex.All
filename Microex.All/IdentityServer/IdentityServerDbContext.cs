using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microex.All.IdentityServer.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using UserClaim = Microex.All.IdentityServer.Identity.UserClaim;

namespace Microex.All.IdentityServer
{
    public class IdentityServerDbContext: IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>,
        IConfigurationDbContext, IPersistedGrantDbContext
    {
        protected readonly ConfigurationStoreOptions _configurationStoreOptions = new ConfigurationStoreOptions();
        protected readonly OperationalStoreOptions _operationalStoreOptions = new OperationalStoreOptions();

        public IdentityServerDbContext(DbContextOptions options):base(options)
        {
            
        }
        public IdentityServerDbContext(DbContextOptions options,
            ConfigurationStoreOptions configurationStoreOptions,
            OperationalStoreOptions operationalStoreOptions) : base(options)
        {
            _configurationStoreOptions = configurationStoreOptions;
            _operationalStoreOptions = operationalStoreOptions;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // lulus:配置客户端和资源相关表
            base.OnModelCreating(builder);
            ConfigureIdentityContext(builder);
            builder.ConfigureClientContext(_configurationStoreOptions);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions);
            builder.ConfigureResourcesContext(_configurationStoreOptions);
            //// lulus:配置授权相关表
            //builder.ConfigurePersistedGrantEntity();
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        public DbSet<Client> Clients { get; set; }
        public DbSet<IdentityResource> IdentityResources { get; set; }
        public DbSet<ApiResource> ApiResources { get; set; }
        public DbSet<ApiSecret> ApiSecrets { get; set; }

        public DbSet<ApiScope> ApiScopes { get; set; }

        public DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

        public DbSet<IdentityClaim> IdentityClaims { get; set; }

        public DbSet<ApiResourceClaim> ApiResourceClaims { get; set; }
        public DbSet<ClientGrantType> ClientGrantTypes { get; set; }

        public DbSet<ClientScope> ClientScopes { get; set; }

        public DbSet<ClientSecret> ClientSecrets { get; set; }

        public DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }

        public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }

        public DbSet<ClientIdPRestriction> ClientIdPRestrictions { get; set; }

        public DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }

        public DbSet<ClientClaim> ClientClaims { get; set; }

        public DbSet<ClientProperty> ClientProperties { get; set; }

        private void ConfigureIdentityContext(ModelBuilder builder)
        {
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<RoleClaim>().ToTable("RoleClaims");
            builder.Entity<UserRole>().ToTable("UserRoles");

            builder.Entity<User>().ToTable("Users");
            builder.Entity<UserLogin>().ToTable("UserLogins");
            builder.Entity<UserClaim>().ToTable("UserClaims");
            builder.Entity<UserToken>().ToTable("UserTokens");
        }

    }
}

