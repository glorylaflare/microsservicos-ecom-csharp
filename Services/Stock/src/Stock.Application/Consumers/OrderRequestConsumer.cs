using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using Serilog;
using Stock.Application.Interfaces;
using Stock.Domain.Interfaces;

namespace Stock.Application.Consumers;

public class OrderRequestConsumer : IIntegrationEventHandler<OrderRequestedEvent>
{
    private readonly IProductRepository _productRepository;
    private readonly IDbTransactionManager _dbTransactionManager;
    private readonly IEventBus _eventBus;
    private readonly ILogger _logger;

    public OrderRequestConsumer(IProductRepository productRepository, IEventBus eventBus, IDbTransactionManager dbTransactionManager)
    {
        _productRepository = productRepository;
        _eventBus = eventBus;
        _dbTransactionManager = dbTransactionManager;
        _logger = Log.ForContext<OrderRequestConsumer>();
    }

    public async Task HandleAsync(OrderRequestedEvent @event)
    {
        _logger.Information("Handling {EventName} for Order ID: {OrderId}", nameof(OrderRequestedEvent), @event.OrderId);

        var correlationId = @event.CorrelationId ?? Guid.NewGuid().ToString();

        try
        {
            var totalAmount = 0m;

            await _dbTransactionManager.ExecuteResilientTransactionAsync(async () =>
            {
                foreach (var item in @event.Items)
                {
                    var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                    if (product is null)
                    {
                        throw new InvalidOperationException("Product not found.");
                    }
                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new InvalidOperationException("Insufficient stock.");
                    }

                    product.DecreaseStock(item.Quantity);
                    totalAmount += item.Quantity * product.Price;
                }
                await _productRepository.SaveChangesAsync();
            });

            await _eventBus.PublishAsync(new StockReservedEvent(@event.OrderId, @event.Items, totalAmount, correlationId));

            _logger.Information("{EventName} for Order ID: {OrderId} handled successfully", nameof(OrderRequestedEvent), @event.OrderId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning("Stock reservation failed for Order ID: {OrderId} due to: {Reason}", @event.OrderId, ex.Message);

            await _eventBus.PublishAsync(new StockRejectedEvent(@event.OrderId, ex.Message, correlationId));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while handling {EventName} for Order ID: {OrderId}", nameof(OrderRequestedEvent), @event.OrderId);
            throw;
        }
    }
}
