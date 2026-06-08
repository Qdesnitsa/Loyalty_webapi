namespace Loyalty.Api.Authentication;

/// <summary>API key authentication options</summary>
public sealed class ApiKeyOptions
{
    /// <summary>Configuration section name</summary>
    public const string SectionName = "ApiKey";

    /// <summary>HTTP header name for the API key</summary>
    public const string HeaderName = "X-Loyalty-Api-Key";

    /// <summary>Authentication scheme name</summary>
    public const string SchemeName = "ApiKey";

    /// <summary>Expected API key value</summary>
    public required string Key { get; init; }
}
