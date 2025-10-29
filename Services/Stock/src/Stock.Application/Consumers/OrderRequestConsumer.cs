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
        try
        {
            var totalAmount = 0m;

            _logger.Debug("Reserving stock for Order ID: {OrderId} with {ItemsCount} items", @event.OrderId, @event.Items.Count);
            await _dbTransactionManager.ExecuteResilientTransactionAsync(async () =>
            {
                _logger.Debug("Checking stock availability for Order ID: {OrderId}", @event.OrderId);
                foreach (var item in @event.Items)
                {
                    _logger.Debug("Retrieving product with ID: {ProductId} for stock reservation", item.ProductId);
                    var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                    if (product is null)
                    {
                        _logger.Warning("Product with ID: {ProductId} not found for Order ID: {OrderId}", item.ProductId, @event.OrderId);
                        throw new InvalidOperationException("Product not found.");
                    }
                    if (product.StockQuantity < item.Quantity)
                    {
                        _logger.Warning("Insufficient stock for Product ID: {ProductId} for Order ID: {OrderId}", item.ProductId, @event.OrderId);
                        throw new InvalidOperationException("Insufficient stock.");
                    }

                    _logger.Debug("Decreasing stock for Product ID: {ProductId} by {Quantity}", item.ProductId, item.Quantity);
                    product.DecreaseStock(item.Quantity);

                    _logger.Debug("Calculating total amount for Order ID: {OrderId}", @event.OrderId);
                    totalAmount += item.Quantity * product.Price;
                }

                _logger.Debug("Saving stock changes for Order ID: {OrderId}", @event.OrderId);
                await _productRepository.SaveChangesAsync();

                _logger.Information("Stock reserved successfully for Order ID: {OrderId} with Total Amount: {TotalAmount}", @event.OrderId, totalAmount);
            });

            await _eventBus.PublishAsync(new StockReservedEvent(@event.OrderId, @event.Items, totalAmount));
            _logger.Information("{EventName} for Order ID: {OrderId} handled successfully", nameof(OrderRequestedEvent), @event.OrderId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning("Stock reservation failed for Order ID: {OrderId} due to: {Reason}", @event.OrderId, ex.Message);
            await _eventBus.PublishAsync(new StockRejectedEvent(@event.OrderId, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while handling {EventName} for Order ID: {OrderId}", nameof(OrderRequestedEvent), @event.OrderId);
            throw;
        }
    }
}
