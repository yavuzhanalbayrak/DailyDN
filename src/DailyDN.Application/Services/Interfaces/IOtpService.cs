using DailyDN.Application.Dtos.Otp;
using DailyDN.Infrastructure.Models;

namespace DailyDN.Application.Services.Interfaces
{
    public interface IOtpService
    {
        OtpDto CreateOtp();
        public Task<TokenResponse?> VerifyOtpAsync(Guid guid, string otp);
    }
}