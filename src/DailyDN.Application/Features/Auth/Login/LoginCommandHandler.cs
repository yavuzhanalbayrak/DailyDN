using DailyDN.Application.Common.Model;
using DailyDN.Application.Messaging;
using DailyDN.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DailyDN.Application.Features.Auth.Login;

public class LoginCommandHandler(IAuthService authService, ILogger<LoginCommandHandler> logger) : ICommandHandler<LoginCommand>
{
    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("User logging in. Email: {Email}", request.Email);

        return await authService.LoginAsync(request.Email, request.Password);
    }
}