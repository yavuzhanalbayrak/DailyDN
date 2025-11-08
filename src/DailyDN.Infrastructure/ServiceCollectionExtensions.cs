using DailyDN.Infrastructure.Contexts;
using DailyDN.Infrastructure.Redis;
using DailyDN.Infrastructure.Repositories;
using DailyDN.Infrastructure.Repositories.Impl;
using DailyDN.Infrastructure.Services;
using DailyDN.Infrastructure.Services.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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


            return services;
        }
    }
}