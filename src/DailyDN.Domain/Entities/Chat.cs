namespace DailyDN.Domain.Entities
{
    public class Chat : Entity
    {
        public string Name { get; set; } = null!;
        public List<ChatMessage> Messages { get; set; } = [];
        public ICollection<UserChat> UserChats { get; set; } = [];
    }
}