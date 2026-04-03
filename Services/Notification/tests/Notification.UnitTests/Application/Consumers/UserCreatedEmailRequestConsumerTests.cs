using BuildingBlocks.Contracts.Datas;
using BuildingBlocks.Contracts.Events;
using MediatR;
using Moq;
using Notification.Application.Commands.UserCreated;
using Notification.Application.Consumers;

namespace Notification.UnitTests.Application.Consumers;

public class UserCreatedEmailRequestConsumerTests
{
    private readonly Mock<IMediator> _mockMediator = new();

    [Fact]
    public async Task HandleAsync_ShouldSendUserCreatedEmailCommand()
    {
        // Arrange
        var evt = new UserCreatedEmailRequestEvent(new UserCreatedEmailRequestData("user@email.com", "junin"));

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UserCreatedEmailCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var consumer = new UserCreatedEmailRequestConsumer(_mockMediator.Object);

        // Act
        await consumer.HandleAsync(evt);

        // Assert
        _mockMediator.Verify(
            m => m.Send(
                It.Is<UserCreatedEmailCommand>(c =>
                    c.Email == "user@email.com" &&
                    c.Username == "junin"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
