using Microsoft.EntityFrameworkCore;
using DailyDN.Infrastructure.Contexts;
using System.Linq.Expressions;
using DailyDN.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DailyDN.Infrastructure.Repositories
{
    public class GenericRepository<T>(IApplicationContext context, ILogger<GenericRepository<T>> logger) : IGenericRepository<T> where T : Entity
    {
        protected readonly IApplicationContext _context = context ?? throw new ArgumentNullException(nameof(context));
        protected readonly DbSet<T> _dbSet = context.Set<T>();
        protected readonly ILogger<GenericRepository<T>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        {
            _logger.LogInformation("Getting all entities of type {EntityType}", typeof(T).Name);
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>> predicate = null,
            string includeString = null,
            bool disableTracking = true
        )
        {
            _logger.LogInformation("Getting paginated entities of type {EntityType}, Page: {Page}, PageSize: {PageSize}", typeof(T).Name, page, pageSize);

            IQueryable<T> query = _dbSet;

            if (disableTracking)
                query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(includeString))
                query = query.Include(includeString);

            if (predicate != null)
                query = query.Where(predicate);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public virtual async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize,
            Func<IQueryable<T>, IQueryable<T>> includes,
            Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, object>> orderBy = null,
            bool orderDescending = true,
            bool disableTracking = true
        )
        {
            _logger.LogInformation("Getting paginated entities of type {EntityType}, Page: {Page}, PageSize: {PageSize}", typeof(T).Name, page, pageSize);

            IQueryable<T> query = _dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (includes != null)
                query = includes(query);

            if (predicate != null)
                query = query.Where(predicate);

            var totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderDescending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);
            }
            else
                query = query.OrderByDescending(e => e.CreatedAt);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }



        public virtual async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            _logger.LogInformation("Getting entities of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<IReadOnlyList<T>> GetAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeString = null,
            bool disableTracking = true)
        {
            _logger.LogInformation("Getting entities of type {EntityType} with predicate and ordering", typeof(T).Name);

            IQueryable<T> query = _dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeString))
                query = query.Include(includeString);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public virtual async Task<IReadOnlyList<T>> GetAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true)
        {
            _logger.LogInformation("Getting entities of type {EntityType} with complex parameters", typeof(T).Name);

            IQueryable<T> query = _dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (includes != null)
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            _logger.LogInformation("Getting entities of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.Where(predicate).FirstOrDefaultAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate = null,
            string includeString = null,
            bool disableTracking = true)
        {
            _logger.LogInformation("Getting entities of type {EntityType} with predicate and ordering", typeof(T).Name);

            IQueryable<T> query = _dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeString))
                query = query.Include(includeString);

            if (predicate != null)
                query = query.Where(predicate);

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate = null,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true)
        {
            _logger.LogInformation("Getting entities of type {EntityType} with complex parameters", typeof(T).Name);

            IQueryable<T> query = _dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (includes != null)
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null)
                query = query.Where(predicate);

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting entity of type {EntityType} with id {EntityId}", typeof(T).Name, id);
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(int id, List<Expression<Func<T, object>>> includes = null)
        {
            _logger.LogInformation("Getting entity of type {EntityType} with id {EntityId}", typeof(T).Name, id);

            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }


        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding new entity of type {EntityType}", typeof(T).Name);


            await _context.AddAsync(entity, cancellationToken);

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _logger.LogInformation("Updating entity of type {EntityType} with id {EntityId}", typeof(T).Name, entity.Id);
            _context.Update(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _logger.LogInformation("Deleting entity of type {EntityType} with id {EntityId}", typeof(T).Name, entity.Id);
            _context.Remove(entity);
        }
    }
}