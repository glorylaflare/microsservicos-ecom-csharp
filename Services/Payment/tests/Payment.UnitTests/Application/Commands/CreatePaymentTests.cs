using BuildingBlocks.Contracts;
using FluentResults;
using FluentAssertions;
using MediatR;
using MercadoPago.Resource.Preference;
using Moq;
using Payment.Application.Commands.CreatePayment;
using Payment.Application.Interfaces;
using Payment.Application.Requests;
using Payment.Domain.Interface;

namespace Payment.UnitTests.Application.Commands;

public class CreatePaymentTests
{
    private readonly CreatePaymentCommand _request = new CreatePaymentCommand(Guid.NewGuid(), 1, 190m, new List<ProductItemDto> { 
        new ProductItemDto(1, "Produto", "Descrição", 2, 10) 
    });

    private readonly Mock<FluentValidation.IValidator<CreatePaymentCommand>> _mockValidator = new();
    private readonly Mock<BuildingBlocks.Messaging.IEventBus> _mockEventBus = new();
    private readonly Mock<IPaymentRepository> _mockRepo = new();
    private readonly Mock<IMercadoPagoPaymentService> _mockMercadoPagoService = new();

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

        var handler = new CreatePaymentCommandHandler(_mockRepo.Object, _mockMercadoPagoService.Object, _mockEventBus.Object, _mockValidator.Object);

        //Act
        var result = await handler.Handle(_request, _cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Payment.Domain.Models.Payment>()), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<BuildingBlocks.Messaging.IntegrationEventBase>()), Times.Never);
        _mockMercadoPagoService.Verify(m => m.CreateMercadoPagoPayment(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreatePayment_WhenMercadoPagoPreferenceFails_ShouldNotPersistPayment()
    {
        //Arrange
        var _cancellationToken = It.IsAny<CancellationToken>();

        _mockValidator
            .Setup(v => v.ValidateAsync(_request, _cancellationToken))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockMercadoPagoService
            .Setup(m => m.CreateMercadoPagoPayment(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<Preference>("Failed to create payment preference"));

        var handler = new CreatePaymentCommandHandler(_mockRepo.Object, _mockMercadoPagoService.Object, _mockEventBus.Object, _mockValidator.Object);

        //Act
        var result = await handler.Handle(_request, _cancellationToken);

        //Assert
        result.Should().Be(Unit.Value);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Payment.Domain.Models.Payment>()), Times.Never);
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<BuildingBlocks.Messaging.IntegrationEventBase>()), Times.Never);
    }
}
