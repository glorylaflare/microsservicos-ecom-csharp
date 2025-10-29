using Microsoft.IdentityModel.Tokens;

namespace ApiGateways.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = configuration["IdentityServer:Authority"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

        return services;
    }
}
