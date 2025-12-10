using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyDN.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
       public void Configure(EntityTypeBuilder<User> builder)
       {
              builder.ToTable("Users");
              builder.HasKey(u => u.Id);

              builder.OwnsOne(u => u.FullName, fn =>
              {
                     fn.Property(p => p.Name)
                     .HasColumnName("Name")
                     .HasMaxLength(100)
                     .IsRequired();

                     fn.Property(p => p.Surname)
                     .HasColumnName("Surname")
                     .HasMaxLength(100)
                     .IsRequired();
              });

              builder.OwnsOne(u => u.Email, email =>
              {
                     email.Property(p => p.Value)
                     .HasColumnName("Email")
                     .HasMaxLength(150)
                     .IsRequired();

                     email.HasIndex(p => p.Value)
                     .IsUnique()
                     .HasFilter("[IsDeleted] = 0");
              });

              builder.OwnsOne(u => u.PhoneNumber, phone =>
              {
                     phone.Property(p => p.Value)
                     .HasColumnName("PhoneNumber")
                     .HasMaxLength(20)
                     .IsRequired();
              });

              builder.OwnsOne(u => u.PasswordHash, pass =>
              {
                     pass.Property(p => p.Value)
                     .HasColumnName("PasswordHash")
                     .IsRequired();
              });

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
