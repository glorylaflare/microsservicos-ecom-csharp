using FluentAssertions;
using Moq;
using Payment.Application.Commands.ReceiveMercadoPagoWebhook;
using Payment.Application.Commands.Webhooks;
using Payment.Domain.Interfaces;
using Payment.Domain.Models;

namespace Payment.UnitTests.Application.Commands;

public class ReceiveMercadoPagoWebhookCommandHandlerTests
{
    private readonly Mock<IWebhookRepository> _mockWebhookRepository = new();

    [Fact]
    public async Task Handle_WhenPayloadIsValid_ShouldPersistWebhookAndReturnSuccess()
    {
        // Arrange
        var payload = """
        {
          "id": 123,
          "type": "payment",
          "data": {
            "id": "999"
          }
        }
        """;
        var command = new ReceiveMercadoPagoWebhookCommand(payload);
        var handler = new ReceiveMercadoPagoWebhookCommandHandler(_mockWebhookRepository.Object);
        var cancellationToken = CancellationToken.None;
        WebhookEvent? capturedWebhook = null;

        _mockWebhookRepository
            .Setup(r => r.AddAsync(It.IsAny<WebhookEvent>(), It.IsAny<CancellationToken>()))
            .Callback<WebhookEvent, CancellationToken>((webhook, _) => capturedWebhook = webhook)
            .Returns(Task.CompletedTask);
        _mockWebhookRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedWebhook.Should().NotBeNull();
        capturedWebhook!.EventId.Should().Be("123");
        capturedWebhook.Payload.Data.Id.Should().Be("999");
        capturedWebhook.Status.Should().Be(WebhookStatus.Pending);

        _mockWebhookRepository.Verify(r => r.AddAsync(It.IsAny<WebhookEvent>(), cancellationToken), Times.Once);
        _mockWebhookRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldReturnFailure()
    {
        // Arrange
        var payload = """
        {
          "id": 123,
          "type": "payment",
          "data": {
            "id": "999"
          }
        }
        """;
        var command = new ReceiveMercadoPagoWebhookCommand(payload);
        var handler = new ReceiveMercadoPagoWebhookCommandHandler(_mockWebhookRepository.Object);

        _mockWebhookRepository
            .Setup(r => r.AddAsync(It.IsAny<WebhookEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(x => x.Message == "An error occurred while storing webhook");
        _mockWebhookRepository.Verify(r => r.AddAsync(It.IsAny<WebhookEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockWebhookRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPayloadIsInvalidJson_ShouldReturnSuccessAndNotPersist()
    {
        // Arrange
        var command = new ReceiveMercadoPagoWebhookCommand("{invalid-json}");
        var handler = new ReceiveMercadoPagoWebhookCommandHandler(_mockWebhookRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockWebhookRepository.Verify(r => r.AddAsync(It.IsAny<WebhookEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockWebhookRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}