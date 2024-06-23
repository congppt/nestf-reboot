using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;
using NestF.Domain.Enums;

namespace NestF.Infrastructure.Implements.Repositories;

public class ProductRepo : GenericRepo<Product>, IProductRepo 
{
    public ProductRepo(AppDbContext context, IDistributedCache cache, ITimeService timeService) : base(context, cache,
        timeService)
    {
    }

    public IQueryable<Product> GetProducts(bool includeUnavailable)
    {
        var source = context.Products
            .AsNoTrackingWithIdentityResolution()
            .OrderByDescending(p => p.Id);
        return includeUnavailable ? source : source.Where(p => p.Status == ProductStatus.Available);
    }
    
}