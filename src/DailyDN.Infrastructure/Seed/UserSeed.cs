using DailyDN.Domain.Entities;
using DailyDN.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DailyDN.Infrastructure.Seed
{
    public static class UserSeed
    {
        public static void SeedUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new
            {
                Id = 1,
                AvatarUrl = (string?)null,
                IsEmailVerified = true,
                IsEmailVerificationTokenUsed = true,
                IsForgotPasswordTokenUsed = false,
                IsGuidUsed = false
            });

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.FullName)
                .HasData(new
                {
                    UserId = 1,
                    Name = "john",
                    Surname = "doe"
                });

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.Email)
                .HasData(new
                {
                    UserId = 1,
                    Value = "johndoe@example.com"
                });

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.PhoneNumber)
                .HasData(new
                {
                    UserId = 1,
                    Value = "05002001020"
                });

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.PasswordHash)
                .HasData(new
                {
                    UserId = 1,
                    Value = "AQAAAAIAAYagAAAAELAUs+nJPSlymbpaEf2On5XTsZilCbc+jpMAqhini8fYQU/yeTEKm1diq/A5/pcfWw=="
                });
        }
    }

}
