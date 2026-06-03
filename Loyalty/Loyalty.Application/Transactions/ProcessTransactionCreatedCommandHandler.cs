using System.Globalization;
using Loyalty.Application.Abstractions;
using Loyalty.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Loyalty.Application.Transactions;

public sealed class ProcessTransactionCreatedCommandHandler(
    ITransactionRepository transactionRepository,
    IProgramRepository programRepository,
    IParticipationRepository participationRepository,
    IWebMoneyBonusClient webMoneyBonusClient,
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

        await ApplyProgramRewardAsync(
            transactionId,
            payload.CardId,
            programId,
            payload.Amount,
            cancellationToken);
    }

    private async Task ApplyProgramRewardAsync(
        string transactionId,
        int cardId,
        string programId,
        decimal transactionAmount,
        CancellationToken cancellationToken)
    {
        var program = await programRepository.GetByIdAsync(programId, cancellationToken);
        if (program is null)
        {
            logger.LogWarning(
                "Program {ProgramId} was not found while applying reward for card {CardId}",
                programId,
                cardId);
            return;
        }

        var requiredCount = program.Achievement.TransactionsCountToApplyAchievement;
        var shouldAccrue = false;

        if (requiredCount <= 1)
        {
            shouldAccrue = true;
        }
        else
        {
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
                shouldAccrue = true;
            }
            else
            {
                logger.LogDebug(
                    "Card {CardId} participation progress in program {ProgramId}: {Count}/{Required}",
                    cardId,
                    programId,
                    progress.QualifyingTransactionCount,
                    requiredCount);
            }
        }

        if (!shouldAccrue)
        {
            return;
        }

        var bonusAmount = RewardCalculator.Calculate(program.Achievement.Reward, transactionAmount);
        if (bonusAmount <= 0)
        {
            logger.LogWarning(
                "Skipping bonus accrual for transaction {TransactionId}: calculated amount is {BonusAmount}",
                transactionId,
                bonusAmount);
            return;
        }

        await webMoneyBonusClient.AccrueBonusAsync(
            cardId,
            bonusAmount,
            transactionId,
            programId,
            cancellationToken);

        logger.LogInformation(
            "Accrued {BonusAmount} bonus for card {CardId} from transaction {TransactionId} (program {ProgramId})",
            bonusAmount,
            cardId,
            transactionId,
            programId);
    }
}
