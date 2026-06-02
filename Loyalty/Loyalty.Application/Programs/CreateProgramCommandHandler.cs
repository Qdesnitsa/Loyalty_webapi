using Loyalty.Application.Abstractions;
using Loyalty.Application.Programs.Models;
using DomainProgram = Loyalty.Domain.Entities.Program;

namespace Loyalty.Application.Programs;

public sealed class CreateProgramCommandHandler(IProgramRepository programRepository)
    : ICommandHandler<CreateProgramCommand, Program>
{
    public async Task<Program> Handle(CreateProgramCommand command, CancellationToken cancellationToken)
    {
        if (await programRepository.ExistsAsync(command.Id, cancellationToken))
        {
            throw new InvalidCommandException("Program already exists", ExceptionTypes.ResourceAlreadyExists);
        }

        var domainProgram = DomainProgram.Create(
            command.Id,
            command.Title,
            command.Description,
            command.State,
            command.StartDate,
            command.FinishDate,
            command.MinTransactionAmount,
            command.MaxTransactionAmount,
            command.TransactionType,
            command.Achievement,
            command.CreatedBy);

        await programRepository.AddAsync(domainProgram, cancellationToken);

        return Program.FromDomain(domainProgram);
    }
}
