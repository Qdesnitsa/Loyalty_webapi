using Loyalty.Domain.Entities;
using ApplicationProgram = Loyalty.Application.Programs.Models.Program;

namespace Loyalty.Api.Contracts.Programs;

public sealed record Program(
    string Id,
    string Title,
    string? Description,
    ProgramState State,
    DateTime StartDate,
    DateTime FinishDate,
    decimal MinTransactionAmount,
    decimal MaxTransactionAmount,
    Achievement Achievement,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime UpdatedAt,
    string UpdatedBy,
    bool IsDeleted)
{
    public static Program FromApplication(ApplicationProgram program) =>
        new(
            program.Id,
            program.Title,
            program.Description,
            program.State,
            program.StartDate,
            program.FinishDate,
            program.MinTransactionAmount,
            program.MaxTransactionAmount,
            Achievement.FromApplication(program.Achievement),
            program.CreatedAt,
            program.CreatedBy,
            program.UpdatedAt,
            program.UpdatedBy,
            program.IsDeleted);
}

public sealed record Achievement(
    string Id,
    int? TransactionsCountToApplyAchievement,
    Reward Reward)
{
    public static Achievement FromApplication(Application.Programs.Models.Achievement achievement) =>
        new(
            achievement.Id,
            achievement.TransactionsCountToApplyAchievement,
            Reward.FromApplication(achievement.Reward));
}

public sealed record Reward(
    string Id,
    decimal Amount,
    RewardValueType Type,
    RewardTarget Target,
    RewardValueUsageType UsageType)
{
    public static Reward FromApplication(Application.Programs.Models.Reward reward) =>
        new(reward.Id, reward.Amount, reward.Type, reward.Target, reward.UsageType);
}

public sealed record CreateProgramResponse(Program Program);

public sealed record UpdateProgramResponse(Program Program);

public sealed record GetProgramResponse(Program Program);
