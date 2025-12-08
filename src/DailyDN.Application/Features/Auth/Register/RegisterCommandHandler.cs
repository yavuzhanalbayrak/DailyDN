using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;

namespace DailyDN.Application.Features.Auth.Register
{
    public class RegisterCommandHandler(IAuthService authService)
        : ICommandHandler<RegisterCommand>
    {
        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await authService.RegisterAsync(
                request.Name,
                request.Surname,
                request.Email,
                request.PhoneNumber,
                request.Password,
                cancellationToken
            );
        }
    }

}