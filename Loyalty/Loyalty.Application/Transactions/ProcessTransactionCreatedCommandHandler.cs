using System.Globalization;
using Loyalty.Application.Abstractions;
using Loyalty.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Loyalty.Application.Transactions;

public sealed class ProcessTransactionCreatedCommandHandler(
    ITransactionRepository transactionRepository,
    IProgramRepository programRepository,
    ILogger<ProcessTransactionCreatedCommandHandler> logger)
    : ICommandHandler<ProcessTransactionCreatedCommand>
{
    public async Task Handle(ProcessTransactionCreatedCommand command, CancellationToken cancellationToken)
    {
        var @event = command.Event;
        var payload = @event.Payload;
        var transactionId = payload.TransactionId.ToString(CultureInfo.InvariantCulture);

        var existingTransaction = await transactionRepository.GetByIdAsync(transactionId, cancellationToken);
        if (existingTransaction is not null)
        {
            logger.LogDebug(
                "Skipping already recorded transaction {TransactionId} (event {EventId})",
                transactionId,
                @event.EventId);
            return;
        }

        var programs = await programRepository.GetAllNotDeletedAsync(cancellationToken);
        var programId = ProgramParticipationResolver.ResolveProgramId(programs, payload);

        var transaction = Transaction.CreateFromWebMoney(
            payload.TransactionId,
            @event.EventId,
            payload.Rrn,
            payload.CardId,
            payload.Amount,
            payload.TransactionType,
            payload.TransactionStatus,
            payload.CategoryId,
            programId,
            payload.CreatedAt,
            payload.CreatedBy);

        try
        {
            await transactionRepository.AddAsync(transaction, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            logger.LogDebug(
                "Transaction {TransactionId} was created concurrently (event {EventId})",
                transactionId,
                @event.EventId);
            return;
        }

        logger.LogInformation(
            "Recorded transaction {TransactionId} from WebMoney (event {EventId}, card {CardId}, amount {Amount}, program {ProgramId})",
            transactionId,
            @event.EventId,
            payload.CardId,
            payload.Amount,
            programId ?? "none");
    }
}
