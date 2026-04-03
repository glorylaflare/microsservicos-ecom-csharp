using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using MediatR;
using Notification.Application.Commands.UserCreated;
using Serilog;

namespace Notification.Application.Consumers;

public class UserCreatedEmailRequestConsumer : IIntegrationEventHandler<UserCreatedEmailRequestEvent>
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public UserCreatedEmailRequestConsumer(IMediator mediator)
    {
        _mediator = mediator;
        _logger = Log.ForContext<UserCreatedEmailRequestConsumer>();
    }

    public async Task HandleAsync(UserCreatedEmailRequestEvent @event)
    {
        _logger.Information("[INFO] Handling {EventName}", nameof(UserCreatedEmailRequestEvent));

        await _mediator.Send(new UserCreatedEmailCommand(@event.Data.Email, @event.Data.Username));
    }
}
