using NestF.Domain.Enums;

namespace NestF.Domain.Entities;
#pragma warning disable CS8618
public class Transaction
{
    public int Id { get; set; }
    public string BankTransCode { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Gateway Gateway { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public int AccountId { get; set; }
    public Account Account { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
}