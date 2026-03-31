using MediatR;
using Payment.Application.Commands.ProcessPayment;
using Payment.Application.Interfaces;
using Payment.Application.Specifications;
using Payment.Domain.Interfaces;
using Serilog;

namespace Payment.Application.Services;

public class WebhookProcessorService : IWebhookProcessorService
{
    private readonly IWebhookRepository _webhookRepository;
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    public WebhookProcessorService(IWebhookRepository webhookRepository, IMediator mediator)
    {
        _webhookRepository = webhookRepository;
        _mediator = mediator;
        _logger = Log.ForContext<WebhookProcessorService>();
    }

    public async Task ProcessWebhookAsync()
    {
        var cancellationToken = CancellationToken.None;

        var pendingEvents = await _webhookRepository.WhereAsync(new GetPendingWebhookEventsSpec());

        foreach (var webhook in pendingEvents)
        {
            try
            {
                var result = await _mediator.Send(new ProcessPaymentCommand(webhook.Payload));
                if (result.IsFailed) continue;
                webhook.MarkAsProcessed();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[ERROR] Error processing webhook {EventId}", webhook.EventId);
            }
        }

        await _webhookRepository.SaveChangesAsync(cancellationToken);
    }
}
