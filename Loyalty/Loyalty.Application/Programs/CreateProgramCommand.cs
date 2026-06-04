using Loyalty.Domain.Entities;
using ApplicationProgram = Loyalty.Application.Programs.Models.Program;

namespace Loyalty.Application.Programs;

public sealed record CreateProgramCommand(
    string Title,
    string? Description,
    ProgramState State,
    DateTime StartDate,
    DateTime FinishDate,
    decimal MinTransactionAmount,
    decimal MaxTransactionAmount,
    string TransactionType,
    CreateProgramAchievementData Achievement,
    string CreatedBy) : ICommand<ApplicationProgram>;

public sealed record CreateProgramAchievementData(
    int TransactionsCountToApplyAchievement,
    CreateProgramRewardData Reward);

public sealed record CreateProgramRewardData(
    decimal Amount,
    RewardValueType Type,
    RewardTarget Target,
    RewardValueUsageType UsageType);
