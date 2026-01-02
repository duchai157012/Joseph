namespace Order.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Created = 1,
    PaymentReceived = 2,
    Completed = 3,
    Cancelled = 4
}
