namespace DailyDN.Domain.Entities
{
    public class Role : Entity
    {
        public string Name { get; private set; } = null!;
        public ICollection<UserRole> UserRoles { get; private set; } = [];
        public ICollection<RoleClaim> RoleClaims { get; private set; } = [];

        private Role() { }

        public Role(Enums.Role role)
        {
            Id = (int)role;
            Name = role.ToString();
        }
    }
}