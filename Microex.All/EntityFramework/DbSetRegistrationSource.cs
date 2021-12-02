using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using Autofac.Builder;
using Autofac.Core;

using Microsoft.EntityFrameworkCore;

namespace Microex.All.EntityFramework
{
    public class DbSetRegistrationSource<TContext> : IRegistrationSource where TContext : DbContext
    {
        /// <summary>
        /// Retrieve registrations for an unregistered service, to be used
        /// by the container.
        /// </summary>
        /// <param name="service">The service that was requested.</param>
        /// <param name="registrationAccessor">A function that will return existing registrations for a service.</param>
        /// <returns>Registrations providing the service.</returns>
        /// <remarks>
        /// If the source is queried for service s, and it returns a component that implements both s and s', then it
        /// will not be queried again for either s or s'. This means that if the source can return other implementations
        /// of s', it should return these, plus the transitive closure of other components implementing their
        /// additional services, along with the implementation of s. It is not an error to return components
        /// that do not implement <paramref name="service" />.
        /// </remarks>
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            var swt = service as IServiceWithType;
            if (swt == null || !swt.ServiceType.IsGenericType)
                yield break;

            var def = swt.ServiceType.GetGenericTypeDefinition();
            if (def != typeof(DbSet<>))
                yield break;

            // if you have one `IDBContext` registeration you don't need the
            // foreach over the registrationAccessor(dbContextServices)

            yield return RegistrationBuilder.ForDelegate((c, p) =>
            {
                var dBContext = c.Resolve<TContext>();
                var m = dBContext.GetType().GetMethod(nameof(DbContext.Set), new Type[] { });
                var method =
                    m.MakeGenericMethod(swt.ServiceType.GetGenericArguments());
                return method.Invoke(dBContext, null);
            })
                    .As(service)
                    .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents
        {
            get { return true; }
        }
    }
}
