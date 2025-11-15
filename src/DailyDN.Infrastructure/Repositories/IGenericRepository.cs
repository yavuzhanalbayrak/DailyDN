using DailyDN.Domain.Entities;
using System.Linq.Expressions;

namespace DailyDN.Infrastructure.Repositories
{
    public interface IGenericRepository<T> where T : Entity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<(IReadOnlyList<T> Items, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>> predicate = null,
            string includeString = null,
            bool disableTracking = true
        );
        Task<(IReadOnlyList<T> Items, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize,
            Func<IQueryable<T>, IQueryable<T>> includes,
            Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, object>> orderBy = null,
            bool orderDescending = true,
            bool disableTracking = true
        );
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<IReadOnlyList<T>> GetAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeString = null,
            bool disableTracking = true);
        Task<IReadOnlyList<T>> GetAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate = null,
            string includeString = null,
            bool disableTracking = true);
        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate = null,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(int id, List<Expression<Func<T, object>>> includes = null);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}