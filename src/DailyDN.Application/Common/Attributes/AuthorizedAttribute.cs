using DailyDN.Application.Exceptions;
using DailyDN.Application.Exceptions.Enum;
using DailyDN.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DailyDN.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class AuthorizedAttribute(string requiredClaim) : Attribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authenticatedUser = context.HttpContext.RequestServices.GetService<IAuthenticatedUser>();

            if (authenticatedUser?.Claims is null || authenticatedUser.Claims.Count == 0)
            {
                throw new ApiAuthenticationException($"Uygulama Yöneticisinden Yetki Tanımlaması Yapmasını İsteyeniz.")
                {
                    FailCode = FailCode.InvalidClaim,
                    StatusCode = 403
                };
            }


            var userRole = authenticatedUser.Claims.Contains(requiredClaim);

            if (!userRole)
            {
                throw new ApiAuthenticationException($"User is not authorized. Claim Name : {requiredClaim}")
                {
                    FailCode = FailCode.InvalidClaim,
                    StatusCode = 403

                };
            }
        }
    }
}