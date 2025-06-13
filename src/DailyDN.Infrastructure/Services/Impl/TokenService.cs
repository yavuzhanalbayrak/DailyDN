using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public async Task<TokenResponse> CreateToken(int userId)
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
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                SigningCredentials = credential,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string? jwt = tokenHandler.WriteToken(securityToken);
            return new TokenResponse
            {
                Token = jwt,
                UserId = user.Id,
            };
        }
    }
}