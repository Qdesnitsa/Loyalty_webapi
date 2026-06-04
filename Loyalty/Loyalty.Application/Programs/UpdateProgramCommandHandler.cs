using Loyalty.Application.Abstractions;
using Loyalty.Application.Programs.Models;
using DomainAchievement = Loyalty.Domain.Entities.Achievement;
using DomainReward = Loyalty.Domain.Entities.Reward;

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

        var achievement = new DomainAchievement
        {
            Id = existingProgram.Achievement.Id,
            TransactionsCountToApplyAchievement = command.Achievement.TransactionsCountToApplyAchievement,
            Reward = new DomainReward
            {
                Id = existingProgram.Achievement.Reward.Id,
                Amount = command.Achievement.Reward.Amount,
                Type = command.Achievement.Reward.Type,
                Target = command.Achievement.Reward.Target,
                UsageType = command.Achievement.Reward.UsageType
            }
        };

        var updatedProgram = existingProgram.Update(
            command.Title,
            command.Description,
            command.State,
            command.StartDate,
            command.FinishDate,
            command.MinTransactionAmount,
            command.MaxTransactionAmount,
            command.TransactionType,
            achievement,
            command.UpdatedBy);

        await programRepository.UpdateAsync(updatedProgram, cancellationToken);

        return Program.FromDomain(updatedProgram);
    }
}
