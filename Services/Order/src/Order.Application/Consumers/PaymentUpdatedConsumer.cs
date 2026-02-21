using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using MediatR;
using Order.Application.Commands;
using Serilog;

namespace Order.Application.Consumers;

public class PaymentUpdatedConsumer : IIntegrationEventHandler<PaymentUpdatedEvent>
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public PaymentUpdatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
        _logger = Log.ForContext<PaymentUpdatedConsumer>();
    }

    public async Task HandleAsync(PaymentUpdatedEvent @event)
    {
        _logger.Information("[INFO] Handling {EventName} for OrderId: {OrderId} with Status: {Status}", nameof(PaymentUpdatedEvent), @event.Data.OrderId, @event.Data.Status);

        await _mediator.Send(new PaymentUpdatedCommand(@event.Data.OrderId, @event.Data.Status));
    }
}
