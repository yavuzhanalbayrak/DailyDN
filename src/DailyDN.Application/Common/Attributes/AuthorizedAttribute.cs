using DailyDN.Application.Exceptions;
using DailyDN.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DailyDN.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizedAttribute(string requiredClaim) : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                throw new AuthorizationException("Unauthorized", 401, "User is not authenticated.");
            }

            var authenticatedUser = context.HttpContext.RequestServices.GetRequiredService<IAuthenticatedUser>();

            if (authenticatedUser.Claims == null || !authenticatedUser.Claims.Contains(requiredClaim))
            {
                throw new AuthorizationException("Forbidden", 403, $"User is not authorized. Required claim: {requiredClaim}");
            }
        }
    }
}