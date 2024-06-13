namespace NestF.Domain.Entities;
#pragma warning disable CS8618
public class OrderDetail
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public List<string>? ImgPaths { get; set; }
}