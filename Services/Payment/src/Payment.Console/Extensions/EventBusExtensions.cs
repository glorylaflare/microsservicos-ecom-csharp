using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Consumer;

namespace Payment.Console.Extensions;

public static class EventBusExtensions
{
    public static async Task ConfigureEventBus(this IServiceProvider service, CancellationToken cancellationToken)
    {
        var scope = service.CreateScope();

        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        await eventBus.StartAsync(cancellationToken);

        await eventBus.SubscribeAsync<StockReservationResultEvent, StockReservationResultConsumer>();
    }
}
