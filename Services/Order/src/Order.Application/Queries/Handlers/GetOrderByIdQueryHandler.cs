using FluentResults;
using MediatR;
using Order.Application.Interfaces;
using Order.Application.Responses;
using Serilog;
namespace Order.Application.Queries.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<GetOrderComposeResponse>>
{
    private readonly IOrderReadService _orderService;
    private readonly IUserReadService _userService;
    private readonly ILogger _logger;
    public GetOrderByIdQueryHandler(IOrderReadService orderService, IUserReadService userService)
    {
        _orderService = orderService;
        _userService = userService;
        _logger = Log.ForContext<GetOrderByIdQueryHandler>();
    }
    public async Task<Result<GetOrderComposeResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
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

            var user = await _userService.GetByIdAsync(order.UserId);

            var response = new GetOrderComposeResponse(
                new GetUserResponse(
                    user.Username,
                    user.Email
                ),
                new GetOrderResponse(
                    order.Id,
                    order.Items,
                    order.TotalAmount,
                    order.Status.ToString(),
                    order.CreatedAt,
                    order.UpdatedAt
                )
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