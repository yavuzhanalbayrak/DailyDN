using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DailyDN.Application.Features.Auth.ResetPassword
{
    public class ResetPasswordCommandHandler(IAuthService authService, ILogger<ResetPasswordCommandHandler> logger) : ICommandHandler<ResetPasswordCommand>
    {
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("User password is reseting.");
            return await authService.ResetPasswordAsync(request.Token, request.NewPassword);
        }
    }
}