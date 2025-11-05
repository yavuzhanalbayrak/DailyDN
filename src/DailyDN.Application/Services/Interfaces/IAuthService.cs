using DailyDN.Application.Common.Model;
using DailyDN.Infrastructure.Models;

namespace DailyDN.Application.Services.Interfaces;

public interface IAuthService
{
    public Task<Result> LoginAsync(string Email, string Password);
    public Task<Result> RegisterAsync(
        string Name,
        string Surname,
        string Email,
        string Password,
        CancellationToken cancellationToken
    );
    public Task<TokenResponse?> VerifyOtpAsync(Guid Guid, string Otp);
}