using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DailyDN.Application.Features.Auth.Register
{
    public class RegisterCommandHandler(IAuthService authService, ILogger<RegisterCommandHandler> logger)
        : ICommandHandler<RegisterCommand>
    {
        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("User registering. Email: {Email}", request.Email);

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