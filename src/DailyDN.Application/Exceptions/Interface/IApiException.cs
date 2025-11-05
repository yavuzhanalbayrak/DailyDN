using DailyDN.Application.Exceptions.Enum;

namespace DailyDN.Application.Exceptions.Interface
{
    public interface IApiException
    {
        public FailCode FailCode { get; set; }
        public int StatusCode { get; }
    }
}
