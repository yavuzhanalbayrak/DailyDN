using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;

namespace DailyDN.Application.Features.Auth.VerifyOtp
{
    public class VerifyOtpCommandHandler(
        IMapper mapper,
        IOtpService otpService
    ) : ICommandHandler<VerifyOtpCommand, VerifyOtpCommandResponse>
    {
        public async Task<Result<VerifyOtpCommandResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var token = await otpService.VerifyOtpAsync(request.Guid, request.Otp);

            if (token is null)
                return Result.Failure<VerifyOtpCommandResponse>(new Error("Otp.Invalid", "OTP is invalid or has expired."));

            var response = mapper.Map<VerifyOtpCommandResponse>(token);
            return Result.Success(response);
        }
    }
}