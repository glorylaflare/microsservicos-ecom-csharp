using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using Order.Domain.Interfaces;
using Serilog;

namespace Order.Application.Consumers;

public class StockRejectedConsumer : IIntegrationEventHandler<StockRejectedEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;

    public StockRejectedConsumer(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        _logger = Log.ForContext<StockRejectedConsumer>();
    }

    public async Task HandleAsync(StockRejectedEvent @event)
    {
        try
        {
            _logger.Information("Handling {EventName} for OrderId: {OrderId}, Reason: {Reason}", nameof(StockRejectedEvent), @event.OrderId, @event.Reason);

            _logger.Debug("Retrieving order with ID: {OrderId} from repository", @event.OrderId);
            var order = await _orderRepository.GetOrderByIdAsync(@event.OrderId);
            if (order is null)
            {
                _logger.Warning("Order with ID {OrderId} not found", @event.OrderId);
                return;
            }

            _logger.Debug("Cancelling order with ID: {OrderId} due to stock rejection", order.Id);
            order.Cancelled();

            await _orderRepository.SaveChangesAsync();
            _logger.Information("Order with ID: {OrderId} has been cancelled", order.Id);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while handling {EventName} for OrderId {OrderId}", nameof(StockRejectedEvent), @event.OrderId);
            throw;
        }
    }
}
