using Loyalty.Application.Transactions;

namespace Loyalty.Application.Abstractions;

public interface IParticipationRepository
{
    Task<ParticipationProgressResult> RecordQualifyingTransactionAsync(
        int cardId,
        string programId,
        int requiredTransactionCount,
        CancellationToken cancellationToken = default);
}
