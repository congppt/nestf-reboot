using Amazon.S3;
using Amazon.S3.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NestF.Application.DTOs.Account;
using NestF.Application.DTOs.Generic;
using NestF.Application.DTOs.Product;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;
using NestF.Infrastructure.Constants;

namespace NestF.Infrastructure.Implements.Services;

public class ProductService : GenericService<Product>, IProductService
{
    private readonly IAmazonS3 _s3;
    private readonly IConfiguration _config;
    public ProductService(IUnitOfWork uow,
        IClaimService claimService,
        ITimeService timeService, IAmazonS3 s3, IConfiguration config) : base(uow,
        claimService,
        timeService)
    {
        _s3 = s3;
        _config = config;
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

    public async Task<ProductBasicInfo> CreateProductAsync(ProductCreate model)
    {
        var product = model.Adapt<Product>();
        await _uow.GetRepo<Product>().AddAsync(product);
        if (!await _uow.SaveChangesAsync()) throw new DbUpdateException();
        return product.Adapt<ProductBasicInfo>();
    }

    public async Task<string> GetPreSignedUrlAsync()
    {
        var objectKey = Guid.NewGuid() + ".jpg";
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _config["AWS:S3:BucketName"],
            Key = objectKey,
            Expires = _timeService.Now.AddMinutes(DefaultConstants.CACHE_MINUTE),
            Verb = HttpVerb.PUT,
            ContentType = "image/jpeg",
            
        };
        var url = await _s3.GetPreSignedURLAsync(request);
        
        return url;
    }
}