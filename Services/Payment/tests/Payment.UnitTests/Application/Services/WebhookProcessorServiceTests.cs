using FluentAssertions;
using FluentResults;
using MediatR;
using Moq;
using Payment.Application.Commands.ProcessPayment;
using Payment.Application.Services;
using Payment.Domain.Interfaces;
using Payment.Domain.Models;

namespace Payment.UnitTests.Application.Services;

public class WebhookProcessorServiceTests
{
    private readonly Mock<IWebhookRepository> _mockWebhookRepository = new();
    private readonly Mock<IMediator> _mockMediator = new();

    private static WebhookEvent CreateWebhookEvent(long eventId, string paymentId)
    {
        return new WebhookEvent(
            eventId.ToString(),
            new WebhookPayload("payment.updated", "v1", new Data { Id = paymentId }, DateTime.UtcNow, eventId, false, "payment", "1")
        );
    }

    [Fact]
    public async Task ProcessWebhookAsync_WithPendingEvents_ShouldProcessAllAndSave()
    {
        // Arrange
        var webhook1 = CreateWebhookEvent(1, "101");
        var webhook2 = CreateWebhookEvent(2, "202");
        var pendingEvents = new List<WebhookEvent> { webhook1, webhook2 };

        _mockWebhookRepository
            .Setup(r => r.WhereAsync(It.IsAny<Payment.Application.Specifications.GetPendingWebhookEventsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingEvents);

        _mockMediator
            .Setup(m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Unit.Value));

        _mockWebhookRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new WebhookProcessorService(_mockWebhookRepository.Object, _mockMediator.Object);

        // Act
        await service.ProcessWebhookAsync();

        // Assert
        _mockMediator.Verify(m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        webhook1.Status.Should().Be(WebhookStatus.Processed);
        webhook2.Status.Should().Be(WebhookStatus.Processed);
        _mockWebhookRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessWebhookAsync_WhenOneEventFails_ShouldContinueWithOthers()
    {
        // Arrange
        var webhook1 = CreateWebhookEvent(1, "101");
        var webhook2 = CreateWebhookEvent(2, "202");
        var pendingEvents = new List<WebhookEvent> { webhook1, webhook2 };

        _mockWebhookRepository
            .Setup(r => r.WhereAsync(It.IsAny<Payment.Application.Specifications.GetPendingWebhookEventsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingEvents);

        _mockMediator
            .Setup(m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()))
            .Returns<ProcessPaymentCommand, CancellationToken>((command, _) =>
            {
                if (command.WebhookPayload.ExternalId == 1)
                {
                    throw new InvalidOperationException("processing error");
                }

                return Task.FromResult(Result.Ok(Unit.Value));
            });

        _mockWebhookRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new WebhookProcessorService(_mockWebhookRepository.Object, _mockMediator.Object);

        // Act
        await service.ProcessWebhookAsync();

        // Assert
        _mockMediator.Verify(m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        webhook1.Status.Should().Be(WebhookStatus.Pending);
        webhook2.Status.Should().Be(WebhookStatus.Processed);
        _mockWebhookRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}