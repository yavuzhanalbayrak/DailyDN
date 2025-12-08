using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;

namespace DailyDN.Application.Features.Auth.ForgotPassword
{
    public class ForgotPasswordCommandHandler(IAuthService authService) : ICommandHandler<ForgotPasswordCommand>
    {
        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            await authService.ForgotPasswordAsync(request.Email);

            return Result.SuccessWithMessage("If an account with this email exists, a password reset link has been sent.");
        }
    }
}