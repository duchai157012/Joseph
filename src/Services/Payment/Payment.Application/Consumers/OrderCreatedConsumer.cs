using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Payment.Application.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        _logger.LogInformation("Processing Payment for Order {OrderId}...", context.Message.OrderId);

        await Task.Delay(1000);

        _logger.LogInformation("Payment Successful for Order {OrderId}", context.Message.OrderId);

        await _publishEndpoint.Publish<PaymentSucceededEvent>(new
        {
            OrderId = context.Message.OrderId,
            PaymentId = Guid.NewGuid(),
            PaidAt = DateTime.UtcNow
        });
    }
}
