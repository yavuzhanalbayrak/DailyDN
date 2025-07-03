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
                new("john","doe","johndoe@example.com","hashpassword",id:1)
            };

            modelBuilder.Entity<User>().HasData(users);
        }
    }
}