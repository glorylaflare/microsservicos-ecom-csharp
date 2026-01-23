using FluentAssertions;
using Stock.Domain.Models;
namespace Stock.UnitTests.Domain.Models;

public class ProductTests
{
    private const string _name = "Test Product";
    private const string _description = "This is a test product.";
    private const decimal _price = 99.99m;
    private const int _stockQuantity = 100;
    [Fact]
    public void CreateProduct_WhenValid_ShouldReturnValidParameters()
    {
        // Act
        var product = new Product(_name, _description, _price, _stockQuantity);
        // Assert
        product.Name.Should().Be(_name);
    }
    [Fact]
    public void UpdateProduct_WhenValid_ShouldUpdateProperties()
    {
        // Arrange
        var product = new Product(_name, _description, _price, _stockQuantity);
        var newName = "Updated Product";
        var newDescription = "This is an updated test product.";
        var newPrice = 149.99m;
        // Act
        product.UpdateProduct(newName, newDescription, newPrice);
        // Assert
        product.Name.Should().Be(newName);
    }
    [Fact]
    public void DecreaseStock_WhenValid_ShouldDecreaseStockQuantity()
    {
        // Arrange
        var product = new Product(_name, _description, _price, _stockQuantity);
        var decreaseBy = 10;
        var expectedStock = _stockQuantity - decreaseBy;
        // Act
        product.DecreaseStock(decreaseBy);
        // Assert
        product.StockQuantity.Should().Be(expectedStock);
    }
}