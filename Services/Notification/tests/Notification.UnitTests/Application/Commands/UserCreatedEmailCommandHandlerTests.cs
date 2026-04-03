using FluentAssertions;
using MediatR;
using Moq;
using Notification.Application.Commands.UserCreated;
using Notification.Application.Interfaces;
using Notification.Domain.Exceptions;
using Notification.Domain.Models;

namespace Notification.UnitTests.Application.Commands;

public class UserCreatedEmailCommandHandlerTests
{
    private readonly Mock<IEmailSender> _mockEmailSender = new();
    private readonly Mock<ITemplateRenderer> _mockTemplateRenderer = new();

    [Fact]
    public async Task Handle_WithValidRequest_ShouldRenderTemplateAndSendEmail()
    {
        // Arrange
        var command = new UserCreatedEmailCommand("user@email.com", "junin");
        var cancellationToken = It.IsAny<CancellationToken>();
        const string expectedHtml = "<h1>Bem-vindo</h1>";

        _mockTemplateRenderer
            .Setup(t => t.RenderAsync<UserCreatedEmailCommand>(
                It.Is<Dictionary<string, string>>(d =>
                    d["Username"] == "junin" &&
                    d["Email"] == "user@email.com")))
            .ReturnsAsync(expectedHtml);

        _mockEmailSender
            .Setup(s => s.SendAsync(It.IsAny<Message>()))
            .Returns(Task.CompletedTask);

        var handler = new UserCreatedEmailCommandHandler(_mockEmailSender.Object, _mockTemplateRenderer.Object);

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _mockTemplateRenderer.Verify(
            t => t.RenderAsync<UserCreatedEmailCommand>(It.IsAny<Dictionary<string, string>>()),
            Times.Once);
        _mockEmailSender.Verify(
            s => s.SendAsync(It.Is<Message>(m =>
                m.To == "user@email.com" &&
                m.Subject == "Bem-vindo(a) ao nosso serviço de E-Commerce" &&
                m.Body == expectedHtml)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailSenderThrows_ShouldPropagateEmailSendException()
    {
        // Arrange
        var command = new UserCreatedEmailCommand("user@email.com", "junin");
        var cancellationToken = It.IsAny<CancellationToken>();

        _mockTemplateRenderer
            .Setup(t => t.RenderAsync<UserCreatedEmailCommand>(It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("<html>user created</html>");

        _mockEmailSender
            .Setup(s => s.SendAsync(It.IsAny<Message>()))
            .ThrowsAsync(new EmailSendException("send failed"));

        var handler = new UserCreatedEmailCommandHandler(_mockEmailSender.Object, _mockTemplateRenderer.Object);

        // Act
        Func<Task> action = async () => await handler.Handle(command, cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EmailSendException>().WithMessage("send failed");
    }
}
