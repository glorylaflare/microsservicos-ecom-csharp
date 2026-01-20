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
    private readonly OrderRepository _repository;

    public OrderRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new OrderRepository(_fixture._context);
    }

    [Fact]
    public async Task AddAsync_WhenValid_ShouldCreateOrder()
    {
        //Act
        await _repository.AddAsync(_order);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(_order.Id);
        //Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_WhenValid_ShouldReturnStatusPending()
    {
        //Act
        await _repository.AddAsync(_order);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(_order.Id);
        //Assert
        result!.Status.Should().Be(Status.Pending);
    }

    [Fact]
    public async Task Update_WhenValid_ShouldUpdateOrderStatus()
    {
        //Arrange
        await _repository.AddAsync(_order);
        await _repository.SaveChangesAsync();
        //Act
        _order.Confirmed();
        _repository.Update(_order);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(_order.Id);
        //Assert
        result!.Status.Should().Be(Status.Reserved);
    }
}
