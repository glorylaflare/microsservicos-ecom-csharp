using FluentResults;
using MediatR;
using Payment.Domain.Interface;
using Serilog;

namespace Payment.Application.Commands.Handlers;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<Unit>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger _logger;

    public ProcessPaymentCommandHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
        _logger = Log.ForContext<ProcessPaymentCommandHandler>();
    }

    public async Task<Result<Unit>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {CommandName}", nameof(ProcessPaymentCommand));

        var orderId = request.Data.Metadata!["orderId"]; // TODO: Verificar o motivo de não estar recebendo o metadata

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
