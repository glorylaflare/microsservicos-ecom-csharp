namespace ApiGateways.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationService(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AuthenticateUser", policy =>
            {
                policy.RequireAuthenticatedUser();
            });
            options.AddPolicy("AllowAnonymous", policy =>
            {
                policy.RequireAssertion(_ => true);
            });
        });

        return services;
    }
}
