using FluentValidation;
namespace Payment.Application.Commands;

public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Payment type is required.");
        RuleFor(x => x.Data)
            .NotNull()
            .WithMessage("Payment data is required.");
        RuleFor(x => x.Data.Id)
            .NotEmpty()
            .WithMessage("Payment ID is required.");
    }
}