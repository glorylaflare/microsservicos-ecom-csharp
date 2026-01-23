using FluentValidation;
namespace Payment.Application.Commands;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required.");
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("OrderId must be greater than zero.");
        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("TotalAmount must be greater than zero.");
        RuleFor(x => x.Items)
            .NotNull().WithMessage("Items cannot be null.")
            .Must(items => items != null && items.Count > 0)
            .WithMessage("At least one item is required.");
    }
}