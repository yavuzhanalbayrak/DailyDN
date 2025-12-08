
using Microsoft.Extensions.Logging;

namespace DailyDN.Infrastructure.Services.Impl
{
    public class SmsService(ISmsProvider provider, ILogger<SmsService> logger) : ISmsService
    {
        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            logger.LogInformation("SMS sending started to {Phone}", phoneNumber);

            await provider.SendAsync(phoneNumber, message);

            logger.LogInformation("SMS sent to {Phone}", phoneNumber);
        }
    }
}