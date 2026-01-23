using Polly;
using Polly.CircuitBreaker;
using Polly.Contrib.WaitAndRetry;
using Polly.Fallback;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;
using Serilog;
using System.Net;
namespace ApiGateways.Extensions;

public static class CircuitBreakerExtensions
{
    private static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy()
    {
        var retruDelays = Backoff.DecorrelatedJitterBackoffV2(
            medianFirstRetryDelay: TimeSpan.FromMilliseconds(200),
            retryCount: 5
        );
        var retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(msg => (int)msg.StatusCode >= 500)
            .WaitAndRetryAsync(retruDelays, onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Log.Warning("Retrying... Attempt: {RetryAttempt}, Delay: {Delay}. Exception: {Exception}",
                    retryAttempt, timespan,
                    outcome.Exception?.Message
                    ?? outcome.Result?.StatusCode.ToString());
            });
        return retryPolicy;
    }
    private static AsyncTimeoutPolicy<HttpResponseMessage> CreateTimeOutPolicy()
    {
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
        return timeoutPolicy;
    }
    private static AsyncCircuitBreakerPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
    {
        var circuitPolicy = Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .OrResult(msg => (int)msg.StatusCode >= 500)
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 5,
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (outcome, ts) =>
                        {
                            Log.Error("Circuit broken! Breaking for {TimeSpan}. Exception: {Exception}",
                                ts, outcome.Exception?.Message
                                ?? outcome.Result?.StatusCode.ToString());
                        },
                        onReset: () =>
                        {
                            Log.Information("Circuit reset! Operations will be allowed again.");
                        }
                    );
        return circuitPolicy;
    }
    private static AsyncFallbackPolicy<HttpResponseMessage> CreateFallbackPolicy()
    {
        var fallbackRespose = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("Service is currently unavailable. Please try again later.")
        };
        var fallbackPolicy = Policy<HttpResponseMessage>
            .Handle<Exception>()
            .OrResult(msg => (int)msg.StatusCode >= 500)
            .FallbackAsync(fallbackRespose, async (outcome, context) =>
            {
                Log.Warning("Fallback executed. Exception: {Exception}",
                    outcome.Exception?.Message
                    ?? outcome.Result?.StatusCode.ToString());
                await Task.CompletedTask;
            });
        return fallbackPolicy;
    }
    public static IServiceCollection AddCircuitBreaker(this IServiceCollection services)
    {
        var registry = new PolicyRegistry();
        var fallbackPolicy = CreateFallbackPolicy();
        var circuitPolicy = CreateCircuitBreakerPolicy();
        var retryPolicy = CreateRetryPolicy();
        var timeoutPolicy = CreateTimeOutPolicy();
        var combinePolicy = Policy.WrapAsync(timeoutPolicy, retryPolicy, circuitPolicy, fallbackPolicy);
        registry.Add("DefaultPolicy", combinePolicy);
        services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);
        services.AddSingleton<IPolicyRegistry<string>>(registry);
        return services;
    }
}