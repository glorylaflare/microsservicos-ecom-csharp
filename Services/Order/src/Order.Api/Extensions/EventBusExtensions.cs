using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Messaging;
using Order.Application.Consumers;
using Order.Infra.Projectors;
namespace Order.Api.Extensions;

public static class EventBusExtensions
{
    public static async Task ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        await eventBus.SubscribeAsync<StockReservationResultEvent, StockReservationResultConsumer>();

        #region MongoDb Integration Events
        await eventBus.SubscribeAsync<OrderCreatedEvent, OrderCreatedProjector>();
        await eventBus.SubscribeAsync<OrderUpdatedEvent, OrderUpdatedProjector>();
        await eventBus.SubscribeAsync<UserUpdatedEvent, UserUpdatedProjector>();
        #endregion
    }
}