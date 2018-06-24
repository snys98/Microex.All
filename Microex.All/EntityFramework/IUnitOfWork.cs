using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Microex.All.EntityFramework
{
    public interface IUnitOfWork
    {
        List<INotification> DomainEvents { get; }
        Task<int> SaveChangesAsync();
    }
}
