using Loyalty.Domain.Entities;

namespace Loyalty.Api.Contracts.Programs;

public sealed record ProgramResponse
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public ProgramState State { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime FinishDate { get; init; }
    public decimal MinTransactionAmount { get; init; }
    public decimal MaxTransactionAmount { get; init; }
    public required AchievementResponse Achievement { get; init; }
    public DateTime CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }

    public static ProgramResponse FromDomain(Loyalty.Domain.Entities.Program program) =>
        new()
        {
            Id = program.Id,
            Title = program.Title,
            Description = program.Description,
            State = program.State,
            StartDate = program.StartDate,
            FinishDate = program.FinishDate,
            MinTransactionAmount = program.MinTransactionAmount,
            MaxTransactionAmount = program.MaxTransactionAmount,
            Achievement = AchievementResponse.FromDomain(program.Achievement),
            CreatedAt = program.CreatedAt,
            CreatedBy = program.CreatedBy,
            UpdatedAt = program.UpdatedAt,
            UpdatedBy = program.UpdatedBy
        };
}

public sealed record AchievementResponse
{
    public required string Id { get; init; }
    public int? TransactionsCountToApplyAchievement { get; init; }
    public required RewardResponse Reward { get; init; }

    public static AchievementResponse FromDomain(Achievement achievement) =>
        new()
        {
            Id = achievement.Id,
            TransactionsCountToApplyAchievement = achievement.TransactionsCountToApplyAchievement,
            Reward = RewardResponse.FromDomain(achievement.Reward)
        };
}

public sealed record RewardResponse
{
    public decimal Amount { get; init; }
    public RewardValueType Type { get; init; }
    public RewardTarget Target { get; init; }
    public RewardValueUsageType UsageType { get; init; }

    public static RewardResponse FromDomain(Reward reward) =>
        new()
        {
            Amount = reward.Amount,
            Type = reward.Type,
            Target = reward.Target,
            UsageType = reward.UsageType
        };
}

public sealed record CreateProgramResponse(ProgramResponse Program);

public sealed record GetProgramResponse(ProgramResponse Program);
