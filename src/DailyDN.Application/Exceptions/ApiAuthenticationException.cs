using DailyDN.Application.Exceptions.Enum;
using DailyDN.Application.Exceptions.Interface;
using System.Security.Authentication;

namespace DailyDN.Application.Exceptions
{
    public class ApiAuthenticationException : AuthenticationException, IApiException
    {
        public ApiAuthenticationException(string? message, int statusCode, FailCode failCode) : base(message)
        {
            StatusCode = statusCode;
            FailCode = failCode;
        }

        public ApiAuthenticationException(string? message) : base(message)
        {
        }

        public ApiAuthenticationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public required FailCode FailCode { get; set; }
        public int StatusCode { get; set; } = 401;
    }
}
