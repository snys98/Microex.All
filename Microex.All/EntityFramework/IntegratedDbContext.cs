using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Microex.All.EntityFramework
{
    public class IntegratedDbContext<TDbContext> :DbContext where TDbContext: DbContext
    {
        public IntegratedDbContext(DbContextOptions options)
        :base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigIEntities<TDbContext>();
            base.OnModelCreating(modelBuilder);
        }

        public new Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
            //return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public new int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
            //return base.SaveChanges(acceptAllChangesOnSuccess);
        }
    }
}
