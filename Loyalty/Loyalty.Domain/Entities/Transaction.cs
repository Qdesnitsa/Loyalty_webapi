using System.Globalization;

namespace Loyalty.Domain.Entities;

public sealed class Transaction
{
    public required string Id { get; init; }
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

    public static Transaction CreateFromWebMoney(
        int transactionId,
        Guid eventId,
        string rrn,
        int cardId,
        decimal amount,
        string transactionType,
        string transactionStatus,
        int? categoryId,
        string? programId,
        DateTime createdAt,
        string createdBy)
    {
        if (transactionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(transactionId));
        }

        if (string.IsNullOrWhiteSpace(rrn))
        {
            throw new ArgumentException("Rrn is required.", nameof(rrn));
        }

        if (string.IsNullOrWhiteSpace(transactionType))
        {
            throw new ArgumentException("Transaction type is required.", nameof(transactionType));
        }

        if (string.IsNullOrWhiteSpace(transactionStatus))
        {
            throw new ArgumentException("Transaction status is required.", nameof(transactionStatus));
        }

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new ArgumentException("CreatedBy is required.", nameof(createdBy));
        }

        return new Transaction
        {
            Id = transactionId.ToString(CultureInfo.InvariantCulture),
            EventId = eventId,
            Rrn = rrn,
            CardId = cardId,
            Amount = amount,
            TransactionType = transactionType,
            TransactionStatus = transactionStatus,
            CategoryId = categoryId,
            ProgramId = programId,
            CreatedAt = createdAt,
            CreatedBy = createdBy,
            ReceivedAt = DateTime.UtcNow
        };
    }
}
