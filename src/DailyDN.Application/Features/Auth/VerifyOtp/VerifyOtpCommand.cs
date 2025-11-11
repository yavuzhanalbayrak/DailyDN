using DailyDN.Application.Messaging;

namespace DailyDN.Application.Features.Auth.VerifyOtp
{
    public record VerifyOtpCommand(Guid Guid, string Otp) : ICommand<VerifyOtpCommandResponse>;
}