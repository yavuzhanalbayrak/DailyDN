namespace DailyDN.Domain.Entities
{
    public class Role : Entity
    {
        public string Name { get; private set; } = null!;
        public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
        public ICollection<RoleClaim> RoleClaims { get; private set; } = new List<RoleClaim>();

        private Role() { }

        public Role(string name)
        {
            Name = name;
        }
    }
}