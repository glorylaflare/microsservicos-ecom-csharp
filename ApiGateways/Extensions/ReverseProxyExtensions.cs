namespace ApiGateways.Extensions;

public static class ReverseProxyExtensions
{
    public static IServiceCollection AddReverseProxyServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        return services;
    }
}
