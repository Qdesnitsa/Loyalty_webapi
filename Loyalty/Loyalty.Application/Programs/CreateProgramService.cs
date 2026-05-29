using Loyalty.Application.Abstractions;
using Loyalty.Domain.Entities;
using Program = Loyalty.Domain.Entities.Program;

namespace Loyalty.Application.Programs;

public sealed class CreateProgramService(IProgramRepository programRepository)
{
    public async Task<Program> CreateAsync(CreateProgramCommand command, CancellationToken cancellationToken = default)
    {
        if (await programRepository.ExistsAsync(command.Id, cancellationToken))
            throw new ProgramAlreadyExistsException(command.Id);

        var program = Program.Create(
            command.Id,
            command.Title,
            command.Description,
            command.State,
            command.StartDate,
            command.FinishDate,
            command.MinTransactionAmount,
            command.MaxTransactionAmount,
            command.Achievement,
            command.CreatedBy);

        await programRepository.AddAsync(program, cancellationToken);

        return program;
    }
}
