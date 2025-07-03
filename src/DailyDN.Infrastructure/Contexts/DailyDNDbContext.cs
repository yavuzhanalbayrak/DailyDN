using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DailyDN.Infrastructure.Contexts
{
    public class DailyDNDbContext(DbContextOptions<DailyDNDbContext> options, ILogger<DailyDNDbContext> logger, int currentUser = 0) : ApplicationContext(options, logger, currentUser)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(t => t.ClrType.BaseType == typeof(Entity)))
            {
                var builder = modelBuilder.Entity(entityType.ClrType);

                builder.Property(nameof(Entity.CreatedAt))
                    .HasDefaultValueSql("GETDATE()");

                builder.Property(nameof(Entity.CreatedBy))
                    .HasDefaultValueSql("0");

                builder.Property(nameof(Entity.UpdatedAt))
                    .HasDefaultValue(null);

                builder.Property(nameof(Entity.IsDeleted))
                    .HasDefaultValue(false);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(InfrastructureAssembly.Instance);

            modelBuilder.SeedUsers();

            ApplyGlobalFilters<User>(modelBuilder);
            ApplyGlobalFilters<Claim>(modelBuilder);
            ApplyGlobalFilters<Role>(modelBuilder);
            ApplyGlobalFilters<UserRole>(modelBuilder);
            ApplyGlobalFilters<RoleClaim>(modelBuilder);
            ApplyGlobalFilters<UserSession>(modelBuilder);
        }
    }
}