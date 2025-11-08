using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DailyDN.Infrastructure.Redis
{
    public class RedisConnectionFactory(IOptions<RedisSettings> redisSettings)
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection = new(
                () => ConnectionMultiplexer.Connect(redisSettings.Value.ConnectionString)
            );

        public ConnectionMultiplexer Connection => _lazyConnection.Value;
    }
}