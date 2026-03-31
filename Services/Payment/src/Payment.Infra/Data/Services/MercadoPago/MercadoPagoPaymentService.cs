using FluentResults;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Error;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Options;
using Payment.Application.Interfaces;
using Payment.Application.Requests;
using Payment.Application.Responses;
using Payment.Infra.Configurations;
using Serilog;

namespace Payment.Infra.Data.Services.MercadoPago;

public class MercadoPagoPaymentService : IMercadoPagoPaymentService
{
    private readonly ILogger _logger ;
    private readonly MercadoPagoSettings _settings;
    private const string OrderIdKey = "order_id";

    public MercadoPagoPaymentService(IOptions<MercadoPagoSettings> options)
    {
        _logger = Log.ForContext<MercadoPagoPaymentService>();
        _settings = options.Value;
    }

    public async Task<Result<Preference>> CreateMercadoPagoPayment(PaymentRequest request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Creating payment preference for {EventId}", request.EventId);

        try
        {
            var preference = await CreateMercadoPagoPaymentPreference(request, cancellationToken);
            if (preference.IsFailed)
            {
                _logger.Error("[ERROR] Failed to create payment preference for {EventId}", request.EventId);
                return Result.Fail(preference.Errors);
            }
            
            return Result.Ok(preference.Value);
        }

        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] An unexpected error occurred while creating payment preference for {EventId}.", request.EventId);
            return Result.Fail("An unexpected error occurred while creating payment preference");
        }
    }

    public async Task<Result<PaymentProcessResponse>> ProcessMercadoPagoPayment(long paymentId, CancellationToken cancellationToken)
    {
        var paymentClient = new PaymentClient();
        var paymentData = await paymentClient.GetAsync(
            id: paymentId,
            cancellationToken: cancellationToken
        );

        if (paymentData == null)
        {
            _logger.Error("[ERROR] Paymentdata not found");
            return Result.Fail($"Payment {paymentId} not found in MercadoPago");
        }

        var metadata = paymentData?.Metadata;
        var status = paymentData?.Status;

        if (metadata == null || !metadata.Any())
        {
            _logger.Error("[ERROR] Metadata not found in payment data");
            return Result.Fail("Metadata not found in payment data");
        }

        if (!metadata.TryGetValue(OrderIdKey, out var orderIdValue))
        {
            _logger.Error("[ERROR] order_id not found in payment metadata");
            return Result.Fail("order_id not found in payment metadata");
        }

        if (!int.TryParse(orderIdValue.ToString(), out int orderId))
        {
            _logger.Error("[ERROR] Invalid order_id format: {OrderId}", orderIdValue);
            return Result.Fail("Invalid order_id format in payment metadata");
        }

        var response = new PaymentProcessResponse(orderId, status ?? "unknown");

        return Result.Ok(response);
    }

    private async Task<Result<Preference>> CreateMercadoPagoPaymentPreference(PaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentItems = request.Items.Select(item => new PreferenceItemRequest
            {
                Title = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList();

            var paymentRequest = new PreferenceRequest
            {
                Items = paymentItems,
                NotificationUrl = _settings.NotificationUrl,
                ExternalReference = request.EventId.ToString(),
                Expires = true,
                ExpirationDateTo = DateTime.UtcNow.AddMinutes(_settings.DefaultExpirationMinutes),
                Metadata = new Dictionary<string, object>
                {
                    { OrderIdKey, request.OrderId }
                }
            };

            _logger.Information("[INFO] Preference request created with {ItemCount} items", paymentItems.Count);

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(paymentRequest, cancellationToken: cancellationToken);

            _logger.Information("[INFO] Payment preference created with ID: {PreferenceId}", preference.Id);

            return Result.Ok(preference);
        }
        catch (MercadoPagoApiException ex)
        {
            _logger.Error(
                ex,
                "MercadoPago API error while creating payment preference. StatusCode: {StatusCode}, Message: {Message}",
                ex.StatusCode, ex.Message
            );
            return Result.Fail("MercadoPago API error while creating payment preference.");
        }
    }
}
