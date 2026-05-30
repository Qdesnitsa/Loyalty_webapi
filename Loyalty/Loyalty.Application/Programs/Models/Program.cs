using Loyalty.Domain.Entities;
using DomainProgram = Loyalty.Domain.Entities.Program;

namespace Loyalty.Application.Programs.Models;

public sealed record Reward(
    string Id,
    decimal Amount,
    RewardValueType Type,
    RewardTarget Target,
    RewardValueUsageType UsageType)
{
    public static Reward FromDomain(Domain.Entities.Reward reward) =>
        new(reward.Id, reward.Amount, reward.Type, reward.Target, reward.UsageType);
}

public sealed record Achievement(
    string Id,
    int? TransactionsCountToApplyAchievement,
    Reward Reward)
{
    public static Achievement FromDomain(Domain.Entities.Achievement achievement) =>
        new(
            achievement.Id,
            achievement.TransactionsCountToApplyAchievement,
            Reward.FromDomain(achievement.Reward));
}

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
    public static Program FromDomain(DomainProgram program) =>
        new(
            program.Id,
            program.Title,
            program.Description,
            program.State,
            program.StartDate,
            program.FinishDate,
            program.MinTransactionAmount,
            program.MaxTransactionAmount,
            Achievement.FromDomain(program.Achievement),
            program.CreatedAt,
            program.CreatedBy,
            program.UpdatedAt,
            program.UpdatedBy,
            program.IsDeleted);
}
