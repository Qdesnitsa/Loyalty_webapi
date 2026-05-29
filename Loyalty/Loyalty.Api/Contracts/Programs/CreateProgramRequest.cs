using Loyalty.Domain.Entities;

namespace Loyalty.Api.Contracts.Programs;

public sealed record CreateProgramRequest(
    string Id,
    string Title,
    string? Description,
    ProgramState State,
    DateTime StartDate,
    DateTime FinishDate,
    decimal MinTransactionAmount,
    decimal MaxTransactionAmount,
    AchievementRequest Achievement,
    string CreatedBy);

public sealed record AchievementRequest(
    string Id,
    int? TransactionsCountToApplyAchievement,
    RewardRequest Reward);

public sealed record RewardRequest(
    decimal Amount,
    RewardValueType Type,
    RewardTarget Target,
    RewardValueUsageType UsageType);
