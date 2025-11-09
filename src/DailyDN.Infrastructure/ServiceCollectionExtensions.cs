using DailyDN.Infrastructure.Contexts;
using DailyDN.Infrastructure.Redis;
using DailyDN.Infrastructure.Repositories;
using DailyDN.Infrastructure.Repositories.Impl;
using DailyDN.Infrastructure.Services;
using DailyDN.Infrastructure.Services.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using StackExchange.Redis;

namespace DailyDN.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IApplicationContext, DailyDNDbContext>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddSingleton<RedisConnectionFactory>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddSingleton<AsyncPolicy>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RedisCacheService>>();

                var retryPolicy = Policy
                    .Handle<RedisConnectionException>()
                    .Or<RedisTimeoutException>()
                    .Or<RedisServerException>()
                    .WaitAndRetryAsync(2, attempt => TimeSpan.FromMilliseconds(200 * attempt),
                        (ex, ts, count, ctx) => logger.LogWarning(ex, "Redis retry {Count}", count));

                var circuitBreakerPolicy = Policy
                    .Handle<RedisConnectionException>()
                    .Or<RedisTimeoutException>()
                    .Or<RedisServerException>()
                    .CircuitBreakerAsync(1, TimeSpan.FromMinutes(15),
                        onBreak: (ex, _) => logger.LogError(ex, "Circuit opened"),
                        onReset: () => logger.LogInformation("Circuit closed"),
                        onHalfOpen: () => logger.LogWarning("Circuit half-open"));

                var fallbackPolicy = Policy
                    .Handle<Exception>()
                    .FallbackAsync(
                        fallbackAction: async ct =>
                        {
                            logger.LogError("Redis fallback executed — cache bypass");
                            await Task.CompletedTask;
                        },
                        onFallbackAsync: async (ex) =>
                        {
                            logger.LogWarning(ex, "Redis fallback triggered due to exception: {Message}", ex.Message);
                            await Task.CompletedTask;
                        });

                return Policy.WrapAsync(fallbackPolicy, circuitBreakerPolicy, retryPolicy);
            });

            return services;
        }
    }
}