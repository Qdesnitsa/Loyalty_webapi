using Loyalty.Domain.Entities;
using Loyalty.Infrastructure.Data.Documents;

namespace Loyalty.Infrastructure.Data;

internal static class TransactionDocumentMapper
{
    public static TransactionDocument ToDocument(Transaction transaction) =>
        new()
        {
            Id = transaction.Id,
            EventId = transaction.EventId,
            Rrn = transaction.Rrn,
            CardId = transaction.CardId,
            Amount = transaction.Amount,
            TransactionType = transaction.TransactionType,
            TransactionStatus = transaction.TransactionStatus,
            CategoryId = transaction.CategoryId,
            ProgramId = transaction.ProgramId,
            CreatedAt = transaction.CreatedAt,
            CreatedBy = transaction.CreatedBy,
            ReceivedAt = transaction.ReceivedAt
        };

    public static Transaction ToDomain(TransactionDocument document) =>
        new()
        {
            Id = document.Id,
            EventId = document.EventId,
            Rrn = document.Rrn,
            CardId = document.CardId,
            Amount = document.Amount,
            TransactionType = document.TransactionType,
            TransactionStatus = document.TransactionStatus,
            CategoryId = document.CategoryId,
            ProgramId = document.ProgramId,
            CreatedAt = document.CreatedAt,
            CreatedBy = document.CreatedBy,
            ReceivedAt = document.ReceivedAt
        };
}
