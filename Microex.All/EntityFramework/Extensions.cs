using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microex.All.IdentityServer;
using Microex.All.IdentityServer.Identity;
using Microex.All.IdentityServer.PredefinedConfigurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Microex.All.EntityFramework
{
    public static class Extensions
    {
        /// <summary>
        /// 用于自动迁移dbcontext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IWebHost EnsurePredefinedIdentityServerConfigs<TContext>(this IWebHost host) where TContext : IdentityServerDbContext
        {
            using (var scope = host.Services.CreateScope())
            {//只在本区间内有效
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try
                {
                    
                    var context = services.GetRequiredService<TContext>();
                    context.Database.Migrate();
                    context.EnsureIdentityServerSeedData(new[] { ClientConfiguration.LocalServer },
                        ResourceConfiguration.IdentityResources,
                        ResourceConfiguration.ApiResources,
                        IdentityConfiguration.UserRoles);
                    logger.LogInformation($"AutoMigrateDbContext {typeof(TContext).Name} 执行成功");
                }
                catch (Exception e)
                {
                    logger.LogCritical(e,"数据库自动migration失败");
                    throw;
                }
                
            }

            return host;
        }

        /// <summary>
        /// 自动添加包含符合标准主键的表到Repository中
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepository<TContext>(this IServiceCollection serviceCollection) where TContext : DbContext,IUnitOfWork
        {
            var dbSetsPropertyInfos = typeof(TContext).GetProperties().Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).ToList();
            foreach (var table in dbSetsPropertyInfos)
            {
                var entityType = table.PropertyType.GenericTypeArguments[0];
                var contextType = typeof(TContext);
                var keyType = entityType?.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(KeyAttribute), true).Any() || x.Name == "Id" || x.Name == $"{x.Name}Id")?.PropertyType;
                if (keyType == null)
                {
                    continue;
                }
                serviceCollection.TryAddScoped(
                    typeof(IRepository<>).MakeGenericType(table.PropertyType.GenericTypeArguments), typeof(BasicRepository<,,>).MakeGenericType(entityType, contextType, keyType));

            }

            return serviceCollection;
        }
        public static IQueryable<T> PageBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> orderBy, int page, int pageSize, bool orderByDescending = true)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            // It is necessary sort items before it
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Configures IEntity.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        public static void ConfigIEntities<TContext>(this ModelBuilder modelBuilder) where TContext : DbContext
        {
            var dbSets = typeof(TContext).GetProperties()
                .Where(x => x.PropertyType.IsGenericType &&
                            x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                            x.PropertyType.GenericTypeArguments[0].GetInterface(nameof(IEntity)) != null)
                .ToList();

            void ConfigIEntity(EntityTypeBuilder options)
            {
                options.HasKey(nameof(IEntity.Id));
                options.Property<string>(nameof(IEntity.Id)).HasValueGenerator<PrettyStringGuidGenerator>();
                options.Property<DateTime>(nameof(IEntity.CreateTime)).ValueGeneratedOnAdd();
                options.Property<DateTime>(nameof(IEntity.LastModifyTime)).ValueGeneratedOnUpdate();
            }

            foreach (var table in dbSets)
            {
                var entityType = table.PropertyType.GenericTypeArguments[0];
                modelBuilder.Entity(entityType, ConfigIEntity);
            }
        }
    }
}
