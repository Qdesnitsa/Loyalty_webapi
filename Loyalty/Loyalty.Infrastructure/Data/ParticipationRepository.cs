using Loyalty.Application.Abstractions;
using Loyalty.Application.Transactions;
using Loyalty.Domain.Entities;
using Loyalty.Infrastructure.Data.Documents;
using MongoDB.Driver;

namespace Loyalty.Infrastructure.Data;

public sealed class ParticipationRepository(IMongoDatabase database) : IParticipationRepository
{
    private readonly IMongoCollection<ParticipationDocument> _participations =
        database.GetCollection<ParticipationDocument>(CollectionNames.Participations);

    public async Task<ParticipationProgressResult> RecordQualifyingTransactionAsync(
        int cardId,
        string programId,
        int requiredTransactionCount,
        CancellationToken cancellationToken = default)
    {
        if (requiredTransactionCount <= 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requiredTransactionCount),
                "Participation counter applies only when more than one transaction is required.");
        }

        var participationId = Participation.BuildId(cardId, programId);
        var existing = await _participations
            .Find(p => p.Id == participationId)
            .FirstOrDefaultAsync(cancellationToken);

        var currentCount = existing?.QualifyingTransactionCount ?? 0;
        if (currentCount >= requiredTransactionCount)
        {
            currentCount = 0;
        }

        var newCount = currentCount + 1;
        var rewardThresholdReached = newCount >= requiredTransactionCount;

        var now = DateTime.UtcNow;
        var participation = new Participation
        {
            Id = participationId,
            CardId = cardId,
            ProgramId = programId,
            QualifyingTransactionCount = newCount,
            UpdatedAt = now
        };

        if (existing is null)
        {
            await _participations.InsertOneAsync(
                ParticipationDocumentMapper.ToDocument(participation),
                cancellationToken: cancellationToken);
        }
        else
        {
            await _participations.ReplaceOneAsync(
                p => p.Id == participationId,
                ParticipationDocumentMapper.ToDocument(participation),
                cancellationToken: cancellationToken);
        }

        return new ParticipationProgressResult(newCount, rewardThresholdReached);
    }
}
