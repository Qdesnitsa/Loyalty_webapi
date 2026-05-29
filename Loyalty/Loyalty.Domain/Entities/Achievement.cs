namespace Loyalty.Domain.Entities;

public class Achievement
{
    public required string Id { get; init; }
    public int? TransactionsCountToApplyAchievement { get; init; }
    public required Reward Reward { get; init; }
}
