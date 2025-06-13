namespace DailyDN.Domain.Entities
{
    public class UserRole : Entity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}