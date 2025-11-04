using FluentResults;
using MediatR;
using Order.Application.Responses;
using Order.Domain.Interfaces;
using Serilog;

namespace Order.Application.Queries.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<GetOrderResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        _logger = Log.ForContext<GetOrderByIdQueryHandler>();
    }

    public async Task<Result<GetOrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Handling {EventName} for Order ID: {OrderId}", nameof(GetOrderByIdQuery), request.Id);

            var order = await _orderRepository.GetOrderByIdAsync(request.Id);
            if (order is null)
            {
                _logger.Warning("Order with ID: {OrderId} not found", request.Id);
                return Result.Fail(new Error("Order not found."));
            }

            var response = new GetOrderResponse(
                order.Id,
                order.Items,
                order.TotalAmount,
                order.Status,
                order.CreatedAt,
                order.UpdatedAt
            );

            _logger.Information("{EventName} for Order ID: {OrderId} handled successfully", nameof(GetOrderByIdQuery), request.Id);
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while handling {EventName} for Order ID: {OrderId}", nameof(GetOrderByIdQuery), request.Id);
            return Result.Fail("An error occurred while retrieving the order.");
        }
    }
}
