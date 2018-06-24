using System;
using System.Collections.Generic;
using System.Linq;

namespace Microex.All.Common
{
    public class PagedList<T> where T : class
    {
        public PagedList()
        {
            Data = new List<T>();
        }

        public List<T> Data { get; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public PagedList<TTarget> Cast<TTarget>(Func<T, TTarget> changeExpression) where TTarget:class 
        {
            var result =  new PagedList<TTarget>()
            {
                PageSize = this.PageSize,
                TotalCount = this.TotalCount
            };
            result.Data.AddRange(this.Data.Select(changeExpression).ToArray());
            return result;
        }
    }
}
