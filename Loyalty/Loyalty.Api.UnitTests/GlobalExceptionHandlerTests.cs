using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Loyalty.Api.ExceptionHandling;
using Loyalty.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;

namespace Loyalty.Api.UnitTests;

public class GlobalExceptionHandlerTests
{
    [Theory]
    [InlineData(ExceptionTypes.ResourceNotFound, StatusCodes.Status404NotFound)]
    [InlineData(ExceptionTypes.ResourceAlreadyExists, StatusCodes.Status409Conflict)]
    public async Task TryHandleAsync_ApplicationExceptionTypes_ReturnExpectedStatusCode(
        string exceptionType,
        int expectedStatusCode)
    {
        var (statusCode, problem) = await HandleAsync(
            new InvalidQueryException("Application error", exceptionType));

        statusCode.ShouldBe(expectedStatusCode);
        problem.Type.ShouldBe(exceptionType);
        problem.Status.ShouldBe(expectedStatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_ValidationException_Returns400WithValidationProblemDetails()
    {
        var exception = new ValidationException(
        [
            new ValidationFailure("Title", "Title is required")
        ]);

        var context = CreateHttpContext();
        var handler = CreateHandler();

        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        handled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);

        var problem = await ReadResponseAsync<ValidationProblemDetails>(context);
        problem.Title.ShouldBe("Validation failed");
        problem.Errors.ShouldContainKey("Title");
        problem.Errors["Title"].ShouldContain("Title is required");
    }

    [Fact]
    public async Task TryHandleAsync_UnknownApplicationExceptionType_Returns400()
    {
        var (statusCode, problem) = await HandleAsync(
            new InvalidCommandException("Unknown error", "UnknownType"));

        statusCode.ShouldBe(StatusCodes.Status400BadRequest);
        problem.Type.ShouldBe("UnknownType");
        problem.Title.ShouldBe("Unknown error");
    }

    [Fact]
    public async Task TryHandleAsync_UnhandledException_Returns500()
    {
        var (statusCode, problem) = await HandleAsync(new InvalidOperationException("Unexpected"));

        statusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        problem.Title.ShouldBe("An unexpected error occurred");
    }

    private static GlobalExceptionHandler CreateHandler(IHostEnvironment? environment = null) =>
        new(environment ?? CreateDevelopmentEnvironment(), NullLogger<GlobalExceptionHandler>.Instance);

    private static IHostEnvironment CreateDevelopmentEnvironment()
    {
        var environment = Substitute.For<IHostEnvironment>();
        environment.EnvironmentName.Returns(Environments.Development);
        return environment;
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/programs/test";
        context.Request.Method = HttpMethods.Get;
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<(int StatusCode, ProblemDetails Problem)> HandleAsync(Exception exception)
    {
        var context = CreateHttpContext();
        var handler = CreateHandler();

        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        handled.ShouldBeTrue();
        var problem = await ReadResponseAsync<ProblemDetails>(context);
        return (context.Response.StatusCode, problem);
    }

    private static async Task<T> ReadResponseAsync<T>(HttpContext context) where T : class
    {
        context.Response.Body.Position = 0;
        var result = await JsonSerializer.DeserializeAsync<T>(
            context.Response.Body,
            cancellationToken: CancellationToken.None);

        result.ShouldNotBeNull();
        return result;
    }
}
