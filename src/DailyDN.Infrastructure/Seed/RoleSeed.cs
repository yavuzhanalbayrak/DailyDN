using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyDN.Infrastructure.Seed
{
    public static class RoleSeed
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            var roles = new List<Role>
            {
                new(Domain.Enums.Role.Admin),
                new(Domain.Enums.Role.User)
            };

            modelBuilder.Entity<Role>().HasData(roles);
        }
    }
}