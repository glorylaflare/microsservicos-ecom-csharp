namespace Stock.Domain.Exceptions;

public sealed class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(int productId) : base(
        $"Product with ID {productId} not found.")
    {
    }
}