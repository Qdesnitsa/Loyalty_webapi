using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Loyalty.Infrastructure.Data.Documents;

public sealed class ParticipationDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required string Id { get; init; }

    public int CardId { get; init; }
    public required string ProgramId { get; init; }
    public int QualifyingTransactionCount { get; init; }
    public DateTime UpdatedAt { get; init; }
}
