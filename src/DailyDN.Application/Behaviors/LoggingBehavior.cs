using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using DailyDN.Infrastructure.Services;
using DailyDN.Application.Common.Attributes;

namespace DailyDN.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    IAuthenticatedUser? authenticatedUser = null
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var userInfo = authenticatedUser?.UserId.ToString() ?? "Anonymous";

        var loggableProperties = request.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<DoNotLogAttribute>() == null)
            .ToDictionary(p => p.Name, p => p.GetValue(request));

        logger.LogInformation("Handling {RequestName} by user ID: {@User}. Payload: {@Request}",
            typeof(TRequest).Name,
            userInfo,
            loggableProperties
        );

        var response = await next(cancellationToken);

        logger.LogInformation("Handled {RequestName} by user ID: {@User}",
            typeof(TRequest).Name,
            userInfo
        );

        return response;
    }
}
