using Loyalty.Domain.Entities;

namespace Loyalty.Application.Transactions;

public static class RewardCalculator
{
    public static decimal Calculate(Reward reward, decimal transactionAmount)
    {
        if (reward.Target != RewardTarget.Bonus || reward.UsageType != RewardValueUsageType.Add)
        {
            return 0;
        }

        var rawAmount = reward.Type switch
        {
            RewardValueType.Percent => transactionAmount * reward.Amount / 100m,
            RewardValueType.Fixed => reward.Amount,
            _ => 0m
        };

        return Math.Round(rawAmount, 2, MidpointRounding.AwayFromZero);
    }
}
