namespace DailyDN.Domain.Entities
{
    public class Claim : Entity
    {
        public string Type { get; private set; } = null!;
        public string Value { get; private set; } = null!;
        public ICollection<RoleClaim> RoleClaims { get; private set; } = new List<RoleClaim>();

        private Claim() { }

        public Claim(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}