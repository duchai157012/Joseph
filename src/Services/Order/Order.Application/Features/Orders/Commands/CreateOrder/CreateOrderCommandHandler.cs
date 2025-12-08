using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Order.Application.Interfaces;
using Order.Domain.Entities;

namespace Order.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderCommandHandler(IOrderDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            TotalPrice = request.TotalPrice,
            CreatedAt = DateTime.UtcNow,
            Status = "Created"
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish Event to RabbitMQ
        await _publishEndpoint.Publish<OrderCreatedEvent>(new
        {
            OrderId = order.Id,
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt
        }, cancellationToken);

        return order.Id;
    }
}
