using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyDN.Infrastructure.Seed
{
    public static class UserRoleSeed
    {
        public static void SeedUserRoles(this ModelBuilder modelBuilder)
        {
            var userRoles = new List<UserRole>
            {
                new(1,2){Id = 1}
            };

            modelBuilder.Entity<UserRole>().HasData(userRoles);
        }
    }
}