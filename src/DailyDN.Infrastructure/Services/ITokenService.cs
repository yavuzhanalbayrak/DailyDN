using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Models;

namespace DailyDN.Infrastructure.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateTokens(int userId, string ipAddress, string userAgent);
        Task<TokenResponse> RotateRefreshToken(string oldRefreshToken, string ipAddress, string userAgent);
    }
}