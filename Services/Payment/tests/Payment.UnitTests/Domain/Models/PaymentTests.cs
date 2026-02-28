using FluentAssertions;
using Payment.Domain.Models;

namespace Payment.UnitTests.Domain.Models;

public class PaymentTests
{
    #region Constants
    private const int _ORDERID = 1;
    private const decimal _AMOUNT = 10m;
    private const string _CHECKOUTURL = "https://url.com";
    private DateTime _expirationDate = DateTime.UtcNow.AddMinutes(30);
    #endregion

    [Fact]
    public void CreatePayment_WithValidParameters_ShouldHavePendingStatus()
    {
        // Act
        var payment = new Payment.Domain.Models.Payment(_ORDERID, _AMOUNT, _CHECKOUTURL, _expirationDate);
        // Assert
        payment.Status.Should().Be(PaymentStatus.Pending);
    }

    [Theory]
    [InlineData(PaymentStatus.Paid)]
    [InlineData(PaymentStatus.Failed)]
    public void SetStatus_WithValidStatus_ShouldUpdatePaymentStatus(PaymentStatus status)
    {
        //Arrange
        var payment = new Payment.Domain.Models.Payment(_ORDERID, _AMOUNT, _CHECKOUTURL, _expirationDate);
        var expectedStatus = status;
        // Act
        payment.SetStatus(status);
        // Assert
        payment.Status.Should().Be(status);
    }
}