using Microsoft.Extensions.Http;
using Polly;
using Polly.Registry;

namespace ApiGateways.Helpers;

public class PolicyHttpMessageHandlerFilter : IHttpMessageHandlerBuilderFilter
{
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public PolicyHttpMessageHandlerFilter(IReadOnlyPolicyRegistry<string> policyRegistry)
    {
        _policyRegistry = policyRegistry;
    }

    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
    {
        return builder =>
        {
            next(builder);

            if (_policyRegistry.TryGet<IAsyncPolicy<HttpResponseMessage>>("DefaultPolicy", out var policy))
            {
                builder.AdditionalHandlers.Add(new PolicyHttpMessageHandler(policy));
            }
        };
    }
}
