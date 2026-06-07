using FluentValidation;

namespace Loyalty.Application.Programs;

public sealed class CreateProgramAchievementDataValidator : AbstractValidator<CreateProgramAchievementData>
{
    public CreateProgramAchievementDataValidator()
    {
        RuleFor(achievement => achievement.TransactionsCountToApplyAchievement).GreaterThan(0);
        RuleFor(achievement => achievement.Reward).SetValidator(new CreateProgramRewardDataValidator());
    }
}
