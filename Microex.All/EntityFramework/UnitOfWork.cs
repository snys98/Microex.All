using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microex.All.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microex.All.EntityFramework
{
    public class UnitOfWork<TDbContext> where TDbContext : IntegratedDbContext
    {
        private IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;
        private IServiceScope _currentScope;
        private bool _isSaving;

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _currentScope = _serviceProvider.CreateScope();
            _mediator = _currentScope.ServiceProvider.GetRequiredService<IMediator>();
            DbContext = _currentScope.ServiceProvider.GetRequiredService<TDbContext>();
        }

        

        public ConcurrentQueue<IRequest<bool>> DomainEvents { get; } = new ConcurrentQueue<IRequest<bool>>();
        public TDbContext DbContext { get; private set; }
        public async Task<bool> SaveChangesAsync()
        {
           
            while (DomainEvents.TryDequeue(out var domainEvent))
            {
                if (await _mediator.Send(domainEvent))
                {
                    continue;
                }
                // bug: 日志记录
                //throw new Exception("domain event failed." + domainEvent.ToJson());
                this.Refresh();
                return false;
            }
            while (this._isSaving)
            {
                await Task.Delay(100);
            }

            this._isSaving = true;
            var result = this.DbContext.UowSaveChangesAsync().Result > 0;
            this._isSaving = false;
            return result;
            


            //if (DomainEvents.TryPeek(out var domainEvent))
            //{
            //    if (domainEvent is ISyncNotification syncNotification)
            //    {
            //        if (syncNotification.FinishTokenSource.IsCancellationRequested)
            //        {
            //            DomainEvents.TryDequeue(out _);
            //        }
            //        else
            //        {
            //            await syncNotification.Wait();
            //            await _mediator.Publish(syncNotification);
            //        }                   
            //    }
            //    else
            //    {
            //        await _mediator.Publish(domainEvent);
            //        DomainEvents.TryDequeue(out _);
            //    }
            //}

            //return await this.SaveChangesAsync();
        }

        public void Refresh()
        {
            //无论保存成功失败,都清除local内的临时值
            this.DbContext.ChangeTracker.AcceptAllChanges();
            _currentScope.Dispose();
            _currentScope = _serviceProvider.CreateScope();
            _mediator = _currentScope.ServiceProvider.GetRequiredService<IMediator>();
            DbContext = _currentScope.ServiceProvider.GetRequiredService<TDbContext>();
        }
    }
}