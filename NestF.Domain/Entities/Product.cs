using System.ComponentModel.DataAnnotations.Schema;
using NestF.Domain.Enums;

namespace NestF.Domain.Entities;
#pragma warning disable CS8618
public class Product
{
    public int Id { get; set; }
    [Column(TypeName = "citext")]
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Stock { get; set; }
    public double Rating { get; set; }
    public List<string> ImgPaths { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ProductStatus Status { get; set; }
    public HashSet<OrderDetail> OrderDetails { get; set; }
    
}