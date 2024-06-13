using NestF.Domain.Enums;

namespace NestF.Domain.Entities;
#pragma warning disable CS8618
public class Order
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public Account Account { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderTrace> Traces { get; set; } 
    public decimal Total { get; set; }
    public HashSet<OrderDetail> Details { get; set; }
    public HashSet<Transaction> Transactions { get; set; }
}

public class OrderTrace
{
    public string Description { get; set; }
    public DateTime ModifiedAt { get; set; }
    public OrderStatus Status { get; set; }
}