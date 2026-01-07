using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Commands;

namespace Payment.Console.Extensions;

public static class ValidatorExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreatePaymentCommand>, CreatePaymentCommandValidator>();

        return services;
    }
}
