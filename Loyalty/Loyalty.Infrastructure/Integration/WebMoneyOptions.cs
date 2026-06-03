namespace Loyalty.Infrastructure.Integration;

public sealed class WebMoneyOptions
{
    public const string SectionName = "WebMoney";
    public required string BaseUrl { get; init; }
    public required string ApiKey { get; init; }
}
