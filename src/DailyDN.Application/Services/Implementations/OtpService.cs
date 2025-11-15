using DailyDN.Application.Dtos.Otp;
using DailyDN.Application.Services.Interfaces;

namespace DailyDN.Application.Services.Implementations
{
    public class OtpService : IOtpService
    {
        public OtpDto CreateOtp()
        {
            return new OtpDto();
        }
    }
}