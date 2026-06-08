using Loyalty.Domain.Entities;

namespace Loyalty.Api.Contracts.Programs;

/// <summary>Create program request</summary>
/// <param name="Title">Program title</param>
/// <param name="Description">Program description (optional)</param>
/// <param name="State">Program state</param>
/// <param name="StartDate">Program start date</param>
/// <param name="FinishDate">Program finish date</param>
/// <param name="MinTransactionAmount">Minimum transaction amount to apply the program</param>
/// <param name="MaxTransactionAmount">Maximum transaction amount to apply the program</param>
/// <param name="TransactionType">Applicable transaction type (Deposit or Withdrawal)</param>
/// <param name="Achievement">Achievement and reward configuration</param>
/// <param name="CreatedBy">User who created the program</param>
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

/// <summary>Achievement configuration for create/update program requests</summary>
/// <param name="TransactionsCountToApplyAchievement">Number of qualifying transactions required before reward is granted</param>
/// <param name="Reward">Reward configuration</param>
public sealed record CreateAchievementRequest(
    int TransactionsCountToApplyAchievement,
    CreateRewardRequest Reward);

/// <summary>Reward configuration for create/update program requests</summary>
/// <param name="Amount">Reward amount (percent value or fixed amount depending on type)</param>
/// <param name="Type">Reward value type</param>
/// <param name="Target">Reward target</param>
/// <param name="UsageType">How the reward value is applied</param>
public sealed record CreateRewardRequest(
    decimal Amount,
    RewardValueType Type,
    RewardTarget Target,
    RewardValueUsageType UsageType);
