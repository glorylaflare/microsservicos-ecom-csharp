using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using MediatR;
using Serilog;
using Stock.Application.Commands;
namespace Stock.Application.Consumers;

public class OrderRequestConsumer : IIntegrationEventHandler<OrderRequestedEvent>
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;
    public OrderRequestConsumer(IMediator mediator)
    {
        _mediator = mediator;
        _logger = Log.ForContext<OrderRequestConsumer>();
    }
    public async Task HandleAsync(OrderRequestedEvent @event)
    {
        _logger.Information("[INFO] Handling {EventName} for Order ID: {OrderId}", nameof(OrderRequestedEvent), @event.Data.OrderId);
        await _mediator.Send(new OrderRequestCommand(@event.Data.OrderId, @event.Data.Items));
    }
}