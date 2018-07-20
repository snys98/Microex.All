using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Microex.All.EntityFramework
{
    public class IntegratedDbContext:DbContext
    {
        public IntegratedDbContext(DbContextOptions options)
        :base(options)
        {
            
        }

        //public Task<int> SaveChangesAsync([CallerMemberName]string callMemberName = null, CancellationToken cancellationToken = new CancellationToken())
        //{
        //    if (callMemberName != null && callMemberName.Contains("SaveChangesAsync"))
        //    {
        //        return base.SaveChangesAsync(cancellationToken);
        //    }
        //    throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
        //}
        public Task<int> UowSaveChangesAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = new CancellationToken(),[CallerMemberName]string callMemberName = null)
        {
            if (callMemberName == nameof(UnitOfWork<IntegratedDbContext>.SaveChangesAsync))
            {
                return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
            //return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
            //return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
            //return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
            //return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override int SaveChanges()
        {
            throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
            //return base.SaveChanges();
        }
    }
}
