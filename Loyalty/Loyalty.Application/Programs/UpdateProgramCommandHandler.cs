using Loyalty.Application.Abstractions;
using Loyalty.Application.Programs.Models;

namespace Loyalty.Application.Programs;

public sealed class UpdateProgramCommandHandler(IProgramRepository programRepository)
    : ICommandHandler<UpdateProgramCommand, Program>
{
    public async Task<Program> Handle(UpdateProgramCommand command, CancellationToken cancellationToken)
    {
        var existingProgram = await programRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingProgram is null)
        {
            throw new InvalidCommandException("Program not found", ExceptionTypes.ResourceNotFound);
        }

        var updatedProgram = existingProgram.Update(
            command.Title,
            command.Description,
            command.State,
            command.StartDate,
            command.FinishDate,
            command.MinTransactionAmount,
            command.MaxTransactionAmount,
            command.Achievement,
            command.UpdatedBy);

        await programRepository.UpdateAsync(updatedProgram, cancellationToken);

        return Program.FromDomain(updatedProgram);
    }
}
