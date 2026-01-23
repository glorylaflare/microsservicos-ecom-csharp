namespace Stock.Domain.Exceptions;

public sealed class InsufficientStockException : DomainException
{
    public InsufficientStockException(int productId, int requestedQuantity, int availableQuantity)
        : base($"Insufficient stock for Product ID {productId}. Requested: {requestedQuantity}, Available: {availableQuantity}.")
    {
    }
}
