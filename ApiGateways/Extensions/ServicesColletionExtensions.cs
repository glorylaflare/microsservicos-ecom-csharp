using ApiGateways.Helpers;
using Microsoft.Extensions.Http;
namespace ApiGateways.Extensions;

public static class ServicesCollectionExtensions
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

    public static IServiceCollection AddReverseProxyServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        services.AddSingleton<IHttpMessageHandlerBuilderFilter, PolicyHttpMessageHandlerFilter>();
        
        return services;
    }
}