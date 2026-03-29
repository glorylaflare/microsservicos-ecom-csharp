using FluentResults;
using MediatR;
using Payment.Application.Interfaces;
using Payment.Application.Responses;
using Payment.Application.Specifications;
using Serilog;

namespace Payment.Application.Queries.GetPaymentById;

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, Result<PaymentResponse>>
{
    private readonly IPaymentReadService _paymentService;
    private readonly ILogger _logger;

    public GetPaymentByIdQueryHandler(IPaymentReadService paymentService)
    {
        _paymentService = paymentService;
        _logger = Log.ForContext<GetPaymentByIdQueryHandler>();
    }

    public async Task<Result<PaymentResponse>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("[INFO] Handling {EventName} for Id: {PaymentId}", nameof(GetPaymentByIdQuery), request.id);
            var payment = await _paymentService.FindOneAsync(new PaymentByPaymentIdSpec(request.id), cancellationToken);
            
            if (payment == null)
            {
                _logger.Warning("[WARN] Payment not found for Id: {PaymentId}", request.id);
                return Result.Fail(new Error("Payment not found"));
            }

            var response = new PaymentResponse(payment.Id, payment.Status!.ToString(), payment.CreatedAt);
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error handling {EventName} for Id: {PaymentId}", nameof(GetPaymentByIdQuery), request.id);
            return Result.Fail(new Error("An error occurred while processing the request"));
        }
    }
}