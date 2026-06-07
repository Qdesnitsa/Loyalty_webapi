using FluentValidation;
using Loyalty.Domain.Entities;

namespace Loyalty.Application.Programs;

public sealed class CreateProgramRewardDataValidator : AbstractValidator<CreateProgramRewardData>
{
    public CreateProgramRewardDataValidator()
    {
        RuleFor(reward => reward.Amount).GreaterThan(0);
        RuleFor(reward => reward.Type).IsInEnum();
        RuleFor(reward => reward.Target).IsInEnum();
        RuleFor(reward => reward.UsageType).IsInEnum();
    }
}
