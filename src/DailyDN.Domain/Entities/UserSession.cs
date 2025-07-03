namespace DailyDN.Domain.Entities
{
    public class UserSession : Entity
    {
        private UserSession() { }

        public UserSession(int userId, string refreshToken, string ipAddress, string userAgent, DateTime expiresAt)
        {
            UserId = userId;
            RefreshToken = refreshToken;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            ExpiresAt = expiresAt;
            IsRevoked = false;
            CreatedAt = DateTime.UtcNow;
        }

        public int UserId { get; private set; }
        public User? User { get; private set; }
        public string RefreshToken { get; private set; } = string.Empty;
        public string IpAddress { get; private set; } = string.Empty;
        public string UserAgent { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }

        public bool IsActive()
        {
            return !IsRevoked && ExpiresAt > DateTime.UtcNow;
        }

        public void Revoke()
        {
            if (IsRevoked)
                throw new InvalidOperationException("Session is already revoked.");
            IsRevoked = true;
        }

        public void Rotate(string newRefreshToken, DateTime newExpiry)
        {
            RefreshToken = newRefreshToken;
            ExpiresAt = newExpiry;
        }
    }
}
