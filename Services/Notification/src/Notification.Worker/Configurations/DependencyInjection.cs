using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Consumers;

namespace Notification.Worker.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddTransient<OrderEmailRequestConsumer>();
        return services;
    }

    public static async Task SubscribeEventsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
        await eventBus.SubscribeAsync<OrderEmailRequestEvent, OrderEmailRequestConsumer>();
    }
}
