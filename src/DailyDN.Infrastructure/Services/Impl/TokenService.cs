using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Contexts;
using DailyDN.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DailyDN.Infrastructure.Services.Impl
{
    public class TokenService(IOptions<JwtSettings> jwtOptions, IApplicationContext context) : ITokenService
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        private async Task<TokenResponse> GenerateAccessToken(int userId)
        {
            var user = await context.Set<User>()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RoleClaims)
                            .ThenInclude(rc => rc.Claim)
                .FirstAsync(u => u.Id == userId);

            var claims = new List<System.Security.Claims.Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, $"{user.Name} {user.Surname}"),
                new(ClaimTypes.Email, user.Email),
            };

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, userRole.Role.Name));

                foreach (var roleClaim in userRole.Role.RoleClaims)
                {
                    claims.Add(new System.Security.Claims.Claim(roleClaim.Claim.Type, roleClaim.Claim.Value));
                }
            }

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

            context.Set<UserSession>().Add(new UserSession
            (
                userId,
                refreshTokenHash,
                ipAddress,
                userAgent,
                refreshTokenExpiry
            ));
            await context.SaveChangesAsync();

            token.RefreshToken = rawRefreshToken;
            token.RefreshTokenExpiration = refreshTokenExpiry;

            return token;
        }

        public async Task<TokenResponse> RotateRefreshToken(string oldRefreshToken, string ipAddress, string userAgent)
        {
            var oldHash = HashToken(oldRefreshToken);

            var session = await context.Set<UserSession>()
                .FirstOrDefaultAsync(s => s.RefreshToken == oldHash && !s.IsRevoked);

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

            context.Set<UserSession>().Add(newSession);
            await context.SaveChangesAsync();

            var accessToken = await GenerateAccessToken(session.UserId);
            accessToken.RefreshToken = newRaw;
            accessToken.RefreshTokenExpiration = newSession.ExpiresAt;

            return accessToken;
        }

    }
}