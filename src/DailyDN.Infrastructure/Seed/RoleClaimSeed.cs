using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyDN.Infrastructure.Seed
{
    public static class RoleClaimSeed
    {
        public static void SeedRoleClaims(this ModelBuilder modelBuilder)
        {
            var roleClaims = new List<RoleClaim>
            {
                // User: all claims
                new() { Id = 1, RoleId = 2, ClaimId = 1 },
                new() { Id = 2, RoleId = 2, ClaimId = 2 },
                new() { Id = 3, RoleId = 2, ClaimId = 3 },
                new() { Id = 4, RoleId = 2, ClaimId = 4 },
            };

            modelBuilder.Entity<RoleClaim>().HasData(roleClaims);
        }
    }
}