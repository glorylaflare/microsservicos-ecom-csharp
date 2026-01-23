using FluentResults;
using MediatR;
using Order.Application.Interfaces;
using Order.Application.Responses;
using Serilog;
namespace Order.Application.Queries.Handlers;
public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<GetOrderResponse>>
{
    private readonly IOrderReadService _orderService;
    private readonly ILogger _logger;
    public GetOrderByIdQueryHandler(IOrderReadService orderService)
    {
        _orderService = orderService;
        _logger = Log.ForContext<GetOrderByIdQueryHandler>();
    }
    public async Task<Result<GetOrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("[INFO] Handling {EventName} for Order ID: {OrderId}", nameof(GetOrderByIdQuery), request.Id);
            var order = await _orderService.GetByIdAsync(request.Id);
            if (order is null)
            {
                _logger.Warning("[WARN] Order with ID: {OrderId} not found", request.Id);
                return Result.Fail(new Error("Order not found."));
            }
            var response = new GetOrderResponse(
                order.Id,
                order.Items,
                order.TotalAmount,
                order.Status.ToString(),
                order.CreatedAt,
                order.UpdatedAt
            );
            _logger.Information("[INFO] {EventName} for Order ID: {OrderId} handled successfully", nameof(GetOrderByIdQuery), request.Id);
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error while handling {EventName} for Order ID: {OrderId}", nameof(GetOrderByIdQuery), request.Id);
            return Result.Fail("An error occurred while retrieving the order.");
        }
    }
}