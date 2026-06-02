using Loyalty.Domain.Entities;
using WebMoney.Loyalty.Events.Transactions;

namespace Loyalty.Application.Transactions;

public static class ProgramParticipationResolver
{
    public static string? ResolveProgramId(
        IReadOnlyList<Program> programs,
        TransactionCreatedPayload payload)
    {
        foreach (var program in programs)
        {
            if (Matches(program, payload))
            {
                return program.Id;
            }
        }

        return null;
    }

    private static bool Matches(Program program, TransactionCreatedPayload payload) =>
        !program.IsDeleted
        && program.State == ProgramState.Active
        && string.Equals(program.TransactionType, payload.TransactionType, StringComparison.Ordinal)
        && payload.CreatedAt >= program.StartDate
        && payload.CreatedAt <= program.FinishDate
        && payload.Amount >= program.MinTransactionAmount
        && payload.Amount <= program.MaxTransactionAmount;
}
