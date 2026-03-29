using BuildingBlocks.SharedKernel.Common;
using FluentResults;
using MediatR;
using Payment.Application.Interfaces;
using Payment.Application.Responses;
using Payment.Application.Specifications;
using Serilog;

namespace Payment.Application.Queries.GetAllPayments;

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, Result<PageResult<PaymentResponse>>>
{
    private readonly IPaymentReadService _paymentService;
    private readonly ILogger _logger;

    public GetAllPaymentsQueryHandler(IPaymentReadService paymentService)
    {
        _paymentService = paymentService;
        _logger = Log.ForContext<GetAllPaymentsQueryHandler>();
    }

    public async Task<Result<PageResult<PaymentResponse>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("[INFO] Handling {EventName}", nameof(GetAllPaymentsQuery));
            var payments = await _paymentService.WhereAsync(new AllPaymentsSpec(request.Skip, request.Take));
            if (payments is null || !payments.Any())
            {
                _logger.Warning("[WARN] No payments found while handling {EventName}", nameof(GetAllPaymentsQuery));
                return Result.Fail("No payments found");
            }

            var response = payments.Select(payment => new PaymentResponse(
                payment.Id,
                payment.Status!.ToString(),
                payment.CreatedAt
            ));

            _logger.Information("[INFO] Successfully fetched and mapped {PaymentCount} payments", response.Count());

            return Result.Ok(new PageResult<PaymentResponse> 
            {
                Items = response,
                Total = response.Count(),
                Skip = request.Skip,
                Take = request.Take
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error handling {EventName}", nameof(GetAllPaymentsQuery));
            return Result.Fail("An error occurred while processing the request");
        }
    }
}