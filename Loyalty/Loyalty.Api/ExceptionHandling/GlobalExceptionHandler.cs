using System.Text.Json;
using Loyalty.Application.Common;
using Loyalty.Application.Programs;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Loyalty.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    IHostEnvironment environment,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        else
            logger.LogWarning(exception, "Request failed with {StatusCode} for {Method} {Path}",
                statusCode, httpContext.Request.Method, httpContext.Request.Path);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = GetDetail(exception, statusCode),
            Instance = httpContext.Request.Path
        };

        if (exception is ProgramAlreadyExistsException alreadyExists)
            problem.Extensions["programId"] = alreadyExists.ProgramId;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title) MapException(Exception exception) =>
        exception switch
        {
            ProgramAlreadyExistsException => (StatusCodes.Status409Conflict, "Program already exists"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Validation failed"),
            JsonException => (StatusCodes.Status400BadRequest, "Invalid JSON"),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Invalid request"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

    private string? GetDetail(Exception exception, int statusCode)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError && !environment.IsDevelopment())
            return null;

        return exception.Message;
    }
}
