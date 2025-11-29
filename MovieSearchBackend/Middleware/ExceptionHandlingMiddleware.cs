using MovieSearchBackend.Exceptions;
using System.Net;
using System.Text.Json;

namespace MovieSearchBackend.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        int status = (int)HttpStatusCode.InternalServerError;
        string title = "An unexpected error occurred.";
        string? errorCode = null;
        string? detail = null;

        if (exception is ApiException apiEx)
        {
            status = apiEx.StatusCode;
            title = exception.Message;
            errorCode = apiEx.ErrorCode;
        }

        var problem = new
        {
            title,
            status,
            errorCode,
            detail,
            traceId = context.TraceIdentifier
        };

        context.Response.StatusCode = status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
