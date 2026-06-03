namespace Loyalty.Application.Transactions;

public sealed record ParticipationProgressResult(
    int QualifyingTransactionCount,
    bool RewardThresholdReached);
