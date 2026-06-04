namespace Loyalty.Api.Authentication;

public sealed class ApiKeyOptions
{
    public const string SectionName = "ApiKey";
    public const string HeaderName = "X-Loyalty-Api-Key";
    public const string SchemeName = "ApiKey";

    public required string Key { get; init; }
}
