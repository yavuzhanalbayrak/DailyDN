using Microsoft.Extensions.Logging;

namespace DailyDN.Infrastructure.Services.Impl;

public class FakeSmsProvider(ILogger<FakeSmsProvider> logger) : ISmsProvider
{
    public Task SendAsync(string phoneNumber, string message)
    {
        logger.LogWarning(
            "[FAKE SMS] → To: {Phone} | Message: {Message}",
            phoneNumber,
            message
        );

        return Task.CompletedTask;
    }
}