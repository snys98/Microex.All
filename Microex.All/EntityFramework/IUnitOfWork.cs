using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Microex.All.EntityFramework
{
    public interface IUnitOfWork<TDbContext> where TDbContext:DbContext
    {
        List<INotification> DomainEvents { get; }
        TDbContext DbContext { get; }
        Task<int> SaveChangesAsync();
        void DiscardChanges();
    }
}
