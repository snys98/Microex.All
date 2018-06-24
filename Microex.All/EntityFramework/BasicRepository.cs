using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microex.All.Common;
using Microsoft.EntityFrameworkCore;

namespace Microex.All.EntityFramework
{
    public class BasicRepository<TEntity,TContext,TKey> :IRepository<TEntity, TKey> where TEntity:class, IEntity<TKey> where TContext:DbContext,IUnitOfWork
    {
        private readonly TContext _dbContext;

        public BasicRepository(TContext dbContext)
        {
            _dbContext = dbContext;
            this.UnitOfWork = dbContext;
        }

        public IUnitOfWork UnitOfWork { get; }

        public virtual async Task<PagedList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, IComparable>> orderBy = null, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<TEntity>();
            IQueryable<TEntity> query = _dbContext.Query<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            pagedList.TotalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = query.PageBy(orderBy, page, pageSize);
                
            }
            List<TEntity> data = await query
                .ToListAsync();
            pagedList.Data.AddRange(data);
            pagedList.PageSize = pageSize;
            return pagedList;
        }

        public virtual async Task<TEntity> GetAsync(TKey entityId)
        {
            return await _dbContext.FindAsync<TEntity>(entityId);
        }

        public virtual async Task<int> AddAsync(TEntity entity)
        {
            var addResult = await _dbContext.AddAsync(entity);
            if (addResult.State == EntityState.Added)
            {
                return 1;
            }

            return 0;
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            var addResult = _dbContext.Update(entity);
            if (addResult.State == EntityState.Modified)
            {
                return 1;
            }

            return 0;
        }

        public virtual async Task<int> DeleteAsync(TKey entityId)
        {
            var addResult = _dbContext.Remove(_dbContext.Find<TEntity>(entityId));
            if (addResult.State == EntityState.Deleted)
            {
                return 1;
            }

            return 0;
        }

        public virtual async Task<bool> Exists(TKey id)
        {
            var existsWithClientName = await _dbContext.FindAsync<TEntity>(id);
            return existsWithClientName == null;
        }
    }
}
