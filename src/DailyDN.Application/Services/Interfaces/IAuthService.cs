using DailyDN.Application.Common.Model;
using DailyDN.Infrastructure.Models;

namespace DailyDN.Application.Services.Interfaces;

public interface IAuthService
{
    public Task<Result> LoginAsync(string email, string password);
    public Task<Result> RegisterAsync(
        string name,
        string surname,
        string email,
        string phoneNumber,
        string password,
        CancellationToken cancellationToken
    );
    public Task<TokenResponse?> VerifyOtpAsync(Guid guid, string otp);

    public Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
    public Task ForgotPasswordAsync(string email);
    public Task<Result> ResetPasswordAsync(Guid token, string newPassword);
}
