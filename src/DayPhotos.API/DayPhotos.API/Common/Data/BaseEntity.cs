using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DayPhoto.API.Infrastructure.Data
{
    public abstract class BaseEntity<TKey>
    {
        public virtual TKey Id { get; set; }
    }

    public interface IAduitEntity
    {
        string CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        string UpdatedBy { get; set; }
        DateTime UpdatedOn { get; set; }
    }
}
