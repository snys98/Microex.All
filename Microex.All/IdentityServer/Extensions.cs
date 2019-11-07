using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microex.All.IdentityServer.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;

namespace Microex.All.IdentityServer
{
    public static class Extensions
    {
        public static void EnsureIdentityServerSeedData<TUser>(this IdentityServerDbContext<TUser> context,
            IEnumerable<Client> clients, IEnumerable<IdentityResource> identityResources,
            IEnumerable<ApiResource> apiResources,
            IEnumerable<(TUser user, Role role)> identityConfigs) where TUser : GeexUser 
        {
            if (!(context as IConfigurationDbContext).Clients.Any())
            {
                foreach (var client in clients)
                {
                    (context as IConfigurationDbContext).Clients.Add(client.ToEntity());
                }
            }

            if (!(context as IConfigurationDbContext).IdentityResources.Any())
            {

                foreach (var resource in identityResources)
                {
                    (context as IConfigurationDbContext).IdentityResources.Add(resource.ToEntity());
                }
            }

            if (!(context as IConfigurationDbContext).ApiResources.Any())
            {
                foreach (var resource in apiResources)
                {
                    (context as IConfigurationDbContext).ApiResources.Add(resource.ToEntity());
                }
            }

            if (!context.Users.Any())
            {
                foreach (var (user, role) in identityConfigs.DistinctBy(x => x.user))
                {
                    var userEntry = context.Users.Add(user);
                    context.SaveChanges();
                    user.Id = userEntry.Entity.Id;
                }

                foreach (var (user, role) in identityConfigs.DistinctBy(x => x.role))
                {
                    var roleEntry = context.Roles.Add(role);
                    context.SaveChanges();
                    role.Id = roleEntry.Entity.Id;
                }
                context.UserRoles.AddRange(identityConfigs.Select(x => new UserRole() { RoleId = x.role.Id, UserId = x.user.Id }));
                //foreach (var pair in identityConfigs)
                //{

                //    context.Users.Add(user);
                //}
                //foreach (var role in identityConfigs.roles)
                //{
                //    context.Roles.Add(role);
                //}
                //foreach (var userRole in identityConfigs.userRoles)
                //{
                //    context.UserRoles.Add(userRole);
                //}
            }
            context.SaveChanges();
        }
    }
}
