using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using IdentityServer4.Models;

using Microex.All.Common;
using Microex.All.IdentityServer;
using Microex.All.IdentityServer.Identity;
using Microex.All.IdentityServer.PredefinedConfigurations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
        public static void EnsurePredefinedIdentityServerConfigs<TContext, TUser>(this IApplicationBuilder host, Action<TContext> seedAction = null) where TContext : IdentityServerDbContext<TUser> where TUser : User, new()
        {
            var services = host.ApplicationServices;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            try
            {
                var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TContext>();
                //只在本区间内有效
                context.Database.Migrate();
                context.EnsureIdentityServerSeedData<TUser>(
                    new[] { ClientPredefinedConfiguration.AdminManageClient },
                    ResourcePredefinedConfiguration.IdentityResources,
                    new ApiResource[] { },
                    IdentityPredefinedConfiguration<TUser>.UserRoles);
                seedAction?.Invoke(context);
                context.SaveChanges();
                logger.LogInformation($"AutoMigrateDbContext {typeof(TContext).Name} 执行成功");
                throw new Exception("自动创建数据库失败.");
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "数据库自动migration失败");
                throw;
            }
        }

        /// <summary>
        /// 用于自动迁移dbcontext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IWebHost EnsureMigrations<TContext>(this IWebHost host, Action<TContext> seedAction = null) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {//只在本区间内有效
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try
                {

                    var context = services.GetRequiredService<TContext>();
                    context.Database.Migrate();
                    seedAction?.Invoke(context);
                    logger.LogInformation($"AutoMigrateDbContext {typeof(TContext).Name} 执行成功");
                }
                catch (Exception e)
                {
                    logger.LogCritical(e, "数据库自动migration失败");
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
        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection serviceCollection) where TContext : IntegratedDbContext
        {
            //单例的uow,保证事务完整性
            serviceCollection.AddSingleton<UnitOfWork<TContext>>();
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

        public static async Task<PagedList<TEntity>> ToPagedListAsync<TEntity>(this IQueryable<TEntity> rawData, Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, IComparable>> orderBy = null, int page = 1, int pageSize = 10, bool orderByDescending = true) where TEntity : class, IEntity
        {
            var pagedList = new PagedList<TEntity>();
            IQueryable<TEntity> query = rawData;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            pagedList.TotalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = query.PageBy(orderBy, page, pageSize, orderByDescending);

            }
            List<TEntity> data = await query
                .ToListAsync();
            pagedList.Data.AddRange(data);
            pagedList.PageSize = pageSize;
            return pagedList;
        }
    }
}
