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
                new("Admin") { Id = 1 },
                new("User")  { Id = 2 } 
            };

            modelBuilder.Entity<Role>().HasData(roles);
        }
    }
}