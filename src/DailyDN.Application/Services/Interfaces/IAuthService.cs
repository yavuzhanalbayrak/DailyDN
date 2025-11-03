using DailyDN.Application.Common.Model;

namespace DailyDN.Application.Services.Interfaces;

public interface IAuthService
{
    public Task LoginAsync();
    public Task<Result> RegisterAsync(
        string Name,
        string Surname,
        string Email,
        string Password,
        CancellationToken cancellationToken
    );
    public Task VerifyOtpAsync();
}