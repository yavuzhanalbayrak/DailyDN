using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyDN.Infrastructure.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Caption)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(p => p.MediaUrl)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(p => p.MediaType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(p => p.User)
                   .WithMany(u => u.Posts)
                   .HasForeignKey(p => p.CreatedBy);
        }
    }
}