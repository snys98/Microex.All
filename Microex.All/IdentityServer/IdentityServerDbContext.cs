using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace Microex.All.IdentityServer
{
    public class IdentityServerDbContext<TUser>: IdentityDbContext<TUser>, IConfigurationDbContext, IPersistedGrantDbContext where TUser : IdentityUser
    {
        protected readonly ConfigurationStoreOptions _configurationStoreOptions;
        protected readonly OperationalStoreOptions _operationalStoreOptions;

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
            builder.ConfigureClientContext(_configurationStoreOptions);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions);
            builder.ConfigureResourcesContext(_configurationStoreOptions);
            //// lulus:配置授权相关表
            //builder.ConfigurePersistedGrantEntity();
            base.OnModelCreating(builder);
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

    }
}

