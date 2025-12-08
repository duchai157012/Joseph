using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Notification.Application.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        _logger.LogInformation("Sending Order Confirmation Email for Order {OrderId}. Total: {TotalPrice}", 
            context.Message.OrderId, context.Message.TotalPrice);
            
        return Task.CompletedTask;
    }
}
