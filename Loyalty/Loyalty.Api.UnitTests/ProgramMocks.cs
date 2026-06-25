using Loyalty.Domain.Entities;
using DomainProgram = Loyalty.Domain.Entities.Program;

namespace Loyalty.Api.UnitTests;

internal static class ProgramMocks
{
    public static DomainProgram Default =>
        DomainProgram.Create(
            "default-program",
            "Default Program",
            "Default program description",
            ProgramState.Active,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(30),
            100,
            1000,
            "Deposit",
            new Achievement
            {
                Id = "achievement-1",
                TransactionsCountToApplyAchievement = 1,
                Reward = new Reward
                {
                    Id = "reward-1",
                    Amount = 10,
                    Type = RewardValueType.Percent,
                    Target = RewardTarget.Bonus,
                    UsageType = RewardValueUsageType.Add
                }
            },
            "test-user");
}
