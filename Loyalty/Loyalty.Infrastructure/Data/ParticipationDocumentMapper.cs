using Loyalty.Domain.Entities;
using Loyalty.Infrastructure.Data.Documents;

namespace Loyalty.Infrastructure.Data;

internal static class ParticipationDocumentMapper
{
    public static ParticipationDocument ToDocument(Participation participation) =>
        new()
        {
            Id = participation.Id,
            CardId = participation.CardId,
            ProgramId = participation.ProgramId,
            QualifyingTransactionCount = participation.QualifyingTransactionCount,
            UpdatedAt = participation.UpdatedAt
        };

    public static Participation ToDomain(ParticipationDocument document) =>
        new()
        {
            Id = document.Id,
            CardId = document.CardId,
            ProgramId = document.ProgramId,
            QualifyingTransactionCount = document.QualifyingTransactionCount,
            UpdatedAt = document.UpdatedAt
        };
}
