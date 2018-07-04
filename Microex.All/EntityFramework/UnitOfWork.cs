using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Microex.All.EntityFramework
{
    public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext> where TDbContext : DbContext
    {
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            var uowScope = serviceProvider.CreateScope();
            _serviceProvider = uowScope.ServiceProvider;
            _mediator = _serviceProvider.GetRequiredService<IMediator>();
            DbContext = _serviceProvider.GetRequiredService<TDbContext>();
        }

        

        public List<INotification> DomainEvents { get; } = new List<INotification>();
        public TDbContext DbContext { get; private set; }
        public async Task<int> SaveChangesAsync()
        {
            //bug: domainevent的处理函数里面可能继续添加domainevent导致改变循环内容
            foreach (var domainEvent in DomainEvents)
            {
                await _mediator.Publish(domainEvent);
                DomainEvents.Remove(domainEvent);
            }

            return await (DbContext as DbContext).SaveChangesAsync();
        }

        public void DiscardChanges()
        {
            this.DbContext.Dispose();
            this.DomainEvents.Clear();
            this.DbContext = _serviceProvider.GetRequiredService<TDbContext>();
        }
    }
}