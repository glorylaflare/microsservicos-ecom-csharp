using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using MediatR;
using Serilog;
using Stock.Application.Commands;

namespace Stock.Application.Consumers;

public class OrderFailedConsumer : IIntegrationEventHandler<OrderEmailRequestEvent>
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public OrderFailedConsumer(IMediator mediator)
    {
        _mediator = mediator;
        _logger = Log.ForContext<OrderFailedConsumer>();
    }

    public async Task HandleAsync(OrderEmailRequestEvent @event)
    {
        var totalQuantity = @event.Data.Items.Sum(item => item.Quantity);

        _logger.Information("[INFO] Handling order failed event with reason: {@Reason}, total quantity: {TotalQuantity}", @event.Data.Reason, totalQuantity);

        await _mediator.Send(new OrderFailedCommand(@event.Data.Items));
    }
}
