using NestF.Application.DTOs.Account;
using NestF.Domain.Enums;

namespace NestF.Application.DTOs.Order;
#pragma warning disable CS8618
public class OrderBasicInfo
{
    public int Id { get; set; }
    public AccountMinimumInfo Account { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }
}