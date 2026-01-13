using FluentValidation;

namespace Payment.Application.Commands;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required.");

        RuleFor(x => x.Items)
            .NotNull().WithMessage("Items cannot be null.")
            .Must(items => items != null && items.Count > 0)
            .WithMessage("At least one item is required.");
    }
}
