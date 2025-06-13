namespace DailyDN.Domain.Entities
{

    public class RoleClaim : Entity
    {
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public int ClaimId { get; set; }
        public Claim Claim { get; set; } = null!;
    }
}