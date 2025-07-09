using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyDN.Infrastructure.Seed
{
    public static class ClaimSeed
    {
        public static void SeedClaims(this ModelBuilder modelBuilder)
        {
            var claims = new List<Claim>
            {
                new("Post", "Add")   { Id = 1 },
                new("Post", "Update"){ Id = 2 },
                new("Post", "Delete"){ Id = 3 },
                new("Post", "Get")   { Id = 4 }
            };

            modelBuilder.Entity<Claim>().HasData(claims);
        }
    }
}