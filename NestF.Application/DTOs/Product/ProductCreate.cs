﻿namespace NestF.Application.DTOs.Product;
#pragma warning disable CS8618
public class ProductCreate
{
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

}