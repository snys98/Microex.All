using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microex.All.Common;

namespace Microex.All.EntityFramework
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<PagedList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, IComparable>> orderBy = null, int page = 1, int pageSize = 10);

        Task<TEntity> GetAsync(TKey entityId);

        Task<int> AddAsync(TEntity entity);

        Task<int> UpdateAsync(TEntity entity);

        Task<int> DeleteAsync(TKey entityId);

        Task<bool> Exists(TKey entityId);
    }

    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity:class 
    {
    }
}
