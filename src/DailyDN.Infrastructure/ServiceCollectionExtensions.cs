using DailyDN.Infrastructure.Contexts;
using DailyDN.Infrastructure.Repositories;
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
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}