using Loyalty.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Loyalty.Infrastructure.BonusOutbox;

public sealed class BonusOutboxProcessor(
    IOptions<BonusOutboxJobConfig> config,
    IServiceScopeFactory scopeFactory,
    ILogger<BonusOutboxProcessor> logger)
{
    private readonly int _batchSize = config.Value.BatchSize;

    public async Task<int> ProcessPendingAsync(CancellationToken cancellationToken = default)
    {
        var processedCount = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var outboxRepository = scope.ServiceProvider.GetRequiredService<IBonusOutboxRepository>();
            var bonusClient = scope.ServiceProvider.GetRequiredService<IWebMoneyBonusClient>();

            var entries = await outboxRepository.GetPendingAsync(_batchSize, cancellationToken);
            if (entries.Count == 0)
            {
                break;
            }

            logger.LogInformation("Processing {Count} bonus outbox items", entries.Count);

            foreach (var entry in entries)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                using var _ = logger.BeginScope(new Dictionary<string, object>
                {
                    ["SourceTransactionId"] = entry.SourceTransactionId,
                    ["CardId"] = entry.CardId,
                    ["ProgramId"] = entry.ProgramId
                });

                try
                {
                    await bonusClient.AccrueBonusAsync(
                        entry.CardId,
                        entry.Amount,
                        entry.SourceTransactionId,
                        entry.ProgramId,
                        cancellationToken);

                    await outboxRepository.DeleteAsync(entry.SourceTransactionId, cancellationToken);
                    processedCount++;

                    logger.LogDebug(
                        "Successfully delivered bonus outbox item for transaction {SourceTransactionId}",
                        entry.SourceTransactionId);

                    logger.LogInformation(
                        "Delivered bonus accrual for transaction {SourceTransactionId} (card {CardId}, amount {Amount})",
                        entry.SourceTransactionId,
                        entry.CardId,
                        entry.Amount);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Cannot deliver bonus accrual for transaction {SourceTransactionId}",
                        entry.SourceTransactionId);
                }
            }

            if (entries.Count < _batchSize)
            {
                break;
            }
        }

        return processedCount;
    }
}
