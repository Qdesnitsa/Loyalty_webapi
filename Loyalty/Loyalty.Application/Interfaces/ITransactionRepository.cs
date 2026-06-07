using Loyalty.Domain.Entities;

namespace Loyalty.Application.Abstractions;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);

    Task SetBonusAmountToAccrueAsync(
        string id,
        decimal amount,
        CancellationToken cancellationToken = default);
}
