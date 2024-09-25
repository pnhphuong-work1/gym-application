using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GymApplication.api.Middleware;

public class ExceptionHandlingMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        => _logger = logger;
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError("An error occurred: {Message}", exception.Message);

        await HandleExceptionAsync(httpContext, exception);

        return true;
    }
    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var response = new ProblemDetails()
        {
            Title = "An error occurred",
            Detail = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}