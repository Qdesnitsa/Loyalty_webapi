namespace Loyalty.Application.BonusOutbox;

public sealed class BonusOutboxEntry
{
    public required string SourceTransactionId { get; init; }
    public int CardId { get; init; }
    public decimal Amount { get; init; }
    public required string ProgramId { get; init; }
    public DateTime OccurredAt { get; init; }
}
