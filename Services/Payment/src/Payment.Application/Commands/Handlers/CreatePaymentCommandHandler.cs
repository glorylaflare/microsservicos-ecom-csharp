using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using FluentResults;
using FluentValidation;
using MediatR;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Error;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using Payment.Domain.Interface;
using Serilog;
namespace Payment.Application.Commands.Handlers;
public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Unit>
{
    private readonly IConfiguration _configuration;
    private readonly IValidator<CreatePaymentCommand> _validator;
    private readonly IEventBus _eventBus;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger _logger;
    public CreatePaymentCommandHandler(IConfiguration configuration, IPaymentRepository paymentRepository, IEventBus eventBus, IValidator<CreatePaymentCommand> validator)
    {
        _configuration = configuration;
        _paymentRepository = paymentRepository;
        _eventBus = eventBus;
        _validator = validator;
        _logger = Log.ForContext<CreatePaymentCommandHandler>();
    }
    public async Task<Unit> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));
            _logger.Warning("[WARN] Validation failed for {EventName}: {Errors}", nameof(CreatePaymentCommand), errors);
            return Unit.Value;
        }
        MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];
        if (string.IsNullOrEmpty(MercadoPagoConfig.AccessToken))
        {
            _logger.Error("[ERROR] MercadoPago Access Token is not configured.");
            return Unit.Value;
        }
        _logger.Information("[INFO] Creating payment preference for {EventId}", request.EventId);
        try
        {
            var preference = await CreateMercadoPagoPaymentPreference(request, cancellationToken);
            if (preference.IsFailed)
            {
                _logger.Error("[ERROR] Failed to create payment preference for {EventId}", request.EventId);
                return Unit.Value;
            }
            var payment = new Domain.Models.Payment(
                orderId: request.OrderId,
                amount: request.TotalAmount,
                checkoutUrl: preference.Value.InitPoint
            );
            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();
            _logger.Information("[INFO] Payment created with ID: {PaymentId} for EventId: {EventId}", payment.Id, request.EventId);
            var data = new PaymentCreatedData(payment.Id, payment.CheckoutUrl!);
            var evt = new PaymentCreatedEvent(data);
            await _eventBus.PublishAsync(evt);
            _logger.Information("[INFO] PaymentCreatedEvent published for PaymentId: {PaymentId}", payment.Id);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] An unexpected error occurred while creating payment preference for {EventId}.", request.EventId);
        }
        _logger.Information("[INFO] Finished processing CreatePaymentCommand for {EventId}", request.EventId);
        return Unit.Value;
    }
    private async Task<Result<Preference>> CreateMercadoPagoPaymentPreference(CreatePaymentCommand request, CancellationToken cancellationToken)
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
                NotificationUrl = _configuration["MercadoPago:NotificationUrl"],
                ExternalReference = request.EventId.ToString(),
                Expires = true,
                Metadata = new Dictionary<string, object>
                {
                    { "orderId", request.OrderId }
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