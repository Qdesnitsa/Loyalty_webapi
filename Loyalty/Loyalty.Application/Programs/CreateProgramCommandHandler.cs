using Loyalty.Application.Abstractions;
using Loyalty.Application.Programs.Models;
using DomainAchievement = Loyalty.Domain.Entities.Achievement;
using DomainProgram = Loyalty.Domain.Entities.Program;
using DomainReward = Loyalty.Domain.Entities.Reward;

namespace Loyalty.Application.Programs;

public sealed class CreateProgramCommandHandler(IProgramRepository programRepository)
    : ICommandHandler<CreateProgramCommand, Program>
{
    public async Task<Program> Handle(CreateProgramCommand command, CancellationToken cancellationToken)
    {
        var programId = GenerateId();
        var achievement = new DomainAchievement
        {
            Id = GenerateId(),
            TransactionsCountToApplyAchievement = command.Achievement.TransactionsCountToApplyAchievement,
            Reward = new DomainReward
            {
                Id = GenerateId(),
                Amount = command.Achievement.Reward.Amount,
                Type = command.Achievement.Reward.Type,
                Target = command.Achievement.Reward.Target,
                UsageType = command.Achievement.Reward.UsageType
            }
        };

        var domainProgram = DomainProgram.Create(
            programId,
            command.Title,
            command.Description,
            command.State,
            command.StartDate,
            command.FinishDate,
            command.MinTransactionAmount,
            command.MaxTransactionAmount,
            command.TransactionType,
            achievement,
            command.CreatedBy);

        await programRepository.AddAsync(domainProgram, cancellationToken);

        return Program.FromDomain(domainProgram);
    }

    private static string GenerateId() => Guid.NewGuid().ToString("N");
}
