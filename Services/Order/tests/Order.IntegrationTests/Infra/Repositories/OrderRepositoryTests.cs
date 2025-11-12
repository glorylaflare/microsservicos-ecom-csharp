using FluentAssertions;
using Order.Domain.Models;
using Order.Infra.Data.Repositories;
using Order.IntegrationTests.Fixture;

namespace Order.IntegrationTests.Infra.Repositories;

[Collection("Database Collection")]
public class OrderRepositoryTests
{
    private readonly DatabaseFixture _fixture;
    private Domain.Models.Order _order = new Domain.Models.Order(
    new List<OrderItem>
    {
        new OrderItem(productId: 1, quantity: 20)
    });

    public OrderRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_WhenValid_ShouldCreateOrder()
    {
        //Arrange
        var repository = new OrderRepository(_fixture._context);
        //Act
        await repository.AddAsync(_order);
        await repository.SaveChangesAsync();
        var result = await repository.GetByIdAsync(_order.Id);
        //Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_WhenValid_ShouldReturnStatusPending()
    { 
        //Arrange
        var repository = new OrderRepository(_fixture._context);
        //Act
        await repository.AddAsync(_order);
        await repository.SaveChangesAsync();
        var result = await repository.GetByIdAsync(_order.Id);
        //Assert
        result!.Status.Should().Be(Status.Pending);
    }

    [Fact]
    public async Task Update_WhenValid_ShouldUpdateOrderStatus()
    {
        //Arrange
        var repository = new OrderRepository(_fixture._context);
        await repository.AddAsync(_order);
        await repository.SaveChangesAsync();
        //Act
        _order.Confirmed();
        repository.Update(_order);
        await repository.SaveChangesAsync();
        var result = await repository.GetByIdAsync(_order.Id);
        //Assert
        result!.Status.Should().Be(Status.Confirmed);
    }
}
