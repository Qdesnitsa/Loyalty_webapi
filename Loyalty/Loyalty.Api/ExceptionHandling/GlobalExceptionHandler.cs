using System.Text.Json;
using Loyalty.Application;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Loyalty.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    IHostEnvironment environment,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private static readonly Dictionary<string, int> ApplicationErrors = new()
    {
        [ExceptionTypes.ResourceNotFound] = StatusCodes.Status404NotFound,
        [ExceptionTypes.ResourceAlreadyExists] = StatusCodes.Status409Conflict
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = CreateProblemDetails(httpContext, exception);
        var statusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "Request failed with {StatusCode} for {Method} {Path}",
                statusCode, httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }

    private ProblemDetails CreateProblemDetails(HttpContext httpContext, Exception exception) =>
        exception switch
        {
            InvalidQueryException invalidQuery => CreateApplicationProblemDetails(httpContext, invalidQuery.Title, invalidQuery.Type),
            InvalidCommandException invalidCommand => CreateApplicationProblemDetails(httpContext, invalidCommand.Title, invalidCommand.Type),
            ArgumentException argument => CreateProblemDetails(httpContext, StatusCodes.Status400BadRequest, "Validation failed", argument.Message),
            JsonException => CreateProblemDetails(httpContext, StatusCodes.Status400BadRequest, "Invalid JSON", exception.Message),
            BadHttpRequestException => CreateProblemDetails(httpContext, StatusCodes.Status400BadRequest, "Invalid request", exception.Message),
            _ => CreateProblemDetails(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                GetDetail(exception, StatusCodes.Status500InternalServerError))
        };

    private static ProblemDetails CreateApplicationProblemDetails(HttpContext httpContext, string title, string type)
    {
        var statusCode = ApplicationErrors.GetValueOrDefault(type, StatusCodes.Status400BadRequest);

        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = title,
            Instance = httpContext.Request.Path
        };
    }

    private ProblemDetails CreateProblemDetails(HttpContext httpContext, int statusCode, string title, string? detail) =>
        new()
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

    private string? GetDetail(Exception exception, int statusCode)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError && !environment.IsDevelopment())
        {
            return null;
        }

        return exception.Message;
    }
}
