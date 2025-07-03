using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyDN.Infrastructure.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(r => r.UserRoles)
                   .WithOne(ur => ur.Role)
                   .HasForeignKey(ur => ur.RoleId);

            builder.HasMany(r => r.RoleClaims)
                   .WithOne(rc => rc.Role)
                   .HasForeignKey(rc => rc.RoleId);
        }
    }
}