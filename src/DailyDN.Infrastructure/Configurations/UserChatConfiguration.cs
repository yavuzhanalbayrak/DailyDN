using DailyDN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyDN.Infrastructure.Configurations
{
    public class UserChatConfiguration : IEntityTypeConfiguration<UserChat>
    {
        public void Configure(EntityTypeBuilder<UserChat> builder)
        {
            builder.ToTable("UserChats");

            builder.HasIndex(uc => new { uc.UserId, uc.ChatId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasOne(uc => uc.User)
                   .WithMany(u => u.UserChats)
                   .HasForeignKey(uc => uc.UserId);

            builder.HasOne(uc => uc.Chat)
                   .WithMany(c => c.UserChats)
                   .HasForeignKey(uc => uc.ChatId);
        }
    }
}