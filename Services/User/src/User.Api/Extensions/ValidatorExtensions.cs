using FluentValidation;
using User.Application.Commands;
namespace User.Api.Extensions;

public static class ValidatorExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();
        services.AddScoped<IValidator<DeactivateUserCommand>, DeactivateUserCommandValidator>();
        return services;
    }
}