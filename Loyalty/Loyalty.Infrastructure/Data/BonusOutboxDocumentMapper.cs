using Loyalty.Application.BonusOutbox;
using Loyalty.Infrastructure.Data.Documents;

namespace Loyalty.Infrastructure.Data;

internal static class BonusOutboxDocumentMapper
{
    public static BonusOutboxDocument ToDocument(BonusOutboxEntry entry) =>
        new()
        {
            SourceTransactionId = entry.SourceTransactionId,
            CardId = entry.CardId,
            Amount = entry.Amount,
            ProgramId = entry.ProgramId,
            OccurredAt = entry.OccurredAt
        };

    public static BonusOutboxEntry ToDomain(BonusOutboxDocument document) =>
        new()
        {
            SourceTransactionId = document.SourceTransactionId,
            CardId = document.CardId,
            Amount = document.Amount,
            ProgramId = document.ProgramId,
            OccurredAt = document.OccurredAt
        };
}
