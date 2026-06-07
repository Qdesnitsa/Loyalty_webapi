namespace Loyalty.Infrastructure.BonusOutbox;

public sealed class BonusOutboxJobConfig
{
    public const string SectionName = "BonusOutboxJob";

    public int BatchSize { get; set; } = 200;

    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(3);
}
