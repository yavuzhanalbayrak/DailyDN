using DailyDN.API.Middleware.Model;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using ValidationException = FluentValidation.ValidationException;

namespace DailyDN.API.Middleware
{
    public class ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> _logger, IAuthenticatedUser _authenticatedUser) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, context, _logger, _authenticatedUser);
            }
        }

        private static async Task HandleExceptionAsync(Exception ex, HttpContext context, ILogger<ErrorHandlerMiddleware> logger, IAuthenticatedUser authenticatedUser)
        {
            switch (ex)
            {
                case ValidationException validationException:
                    await HandleValidationException(validationException, context);
                    break;
                // case AuthorizationException authEx:
                //     await HandleAuthorizationException(authEx, context);
                //     break;
                default:
                    await HandleGenericException(ex, context, logger);
                    break;

            }
        }
        private static async Task HandleValidationException(ValidationException ex, HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var problemDetails = new ValidationProblemDetails(
                ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Title = "Validation Error",
                Detail = "One or more fields are invalid. Please correct the errors and try again.",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.Request.Path,
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }




        // private static async Task HandleAuthorizationException(AuthorizationException ex, HttpContext context)
        // {
        //     context.Response.StatusCode = ex.StatusCode;

        //     await context.Response.WriteAsJsonAsync(Result.Failure(new Error(
        //         ex.Code,
        //         ex.Message
        //     )));
        // }


        private static async Task HandleGenericException(Exception ex, HttpContext context, ILogger<ErrorHandlerMiddleware> logger)
        {
            logger.LogError(ex, "Unexpected error. Path:{Path}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsJsonAsync(new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error. Kindly contact support"
            }.ToString());
        }
    }
}