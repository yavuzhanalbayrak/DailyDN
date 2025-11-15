namespace DailyDN.Domain.Entities
{
    public class User : Entity
    {
        public string Name { get; private set; } = null!;
        public string Surname { get; private set; } = null!;
        public string? AvatarUrl { get; private set; }

        public string Email { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;

        public string? OtpCode { get; private set; }
        public DateTime? OtpGeneratedAt { get; private set; }
        public Guid? Guid { get; set; }
        public bool IsGuidUsed { get; set; } = false;

        public bool IsEmailVerified { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        public ICollection<UserRole> UserRoles { get; private set; } = [];
        public ICollection<UserSession> UserSessions { get; private set; } = [];
        public ICollection<Post> Posts { get; private set; } = [];
        public ICollection<UserChat> UserChats { get; set; } = [];

        public Guid? ForgotPasswordToken { get; set; }
        public DateTime? ForgotPasswordTokenGeneratedAt { get; set; }
        public bool IsForgotPasswordTokenUsed { get; set; } = false;

        private User() { }

        public User(string name, string surname, string email, string phoneNumber, string passwordHash, string? avatarUrl = null, int id = 0, ICollection<UserRole>? userRoles = null)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
            PhoneNumber = phoneNumber;
            PasswordHash = passwordHash;
            AvatarUrl = avatarUrl;
            UserRoles = userRoles ?? [];
        }

        public void SetOtp(string code, Guid guid)
        {
            OtpCode = code;
            Guid = guid;
            OtpGeneratedAt = DateTime.UtcNow;
            IsGuidUsed = false;
        }

        public bool IsOtpValid(string? inputOtp, TimeSpan validFor)
        {
            return OtpCode == inputOtp &&
                   OtpGeneratedAt.HasValue &&
                   !IsGuidUsed &&
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

        public void Login()
        {
            IsGuidUsed = true;
            LastLoginAt = DateTime.UtcNow;
        }

        public IEnumerable<Claim> GetClaims()
        {
            return UserRoles
                .SelectMany(ur => ur.Role.RoleClaims)
                .Select(rc => rc.Claim)
                .Distinct();
        }

        public string FullName => $"{Name} {Surname}";

        public void GeneratePasswordResetToken()
        {
            ForgotPasswordToken = System.Guid.NewGuid();
            ForgotPasswordTokenGeneratedAt = DateTime.UtcNow;
            IsForgotPasswordTokenUsed = false;
        }

        public bool IsPasswordResetTokenValid(Guid token, TimeSpan validFor)
        {
            return ForgotPasswordToken.HasValue &&
                   ForgotPasswordToken.Value == token &&
                   ForgotPasswordTokenGeneratedAt.HasValue &&
                   !IsForgotPasswordTokenUsed &&
                   ForgotPasswordTokenGeneratedAt.Value.Add(validFor) > DateTime.UtcNow;
        }

        public void ResetPassword(string newHashedPassword)
        {
            PasswordHash = newHashedPassword;
            IsForgotPasswordTokenUsed = true;
            ForgotPasswordToken = null;
            ForgotPasswordTokenGeneratedAt = null;
        }




    }

}