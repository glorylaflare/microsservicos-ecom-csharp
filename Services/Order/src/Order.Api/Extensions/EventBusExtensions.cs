using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using Order.Application.Consumers;

namespace Order.Api.Extensions;

public static class EventBusExtensions
{
    public static async Task ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

        await eventBus.SubscribeAsync<StockReservationResultEvent, StockReservationResultConsumer>();
    }
}
