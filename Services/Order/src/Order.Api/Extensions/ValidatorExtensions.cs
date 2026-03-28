using FluentValidation;
using Order.Application.Commands.CreateOrder;
namespace Order.Api.Extensions;

public static class ValidatorExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
        return services;
    }
}