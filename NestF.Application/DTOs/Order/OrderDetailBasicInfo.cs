using NestF.Application.DTOs.Product;

namespace NestF.Application.DTOs.Order;

public class OrderDetailBasicInfo
{
    public ProductBasicInfo Product { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
}