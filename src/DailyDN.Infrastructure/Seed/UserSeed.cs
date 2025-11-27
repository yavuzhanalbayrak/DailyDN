using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyDN.Infrastructure.Seed
{
    public static class UserSeed
    {
        public static void SeedUsers(this ModelBuilder modelBuilder)
        {
            var users = new List<User>
            {
                new("john","doe","johndoe@example.com","05002001020", "AQAAAAIAAYagAAAAELAUs+nJPSlymbpaEf2On5XTsZilCbc+jpMAqhini8fYQU/yeTEKm1diq/A5/pcfWw==",id:1, isEmailVerified: true)
            };

            modelBuilder.Entity<User>().HasData(users);
        }
    }
}