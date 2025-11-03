using Microsoft.IdentityModel.Tokens;

namespace ApiGateways.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                var domain = configuration["Auth0:Domain"];

                options.Authority = $"https://{domain}/";
                options.Audience = configuration["Auth0:Audience"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = $"https://{domain}/",
                    ValidAudience = configuration["Auth0:Audience"],
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

        return services;
    }
}
