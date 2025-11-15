using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyDN.Infrastructure.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSessions");

            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.RefreshTokenHash);

            builder.HasOne(s => s.User)
                   .WithMany(u => u.UserSessions)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(s => s.RefreshTokenHash)
                   .IsRequired()
                   .HasMaxLength(512);

            builder.Property(s => s.IpAddress)
                   .HasMaxLength(64);

            builder.Property(s => s.UserAgent)
                   .HasMaxLength(512);

            builder.Property(s => s.CreatedAt)
                   .IsRequired();

            builder.Property(s => s.ExpiresAt)
                   .IsRequired();

            builder.Property(s => s.IsRevoked)
                   .IsRequired();

            builder.Property(s => s.IsDeleted)
                   .IsRequired();
        }
    }
}
