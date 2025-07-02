using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.VerifyOtp
{
    public record VerifyOtpCommand(string Guid, int Otp) : ICommand;
}