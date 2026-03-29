using FluentResults;
using MediatR;
using Payment.Application.Commands.ProcessPayment;
using Payment.Application.Commands.ReceiveMercadoPagoWebhook;
using Payment.Domain.Interfaces;
using Payment.Domain.Models;
using Serilog;
using System.Text.Json;

namespace Payment.Application.Commands.Webhooks;

public class ReceiveMercadoPagoWebhookCommandHandler : IRequestHandler<ReceiveMercadoPagoWebhookCommand, Result<Unit>>
{
    private readonly IWebhookRepository _webhookRepository;
    private readonly ILogger _logger;

    public ReceiveMercadoPagoWebhookCommandHandler(IWebhookRepository webhookRepository)
    {
        _webhookRepository = webhookRepository;
        _logger = Log.ForContext<ReceiveMercadoPagoWebhookCommandHandler>();
    }

    public async Task<Result<Unit>> Handle(ReceiveMercadoPagoWebhookCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Receiving MercadoPago webhook in {CommandName}", nameof(ReceiveMercadoPagoWebhookCommandHandler));

        var data = BuildData(request.Payload);
        if (data.IsFailed)
        {
            return Result.Fail(data.Errors.ToList());
        }

        var eventId = data.Value.Id.ToString();

        try
        {
            var webhook = new WebhookEvent(
                eventId: eventId,
                payload: data.Value
            );

            await _webhookRepository.AddAsync(webhook, cancellationToken);
            await _webhookRepository.SaveChangesAsync(cancellationToken);

            _logger.Information("[INFO] Webhook stored successfully. EventId: {EventId}", eventId);

            return Result.Ok(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error while storing webhook. EventId: {EventId}", eventId);
            return Result.Fail("An error occurred while storing webhook");
        }
    }

    private Result<WebhookPayload> BuildData(string payload)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<WebhookPayload>(payload, options);

            if (data?.Id == null)
            {
                _logger.Warning("[WARN] Webhook payload missing Id. Payload: {Payload}", payload);
                return Result.Fail("Webhook Id not found");
            }

            return Result.Ok(data);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Failed to deserialize webhook payload. Payload: {Payload}", payload);
            return Result.Fail("Invalid webhook payload");
        }
    }
}