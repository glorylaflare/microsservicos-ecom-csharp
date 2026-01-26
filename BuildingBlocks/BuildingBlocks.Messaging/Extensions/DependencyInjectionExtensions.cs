using Microsoft.Extensions.DependencyInjection;
namespace BuildingBlocks.Messaging.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        services.AddSingleton<IEventBus, RabbitMQEventBus>();
        services.AddHostedService<EventBusHostedService>();
        return services;
    }
}