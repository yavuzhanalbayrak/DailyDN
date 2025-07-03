using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;


namespace DailyDN.Infrastructure.Contexts
{
    public interface IApplicationContext
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        EntityEntry<TEntity> Entry<TEntity>([NotNull] TEntity entity) where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        int SaveChanges();

        ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>([NotNull] TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;

        EntityEntry<TEntity> Remove<TEntity>([NotNull] TEntity entity) where TEntity : class;

        EntityEntry<TEntity> Update<TEntity>([NotNull] TEntity entity) where TEntity : class;
    }
}