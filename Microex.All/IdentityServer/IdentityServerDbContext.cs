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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserClaim = Microex.All.IdentityServer.Identity.UserClaim;

namespace Microex.All.IdentityServer
{

    public class IdentityServerDbContext : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>,
        IConfigurationDbContext, IPersistedGrantDbContext
    {
        protected readonly ConfigurationStoreOptions _configurationStoreOptions = new ConfigurationStoreOptions();
        protected readonly OperationalStoreOptions _operationalStoreOptions = new OperationalStoreOptions();

        public IdentityServerDbContext(DbContextOptions options) : base(options)
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
            //base.OnModelCreating(builder);
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
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

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

            var storeOptions = this.GetService<IDbContextOptions>()
                            .Extensions.OfType<CoreOptionsExtension>()
                            .FirstOrDefault()?.ApplicationServiceProvider
                            ?.GetService<IOptions<IdentityOptions>>()
                            ?.Value?.Stores;
            var maxKeyLength = storeOptions?.MaxLengthForKeys ?? 0;
            var encryptPersonalData = storeOptions?.ProtectPersonalData ?? false;
            ValueConverter<string,string> converter = null;

            builder.Entity<User>(b =>
                {
                    b.HasKey(u => u.Id);
                    b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();
                    b.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");
                    b.ToTable("Users");
                    b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                    b.Property(u => u.UserName).HasMaxLength(256);
                    b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                    b.Property(u => u.Email).HasMaxLength(256);
                    b.Property(u => u.NormalizedEmail).HasMaxLength(256);

                    if (encryptPersonalData)
                    {
                        var protector = this.GetService<IPersonalDataProtector>();
                        converter = new ValueConverter<string,string>(s => protector.Protect(s), s => protector.Unprotect(s), default);
                        var personalDataProps = typeof(User).GetProperties().Where(
                                    prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
                        foreach (var p in personalDataProps)
                        {
                            if (p.PropertyType != typeof(string))
                            {
                                throw new InvalidOperationException("ProtectedPersonalData only works strings by default.");
                            }
                            b.Property(typeof(string), p.Name).HasConversion(converter);
                        }
                    }

                    b.HasMany<UserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
                    b.HasMany<UserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
                    b.HasMany<UserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();
                });

            builder.Entity<UserClaim>(b =>
            {
                b.HasKey(uc => uc.Id);
                b.ToTable("UserClaims");
            });

            builder.Entity<UserLogin>(b =>
            {
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

                if (maxKeyLength > 0)
                {
                    b.Property(l => l.LoginProvider).HasMaxLength(maxKeyLength);
                    b.Property(l => l.ProviderKey).HasMaxLength(maxKeyLength);
                }

                b.ToTable("UserLogins");
            });

            builder.Entity<UserToken>(b =>
            {
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

                if (maxKeyLength > 0)
                {
                    b.Property(t => t.LoginProvider).HasMaxLength(maxKeyLength);
                    b.Property(t => t.Name).HasMaxLength(maxKeyLength);
                }

                if (encryptPersonalData)
                {
                    var tokenProps = typeof(UserToken).GetProperties().Where(
                                    prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
                    foreach (var p in tokenProps)
                    {
                        if (p.PropertyType != typeof(string))
                        {
                            throw new InvalidOperationException("ProtectedPersonalData only works strings by default.");
                        }
                        b.Property(typeof(string), p.Name).HasConversion(converter);
                    }
                }

                b.ToTable("UserTokens");
            });
            builder.Entity<User>(b =>
            {
                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
            });

            builder.Entity<Role>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();
                b.ToTable("Roles");
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany<RoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            builder.Entity<RoleClaim>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("RoleClaims");
            });

            builder.Entity<UserRole>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                b.ToTable("UserRoles");
            });
        }

    }
}

