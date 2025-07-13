using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyDN.Infrastructure.Configurations
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("ChatMessages");

            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.Content)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne(cm => cm.Chat)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(cm => cm.ChatId);
        }
    }
}