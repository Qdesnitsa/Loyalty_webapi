using FluentValidation;

namespace Loyalty.Application.Programs;

public sealed class DeleteProgramCommandValidator : AbstractValidator<DeleteProgramCommand>
{
    public DeleteProgramCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty().MaximumLength(128);
        RuleFor(command => command.UpdatedBy).NotEmpty().MaximumLength(128);
    }
}
