using BuildingBlocks.Messaging;
using FluentAssertions;
using Moq;
using Payment.Application.Commands.ProcessPayment;
using Payment.Application.Interfaces;
using Payment.Domain.Interface;
using Payment.Domain.Models;

namespace Payment.UnitTests.Application.Commands;

public class ProcessPaymentTests
{
	private readonly Mock<IPaymentRepository> _mockRepo = new();
	private readonly Mock<IMercadoPagoPaymentService> _mockMercadoPagoService = new();
	private readonly Mock<IEventBus> _mockEventBus = new();

	[Fact]
	public async Task ProcessPayment_WithInvalidPaymentId_ShouldReturnFailureResult()
	{
		//Arrange
		var command = new ProcessPaymentCommand
		{
			Type = "payment",
			Data = new PaymentData { Id = "invalid-id" }
		};
		var cancellationToken = It.IsAny<CancellationToken>();
		var handler = new ProcessPaymentCommandHandler(_mockRepo.Object, _mockMercadoPagoService.Object, _mockEventBus.Object);

		//Act
		var result = await handler.Handle(command, cancellationToken);

		//Assert
		result.IsFailed.Should().BeTrue();
		result.Errors.Should().ContainSingle(e => e.Message == "Invalid payment id");
	}

	[Theory]
	[InlineData("approved", PaymentStatus.Paid)]
	[InlineData("rejected", PaymentStatus.Failed)]
	public void ValidateStatus_WithDifferentStatus_ShouldReturnExpectedValue(string status, PaymentStatus expected)
	{
		//Arrange
		var handler = new ProcessPaymentCommandHandler(_mockRepo.Object, _mockMercadoPagoService.Object, _mockEventBus.Object);

		//Act
		var result = handler.validateStatus(status);

		//Assert
		result.Should().Be(expected);
	}
}
