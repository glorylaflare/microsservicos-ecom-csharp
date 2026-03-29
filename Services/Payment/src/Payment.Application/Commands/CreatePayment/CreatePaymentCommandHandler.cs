using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using FluentResults;
using FluentValidation;
using MediatR;
using Payment.Application.Interfaces;
using Payment.Application.Requests;
using Payment.Domain.Interface;
using Serilog;

namespace Payment.Application.Commands.CreatePayment;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<Unit>>
{
    private readonly IValidator<CreatePaymentCommand> _validator;
    private readonly IEventBus _eventBus;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMercadoPagoPaymentService _mercadoPagoService;
    private readonly ILogger _logger;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository, 
        IMercadoPagoPaymentService mercadoPagoService, 
        IEventBus eventBus, 
        IValidator<CreatePaymentCommand> validator)
    {
        _paymentRepository = paymentRepository;
        _mercadoPagoService = mercadoPagoService;
        _eventBus = eventBus;
        _validator = validator;
        _logger = Log.ForContext<CreatePaymentCommandHandler>();
    }

    public async Task<Result<Unit>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {CommandName}", nameof(CreatePaymentCommand));
        
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));
            _logger.Warning("[WARN] Validation failed for {EventName}: {Errors}", nameof(CreatePaymentCommand), errors);
            return Result.Fail(errors);
        }

        try
        {
            var paymentRequest = new PaymentRequest(request.EventId, request.OrderId, request.TotalAmount, request.Items);

            var preference = await _mercadoPagoService.CreateMercadoPagoPayment(paymentRequest);
            if (preference.IsFailed)
            {
                _logger.Warning("[WARN] Failed to create MercadoPago payment preference for OrderId: {OrderId}", request.OrderId);
                return Unit.Value;
            }

            var payment = new Domain.Models.Payment(
                orderId: request.OrderId,
                amount: request.TotalAmount,
                checkoutUrl: preference.Value.InitPoint,
                mercadoPagoPreference: preference.Value.Id,
                expirationDate: preference.Value.ExpirationDateTo
            );

            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _paymentRepository.SaveChangesAsync();

            _logger.Information("[INFO] Payment created with ID: {PaymentId} for EventId: {EventId}", payment.Id, request.EventId);

            var data = new PaymentUpdatedData(
                payment.Id,
                payment.OrderId,
                payment.CheckoutUrl!,
                payment.Status.ToString());
            var evt = new PaymentUpdatedEvent(data);

            await _eventBus.PublishAsync(evt);
            _logger.Information("[INFO] PaymentCreatedEvent published for PaymentId: {PaymentId}", payment.Id);

            _logger.Information("[INFO] Finished processing CreatePaymentCommand for {EventId}", request.EventId);
            return Result.Ok(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] An unexpected error occurred while creating payment preference for {EventId}.", request.EventId);
            return Result.Fail("An error occurred while creating the payment");
        }
    }
}