using FluentAssertions;
using Order.Domain.Models;
namespace Order.UnitTests.Domain.Models
{
    public class OrderTests
    {
        private readonly List<OrderItem> _list = new List<OrderItem>
        {
            new OrderItem(2, 30),
            new OrderItem(1, 44)
        };

        private const string _USERID = "1";

        [Fact]
        public void CreateOrder_WithValidParameters_ShouldHavePendingStatus()
        {
            // Act
            var order = new Order.Domain.Models.Order(_USERID, _list);
            // Assert
            order.Status.Should().Be(Status.Pending);
        }
        [Fact]
        public void SetTotalAmount_WithValidAmount_ShouldUpdateTotalAmount()
        {
            //Arrange
            var order = new Order.Domain.Models.Order(_USERID, _list);
            var expectedAmount = 150.75m;
            // Act
            order.SetTotalAmount(expectedAmount);
            // Assert
            order.TotalAmount.Should().Be(expectedAmount);
        }
        [Fact]
        public void Confirmed_WhenCalled_ShouldUpdateStatusToConfirmed()
        {
            //Arrange
            var order = new Order.Domain.Models.Order(_USERID, _list);
            // Act
            order.Confirmed();
            // Assert
            order.Status.Should().Be(Status.Reserved);
        }
        [Fact]
        public void Cancelled_WhenCalled_ShouldUpdateStatusToCancelled()
        {
            //Arrange
            var order = new Order.Domain.Models.Order(_USERID, _list);
            // Act
            order.Cancelled();
            // Assert
            order.Status.Should().Be(Status.Cancelled);
        }
    }
}