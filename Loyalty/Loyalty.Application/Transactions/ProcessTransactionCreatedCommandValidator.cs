using FluentValidation;

namespace Loyalty.Application.Transactions;

public sealed class ProcessTransactionCreatedCommandValidator : AbstractValidator<ProcessTransactionCreatedCommand>
{
    public ProcessTransactionCreatedCommandValidator()
    {
        RuleFor(command => command.Event.EventId).NotEqual(Guid.Empty);
        RuleFor(command => command.Event.EventType).NotEmpty();
        RuleFor(command => command.Event.Source).NotEmpty();
        RuleFor(command => command.Event.Payload.TransactionId).GreaterThan(0);
        RuleFor(command => command.Event.Payload.Rrn).NotEmpty().MaximumLength(128);
        RuleFor(command => command.Event.Payload.CardId).GreaterThan(0);
        RuleFor(command => command.Event.Payload.TransactionType).NotEmpty().MaximumLength(128);
        RuleFor(command => command.Event.Payload.TransactionStatus).NotEmpty().MaximumLength(128);
        RuleFor(command => command.Event.Payload.CreatedBy).NotEmpty().MaximumLength(128);
    }
}
