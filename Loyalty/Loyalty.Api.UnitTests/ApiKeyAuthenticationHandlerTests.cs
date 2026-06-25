using System.Security.Claims;
using System.Text.Encodings.Web;
using Loyalty.Api.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;

namespace Loyalty.Api.UnitTests;

public class ApiKeyAuthenticationHandlerTests
{
    private const string ValidApiKey = "test-api-key";

    [Fact]
    public async Task AuthenticateAsync_WhenHeaderMissing_ReturnsFail()
    {
        // Arrange
        Action<DefaultHttpContext> configureContext = _ => { };

        // Act
        var result = await AuthenticateAsync(configureContext, ValidApiKey);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Failure?.Message.ShouldBe("Missing API key.");
    }

    [Fact]
    public async Task AuthenticateAsync_WhenHeaderEmpty_ReturnsFail()
    {
        // Arrange
        Action<DefaultHttpContext> configureContext =
            context => context.Request.Headers[ApiKeyOptions.HeaderName] = string.Empty;

        // Act
        var result = await AuthenticateAsync(configureContext, ValidApiKey);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Failure?.Message.ShouldBe("Missing API key.");
    }

    [Fact]
    public async Task AuthenticateAsync_WhenHeaderWhitespace_ReturnsFail()
    {
        // Arrange
        Action<DefaultHttpContext> configureContext =
            context => context.Request.Headers[ApiKeyOptions.HeaderName] = "   ";

        // Act
        var result = await AuthenticateAsync(configureContext, ValidApiKey);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Failure?.Message.ShouldBe("Missing API key.");
    }

    [Fact]
    public async Task AuthenticateAsync_WhenApiKeyNotConfigured_ReturnsFail()
    {
        // Arrange
        Action<DefaultHttpContext> configureContext =
            context => context.Request.Headers[ApiKeyOptions.HeaderName] = ValidApiKey;

        // Act
        var result = await AuthenticateAsync(configureContext, apiKey: string.Empty);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Failure?.Message.ShouldBe("API key is not configured.");
    }

    [Fact]
    public async Task AuthenticateAsync_WhenApiKeyInvalid_ReturnsFail()
    {
        // Arrange
        Action<DefaultHttpContext> configureContext =
            context => context.Request.Headers[ApiKeyOptions.HeaderName] = "wrong-key";

        // Act
        var result = await AuthenticateAsync(configureContext, ValidApiKey);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Failure?.Message.ShouldBe("Invalid API key.");
    }

    [Fact]
    public async Task AuthenticateAsync_WhenApiKeyValid_ReturnsSuccessWithClaim()
    {
        // Arrange
        Action<DefaultHttpContext> configureContext =
            context => context.Request.Headers[ApiKeyOptions.HeaderName] = ValidApiKey;

        // Act
        var result = await AuthenticateAsync(configureContext, ValidApiKey);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Principal?.Identity?.AuthenticationType.ShouldBe(ApiKeyOptions.SchemeName);
        result.Principal?.FindFirst(ClaimTypes.Name)?.Value.ShouldBe("api-key");
    }

    private static async Task<AuthenticateResult> AuthenticateAsync(
        Action<DefaultHttpContext> configureContext,
        string apiKey)
    {
        var context = new DefaultHttpContext();
        configureContext(context);

        var handler = CreateHandler(apiKey);
        await handler.InitializeAsync(
            new AuthenticationScheme(ApiKeyOptions.SchemeName, null, typeof(ApiKeyAuthenticationHandler)),
            context);

        return await handler.AuthenticateAsync();
    }

    private static ApiKeyAuthenticationHandler CreateHandler(string apiKey)
    {
        var optionsMonitor = Substitute.For<IOptionsMonitor<AuthenticationSchemeOptions>>();
        optionsMonitor.Get(Arg.Any<string>()).Returns(new AuthenticationSchemeOptions());

        return new ApiKeyAuthenticationHandler(
            optionsMonitor,
            NullLoggerFactory.Instance,
            UrlEncoder.Default,
            Options.Create(new ApiKeyOptions { Key = apiKey }));
    }
}
