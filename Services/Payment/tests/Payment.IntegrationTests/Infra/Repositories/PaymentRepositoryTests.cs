using FluentAssertions;
using Payment.Domain.Models;
using Payment.Infra.Data.Repositories;
using Payment.IntegrationTests.Fixture;

namespace Payment.IntegrationTests.Infra.Repositories;

[Collection("Database Collection")]
public class PaymentRepositoryTests
{
    private readonly DatabaseFixture _fixture;
    private static int GenerateOrderId() => Random.Shared.Next(10000, 99999);
    private static Domain.Models.Payment CreatePayment() => new Domain.Models.Payment(
        orderId: GenerateOrderId(),
        amount: 10.8m,
        checkoutUrl: "https://",
        mercadoPagoPreference: Guid.NewGuid().ToString(),
        expirationDate: DateTime.UtcNow.AddMinutes(30)
    );
    private readonly PaymentRepository _repository;

    public PaymentRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new PaymentRepository(_fixture._context);
    }

    [Fact]
    public async Task AddAsync_WhenValid_ShouldCreatePayment()
    {
        //Arrange
        var payment = CreatePayment();

        //Act
        await _repository.AddAsync(payment);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(payment.OrderId);

        //Assert
        result.Should().NotBeNull();
        result!.OrderId.Should().Be(payment.OrderId);
    }

    [Fact]
    public async Task Update_WhenValid_ShouldUpdatePayment()
    {
        //Arrange
        var payment = CreatePayment();

        await _repository.AddAsync(payment);
        await _repository.SaveChangesAsync();

        //Act
        payment.SetStatus(PaymentStatus.Paid);
        _repository.Update(payment);
        await _repository.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(payment.OrderId);

        //Assert
        result!.Status.Should().Be(PaymentStatus.Paid);
    }
}