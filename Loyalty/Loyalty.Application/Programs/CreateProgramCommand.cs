using Loyalty.Domain.Entities;

namespace Loyalty.Application.Programs;

public sealed record CreateProgramCommand(
    string Id,
    string Title,
    string? Description,
    ProgramState State,
    DateTime StartDate,
    DateTime FinishDate,
    decimal MinTransactionAmount,
    decimal MaxTransactionAmount,
    Achievement Achievement,
    string CreatedBy);
