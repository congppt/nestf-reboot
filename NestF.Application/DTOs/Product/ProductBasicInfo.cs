namespace NestF.Application.DTOs.Product;
#pragma warning disable CS8618
public class ProductBasicInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public double Rating { get; set; }
    public string ImgPath { get; set; }
    public int CategoryId { get; set; }
}