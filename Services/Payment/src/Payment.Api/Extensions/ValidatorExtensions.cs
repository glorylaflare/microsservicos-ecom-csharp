using FluentValidation;
using Payment.Application.Commands.CreatePayment;
namespace Payment.Api.Extensions;

public static class ValidatorExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreatePaymentCommand>, CreatePaymentCommandValidator>();
        return services;
    }
}