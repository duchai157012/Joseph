namespace BuildingBlocks.Messaging.Events;

public interface PaymentSucceededEvent
{
    Guid OrderId { get; }
    Guid PaymentId { get; }
    DateTime PaidAt { get; }
}
