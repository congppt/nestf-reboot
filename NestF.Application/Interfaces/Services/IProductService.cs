using NestF.Application.DTOs.Generic;
using NestF.Application.DTOs.Product;
using NestF.Domain.Entities;

namespace NestF.Application.Interfaces.Services;

public interface IProductService : IGenericService<Product>
{
    Task<Page<ProductBasicInfo>> GetProductPageAsync(int pageIndex, int pageSize);
    Task<ProductBasicInfo> CreateProductAsync(ProductCreate model);
    Task<string> GetPreSignedUrlAsync();
}