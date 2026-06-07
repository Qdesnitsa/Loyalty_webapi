using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Loyalty.Infrastructure.BonusOutbox;

public sealed class BonusOutboxService(
    BonusOutboxProcessor processor,
    IOptions<BonusOutboxJobConfig> config,
    ILogger<BonusOutboxService> logger) : BackgroundService
{
    private readonly BonusOutboxJobConfig _config = config.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var processedCount = 0;

            try
            {
                processedCount = await processor.ProcessPendingAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process bonus outbox items");
            }

            if (processedCount == 0 || processedCount < _config.BatchSize)
            {
                await Task.Delay(_config.PollingInterval, stoppingToken);
            }
        }
    }
}
