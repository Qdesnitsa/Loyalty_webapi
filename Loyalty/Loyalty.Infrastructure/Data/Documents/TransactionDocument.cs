using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Loyalty.Infrastructure.Data.Documents;

public sealed class TransactionDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required string Id { get; init; }

    [BsonRepresentation(BsonType.String)]
    public Guid EventId { get; init; }

    public required string Rrn { get; init; }

    public int CardId { get; init; }

    public decimal Amount { get; init; }

    public required string TransactionType { get; init; }

    public required string TransactionStatus { get; init; }

    public int? CategoryId { get; init; }

    public string? ProgramId { get; init; }

    public DateTime CreatedAt { get; init; }

    public required string CreatedBy { get; init; }

    public DateTime ReceivedAt { get; init; }
}
