using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Models;
using DailyDN.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DailyDN.Infrastructure.Services.Impl
{
    public class TokenService(IOptions<JwtSettings> jwtOptions, IUnitOfWork uow) : ITokenService
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        private async Task<TokenResponse> GenerateAccessToken(int userId)
        {
            var user = await uow.Users.GetUserWithRolesAndClaimsAsync(userId) ?? throw new ArgumentException("");
            var claims = new List<System.Security.Claims.Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, $"{user.GetFullName()}"),
                new(ClaimTypes.Email, user.Email),
            };

            claims.AddRange(
                user.UserRoles.Select(ur => new System.Security.Claims.Claim(ClaimTypes.Role, ur.Role.Name)));

            claims.AddRange(
                user.UserRoles
                    .SelectMany(ur => ur.Role.RoleClaims)
                    .Select(rc => new System.Security.Claims.Claim(rc.Claim.Type, rc.Claim.Value)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = credential,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string? jwt = tokenHandler.WriteToken(securityToken);
            return new TokenResponse(jwt, "", expiration, DateTime.Now);
        }

        private static string GenerateRefreshToken()
        {
            byte[] randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private static string HashToken(string refreshToken)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToBase64String(hash);
        }

        public async Task<TokenResponse> GenerateTokens(int userId, string ipAddress, string userAgent)
        {
            TokenResponse token = await GenerateAccessToken(userId);

            string rawRefreshToken = GenerateRefreshToken();
            var refreshTokenHash = HashToken(rawRefreshToken);

            DateTime refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays);

            await uow.UserSessions.AddAsync(new UserSession
            (
                userId,
                refreshTokenHash,
                ipAddress,
                userAgent,
                refreshTokenExpiry
            ));
            await uow.SaveChangesAsync();

            token.RefreshTokenHash = rawRefreshToken;
            token.RefreshTokenExpiration = refreshTokenExpiry;

            return token;
        }

        public async Task<TokenResponse> RotateRefreshToken(string oldRefreshToken, string ipAddress, string userAgent)
        {
            var oldRefreshTokenHash = HashToken(oldRefreshToken);

            var session = await uow.UserSessions
                .FirstOrDefaultAsync(s => s.RefreshTokenHash == oldRefreshTokenHash && !s.IsRevoked);

            if (session == null || !session.IsActive())
                throw new SecurityTokenException("Invalid refresh token.");

            session.Revoke();

            var newRaw = GenerateRefreshToken();
            var newHash = HashToken(newRaw);

            var newSession = new UserSession(
                session.UserId,
                newHash,
                ipAddress,
                userAgent,
                DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays)
            );

            await uow.UserSessions.AddAsync(newSession);
            await uow.SaveChangesAsync();

            var accessToken = await GenerateAccessToken(session.UserId);
            accessToken.RefreshTokenHash = newRaw;
            accessToken.RefreshTokenExpiration = newSession.ExpiresAt;

            return accessToken;
        }

    }
}