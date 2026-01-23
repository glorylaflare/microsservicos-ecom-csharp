using MediatR;
using Order.Domain.Interfaces;
using Serilog;

namespace Order.Application.Commands.Handlers;

public class StockReservedCommandHandler : IRequestHandler<StockReservedCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;

    public StockReservedCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        _logger = Log.ForContext<StockReservedCommandHandler>();
    }

    public async Task<Unit> Handle(StockReservedCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {EventName} for OrderId: {OrderId}", nameof(StockReservedCommand), request.OrderId);

        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        if (order is null)
        {
            _logger.Warning("[WARN] Order with ID {OrderId} not found", request.OrderId);
            return Unit.Value;
        }

        order.SetTotalAmount(request.TotalAmount);
        order.Confirmed();

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync();

        _logger.Information("[INFO] Order with ID {OrderId} has been confirmed with total amount {TotalAmount}", order.Id, request.TotalAmount);

        return Unit.Value;
    }
}
