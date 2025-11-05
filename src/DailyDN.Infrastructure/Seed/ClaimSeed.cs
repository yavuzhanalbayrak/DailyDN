using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyDN.Infrastructure.Seed
{
    public static class ClaimSeed
    {
        public static void SeedClaims(this ModelBuilder modelBuilder)
        {
            const string TYPE = "Permissions";
            var claims = new List<Claim>
            {
                new(TYPE, "PostAdd")   { Id = 1 },
                new(TYPE, "PostUpdate"){ Id = 2 },
                new(TYPE, "PostDelete"){ Id = 3 },
                new(TYPE, "PostGet")   { Id = 4 }
            };

            modelBuilder.Entity<Claim>().HasData(claims);
        }
    }
}