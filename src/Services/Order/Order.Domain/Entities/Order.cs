using System;

namespace Order.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}
