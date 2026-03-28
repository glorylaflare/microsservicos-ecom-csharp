using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using MediatR;
using Notification.Application.Commands.OrderCompleted;
using Notification.Application.Commands.OrderFailed;
using Notification.Application.Commands.OrderPending;
using Serilog;

namespace Notification.Application.Consumers;

public class OrderEmailRequestConsumer : IIntegrationEventHandler<OrderEmailRequestEvent>
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public OrderEmailRequestConsumer(IMediator mediator)
    {
        _mediator = mediator;
        _logger = Log.ForContext<OrderEmailRequestConsumer>();
    }

    public async Task HandleAsync(OrderEmailRequestEvent @event)
    {
        _logger.Information("[INFO] OrderEmailRequestEvent consumed. OrderId: {OrderId}, Status: {Status}", @event.Data.OrderId, @event.Data.Status);

        var data = @event.Data;

        switch (data.Status)
        {
            case OrderPaymentStatus.Failed:
                await _mediator.Send(new OrderFailedEmailCommand(data.Email, data.OrderId, data.Items, data.Reason!));
                break;

            case OrderPaymentStatus.Pending:
                await _mediator.Send(new OrderPendingEmailCommand(data.Email, data.OrderId, data.Items, data.CheckoutUrl!));
                break;

            case OrderPaymentStatus.Completed:
                await _mediator.Send(new OrderCompletedEmailCommand(data.Email, data.OrderId, data.Items));
                break;
        }
    }
}
