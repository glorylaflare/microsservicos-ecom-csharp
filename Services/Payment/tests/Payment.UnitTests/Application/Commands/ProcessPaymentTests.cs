using BuildingBlocks.Messaging;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Payment.Application.Commands;
using Payment.Application.Commands.Handlers;
using Payment.Domain.Interface;
using Payment.Domain.Models;

namespace Payment.UnitTests.Application.Commands;

public class ProcessPaymentTests
{
	private readonly Mock<IPaymentRepository> _mockRepo = new();
	private readonly Mock<IConfiguration> _mockConfig = new();
	private readonly Mock<IEventBus> _mockEventBus = new();

	[Fact]
	public async Task ProcessPayment_WithInvalidPaymentId_ShouldThrowFormatException()
	{
		//Arrange
		var command = new ProcessPaymentCommand
		{
			Type = "payment",
			Data = new PaymentData { Id = "invalid-id" }
		};
		var cancellationToken = It.IsAny<CancellationToken>();
		var handler = new ProcessPaymentCommandHandler(_mockRepo.Object, _mockConfig.Object, _mockEventBus.Object);

		//Act
		Func<Task> action = async () => await handler.Handle(command, cancellationToken);

		//Assert
		await action.Should().ThrowAsync<FormatException>();
	}

	[Theory]
	[InlineData("approved", PaymentStatus.Paid)]
	[InlineData("rejected", PaymentStatus.Failed)]
	public void ValidateStatus_WithDifferentStatus_ShouldReturnExpectedValue(string status, PaymentStatus expected)
	{
		//Arrange
		var handler = new ProcessPaymentCommandHandler(_mockRepo.Object, _mockConfig.Object, _mockEventBus.Object);

		//Act
		var result = handler.validateStatus(status);

		//Assert
		result.Should().Be(expected);
	}
}
