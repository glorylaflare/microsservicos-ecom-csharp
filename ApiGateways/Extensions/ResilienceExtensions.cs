using Polly;

namespace ApiGateways.Extensions;

public static class ResilienceExtensions
{
    public static IServiceCollection AddResilienceService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpForwarder();

        services.AddHttpClient("yarp-cb")
            .AddTransientHttpErrorPolicy(p =>
                p.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                )
            )
            .AddTransientHttpErrorPolicy(p =>
                p.WaitAndRetryAsync(
                    retryCount: 3,
                    attempt => TimeSpan.FromMilliseconds(200 * attempt)
                )
            );

        return services;
    }
}
