using Loyalty.Domain.Entities;
using Loyalty.Infrastructure.Data.Documents;
using Program = Loyalty.Domain.Entities.Program;

namespace Loyalty.Infrastructure.Data;

internal static class ProgramDocumentMapper
{
    public static ProgramDocument ToDocument(Program program) =>
        new()
        {
            Id = program.Id,
            Title = program.Title,
            Description = program.Description,
            State = program.State,
            StartDate = program.StartDate,
            FinishDate = program.FinishDate,
            MinTransactionAmount = program.MinTransactionAmount,
            MaxTransactionAmount = program.MaxTransactionAmount,
            TransactionType = program.TransactionType,
            Achievement = new AchievementDocument
            {
                Id = program.Achievement.Id,
                TransactionsCountToApplyAchievement = program.Achievement.TransactionsCountToApplyAchievement,
                Reward = new RewardDocument
                {
                    Id = program.Achievement.Reward.Id,
                    Amount = program.Achievement.Reward.Amount,
                    Type = program.Achievement.Reward.Type,
                    Target = program.Achievement.Reward.Target,
                    UsageType = program.Achievement.Reward.UsageType
                }
            },
            CreatedAt = program.CreatedAt,
            CreatedBy = program.CreatedBy,
            UpdatedAt = program.UpdatedAt,
            UpdatedBy = program.UpdatedBy,
            IsDeleted = program.IsDeleted
        };

    public static Program ToDomain(ProgramDocument document) =>
        new()
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            State = document.State,
            StartDate = document.StartDate,
            FinishDate = document.FinishDate,
            MinTransactionAmount = document.MinTransactionAmount,
            MaxTransactionAmount = document.MaxTransactionAmount,
            TransactionType = document.TransactionType,
            Achievement = new Achievement
            {
                Id = document.Achievement.Id,
                TransactionsCountToApplyAchievement = document.Achievement.TransactionsCountToApplyAchievement ?? 0,
                Reward = new Reward
                {
                    Id = document.Achievement.Reward.Id,
                    Amount = document.Achievement.Reward.Amount,
                    Type = document.Achievement.Reward.Type,
                    Target = document.Achievement.Reward.Target,
                    UsageType = document.Achievement.Reward.UsageType
                }
            },
            CreatedAt = document.CreatedAt,
            CreatedBy = document.CreatedBy,
            UpdatedAt = document.UpdatedAt,
            UpdatedBy = document.UpdatedBy,
            IsDeleted = document.IsDeleted
        };
}
