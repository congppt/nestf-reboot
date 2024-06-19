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
        var source = _uow.GetRepo<Product>().GetAll();
        var count = await source.CountAsync();
        var items = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
        return new Page<ProductBasicInfo>()
        {
            Items = items.Adapt<List<ProductBasicInfo>>(),
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = count
        };
    }

    public async Task<string> GetPreSignedUrlAsync(int id)
    {
        var objectKey = $"{id}_"+ Guid.NewGuid() + ".jpg";
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _config["AWS:S3:BucketName"],
            Key = objectKey,
            Expires = _timeService.Now.AddMinutes(DefaultConstants.PRESIGNED_MINUTE),
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
        await _uow.GetRepo<Product>().AddAsync(product);
        if (!await _uow.SaveChangesAsync()) throw new DbUpdateException();
        await _bgService.ScheduleDeleteTempProductJobAsync(product.Id,
            _timeService.Now.AddMinutes(DefaultConstants.TOKEN_MINUTE));
        return product.Adapt<ProductBasicInfo>();
    }

    public async Task<ProductBasicInfo> UpdateProductAsync(int productId, ProductUpdate model)
    {
        var product = await _uow.GetRepo<Product>().GetByIdAsync(productId) ?? throw new KeyNotFoundException();
        model.Adapt(product);
        product.Status = model.IsAvailable ? ProductStatus.Available : ProductStatus.Hidden;
        if (!await _uow.SaveChangesAsync()) throw new DbUpdateException();
        await _uow.GetRepo<Product>().CacheEntityAsync(product.Id, product);
        return product.Adapt<ProductBasicInfo>();
    }

    public async Task AddProductImageAsync(string imageName)
    {
        var id = int.Parse(imageName.Split('_')[0]);
        var product = await _uow.GetRepo<Product>().GetByIdAsync(id) ?? throw new KeyNotFoundException();
        product.ImgPaths.Add(imageName);
        if (!await _uow.SaveChangesAsync()) return;
        await _uow.GetRepo<Product>().CacheEntityAsync(id, product);
    }
}