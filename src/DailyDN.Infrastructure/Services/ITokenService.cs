using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Models;

namespace DailyDN.Infrastructure.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> CreateToken(int userId);

    }
}