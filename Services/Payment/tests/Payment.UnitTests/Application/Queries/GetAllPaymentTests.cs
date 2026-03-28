using BuildingBlocks.Infra.ReadModels;
using FluentAssertions;
using Moq;
using Payment.Application.Interfaces;
using Payment.Application.Queries.GetAllPayments;
using Payment.Application.Responses;

namespace Payment.UnitTests.Application.Queries;

public class GetAllPaymentTests
{
	private readonly GetAllPaymentsQuery _request = new GetAllPaymentsQuery();
	private readonly Mock<IPaymentReadService> _mockService = new();
	private readonly List<PaymentReadModel> _paymentReadModels = new List<PaymentReadModel>
	{
		new PaymentReadModel
		{
			Id = 1,
			OrderId = 100,
			Amount = 150m,
			Status = "Pending",
			CheckoutUrl = "https://checkout/1",
			ExpirationDate = DateTime.UtcNow.AddMinutes(30),
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = null
		},
		new PaymentReadModel
		{
			Id = 2,
			OrderId = 101,
			Amount = 200m,
			Status = "Paid",
			CheckoutUrl = "https://checkout/2",
			ExpirationDate = DateTime.UtcNow.AddMinutes(30),
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow
		}
	};

	[Fact]
	public async Task GetAllPaymentsQuery_WhenListExists_ShouldReturnSuccess()
	{
		//Arrange
		var cancellationToken = It.IsAny<CancellationToken>();
		_mockService
			.Setup(s => s.GetAllAsync())
			.ReturnsAsync(_paymentReadModels);

		var response = _paymentReadModels.Select(p => new GetPaymentResponse(
			p.Id,
			p.Status!,
			p.CreatedAt
		));

		var handler = new GetAllPaymentsQueryHandler(_mockService.Object);

		//Act
		var result = await handler.Handle(_request, cancellationToken);

		//Assert
		result.IsSuccess.Should().BeTrue();
		result.Value.Should().BeEquivalentTo(response);
	}

	[Fact]
	public async Task GetAllPaymentsQuery_WhenNoPaymentsExist_ShouldReturnFailure()
	{
		//Arrange
		var cancellationToken = It.IsAny<CancellationToken>();
		_mockService
			.Setup(s => s.GetAllAsync())
			.ReturnsAsync(new List<PaymentReadModel>());

		var handler = new GetAllPaymentsQueryHandler(_mockService.Object);

		//Act
		var result = await handler.Handle(_request, cancellationToken);

		//Assert
		result.IsFailed.Should().BeTrue();
	}
}
