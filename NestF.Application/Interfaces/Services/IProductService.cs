using NestF.Application.DTOs.Generic;
using NestF.Application.DTOs.Product;
using NestF.Domain.Entities;

namespace NestF.Application.Interfaces.Services;

public interface IProductService : IGenericService<Product>
{
    Task<Page<ProductBasicInfo>> GetProductPageAsync(int pageIndex, int pageSize);
    Task<string> GetPreSignedUrlAsync(int productId);
    Task<ProductBasicInfo> CreateProductAsync();
    Task<ProductBasicInfo> UpdateProductAsync(int productId, ProductUpdate model);
    Task AddProductImageAsync(int id, string imagePath);
    Task<ProductDetailInfo> GetProductDetailAsync(int id);
}