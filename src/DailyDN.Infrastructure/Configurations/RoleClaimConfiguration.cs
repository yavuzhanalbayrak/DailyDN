using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyDN.Infrastructure.Configurations
{
    public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTable("RoleClaims");

            builder.HasKey(rc => rc.Id);

            builder.HasIndex(a => new { a.RoleId, a.ClaimId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasOne(rc => rc.Role)
                   .WithMany(r => r.RoleClaims)
                   .HasForeignKey(rc => rc.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rc => rc.Claim)
                   .WithMany(c => c.RoleClaims)
                   .HasForeignKey(rc => rc.ClaimId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}