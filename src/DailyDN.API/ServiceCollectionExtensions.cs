
using System.Text;
using DailyDN.API.Middleware;
using DailyDN.Application;
using DailyDN.Infrastructure.Contexts;
using DailyDN.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

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

            services.AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = configuration["JwtSettings:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = configuration["JwtSettings:Audience"],
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                StringValues accessToken = context.Request.Headers["access_token"];
                                PathString path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    path.StartsWithSegments("/hubs", StringComparison.OrdinalIgnoreCase))
                                {
                                    context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            },
                            OnAuthenticationFailed = ctx =>
                            {
                                var errorDetails = new
                                {
                                    message = "Token is invalid or expired.",
                                    error = "token-invalid"
                                };

                                ctx.Response.StatusCode = 401;
                                ctx.Response.ContentType = "application/json";
                                return ctx.Response.WriteAsJsonAsync(errorDetails);

                            }
                        };
                    });

            services.AddAuthorization();
            services.AddScoped<AuthenticatedUserMiddleware>();

            services.Configure<RedisSettings>(
                configuration.GetSection("Redis"));

            services.AddHealthChecks();


            return services;
        }
    }
}