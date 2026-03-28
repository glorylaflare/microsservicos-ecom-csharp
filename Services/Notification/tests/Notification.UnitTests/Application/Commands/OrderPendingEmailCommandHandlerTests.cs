using FluentAssertions;
using MediatR;
using Moq;
using Notification.Application.Commands.OrderPending;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;

namespace Notification.UnitTests.Application.Commands;

public class OrderPendingEmailCommandHandlerTests
{
    private readonly Mock<IEmailSender> _mockEmailSender = new();
    private readonly Mock<ITemplateRenderer> _mockTemplateRenderer = new();

    [Fact]
    public async Task Handle_WithValidRequest_ShouldRenderTemplateAndSendEmail()
    {
        //Arrange
        var command = new OrderPendingEmailCommand(
            "user@email.com",
            123,
            new List<BuildingBlocks.Contracts.OrderItemDto>(),
            "https://checkout.test/order/123"
        );
        var cancellationToken = It.IsAny<CancellationToken>();
        const string expectedHtml = "<h1>Pagamento pendente</h1>";

        _mockTemplateRenderer
            .Setup(t => t.RenderAsync<OrderPendingEmailCommand>(
                It.Is<Dictionary<string, string>>(d =>
                    d["OrderId"] == "123" &&
                    d["CheckoutUrl"] == "https://checkout.test/order/123")))
            .ReturnsAsync(expectedHtml);

        _mockEmailSender
            .Setup(s => s.SendAsync(It.IsAny<Message>()))
            .Returns(Task.CompletedTask);

        var handler = new OrderPendingEmailCommandHandler(_mockEmailSender.Object, _mockTemplateRenderer.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockTemplateRenderer.Verify(
            t => t.RenderAsync<OrderPendingEmailCommand>(It.IsAny<Dictionary<string, string>>()),
            Times.Once);
        _mockEmailSender.Verify(
            s => s.SendAsync(It.Is<Message>(m =>
                m.To == "user@email.com" &&
                m.Subject == "Pagamento pendente" &&
                m.Body == expectedHtml)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailSenderThrows_ShouldPropagateEmailSendException()
    {
        //Arrange
        var command = new OrderPendingEmailCommand(
            "user@email.com",
            321,
            new List<BuildingBlocks.Contracts.OrderItemDto>(),
            "https://checkout.test/order/321"
        );
        var cancellationToken = It.IsAny<CancellationToken>();

        _mockTemplateRenderer
            .Setup(t => t.RenderAsync<OrderPendingEmailCommand>(It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("<html>pending</html>");

        _mockEmailSender
            .Setup(s => s.SendAsync(It.IsAny<Message>()))
            .ThrowsAsync(new EmailSendException("error sending email"));

        var handler = new OrderPendingEmailCommandHandler(_mockEmailSender.Object, _mockTemplateRenderer.Object);

        //Act
        Func<Task> action = async () => await handler.Handle(command, cancellationToken);

        //Assert
        await action.Should().ThrowAsync<EmailSendException>().WithMessage("error sending email");
    }
}
