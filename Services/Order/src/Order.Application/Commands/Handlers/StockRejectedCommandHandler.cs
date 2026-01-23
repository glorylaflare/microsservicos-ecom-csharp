using MediatR;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Serilog;
namespace Order.Application.Commands.Handlers;
public class StockRejectedCommandHandler : IRequestHandler<StockRejectedCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;
    public StockRejectedCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        _logger = Log.ForContext<StockRejectedCommandHandler>();
    }
    public async Task<Unit> Handle(StockRejectedCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {CommandName} for OrderId {OrderId}, Reason: {Reason}", nameof(StockRejectedCommand), request.OrderId, request.Reason);
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        if (order is null)
        {
            _logger.Warning("[WARN] Order with ID {OrderId} not found", request.OrderId);
            return Unit.Value;
        }
        order.Cancelled();
        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync();
        _logger.Information("[INFO] Order with ID {OrderId} has been cancelled due to stock rejection", order.Id);
        return Unit.Value;
    }
}