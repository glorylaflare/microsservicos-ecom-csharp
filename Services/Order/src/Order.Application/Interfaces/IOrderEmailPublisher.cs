namespace Order.Application.Interfaces;

public interface IOrderEmailPublisher
{
    Task PublishPending(Domain.Models.Order order, string checkoutUrl);
    Task PublishCompleted(Domain.Models.Order order);
    Task PublishFailed(Domain.Models.Order order);
}
