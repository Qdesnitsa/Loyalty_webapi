using Loyalty.Application.BonusOutbox;

namespace Loyalty.Application.Abstractions;

public interface IBonusOutboxRepository
{
    Task<bool> TryAddAsync(BonusOutboxEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BonusOutboxEntry>> GetPendingAsync(
        int take,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string sourceTransactionId, CancellationToken cancellationToken = default);
}
