using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Infra.MongoReadModels;
using BuildingBlocks.Messaging;
using MongoDB.Driver;
using Order.Infra.Data.Context;
using Serilog;

namespace Order.Infra.Projectors;

public class OrderUpdatedProjector : IIntegrationEventHandler<OrderUpdatedEvent>
{
    private readonly IMongoCollection<OrderReadModel> _orders;
    private readonly ILogger _logger;

    public OrderUpdatedProjector(ReadDbContext context)
    {
        _orders = context.Orders;
        _logger = Log.ForContext<OrderUpdatedProjector>();
    }

    public async Task HandleAsync(OrderUpdatedEvent @event)
    {
        _logger.Information("[INFO] Projecting {EventName} for Order ID: {OrderId}", nameof(OrderUpdatedEvent), @event.Data.Id);

        await _orders.UpdateOneAsync(
            o => o.Id == @event.Data.Id,
            Builders<OrderReadModel>.Update
                .Set(o => o.Status, @event.Data.Status)
                .Set(o => o.TotalAmount, @event.Data.TotalAmount)
                .Set(o => o.UpdatedAt, @event.Data.UpdatedAt)
        );

        _logger.Information("[INFO] Successfully projected {EventName} for Order ID: {OrderId}", nameof(OrderUpdatedEvent), @event.Data.Id);
    }
}
