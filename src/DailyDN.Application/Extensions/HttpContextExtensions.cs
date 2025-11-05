using System.Globalization;
using System.Security.Claims;

namespace DailyDN.Application.Extensions;

public static class HttpContextExtensions
{
    public static T? GetUserId<T>(this ClaimsPrincipal claimsPrincipal)
    {
        string? userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return default;
        }

        return (T)Convert.ChangeType(userId, typeof(T), CultureInfo.InvariantCulture);
    }
}
