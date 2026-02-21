using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using BuildingBlocks.Messaging;
using FluentResults;
using MediatR;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using Microsoft.Extensions.Configuration;
using Payment.Domain.Interface;
using Payment.Domain.Models;
using Serilog;
namespace Payment.Application.Commands.Handlers;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<Unit>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly IEventBus _eventBus;

    public ProcessPaymentCommandHandler(IPaymentRepository paymentRepository, IConfiguration configuration, IEventBus eventBus)
    {
        _paymentRepository = paymentRepository;
        _configuration = configuration;
        _eventBus = eventBus;
        _logger = Log.ForContext<ProcessPaymentCommandHandler>();
    }

    public async Task<Result<Unit>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {CommandName}", nameof(ProcessPaymentCommand));
        
        MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];
        
        var paymentClient = new PaymentClient();
        var paymentData = await paymentClient.GetAsync(
            id: long.Parse(request.Data.Id),
            cancellationToken: cancellationToken
        );

        if ( paymentData == null ) 
        {
            _logger.Error("[ERROR] Paymentdata not found");
            return Result.Fail("Paymentdata not found in Mercado Pago database");
        }

        var metadata = paymentData?.Metadata;
        var status = paymentData.Status;
        
        if (metadata == null || !metadata.Any())
        {
            _logger.Error("[ERROR] Metadata not found in payment data");
            return Result.Fail("Metadata not found in payment data");
        }

        if (!metadata.TryGetValue("order_id", out var orderIdValue))
        {
            _logger.Error("[ERROR] order_id not found in payment metadata");
            return Result.Fail("order_id not found in payment metadata");
        }

        if (!int.TryParse(orderIdValue.ToString(), out int orderId))
        {
            _logger.Error("[ERROR] Invalid order_id format: {OrderId}", orderIdValue);
            return Result.Fail("Invalid order_id format in payment metadata");
        }

        var payment = await _paymentRepository.GetByIdAsync(orderId);
        if (payment == null)
        {
            _logger.Error("[ERROR] Payment with order ID {OrderId} not found", orderId);
            return Result.Fail($"Payment with order ID {orderId} not found");
        }
        
        _logger.Information("[INFO] Processing payment of type: {PaymentType}", request.Type);

        var currentStatus = validateStatus(status);

        payment.SetStatus(currentStatus);

        _paymentRepository.Update(payment);
        await _paymentRepository.SaveChangesAsync();

        var data = new PaymentUpdatedData(payment.Id, payment.OrderId, payment.CheckoutUrl!, payment.Status.ToString());
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