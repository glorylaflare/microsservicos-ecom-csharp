using FluentResults;
using MediatR;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using Microsoft.Extensions.Configuration;
using Payment.Domain.Interface;
using Serilog;
namespace Payment.Application.Commands.Handlers;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<Unit>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    public ProcessPaymentCommandHandler(IPaymentRepository paymentRepository, IConfiguration configuration)
    {
        _paymentRepository = paymentRepository;
        _configuration = configuration;
        _logger = Log.ForContext<ProcessPaymentCommandHandler>();
    }
    public async Task<Result<Unit>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {CommandName}", nameof(ProcessPaymentCommand));
        MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];
        var paymentClient = new PaymentClient();
        var paymentData = await paymentClient.GetAsync(
            id: long.Parse(request.Data.Id),
            cancellationToken: cancellationToken);

        var metadata = paymentData?.Metadata;
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
        payment.MarkAsPaid();
        _paymentRepository.Update(payment);
        await _paymentRepository.SaveChangesAsync();
        return Result.Ok(Unit.Value);
    }
}