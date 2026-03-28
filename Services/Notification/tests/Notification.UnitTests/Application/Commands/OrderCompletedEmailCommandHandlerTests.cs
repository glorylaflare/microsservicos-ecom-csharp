using BuildingBlocks.Contracts;
using FluentAssertions;
using MediatR;
using Moq;
using Notification.Application.Commands.OrderCompleted;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;

namespace Notification.UnitTests.Application.Commands;

public class OrderCompletedEmailCommandHandlerTests
{
    private readonly Mock<IEmailSender> _mockEmailSender = new();
    private readonly Mock<ITemplateRenderer> _mockTemplateRenderer = new();

    [Fact]
    public async Task Handle_WithValidRequest_ShouldRenderTemplateAndSendEmail()
    {
        //Arrange
        var command = new OrderCompletedEmailCommand(
            "user@email.com",
            456,
            new List<OrderItemDto>
            {
                new(1, 2),
                new(2, 1)
            }
        );
        var cancellationToken = It.IsAny<CancellationToken>();
        const string expectedHtml = "<h1>Pedido concluido</h1>";

        _mockTemplateRenderer
            .Setup(t => t.RenderAsync<OrderCompletedEmailCommand>(
                It.Is<Dictionary<string, string>>(d =>
                    d["OrderId"] == "456" &&
                    d["Items"] == "<li>1 - 2</li><li>2 - 1</li>")))
            .ReturnsAsync(expectedHtml);

        _mockEmailSender
            .Setup(s => s.SendAsync(It.IsAny<Message>()))
            .Returns(Task.CompletedTask);

        var handler = new OrderCompletedEmailCommandHandler(_mockEmailSender.Object, _mockTemplateRenderer.Object);

        //Act
        var result = await handler.Handle(command, cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockTemplateRenderer.Verify(
            t => t.RenderAsync<OrderCompletedEmailCommand>(It.IsAny<Dictionary<string, string>>()),
            Times.Once);
        _mockEmailSender.Verify(
            s => s.SendAsync(It.Is<Message>(m =>
                m.To == "user@email.com" &&
                m.Subject == "Pedido concluído" &&
                m.Body == expectedHtml)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailSenderThrows_ShouldPropagateEmailSendException()
    {
        //Arrange
        var command = new OrderCompletedEmailCommand(
            "user@email.com",
            654,
            new List<OrderItemDto>
            {
                new(10, 1)
            }
        );
        var cancellationToken = It.IsAny<CancellationToken>();

        _mockTemplateRenderer
            .Setup(t => t.RenderAsync<OrderCompletedEmailCommand>(It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("<html>completed</html>");

        _mockEmailSender
            .Setup(s => s.SendAsync(It.IsAny<Message>()))
            .ThrowsAsync(new EmailSendException("send failed"));

        var handler = new OrderCompletedEmailCommandHandler(_mockEmailSender.Object, _mockTemplateRenderer.Object);

        //Act
        Func<Task> action = async () => await handler.Handle(command, cancellationToken);

        //Assert
        await action.Should().ThrowAsync<EmailSendException>().WithMessage("send failed");
    }
}
