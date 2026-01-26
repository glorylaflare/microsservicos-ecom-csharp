using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Infra.MongoReadModels;
using BuildingBlocks.Messaging;
using MongoDB.Driver;
using Order.Infra.Data.Context;
using Serilog;

namespace Order.Infra.Projectors;

public class OrderCreatedProjector : IIntegrationEventHandler<OrderCreatedEvent>
{
    private readonly IMongoCollection<OrderReadModel> _orders;
    private readonly ILogger _logger;

    public OrderCreatedProjector(ReadDbContext context)
    {
        _orders = context.Orders;
        _logger = Log.ForContext<OrderCreatedProjector>();
    }

    public async Task HandleAsync(OrderCreatedEvent @event)
    {
        _logger.Information("[INFO] Projecting {EventName} for Order ID: {OrderId}", nameof(OrderCreatedEvent), @event.Data.Id);

        await _orders.InsertOneAsync(new OrderReadModel
        {
            UserId = @event.Data.UserId,
            Id = @event.Data.Id,
            Items = @event.Data.Items.Select(i => new OrderItemReadModel
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList(),
            Status = @event.Data.Status,
            TotalAmount = 0,
            CreatedAt = @event.Data.CreatedAt
        });

        _logger.Information("[INFO] Successfully projected {EventName} for Order ID: {OrderId}", nameof(OrderCreatedEvent), @event.Data.Id);
    }
}
