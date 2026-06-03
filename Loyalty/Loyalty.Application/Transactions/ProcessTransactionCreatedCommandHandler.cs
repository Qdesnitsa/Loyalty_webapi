using System.Globalization;
using Loyalty.Application.Abstractions;
using Loyalty.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Loyalty.Application.Transactions;

public sealed class ProcessTransactionCreatedCommandHandler(
    ITransactionRepository transactionRepository,
    IProgramRepository programRepository,
    IParticipationRepository participationRepository,
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

        if (programId is null)
        {
            return;
        }

        await UpdateParticipationProgressAsync(
            payload.CardId,
            programId,
            cancellationToken);
    }

    private async Task UpdateParticipationProgressAsync(
        int cardId,
        string programId,
        CancellationToken cancellationToken)
    {
        var program = await programRepository.GetByIdAsync(programId, cancellationToken);
        if (program is null)
        {
            logger.LogWarning(
                "Program {ProgramId} was not found while updating participation for card {CardId}",
                programId,
                cardId);
            return;
        }

        var requiredCount = program.Achievement.TransactionsCountToApplyAchievement;
        if (requiredCount <= 1)
        {
            logger.LogDebug(
                "Card {CardId} is eligible for immediate reward in program {ProgramId}",
                cardId,
                programId);
            return;
        }

        var progress = await participationRepository.RecordQualifyingTransactionAsync(
            cardId,
            programId,
            requiredCount,
            cancellationToken);

        if (progress.RewardThresholdReached)
        {
            logger.LogInformation(
                "Card {CardId} reached reward threshold in program {ProgramId} ({Count}/{Required})",
                cardId,
                programId,
                progress.QualifyingTransactionCount,
                requiredCount);
            return;
        }

        logger.LogDebug(
            "Card {CardId} participation progress in program {ProgramId}: {Count}/{Required}",
            cardId,
            programId,
            progress.QualifyingTransactionCount,
            requiredCount);
    }
}
