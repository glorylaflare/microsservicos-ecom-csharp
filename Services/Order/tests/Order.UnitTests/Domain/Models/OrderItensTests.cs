using FluentAssertions;
using Order.Domain.Models;

namespace Order.UnitTests.Domain.Models;

public class OrderItensTests
{
    [Fact]
    public void CreateOrderItem_WithValidParameters_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        const int productId = 1;
        const int quantity = 3;
        // Act
        var orderItem = new OrderItem(productId, quantity);
        // Assert
        orderItem.Quantity.Should().Be(quantity);
    }
}
