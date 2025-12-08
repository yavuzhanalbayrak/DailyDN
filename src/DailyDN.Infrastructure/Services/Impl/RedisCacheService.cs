using System.Text.Json;
using DailyDN.Infrastructure.Redis;
using Microsoft.Extensions.Logging;
using Polly;
using StackExchange.Redis;

namespace DailyDN.Infrastructure.Services.Impl
{
    public class RedisCacheService(AsyncPolicy policyWrap, ILogger<RedisCacheService> logger, RedisConnectionFactory redisConnectionFactory) : ICacheService
    {
        private readonly IDatabase _database = redisConnectionFactory.Connection.GetDatabase();

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            logger.LogDebug("Attempting to cache value for key: {Key}", key);

            await policyWrap.ExecuteAsync(async () =>
            {
                var json = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key, json, expiry);
                logger.LogInformation("Cached key: {Key} with expiry: {Expiry}", key, expiry);
            });
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            logger.LogDebug("Attempting to retrieve key: {Key}", key);

            T? result = default;

            await policyWrap.ExecuteAsync(async () =>
            {
                var value = await _database.StringGetAsync(key);
                if (value.IsNullOrEmpty)
                {
                    logger.LogDebug("Cache miss for key: {Key}", key);
                    return;
                }

                result = JsonSerializer.Deserialize<T>(value!);
                logger.LogDebug("Cache hit for key: {Key}", key);
            });

            if (EqualityComparer<T>.Default.Equals(result, default))
                logger.LogTrace("No value returned for key: {Key}", key);

            return result;
        }

        public async Task RemoveAsync(string key)
        {
            logger.LogDebug("Attempting to remove cache entry for key: {Key}", key);

            await policyWrap.ExecuteAsync(async () =>
            {
                await _database.KeyDeleteAsync(key);
                logger.LogInformation("Cache entry removed for key: {Key}", key);
            });
        }


        public async Task<bool> ExistsAsync(string key)
        {
            logger.LogDebug("Checking if cache key exists: {Key}", key);


            bool exists = await policyWrap.ExecuteAsync(async () =>
            {
                var value = await _database.KeyExistsAsync(key);
                logger.LogDebug("Cache key {Key} existence: {Exists}", key, value);
                return value;
            });

            if (exists)
                logger.LogTrace("Cache entry confirmed for key: {Key}", key);
            else
                logger.LogTrace("Cache entry not found for key: {Key}", key);

            return exists;
        }
    }
}