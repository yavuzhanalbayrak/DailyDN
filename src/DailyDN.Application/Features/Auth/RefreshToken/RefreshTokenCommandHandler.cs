using AutoMapper;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;

namespace DailyDN.Application.Features.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler(IAuthService authService, IMapper mapper) : ICommandHandler<RefreshTokenCommand, RefreshTokenCommandResponse>
    {
        public async Task<Result<RefreshTokenCommandResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tokenResponse = await authService.RefreshTokenAsync(request.RefreshToken);
                if (tokenResponse is null)
                    return Result.Failure<RefreshTokenCommandResponse>(new Error("Otp.Invalid", "OTP is invalid or has expired."));

                var response = mapper.Map<RefreshTokenCommandResponse>(tokenResponse);

                return Result.SuccessWithMessage(response, "Token refreshed successfully.");
            }
            catch (System.Exception)
            {
                return Result.Failure<RefreshTokenCommandResponse>(new Error("InvalidToken", "Invalid or expired refresh token."));
            }
        }
    }
}