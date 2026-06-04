using Loyalty.Domain.Entities;
using ApplicationProgram = Loyalty.Application.Programs.Models.Program;

namespace Loyalty.Application.Programs;

public sealed record UpdateProgramCommand(
    string Id,
    string Title,
    string? Description,
    ProgramState State,
    DateTime StartDate,
    DateTime FinishDate,
    decimal MinTransactionAmount,
    decimal MaxTransactionAmount,
    string TransactionType,
    CreateProgramAchievementData Achievement,
    string UpdatedBy) : ICommand<ApplicationProgram>;
