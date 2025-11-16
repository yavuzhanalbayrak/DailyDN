using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;

namespace DailyDN.Application.Features.Auth.VerifyEmail
{
    public class VerifyEmailCommandHandler(IAuthService authService) : ICommandHandler<VerifyEmailCommand>
    {
        public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            return await authService.VerifyEmailAsync(request.Token);
        }
    }
}