namespace NestF.Application.DTOs.Product;

public class ProductUpdate : ProductCreate
{
    public bool IsAvailable { get; set; }
}