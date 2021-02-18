using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DayPhoto.API.Infrastructure.Data
{
    public interface IRepository<TDbContext, TEntity, TKey> where TEntity : BaseEntity<TKey> where TDbContext : DbContext
    {
        Task<TEntity> GetByKeyAsync(TKey id);

        TEntity GetByKey(TKey id);

        IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate = null);

        Task<IQueryable<TEntity>> GetQueryableAsync(Expression<Func<TEntity, bool>> predicate = null);

        Task<IList<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null);
        IList<TEntity> GetList(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null);

        Task<Page<TEntity>> GetPageListAsync(
            int? pageIndex = null, int? pageSize = null,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null);

        Page<TEntity> GetPageList(
            int? pageIndex = null, int? pageSize = null,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null);

        Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate);

        bool IsExist(Expression<Func<TEntity, bool>> predicate);

        Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate);

        long Count(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> AddAsync(TEntity entity);

        TEntity Add(TEntity entity);

        Task AddRangeAsync(IList<TEntity> entitys);

        void AddRange(IList<TEntity> entitys);

        TEntity Update(TEntity entity);

        void Delete(TKey id);

        void Delete(TEntity entity);

        void DeleteRange(Expression<Func<TEntity, bool>> predicate);
    }
}
