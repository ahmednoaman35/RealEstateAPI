using RealEstateAPI.Application.DTOs.Common;
using System.Net;
using System.Text.Json;

namespace RealEstateAPI.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            UnauthorizedAccessException => new
            {
                statusCode = (int)HttpStatusCode.Unauthorized,
                response = ApiResponse<object>.FailureResponse(
                    "Unauthorized access",
                    new List<string> { exception.Message })
            },
            ArgumentNullException => new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                response = ApiResponse<object>.FailureResponse(
                    "Bad request",
                    new List<string> { exception.Message })
            },
            InvalidOperationException => new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                response = ApiResponse<object>.FailureResponse(
                    "Invalid operation",
                    new List<string> { exception.Message })
            },
            KeyNotFoundException => new
            {
                statusCode = (int)HttpStatusCode.NotFound,
                response = ApiResponse<object>.FailureResponse(
                    "Resource not found",
                    new List<string> { exception.Message })
            },
            _ => new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                response = ApiResponse<object>.FailureResponse(
                    "An internal server error occurred",
                    new List<string> { exception.Message })
            }
        };

        context.Response.StatusCode = response.statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response.response, jsonOptions));
    }
}

/// <summary>
/// Extension method for registering the middleware
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
