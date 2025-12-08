namespace BuildingBlocks.Messaging.Events;

public interface OrderCreatedEvent
{
    Guid OrderId { get; }
    decimal TotalPrice { get; }
    DateTime CreatedAt { get; }
}
