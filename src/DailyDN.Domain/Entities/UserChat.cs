namespace DailyDN.Domain.Entities
{
    public class UserChat : Entity
    {
        public int UserId { get; set; }
        public int ChatId { get; set; }

        public User User { get; set; } = null!;
        public Chat Chat { get; set; } = null!;
    }
}