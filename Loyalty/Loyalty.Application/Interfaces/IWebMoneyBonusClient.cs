namespace Loyalty.Application.Abstractions;

public interface IWebMoneyBonusClient
{
    Task AccrueBonusAsync(
        int cardId,
        decimal amount,
        string sourceTransactionId,
        string? programId,
        CancellationToken cancellationToken = default);
}
