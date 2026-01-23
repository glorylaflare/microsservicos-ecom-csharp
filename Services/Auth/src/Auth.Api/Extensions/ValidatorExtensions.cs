using Auth.Api.Commands;
using FluentValidation;
namespace Auth.Api.Extensions;
public static class ValidatorExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AuthenticateUserCommand>, AuthenticateUserCommandValidator>();
        return services;
    }
}