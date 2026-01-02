using Order.Domain.Enums;
using Order.Domain.ValueObjects;

namespace Order.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Money TotalPrice { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order() { }

    public static Order Create(decimal totalPrice, DateTime createdAt, string currency = "USD")
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            TotalPrice = Money.Create(totalPrice, currency),
            Status = OrderStatus.Created,
            CreatedAt = createdAt
        };

        return order;
    }

    public void MarkAsPaid()
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot mark cancelled order as paid");

        if (Status == OrderStatus.PaymentReceived || Status == OrderStatus.Completed)
            throw new InvalidOperationException("Order has already been paid");

        Status = OrderStatus.PaymentReceived;
    }

    public void Complete()
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot complete cancelled order");

        if (Status != OrderStatus.PaymentReceived)
            throw new InvalidOperationException("Cannot complete order that hasn't been paid");

        Status = OrderStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed order");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Order is already cancelled");

        Status = OrderStatus.Cancelled;
    }
}
