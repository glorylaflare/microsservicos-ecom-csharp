using FluentResults;
using MediatR;
using Payment.Application.Interfaces;
using Payment.Application.Responses;
using Serilog;

namespace Payment.Application.Queries.GetAllPayments;

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, Result<IEnumerable<GetPaymentResponse>>>
{
    private readonly IPaymentReadService _paymentService;
    private readonly ILogger _logger;

    public GetAllPaymentsQueryHandler(IPaymentReadService paymentService)
    {
        _paymentService = paymentService;
        _logger = Log.ForContext<GetAllPaymentsQueryHandler>();
    }

    public async Task<Result<IEnumerable<GetPaymentResponse>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("[INFO] Handling {EventName}", nameof(GetAllPaymentsQuery));
            var payments = await _paymentService.GetAllAsync();

            if (payments is null || !payments.Any())
            {
                _logger.Warning("[WARN] No payments found while handling {EventName}", nameof(GetAllPaymentsQuery));
                return Result.Fail<IEnumerable<GetPaymentResponse>>("No payments found");
            }
            var response = payments.Select(payment => new GetPaymentResponse(
                payment.Id,
                payment.Status!.ToString(),
                payment.CreatedAt
            ));
            _logger.Information("[INFO] Successfully fetched and mapped {PaymentCount} payments", response.Count());
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error handling {EventName}", nameof(GetAllPaymentsQuery));
            return Result.Fail<IEnumerable<GetPaymentResponse>>("An error occurred while processing the request");
        }
    }
}