using BuildingBlocks.Infra.Specifications;
using FluentAssertions;
using Order.Domain.Models;
using Order.Infra.Data.Repositories;
using Order.IntegrationTests.Fixture;
namespace Order.IntegrationTests.Infra.Repositories;

[Collection("Database Collection")]
public class OrderRepositoryTests
{
    private readonly DatabaseFixture _fixture;
    private readonly OrderRepository _repository;

    private static Domain.Models.Order CreateOrder() => new Domain.Models.Order(
        userId: GenerateUserId(),
        items: new List<OrderItem>
        {
            new OrderItem(productId: 1, quantity: 20)
        }
    );

    private static string GenerateUserId() => Random.Shared.Next(10000, 99999).ToString();
    
    public OrderRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new OrderRepository(_fixture._context);
    }
    
    [Fact]
    public async Task AddAsync_WhenValid_ShouldCreateOrder()
    {
        //Arrange
        var order = CreateOrder();
        var cancellationToken = CancellationToken.None;

        //Act
        await _repository.AddAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        var result = await _repository.FindOneAsync(new OrderByIdSpec(order.Id), cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(Status.Pending);
    }
    
    [Fact]
    public async Task Update_WhenValid_ShouldUpdateOrderStatus()
    {
        //Arrange
        var order = CreateOrder();
        var cancellationToken = CancellationToken.None;

        await _repository.AddAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        //Act
        order.Confirmed();
        _repository.Update(order);
        await _repository.SaveChangesAsync(cancellationToken);
        var result = await _repository.FindOneAsync(new OrderByIdSpec(order.Id), cancellationToken);

        //Assert
        result!.Status.Should().Be(Status.Reserved);
    }

    private sealed class OrderByIdSpec : Specification<Domain.Models.Order>
    {
        public OrderByIdSpec(int id)
        {
            AddCriteria(o => o.Id == id);
            EnableTracking();
        }
    }
}