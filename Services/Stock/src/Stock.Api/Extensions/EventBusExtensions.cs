using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using Stock.Application.Consumers;
namespace Stock.Api.Extensions;

public static class EventBusExtensions
{
    public static async Task ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        await eventBus.SubscribeAsync<OrderRequestedEvent, OrderRequestConsumer>();
    }
}