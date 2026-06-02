using WebMoney.Loyalty.Events.Transactions;

namespace Loyalty.Application.Transactions;

public sealed record ProcessTransactionCreatedCommand(TransactionCreatedEvent Event) : ICommand;
