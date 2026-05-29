namespace Loyalty.Infrastructure.Options;

public sealed class MongoOptions
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; init; } = string.Empty;

    public string Database { get; init; } = string.Empty;
}
