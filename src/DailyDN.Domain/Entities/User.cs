namespace DailyDN.Domain.Entities
{
    public class User : Entity
    {
        public string Name { get; private set; } = null!;
        public string Surname { get; private set; } = null!;
        public string? AvatarUrl { get; private set; }

        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;

        public string? OtpCode { get; private set; }
        public DateTime? OtpGeneratedAt { get; private set; }

        public bool IsEmailVerified { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        public ICollection<UserRole> UserRoles { get; private set; } = [];
        public ICollection<UserSession> UserSessions { get; private set; } = [];

        private User() { }

        public User(string name, string surname, string email, string passwordHash, string? avatarUrl = null, int id = 0)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
            PasswordHash = passwordHash;
            AvatarUrl = avatarUrl;
        }

        public void SetOtp(string code)
        {
            OtpCode = code;
            OtpGeneratedAt = DateTime.UtcNow;
        }

        public bool IsOtpValid(string inputOtp, TimeSpan validFor)
        {
            return OtpCode == inputOtp &&
                   OtpGeneratedAt.HasValue &&
                   OtpGeneratedAt.Value.Add(validFor) > DateTime.UtcNow;
        }

        public void MarkEmailVerified()
        {
            IsEmailVerified = true;
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void SetPassword(string hashedPassword)
        {
            PasswordHash = hashedPassword;
        }

        public void UpdateName(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        public void SetAvatar(string? avatarUrl)
        {
            AvatarUrl = avatarUrl;
        }

        public IEnumerable<Claim> GetClaims()
        {
            return UserRoles
                .SelectMany(ur => ur.Role.RoleClaims)
                .Select(rc => rc.Claim)
                .Distinct();
        }

        public string FullName => $"{Name} {Surname}";

    }

}