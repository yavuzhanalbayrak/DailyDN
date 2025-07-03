using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyDN.Infrastructure.Configurations
{
       public class UserConfiguration : IEntityTypeConfiguration<User>
       {
              public void Configure(EntityTypeBuilder<User> builder)
              {
                     builder.ToTable("Users");

                     builder.HasKey(u => u.Id);

                     builder.HasIndex(u => u.Email)
                         .IsUnique()
                         .HasFilter("[IsDeleted] = 0");

                     builder.Property(u => u.Name)
                            .IsRequired()
                            .HasMaxLength(100);

                     builder.Property(u => u.Surname)
                            .IsRequired()
                            .HasMaxLength(100);

                     builder.Property(u => u.Email)
                            .IsRequired()
                            .HasMaxLength(150);

                     builder.Property(u => u.PasswordHash)
                            .IsRequired();

                     builder.Property(u => u.AvatarUrl)
                            .HasMaxLength(300);

                     builder.Property(u => u.OtpCode)
                            .HasMaxLength(10);

                     builder.Property(u => u.IsEmailVerified)
                            .HasDefaultValue(false);

                     builder.Property(u => u.CreatedAt)
                            .HasDefaultValueSql("GETUTCDATE()");

                     builder.HasMany(u => u.UserRoles)
                            .WithOne(ur => ur.User)
                            .HasForeignKey(ur => ur.UserId)
                            .OnDelete(DeleteBehavior.Cascade);
              }
       }

}