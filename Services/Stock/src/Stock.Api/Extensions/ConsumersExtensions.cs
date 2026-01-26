using Stock.Application.Consumers;
namespace Stock.Api.Extensions;

public static class ConsumersExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddTransient<OrderRequestConsumer>();
        return services;
    }
}