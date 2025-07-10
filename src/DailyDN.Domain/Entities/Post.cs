namespace DailyDN.Domain.Entities
{
    public class Post : Entity
    {
        public string Caption { get; set; } = null!;
        public string MediaUrl { get; set; } = null!;
        public string MediaType { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}