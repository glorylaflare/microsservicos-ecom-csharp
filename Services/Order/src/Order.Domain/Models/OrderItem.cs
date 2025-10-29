namespace Order.Domain.Models;

public class OrderItem
{
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }

    protected OrderItem() { }

    public OrderItem(int productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}
