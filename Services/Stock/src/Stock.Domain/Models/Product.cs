using BuildingBlocks.SharedKernel.Common;

namespace Stock.Domain.Models;

public class Product : EntityBase
{
    public string Name { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public byte[] Version { get; private set; }

    protected Product() { }

    public Product(string name, string description, decimal price, int stockQuantity)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public void UpdateProduct(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecreaseStock(int quantity)
    {
        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
