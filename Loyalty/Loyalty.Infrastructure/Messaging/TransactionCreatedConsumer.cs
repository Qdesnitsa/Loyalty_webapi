using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebMoney.Loyalty.Events.Transactions;

namespace Loyalty.Infrastructure.Messaging;

public sealed class TransactionCreatedConsumer(
    IOptions<KafkaConsumerConfig> config,
    ILogger<TransactionCreatedConsumer> logger) : BackgroundService
{
    private readonly KafkaConsumerConfig _config = config.Value;

    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.Run(() => ConsumeLoop(stoppingToken), stoppingToken);

    private void ConsumeLoop(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _config.BootstrapServers,
            GroupId = _config.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(_config.Topic);

        logger.LogInformation(
            "Kafka consumer subscribed to {Topic} (group {GroupId}, brokers {BootstrapServers})",
            _config.Topic,
            _config.GroupId,
            _config.BootstrapServers);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result.Message.Value is null)
                    {
                        logger.LogWarning("Skipping Kafka message with empty value at offset {Offset}", result.Offset);
                        consumer.Commit(result);
                        continue;
                    }

                    var @event = TransactionCreatedEventJson.Deserialize(result.Message.Value);
                    if (@event is null)
                    {
                        logger.LogWarning(
                            "Skipping Kafka message with invalid payload at offset {Offset}", result.Offset);
                        consumer.Commit(result);
                        continue;
                    }

                    logger.LogInformation(
                        "Received {EventType} for transaction {TransactionId} (event {EventId}, card {CardId}, amount {Amount})",
                        @event.EventType,
                        @event.Payload.TransactionId,
                        @event.EventId,
                        @event.Payload.CardId,
                        @event.Payload.Amount);

                    consumer.Commit(result);
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex, "Kafka consume error");
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Kafka consumer stopping");
        }
        finally
        {
            consumer.Close();
        }
    }
}
