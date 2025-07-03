
using DailyDN.API.Middleware;
using DailyDN.Application;
using DailyDN.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DailyDN.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediator(ApplicationAssembly.Instance, ApiAssembly.Instance);
            services.AddTransient<ErrorHandlerMiddleware>();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddDbContext<DailyDNDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            return services;
        }
    }
}