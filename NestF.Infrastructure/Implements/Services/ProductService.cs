using Amazon.S3;
using Amazon.S3.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NestF.Application.DTOs.Generic;
using NestF.Application.DTOs.Product;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;
using NestF.Domain.Enums;
using NestF.Infrastructure.Constants;

namespace NestF.Infrastructure.Implements.Services;

public class ProductService : GenericService<Product>, IProductService
{
    private readonly IAmazonS3 _s3;
    private readonly IConfiguration _config;
    private readonly IBackgroundService _bgService;
    public ProductService(IUnitOfWork uow,
        IClaimService claimService,
        ITimeService timeService, IAmazonS3 s3, IConfiguration config, IBackgroundService bgService) : base(uow,
        claimService,
        timeService)
    {
        _s3 = s3;
        _config = config;
        _bgService = bgService;
    }

    public async Task<Page<ProductBasicInfo>> GetProductPageAsync(int pageIndex, int pageSize)
    {
        var role = claimService.GetClaim(ClaimConstants.ROLE, Role.Customer);
        var source = uow.ProductRepo.GetProducts(role != Role.Customer);
        var count = await source.CountAsync();
        var items = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
        return new ()
        {
            Items = items.Adapt<List<ProductBasicInfo>>(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = count
        };
    }

    public async Task<string> GetPreSignedUrlAsync(int id)
    {
        var objectKey = DefaultConstants.PRODUCT_IMG_FOLDER + $"/{id}_"+ Guid.NewGuid() + ".jpg";
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _config["AWS:S3:BucketName"],
            Key = objectKey,
            Expires = timeService.Now.AddMinutes(DefaultConstants.PRESIGNED_MINUTE),
            Verb = HttpVerb.PUT,
            ContentType = "image/jpeg",
            
        };
        var url = await _s3.GetPreSignedURLAsync(request);
        return url;
    }

    public async Task<ProductBasicInfo> CreateProductAsync()
    {
        var product = new Product
        {
            Name = DefaultConstants.PRODUCT_NAME,
            Description = string.Empty,
            Price = 0,
            Status = ProductStatus.Temp,
            ImgPaths = [],
            CategoryId = 1
        };
        await uow.ProductRepo.AddAsync(product);
        if (!await uow.SaveChangesAsync()) throw new DbUpdateException();
        await _bgService.ScheduleDeleteTempProductJobAsync(product.Id,
            timeService.Now.AddMinutes(DefaultConstants.ACCESS_TOKEN_MINUTE));
        return product.Adapt<ProductBasicInfo>();
    }

    public async Task<ProductBasicInfo> UpdateProductAsync(int productId, ProductUpdate model)
    {
        var product = await uow.ProductRepo.GetByIdAsync(productId) ?? throw new KeyNotFoundException();
        model.Adapt(product);
        product.Status = model.IsAvailable ? ProductStatus.Available : ProductStatus.Hidden;
        if (!await uow.SaveChangesAsync()) throw new DbUpdateException();
        await uow.ProductRepo.CacheEntityAsync(product.Id, product);
        return product.Adapt<ProductBasicInfo>();
    }

    public async Task AddProductImageAsync(int id, string imagePath)
    {
        var parameters = imagePath.Split(['/', '_']);
        if (parameters[0] != DefaultConstants.PRODUCT_IMG_FOLDER) throw new ArgumentException();
        if (parameters[1] != id.ToString()) throw new ArgumentException();
        var product = await uow.ProductRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        product.ImgPaths.Add(imagePath);
        if (!await uow.SaveChangesAsync()) return;
        await uow.ProductRepo.CacheEntityAsync(id, product);
    }

    public async Task<ProductDetailInfo> GetProductDetailAsync(int id)
    {
        var product = await uow.ProductRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        var role = claimService.GetClaim(ClaimConstants.ROLE, Role.Customer);
        if (product.Status != ProductStatus.Available && role == Role.Customer) throw new KeyNotFoundException();
        return product.Adapt<ProductDetailInfo>();
    }
}