using BuildingBlocks.Contracts;

namespace Order.Application.Integrations.Contexts;

public class OrderEmailContext
{
    public string Email { get; init; } = null!;
    public List<OrderItemDto> Items { get; init; } = new();
}
