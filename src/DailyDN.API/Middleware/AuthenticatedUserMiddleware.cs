using System.Security.Claims;
using DailyDN.Application.Extensions;
using DailyDN.Infrastructure.Services;

namespace DailyDN.API.Middleware
{
    public class AuthenticatedUserMiddleware(
        IAuthenticatedUser _authenticatedUser
    ) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                _authenticatedUser.UserId = context.User.GetUserId<int>();
                _authenticatedUser.IsAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
                _authenticatedUser.Role = context.User.FindFirst(ClaimTypes.Role)?.Value;

                _authenticatedUser.Claims = [.. context.User
                    .FindAll("Permissions")
                    .Select(c => c.Value)];
            }

            await next(context);
        }
    }
}