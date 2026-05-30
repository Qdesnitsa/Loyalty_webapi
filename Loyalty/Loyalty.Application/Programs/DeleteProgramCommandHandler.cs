using Loyalty.Application.Abstractions;

namespace Loyalty.Application.Programs;

public sealed class DeleteProgramCommandHandler(IProgramRepository programRepository)
    : ICommandHandler<DeleteProgramCommand>
{
    public async Task Handle(DeleteProgramCommand command, CancellationToken cancellationToken)
    {
        var existingProgram = await programRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingProgram is null)
        {
            throw new InvalidCommandException("Program not found", ExceptionTypes.ResourceNotFound);
        }

        var deletedProgram = existingProgram.Delete(command.UpdatedBy);
        await programRepository.UpdateAsync(deletedProgram, cancellationToken);
    }
}
