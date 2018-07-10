using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microex.All.EntityFramework
{
    public interface IEntity<TKey>
    {
        /// <summary>
        /// Id
        /// </summary>
        TKey Id { get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        DateTime LastModifyTime { get; }
    }

    public interface IEntity:IEntity<string>
    {
        
    }
}
