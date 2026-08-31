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
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<UnitOfWork.IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<ISmsProvider, FakeSmsProvider>();
            services.AddSingleton<RedisConnectionFactory>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IMailService, SmtpMailService>();
            services.AddScoped<IMailTemplateService, MailTemplateService>();
            services.AddScoped<IFileStorageService, FileStorageService>();

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
                    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
                        onBreak: (ex, _) => logger.LogError(ex, "Redis Circuit Breaker opened for 30 seconds due to consecutive failures."),
                        onReset: () => logger.LogInformation("Redis Circuit Breaker closed. Normal cache operations resumed."),
                        onHalfOpen: () => logger.LogWarning("Redis Circuit Breaker half-open. Testing cache connectivity."));

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