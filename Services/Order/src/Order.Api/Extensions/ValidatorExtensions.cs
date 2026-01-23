using FluentValidation;
using Order.Application.Commands;
namespace Order.Api.Extensions;

public static class ValidatorExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
        return services;
    }
}