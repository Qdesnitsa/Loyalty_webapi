using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Loyalty.Infrastructure.Data.Documents;

public sealed class BonusOutboxDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required string SourceTransactionId { get; init; }

    public int CardId { get; init; }
    public decimal Amount { get; init; }
    public required string ProgramId { get; init; }
    public DateTime OccurredAt { get; init; }
}
