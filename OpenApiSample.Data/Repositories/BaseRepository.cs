using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace OpenApiSample.Data.Repositories
{
    public class BaseRepository<TEntity>(AppDbContext context) : IBaseRepository<TEntity>
    where TEntity : class
    {
        private readonly AppDbContext _context = context;
        private readonly DbSet<TEntity> _set = context.Set<TEntity>();

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var addedEntity = (await _context.AddAsync(entity)).Entity;

            await _context.SaveChangesAsync();

            return addedEntity;
        }

        public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
        {
            await _context.AddRangeAsync(entities);

            await _context.SaveChangesAsync();

            return entities;
        }

        public async Task<TEntity> GetAsync(
          Expression<Func<TEntity, bool>> predicate,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
          bool enableTracking = true,
          CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = _set;

            if (!enableTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (include != null)
            {
                queryable = include(queryable);
            }

            return await queryable.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<ICollection<TEntity>> GetAllAsync(
          Expression<Func<TEntity, bool>>? predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
          Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
          bool enableTracking = true,
          CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = _set;

            if (!enableTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (include != null)
            {
                queryable = include(queryable);
            }

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            if (orderBy != null)
            {
                queryable = orderBy(queryable);
            }

            return await queryable.ToListAsync(cancellationToken);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _context.Update(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            var removedEntity = _context.Remove(entity).Entity;

            await _context.SaveChangesAsync();

            return removedEntity;
        }

        public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities)
        {
            _context.RemoveRange(entities);

            await _context.SaveChangesAsync();

            return entities;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _set.AnyAsync(predicate, cancellationToken);
        }

        public async Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _set.AllAsync(predicate, cancellationToken);
        }
    }

}
