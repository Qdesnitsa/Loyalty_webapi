using FluentValidation;

namespace Loyalty.Application.Programs;

public sealed class GetProgramQueryValidator : AbstractValidator<GetProgramQuery>
{
    public GetProgramQueryValidator()
    {
        RuleFor(query => query.Id).NotEmpty().MaximumLength(128);
    }
}
