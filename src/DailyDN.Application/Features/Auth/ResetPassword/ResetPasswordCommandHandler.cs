using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;

namespace DailyDN.Application.Features.Auth.ResetPassword
{
    public class ResetPasswordCommandHandler(IAuthService authService) : ICommandHandler<ResetPasswordCommand>
    {
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return await authService.ResetPasswordAsync(request.Token, request.NewPassword);
        }
    }
}