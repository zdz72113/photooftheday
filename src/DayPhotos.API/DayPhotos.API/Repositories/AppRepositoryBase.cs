using DayPhoto.API.Entities;
using DayPhoto.API.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DayPhoto.API.Repositories
{
    public interface IDCRepositoryBase<TEntity, TKey> : IRepository<AppDbContext, TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
    }

    public class AppRepositoryBase<TEntity, TKey> : EFRepository<AppDbContext, TEntity, TKey>, IDCRepositoryBase<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public AppRepositoryBase(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
