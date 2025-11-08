using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DailyDN.Application.Features.Auth.ForgotPassword
{
    public class ForgotPasswordCommandHandler(ILogger<ForgotPasswordCommandHandler> logger, IAuthService authService) : ICommandHandler<ForgotPasswordCommand>
    {
        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Password reset requested for user with email: {Email}", request.Email);
            await authService.ForgotPasswordAsync(request.Email);

            return Result.SuccessWithMessage("If an account with this email exists, a password reset link has been sent.");
        }
    }
}