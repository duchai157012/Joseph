using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Interfaces;

namespace Order.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IDateTimeProvider _dateTime;

    public CreateOrderCommandHandler(
        IOrderDbContext context,
        IPublishEndpoint publishEndpoint,
        IDateTimeProvider dateTime)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _dateTime = dateTime;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Domain.Entities.Order.Create(
            totalPrice: request.TotalPrice,
            createdAt: _dateTime.UtcNow);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish<OrderCreatedEvent>(new
        {
            OrderId = order.Id,
            TotalPrice = order.TotalPrice.Amount,
            CreatedAt = order.CreatedAt
        }, cancellationToken);

        return order.Id;
    }
}
