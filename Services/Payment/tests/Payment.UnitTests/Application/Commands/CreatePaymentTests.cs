using BuildingBlocks.Contracts;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Moq;
using Payment.Application.Commands.CreatePayment;
using Payment.Application.Commands.Handlers;
using Payment.Domain.Interface;

namespace Payment.UnitTests.Application.Commands;

public class CreatePaymentTests
{
    private readonly CreatePaymentCommand _request = new CreatePaymentCommand(Guid.NewGuid(), 1, 190m, new List<ProductItemDto> { 
        new ProductItemDto(1, "Produto", "Descrição", 2, 10) 
    });

    private readonly Mock<IConfiguration> _mockConfig = new();
    private readonly Mock<FluentValidation.IValidator<CreatePaymentCommand>> _mockValidator = new();
    private readonly Mock<BuildingBlocks.Messaging.IEventBus> _mockEventBus = new();
    private readonly Mock<IPaymentRepository> _mockRepo = new();

    [Fact]
    public async Task CreatePayment_WithInvalidItems_ShouldReturnUnitValue()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Items", "At least one item is required.")
        };

        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationErrors));

        var handler = new CreatePaymentCommandHandler(_mockConfig.Object, _mockRepo.Object, _mockEventBus.Object, _mockValidator.Object);

        //Act
        var result = await handler.Handle(_request, _cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Payment.Domain.Models.Payment>()), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<BuildingBlocks.Messaging.IntegrationEventBase>()), Times.Never);
    }

    [Fact]
    public async Task CreatePayment_WithoutAccessToken_ShouldNotPersistPayment()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();

        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockConfig
            .Setup(c => c["MercadoPago:AccessToken"])
            .Returns((string?)null);

        var handler = new CreatePaymentCommandHandler(_mockConfig.Object, _mockRepo.Object, _mockEventBus.Object, _mockValidator.Object);

        //Act
        var result = await handler.Handle(_request, _cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Payment.Domain.Models.Payment>()), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<BuildingBlocks.Messaging.IntegrationEventBase>()), Times.Never);
    }
}
