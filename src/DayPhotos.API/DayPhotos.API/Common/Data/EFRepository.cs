using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DayPhoto.API.Infrastructure.Data
{
    public class EFRepository<TDbContext, TEntity, TKey> : IRepository<TDbContext, TEntity, TKey> where TEntity : BaseEntity<TKey> where TDbContext : DbContext
    {
        protected readonly TDbContext _context;
        protected readonly DbSet<TEntity> dbSet;

        public EFRepository(TDbContext context)
        {
            this._context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetByKeyAsync(TKey id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual TEntity GetByKey(TKey id)
        {
            return dbSet.Find(id);
        }

        public virtual async Task<IQueryable<TEntity>> GetQueryableAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }

        public virtual IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }

        public virtual async Task<IList<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public virtual IList<TEntity> GetList(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query.AsNoTracking().ToList();
        }

        public virtual async Task<Page<TEntity>> GetPageListAsync(
            int? pageIndex = null, int? pageSize = null,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null)
        {
            var page = new Page<TEntity>();

            IQueryable<TEntity> query = dbSet;
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalItems = await query.LongCountAsync();
            page.TotalItems = totalItems;

            if (pageIndex != null && pageSize != null)
            {
                if (pageSize.Value == 0) pageSize = 10;
                var totalPages = totalItems != 0 ? (totalItems % pageSize.Value) == 0 ? (totalItems / pageSize.Value) : (totalItems / pageSize.Value) + 1 : 0;
                page.TotalPages = totalPages;
                page.CurrentPage = pageIndex.Value;
                page.ItemsPerPage = pageSize.Value;
                query = query.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value);
            }

            page.Items = totalItems == 0 ? null : await query.AsNoTracking().ToListAsync();
            return page;
        }

        public virtual Page<TEntity> GetPageList(
            int? pageIndex = null, int? pageSize = null,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null)
        {
            var page = new Page<TEntity>();

            IQueryable<TEntity> query = dbSet;
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalItems = query.LongCount();
            page.TotalItems = totalItems;

            if (pageIndex != null && pageSize != null)
            {
                if (pageSize.Value == 0) pageSize = 10;
                var totalPages = totalItems != 0 ? (totalItems % pageSize.Value) == 0 ? (totalItems / pageSize.Value) : (totalItems / pageSize.Value) + 1 : 0;
                page.TotalPages = totalPages;
                page.CurrentPage = pageIndex.Value;
                page.ItemsPerPage = pageSize.Value;
                query = query.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value);
            }

            page.Items = totalItems == 0 ? null : query.AsNoTracking().ToList();
            return page;
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.FirstOrDefault(predicate);
        }

        public virtual async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }

        public virtual bool IsExist(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.Any(predicate);
        }

        public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.LongCountAsync(predicate);
        }

        public virtual long Count(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.LongCount(predicate);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            var result = await dbSet.AddAsync(entity);
            return result.Entity;
        }

        public virtual TEntity Add(TEntity entity)
        {
            var result = dbSet.Add(entity);
            return result.Entity;
        }

        public virtual async Task AddRangeAsync(IList<TEntity> entitys)
        {
            await dbSet.AddRangeAsync(entitys);
        }

        public virtual void AddRange(IList<TEntity> entitys)
        {
            dbSet.AddRange(entitys);
        }

        public virtual TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            this._context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual void Delete(TKey id)
        {
            TEntity entity = dbSet.Find(id);
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            dbSet.Remove(entity);
        }

        public virtual void DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = dbSet.Where(predicate);
            dbSet.RemoveRange(entities);
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (this._context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
        }
    }
}
