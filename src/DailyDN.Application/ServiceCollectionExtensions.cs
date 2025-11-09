using Microsoft.Extensions.DependencyInjection;
using DailyDN.Application.Behaviors;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using DailyDN.Application.Profiles;
using FluentValidation;
using DailyDN.Infrastructure.Services;
using DailyDN.Infrastructure.Services.Impl;
using Microsoft.AspNetCore.Identity;
using DailyDN.Application.Services.Interfaces;
using DailyDN.Application.Services.Implementations;

namespace DailyDN.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies) =>
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddValidatorsFromAssembly(ApplicationAssembly.Instance);
            services.AddScoped<IAuthenticatedUser, AuthenticatedUser>();
            services.AddScoped<IPostsService, PostService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
            services.AddHttpContextAccessor();
            
            return services;
        }
    }
}
