using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DailyDN.Infrastructure.Contexts
{
    public abstract class ApplicationContext(DbContextOptions options, ILogger<DailyDNDbContext> logger, IAuthenticatedUser currentUser) : DbContext(options), IApplicationContext
    {
        private readonly ILogger<DailyDNDbContext> _logger = logger;
        private readonly int _currentUser = currentUser.UserId;

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Entity && (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    (e.State == EntityState.Deleted && e.Entity is Entity)));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.Entity is Entity entity)
                {
                    if (entityEntry.State == EntityState.Added)
                    {
                        entity.CreatedAt = DateTime.UtcNow;
                        entity.CreatedBy = _currentUser;
                        _logger.LogInformation("Entity of type {EntityType} with ID {EntityId} created by {User}",
                            entity.GetType().Name, entity.Id, _currentUser);
                    }
                    else if (entityEntry.State == EntityState.Modified)
                    {
                        entity.UpdatedAt = DateTime.UtcNow;
                        entity.UpdatedBy = _currentUser;
                        _logger.LogInformation("Entity of type {EntityType} with ID {EntityId} updated by {User}",
                            entity.GetType().Name, entity.Id, _currentUser);
                    }
                    else if (entityEntry.State == EntityState.Deleted)
                    {
                        // Soft delete implementation
                        entityEntry.State = EntityState.Modified;
                        entity.IsDeleted = true;
                        entity.UpdatedAt = DateTime.UtcNow;
                        entity.UpdatedBy = _currentUser;
                        _logger.LogInformation("Entity of type {EntityType} with ID {EntityId} soft-deleted by {User}",
                            entity.GetType().Name, entity.Id, _currentUser);
                    }
                }
            }
        }

        protected static void ApplyGlobalFilters<T>(ModelBuilder builder) where T : Entity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}