using Loyalty.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Loyalty.Infrastructure.Data.Documents;

public sealed class ProgramDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public ProgramState State { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime FinishDate { get; init; }
    public decimal MinTransactionAmount { get; init; }
    public decimal MaxTransactionAmount { get; init; }
    public required string TransactionType { get; init; }
    public required AchievementDocument Achievement { get; init; }
    public DateTime CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }
    public bool IsDeleted { get; init; }
}

public sealed class AchievementDocument
{
    public required string Id { get; init; }
    public int? TransactionsCountToApplyAchievement { get; init; }
    public required RewardDocument Reward { get; init; }
}

public sealed class RewardDocument
{
    public required string Id { get; init; }
    public decimal Amount { get; init; }
    public RewardValueType Type { get; init; }
    public RewardTarget Target { get; init; }
    public RewardValueUsageType UsageType { get; init; }
}
