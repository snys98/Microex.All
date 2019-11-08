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
        public bool IsAdapterForIndividualComponents
        {
            get { return true; }
        }

        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
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
    }
}
