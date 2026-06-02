using Loyalty.Domain.Entities;
using ApplicationProgram = Loyalty.Application.Programs.Models.Program;

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
    string TransactionType,
    Achievement Achievement,
    string CreatedBy) : ICommand<ApplicationProgram>;
