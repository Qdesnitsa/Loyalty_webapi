namespace Loyalty.Domain.Entities;

public class Reward
{
    public decimal Amount { get; init; }
    public RewardValueType Type { get; init; }
    public RewardTarget Target { get; init; }
    public RewardValueUsageType UsageType { get; init; }
}

public enum RewardValueType
{
    Percent = 0,
    Fixed = 1
}

public enum RewardTarget
{
    CustomerFee = 0,
    Bonus = 1
}

public enum RewardValueUsageType
{
    Subtract = 0,
    Add = 1,
    Set = 2
}