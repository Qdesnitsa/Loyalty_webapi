using Loyalty.Domain.Entities;

namespace Loyalty.Api.Contracts.Programs;

public sealed record UpdateProgramRequest(
    string Title,
    string? Description,
    ProgramState State,
    DateTime StartDate,
    DateTime FinishDate,
    decimal MinTransactionAmount,
    decimal MaxTransactionAmount,
    AchievementRequest Achievement,
    string UpdatedBy);
