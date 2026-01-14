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
        Status = Status.Created;
    }

    public void SetTotalAmount(decimal amount)
    {
        TotalAmount = amount;
    }

    public void Confirmed()
    {
        Status = Status.StockConfirmed;
    }

    public void Cancelled()
    {
        Status = Status.Cancelled;
    }
}

public enum Status
{
    Created,            
    StockConfirmed,     
    AwaitingPayment,    
    PaymentConfirmed,   
    Completed,          
    Cancelled
}
