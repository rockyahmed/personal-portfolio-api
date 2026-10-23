using System.Net;
using System.Text.Json;
using UserManagement.Application.Common;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Api.Middleware;

/// <summary>
/// Intercepts unhandled exceptions across the HTTP pipeline and returns consistent ApiResponse payloads.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate _next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        this._next = _next;
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
            _logger.LogError(ex, "An unhandled exception occurred during request execution: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            ValidationException valEx => ((int)HttpStatusCode.BadRequest, valEx.Message, valEx.Errors),
            NotFoundException nfEx => ((int)HttpStatusCode.NotFound, nfEx.Message, null),
            ConflictException confEx => ((int)HttpStatusCode.Conflict, confEx.Message, null),
            UnauthorizedAccessException unAuthEx => ((int)HttpStatusCode.Unauthorized, unAuthEx.Message, null),
            AppException appEx => (appEx.StatusCode, appEx.Message, null),
            _ => ((int)HttpStatusCode.InternalServerError, "An unexpected server error occurred. Please try again later.", null)
        };

        context.Response.StatusCode = statusCode;

        var response = ApiResponse.FailureResult(message, statusCode, errors);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
