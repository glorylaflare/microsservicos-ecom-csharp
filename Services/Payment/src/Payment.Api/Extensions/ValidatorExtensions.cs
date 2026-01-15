using FluentValidation;
using Payment.Application.Commands;

namespace Payment.Api.Extensions;

public static class ValidatorExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreatePaymentCommand>, CreatePaymentCommandValidator>();

        return services;
    }
}
