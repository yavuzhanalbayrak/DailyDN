using DailyDN.Domain.ValueObjects;

namespace DailyDN.Domain.Entities
{
    public class User : Entity
    {
        public FullName FullName { get; private set; } = null!;

        public string? AvatarUrl { get; private set; }

        public Email Email { get; private set; } = null!;
        public PhoneNumber PhoneNumber { get; private set; } = null!;
        public PasswordHash PasswordHash { get; private set; } = null!;

        public string? OtpCode { get; private set; }
        public DateTime? OtpGeneratedAt { get; private set; }
        public Guid? Guid { get; set; }
        public bool IsGuidUsed { get; set; } = false;

        public bool IsEmailVerified { get; private set; } = false;
        public DateTime? LastLoginAt { get; private set; }

        public ICollection<UserRole> UserRoles { get; private set; } = [];
        public ICollection<UserSession> UserSessions { get; private set; } = [];
        public ICollection<Post> Posts { get; private set; } = [];
        public ICollection<UserChat> UserChats { get; set; } = [];

        public Guid? ForgotPasswordToken { get; set; }
        public DateTime? ForgotPasswordTokenGeneratedAt { get; set; }
        public bool IsForgotPasswordTokenUsed { get; set; } = false;

        public Guid? EmailVerificationToken { get; private set; }
        public DateTime? EmailVerificationTokenGeneratedAt { get; private set; }
        public bool IsEmailVerificationTokenUsed { get; private set; } = false;

        public User() { }

        public User(FullName fullName, Email email, PhoneNumber phoneNumber, PasswordHash passwordHash)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            PasswordHash = passwordHash;
        }

        public void AddRole(Enums.Role roleType)
        {
            var role = new Role(roleType);
            UserRoles.Add(new UserRole(Id, role.Id)
            {
                Role = role
            });
        }


        public string GetFullName() => FullName.ToString();

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

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void SetPassword(string hashedPassword)
        {
            PasswordHash = new PasswordHash(hashedPassword);
        }

        public void UpdateFullName(string name, string surname)
        {
            FullName = new FullName(name, surname);
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
            PasswordHash = new PasswordHash(newHashedPassword);
            IsForgotPasswordTokenUsed = true;
            ForgotPasswordToken = null;
            ForgotPasswordTokenGeneratedAt = null;
        }

        public void GenerateEmailVerificationToken()
        {
            EmailVerificationToken = System.Guid.NewGuid();
            EmailVerificationTokenGeneratedAt = DateTime.UtcNow;
            IsEmailVerificationTokenUsed = false;
        }

        private bool IsEmailVerificationTokenValid(Guid token, TimeSpan validFor)
        {
            return EmailVerificationToken.HasValue &&
                   EmailVerificationToken.Value == token &&
                   EmailVerificationTokenGeneratedAt.HasValue &&
                   !IsEmailVerificationTokenUsed &&
                   EmailVerificationTokenGeneratedAt.Value.Add(validFor) > DateTime.UtcNow;
        }

        public void VerifyEmailToken(Guid token, TimeSpan validFor)
        {
            if (!IsEmailVerificationTokenValid(token, validFor))
                throw new InvalidOperationException("Invalid or expired email verification token.");

            IsEmailVerified = true;
            IsEmailVerificationTokenUsed = true;
            EmailVerificationToken = null;
            EmailVerificationTokenGeneratedAt = null;
        }






    }

}