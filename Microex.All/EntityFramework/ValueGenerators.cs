using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Microex.All.EntityFramework
{
    public class DatetimeValueGenerator: ValueGenerator<DateTime>
    {
        public override DateTime Next(EntityEntry entry)
        {
            if (entry.State == EntityState.Added)
            {
                return DateTime.Now;
            }

            if (entry.State == EntityState.Modified)
            {
                //if (entry.)
                //{
                    
                //}
            }
            return DateTime.Now;
        }

        public override bool GeneratesTemporaryValues { get; } = false;
    }
}
