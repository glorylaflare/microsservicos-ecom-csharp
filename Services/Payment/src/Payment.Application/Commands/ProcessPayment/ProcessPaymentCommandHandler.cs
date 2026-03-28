using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using FluentResults;
using MediatR;
using Payment.Application.Interfaces;
using Payment.Domain.Interface;
using Payment.Domain.Models;
using Serilog;

namespace Payment.Application.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<Unit>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMercadoPagoPaymentService _mercadoPagoService;
    private readonly ILogger _logger;
    private readonly IEventBus _eventBus;

    public ProcessPaymentCommandHandler(IPaymentRepository paymentRepository, IMercadoPagoPaymentService mercadoPagoService, IEventBus eventBus)
    {
        _paymentRepository = paymentRepository;
        _mercadoPagoService = mercadoPagoService;
        _eventBus = eventBus;
        _logger = Log.ForContext<ProcessPaymentCommandHandler>();
    }

    public async Task<Result<Unit>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {CommandName}", nameof(ProcessPaymentCommand));

        if (!long.TryParse(request.Data.Id, out var paymentId))
        {
            _logger.Error("Invalid payment id: {Id}", request.Data.Id);
            return Result.Fail("Invalid payment id");
        }

        var processResult = await _mercadoPagoService.ProcessMercadoPagoPayment(paymentId);
        if (processResult.IsFailed)
        {
            _logger.Error("[ERROR] Failed to process MercadoPago payment for PaymentId: {PaymentId}", paymentId);
            return Result.Fail("Failed to process MercadoPago payment");
        }

        var orderId = processResult.Value.OrderId;

        var payment = await _paymentRepository.GetByIdAsync(orderId);
        if (payment == null)
        {
            _logger.Error("[ERROR] Payment with order ID {OrderId} not found", orderId);
            return Result.Fail($"Payment with order ID {orderId} not found");
        }

        if (payment.Status == PaymentStatus.Paid)
        {
            _logger.Information("Payment already processed for OrderId: {OrderId}", orderId);
            return Result.Ok(Unit.Value);
        }

        _logger.Information("[INFO] Processing payment of type: {PaymentType}", request.Type);

        var currentStatus = validateStatus(processResult.Value.Status);

        payment.SetStatus(currentStatus);

        _paymentRepository.Update(payment);
        await _paymentRepository.SaveChangesAsync();

        var data = new PaymentUpdatedData(
            payment.Id, 
            payment.OrderId, 
            payment.CheckoutUrl!, 
            payment.Status.ToString());
        var evt = new PaymentUpdatedEvent(data);

        await _eventBus.PublishAsync(evt);

        _logger.Information("[INFO] Payment with order ID {OrderId} processed successfully", orderId);

        return Result.Ok(Unit.Value);
    }

    public PaymentStatus validateStatus(string status)
    {
        if (!status.Equals("approved"))
        {
            _logger.Information("Payment status '{Status}' is not approved; setting payment status to Failed", status);
            return PaymentStatus.Failed;
        }

        _logger.Information("Payment status '{Status}' is approved; payment processed successfully", status);
        return PaymentStatus.Paid;
    }
}