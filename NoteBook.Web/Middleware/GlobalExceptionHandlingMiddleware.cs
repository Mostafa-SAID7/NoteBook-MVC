namespace NoteBook.Web.Middleware;

using FluentValidation;
using NoteBook.Domain.Exceptions;
using System.Net;
using System.Text.Json;

/// <summary>
/// Global exception handling middleware for consistent error responses
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = "An error occurred while processing your request.",
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Validation failed";
                response.Errors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                break;

            case NoteNotFoundException noteNotFoundEx:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = noteNotFoundEx.Message;
                break;

            case DomainException domainEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = domainEx.Message;
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Message = "Unauthorized access.";
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = argEx.Message;
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "Internal server error. Please try again later.";
                response.TraceId = context.TraceIdentifier;
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }

    /// <summary>
    /// Standard error response model
    /// </summary>
    private class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? TraceId { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
