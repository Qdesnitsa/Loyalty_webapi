using Loyalty.Domain.Entities;

namespace Loyalty.Api.Contracts.Programs;

public sealed record CreateProgramRequest(
    string Title,
    string? Description,
    ProgramState State,
    DateTime StartDate,
    DateTime FinishDate,
    decimal MinTransactionAmount,
    decimal MaxTransactionAmount,
    string TransactionType,
    CreateAchievementRequest Achievement,
    string CreatedBy);

public sealed record CreateAchievementRequest(
    int TransactionsCountToApplyAchievement,
    CreateRewardRequest Reward);

public sealed record CreateRewardRequest(
    decimal Amount,
    RewardValueType Type,
    RewardTarget Target,
    RewardValueUsageType UsageType);
