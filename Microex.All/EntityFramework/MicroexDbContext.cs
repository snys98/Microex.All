using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Microex.All.EntityFramework
{
    public class MicroexDbContext<TDbContext> : DbContext, IUnitOfWork where TDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public MicroexDbContext(DbContextOptions options,IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigIEntities<TDbContext>();
            base.OnModelCreating(modelBuilder);
        }

        public List<INotification> DomainEvents { get; } = new List<INotification>();

        public virtual async Task<int> SaveChangesAsync()
        {
            foreach (var domainEvent in DomainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
            return await base.SaveChangesAsync();
        }
    }
}