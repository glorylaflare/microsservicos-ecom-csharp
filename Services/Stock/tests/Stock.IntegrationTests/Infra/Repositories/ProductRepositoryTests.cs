using FluentAssertions;
using Stock.Domain.Models;
using Stock.Infra.Data.Repositories;
using Stock.IntegrationTests.Fixture;
namespace Stock.IntegrationTests.Infra.Repositories;

[Collection("Database Collection")]
public class ProductRepositoryTests
{
    private readonly DatabaseFixture _fixture;
    private readonly ProductRepository _repository;

    private static Product CreateProduct() => new Product(
        name: $"Test Product {Random.Shared.Next(10000, 99999)}",
        description: "This is a test product",
        price: 99.99m,
        stockQuantity: 40
    );

    public ProductRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new ProductRepository(_fixture._context);
    }

    [Fact]
    public async Task AddAsync_WhenValid_ShouldCreateProduct()
    {
        //Arrange
        var product = CreateProduct();

        //Act
        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(product.Id);

        //Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_WhenValid_ShouldUpdateProductStockQuantity()
    {
        //Arrange
        var product = CreateProduct();

        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        //Act
        product.DecreaseStock(20);
        _repository.Update(product);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(product.Id);

        //Assert
        result!.StockQuantity.Should().Be(20);
    }
}