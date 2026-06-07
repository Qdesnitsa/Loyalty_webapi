using FluentValidation;

namespace Loyalty.Application.Programs;

public sealed class UpdateProgramCommandValidator : AbstractValidator<UpdateProgramCommand>
{
    public UpdateProgramCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty().MaximumLength(128);
        RuleFor(command => command.Title).NotEmpty().MinimumLength(3).MaximumLength(128);
        RuleFor(command => command.State).IsInEnum();
        RuleFor(command => command.StartDate).NotEmpty();
        RuleFor(command => command.FinishDate).NotEmpty();
        RuleFor(command => command.FinishDate).GreaterThan(command => command.StartDate);
        RuleFor(command => command.MinTransactionAmount).GreaterThanOrEqualTo(0);
        RuleFor(command => command.MaxTransactionAmount).GreaterThanOrEqualTo(0);
        RuleFor(command => command.MaxTransactionAmount)
            .GreaterThanOrEqualTo(command => command.MinTransactionAmount)
            .WithMessage("Max transaction amount cannot be less than min transaction amount.");
        RuleFor(command => command.TransactionType).NotEmpty().MaximumLength(128);
        RuleFor(command => command.Achievement).SetValidator(new CreateProgramAchievementDataValidator());
        RuleFor(command => command.UpdatedBy).NotEmpty().MaximumLength(128);
    }
}
