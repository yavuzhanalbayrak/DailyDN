
namespace DailyDN.Application.Exceptions
{
    public class AuthorizationException(string code, int statusCode, string message) : Exception(message)
    {
        public string Code { get; } = code;
        public int StatusCode { get; } = statusCode;
    }

}
