using DailyDN.Application.Dtos.Otp;

namespace DailyDN.Application.Services.Interfaces
{
    public interface IOtpService
    {
        OtpDto CreateOtp();
    }
}