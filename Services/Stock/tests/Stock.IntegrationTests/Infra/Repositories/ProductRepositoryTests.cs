using FluentAssertions;
using Stock.Domain.Models;
using Stock.Infra.Data.Repositories;
using Stock.IntegrationTests.Fixture;
namespace Stock.IntegrationTests.Infra.Repositories;

[Collection("Database Collection")]
public class ProductRepositoryTests
{
    private readonly DatabaseFixture _fixture;
    private Product _product = new Product(
        "Test Product",
        "This is a test product",
        99.99m,
        40
    );
    private readonly ProductRepository _repository;
    public ProductRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new ProductRepository(_fixture._context);
    }
    [Fact]
    public async Task AddAsync_WhenValid_ShouldCreateProduct()
    {
        //Act
        await _repository.AddAsync(_product);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(_product.Id);
        //Assert
        result.Should().NotBeNull();
    }
    [Fact]
    public async Task Update_WhenValid_ShouldUpdateProductStockQuantity()
    {
        //Arrange
        await _repository.AddAsync(_product);
        await _repository.SaveChangesAsync();
        //Act
        _product.DecreaseStock(20);
        _repository.Update(_product);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(_product.Id);
        //Assert
        result!.StockQuantity.Should().Be(20);
    }
}