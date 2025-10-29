using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using Order.Domain.Interfaces;
using Serilog;

namespace Order.Application.Consumers;

public class StockReservedConsumer : IIntegrationEventHandler<StockReservedEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;

    public StockReservedConsumer(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        _logger = Log.ForContext<StockReservedConsumer>();
    }

    public async Task HandleAsync(StockReservedEvent @event)
    {
        try
        {
            _logger.Information("Handling {EventName} for OrderId {OrderId}, TotalAmount {TotalAmount}", nameof(StockReservedEvent), @event.OrderId, @event.TotalAmount);

            _logger.Debug("Retrieving order with ID: {OrderId} from repository", @event.OrderId);
            var order = await _orderRepository.GetOrderByIdAsync(@event.OrderId);
            if (order is null)
            {
                _logger.Warning("Order with ID {OrderId} not found", @event.OrderId);
                return;
            }

            _logger.Debug("Confirming order with ID {OrderId} and setting total amount to {TotalAmount}", order.Id, @event.TotalAmount);

            order.SetTotalAmount(@event.TotalAmount);
            order.Confirmed();

            await _orderRepository.SaveChangesAsync();
            _logger.Information("Order with ID {OrderId} has been confirmed with total amount {TotalAmount}", order.Id, @event.TotalAmount);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while handling {EventName} for OrderId {OrderId}", nameof(StockReservedEvent), @event.OrderId);
            throw;
        }
    }
}
