using ApiGateways.Helpers;
using Microsoft.Extensions.Http;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateways.Extensions;

public static class ServicesCollectionExtensions
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

    public static IServiceCollection AddReverseProxyServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        services.AddSingleton<IHttpMessageHandlerBuilderFilter, PolicyHttpMessageHandlerFilter>();

        return services;
    }
}
