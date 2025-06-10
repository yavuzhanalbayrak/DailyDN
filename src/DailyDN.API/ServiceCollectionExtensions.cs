
using DailyDN.API.Middleware;
using DailyDN.Application;
using Microsoft.AspNetCore.Mvc;

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

            return services;
        }
    }
}