using WebMoney.Loyalty.Events.Transactions;

namespace Loyalty.Infrastructure.Messaging;

public sealed class KafkaConsumerConfig
{
    public const string SectionName = "Kafka";

    public string BootstrapServers { get; set; } = "localhost:9094";

    public string GroupId { get; set; } = "loyalty-transactions";

    public string Topic { get; set; } = KafkaTopics.WebMoneyTransactionsCreated;
}
