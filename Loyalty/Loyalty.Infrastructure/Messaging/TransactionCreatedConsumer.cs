using Confluent.Kafka;
using FluentValidation;
using Loyalty.Application.Transactions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebMoney.Loyalty.Events.Transactions;

namespace Loyalty.Infrastructure.Messaging;

public sealed class TransactionCreatedConsumer(
    IOptions<KafkaConsumerConfig> config,
    IServiceScopeFactory scopeFactory,
    ILogger<TransactionCreatedConsumer> logger) : BackgroundService
{
    private readonly KafkaConsumerConfig _config = config.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                ConsumeResult<string, string>? result = null;

                try
                {
                    result = consumer.Consume(stoppingToken);
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
                            "Skipping Kafka message with invalid payload at offset {Offset}",
                            result.Offset);
                        consumer.Commit(result);
                        continue;
                    }

                    using (logger.BeginScope(new Dictionary<string, object>
                    {
                        ["EventId"] = @event.EventId,
                        ["TransactionId"] = @event.Payload.TransactionId,
                        ["KafkaOffset"] = result.Offset.Value
                    }))
                    {
                        using var scope = scopeFactory.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        await mediator.Send(new ProcessTransactionCreatedCommand(@event), stoppingToken);
                    }

                    consumer.Commit(result);
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex, "Kafka consume error");
                }
                catch (ValidationException ex) when (result is not null)
                {
                    logger.LogWarning(
                        ex,
                        "Skipping Kafka message with invalid event data at offset {Offset}",
                        result.Offset);
                    consumer.Commit(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process Kafka message");
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
