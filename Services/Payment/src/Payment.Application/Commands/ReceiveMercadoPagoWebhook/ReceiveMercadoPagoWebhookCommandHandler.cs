using FluentResults;
using MediatR;
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

        var isProcessable = IsProcessable(request.Payload);
        if (!IsProcessable(request.Payload))
        {
            _logger.Information("[INFO] Webhook ignored (not a payment event)");
            return Result.Ok(Unit.Value);
        }

        var data = BuildData(request.Payload);
        if (data.IsFailed)
        {
            return Result.Fail(data.Errors.ToList());
        }

        var eventId = data.Value.ExternalId.ToString();
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

    private bool IsProcessable(string payload)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload);

            if (!doc.RootElement.TryGetProperty("type", out var type))
                return false;

            return type.GetString() == "payment";
        }
        catch
        {
            return false;
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

            if (data?.ExternalId == null)
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