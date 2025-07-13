namespace DailyDN.Domain.Entities
{
    public class ChatMessage : Entity
    {
        public string Content { get; set; } = null!;

        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
    }
}