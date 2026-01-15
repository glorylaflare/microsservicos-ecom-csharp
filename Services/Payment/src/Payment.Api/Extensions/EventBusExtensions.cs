using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using Payment.Application.Consumer;

namespace Payment.Api.Extensions;

public static class EventBusExtensions
{
    public static async Task ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

        await eventBus.StartAsync(CancellationToken.None);

        await eventBus.SubscribeAsync<StockReservationResultEvent, StockReservationResultConsumer>();
    }
}
