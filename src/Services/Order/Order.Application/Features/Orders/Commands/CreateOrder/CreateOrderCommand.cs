using MediatR;

namespace Order.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Guid>
{
    public decimal TotalPrice { get; set; }
}
