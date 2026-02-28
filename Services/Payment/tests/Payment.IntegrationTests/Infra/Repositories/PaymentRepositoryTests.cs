using FluentAssertions;
using Payment.Domain.Models;
using Payment.Infra.Data.Repositories;
using Payment.IntegrationTests.Fixture;

namespace Payment.IntegrationTests.Infra.Repositories;

[Collection("Database Collection")]
public class PaymentRepositoryTests
{
    private readonly DatabaseFixture _fixture;

    private Domain.Models.Payment _payment = new Domain.Models.Payment(1, 10.8m, "https://", DateTime.UtcNow.AddMinutes(30));

    private readonly PaymentRepository _repository;

    public PaymentRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new PaymentRepository(_fixture._context);
    }

    [Fact]
    public async Task AddAsync_WhenValid_ShouldCreatePayment()
    {
        //Act
        await _repository.AddAsync(_payment);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(_payment.Id);

        //Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_WhenValid_ShouldUpdatePayment() 
    {
        //Arrange
        await _repository.AddAsync(_payment);
        await _repository.SaveChangesAsync();

        //Act
        _payment.SetStatus(PaymentStatus.Paid);
        _repository.Update(_payment);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(_payment.OrderId);

        //Assert
        result!.Status.Should().Be(PaymentStatus.Paid);
    }
}