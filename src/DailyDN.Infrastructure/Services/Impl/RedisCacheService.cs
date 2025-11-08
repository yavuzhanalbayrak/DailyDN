using System.Text.Json;
using DailyDN.Infrastructure.Redis;
using StackExchange.Redis;

namespace DailyDN.Infrastructure.Services.Impl
{
    public class RedisCacheService(RedisConnectionFactory redisConnectionFactory) : ICacheService
    {
        private readonly IDatabase _database = redisConnectionFactory.Connection.GetDatabase();

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }
    }
}