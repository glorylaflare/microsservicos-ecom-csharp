using BuildingBlocks.Infra.ReadModels;
using FluentAssertions;
using Moq;
using Payment.Application.Interfaces;
using Payment.Application.Queries;
using Payment.Application.Queries.Handlers;
using Payment.Application.Responses;

namespace Payment.UnitTests.Application.Queries;

public class GetByIdPaymentTests
{
	private const int id = 1;
	private readonly GetPaymentByIdQuery _request = new GetPaymentByIdQuery(id);
	private readonly Mock<IPaymentReadService> _mockService = new();
	private readonly PaymentReadModel _paymentReadModel = new PaymentReadModel
	{
		Id = 1,
		OrderId = 100,
		Amount = 190m,
		Status = "Pending",
		CheckoutUrl = "https://checkout/1",
		ExpirationDate = DateTime.UtcNow.AddMinutes(30),
		CreatedAt = DateTime.UtcNow,
		UpdatedAt = null
	};

	[Fact]
	public async Task GetPaymentByIdQuery_WhenPaymentExists_ShouldReturnSuccess()
	{
		//Arrange
		var cancellationToken = It.IsAny<CancellationToken>();
		_mockService
			.Setup(s => s.GetByIdAsync(id))
			.ReturnsAsync(_paymentReadModel);

		var response = new GetPaymentResponse(
			_paymentReadModel.Id,
			_paymentReadModel.Status!,
			_paymentReadModel.CreatedAt
		);

		var handler = new GetPaymentByIdQueryHandler(_mockService.Object);

		//Act
		var result = await handler.Handle(_request, cancellationToken);

		//Assert
		result.IsSuccess.Should().BeTrue();
		result.Value.Should().BeEquivalentTo(response);
	}

	[Fact]
	public async Task GetPaymentByIdQuery_WhenPaymentDoesNotExist_ShouldReturnFailure()
	{
		//Arrange
		var cancellationToken = It.IsAny<CancellationToken>();
		_mockService
			.Setup(s => s.GetByIdAsync(id))
			.ReturnsAsync((PaymentReadModel?)null);

		var handler = new GetPaymentByIdQueryHandler(_mockService.Object);

		//Act
		var result = await handler.Handle(_request, cancellationToken);

		//Assert
		result.IsFailed.Should().BeTrue();
	}
}
