using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyDN.Infrastructure.Configurations
{
    public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> builder)
        {
            builder.ToTable("Claims");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Type)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Value)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasMany(c => c.RoleClaims)
                   .WithOne(rc => rc.Claim)
                   .HasForeignKey(rc => rc.ClaimId);
        }
    }
}