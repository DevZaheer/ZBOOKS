using System.Net;
using System.Text.Json;

namespace ZBooksApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (InvalidOperationException ex)
        {
           
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
           
            await WriteResponseAsync(context, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing {Path}", context.Request.Path);
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(payload);
    }
}
