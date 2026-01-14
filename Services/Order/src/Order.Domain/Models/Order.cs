using BuildingBlocks.SharedKernel.Common;

namespace Order.Domain.Models;

public class Order : EntityBase
{
    public List<OrderItem> Items { get; private set; }
    public Status Status { get; private set; }
    public decimal TotalAmount { get; private set; }

    protected Order() { }

    public Order(List<OrderItem> items)
    {
        Items = items;
        Status = Status.Pending;
    }

    public void SetTotalAmount(decimal amount)
    {
        TotalAmount = amount;
    }

    public void Confirmed()
    {
        Status = Status.Reserved;
    }

    public void Cancelled()
    {
        Status = Status.Cancelled;
    }
}

public enum Status
{
    Pending,
    Reserved,
    Confirmed,
    Completed,
    Cancelled
}
