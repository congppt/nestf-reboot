namespace NestF.Domain.Entities;
#pragma warning disable CS8618
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public HashSet<Product> Products { get; set; }
}