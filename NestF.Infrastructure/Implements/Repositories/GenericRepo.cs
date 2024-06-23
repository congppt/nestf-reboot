using System.Text.Json;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NestF.Application.DTOs.Generic;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Infrastructure.Constants;
using NestF.Infrastructure.Utils;

namespace NestF.Infrastructure.Implements.Repositories;

public class GenericRepo<T> : IGenericRepo<T> where T : class
{
    public GenericRepo(AppDbContext context, IDistributedCache cache, ITimeService timeService)
    {
        this.context = context;
        this.cache = cache;
        this.timeService = timeService;
    }

    protected readonly AppDbContext context;
    protected readonly IDistributedCache cache;
    protected readonly ITimeService timeService;
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var key = StringUtil.GenerateCacheKey<T>(id);
        var cachedJson = await cache.GetStringAsync(key, ct);
        T? item = null;
        if (string.IsNullOrEmpty(cachedJson))
        {
            item = await context.FindAsync<T>(id);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = timeService.Now.AddMinutes(DefaultConstants.CACHE_MINUTE)
            };
            if (item != null) await cache.SetStringAsync(key, JsonSerializer.Serialize(item), cacheOptions, ct);
            return item;
        }
        item = JsonSerializer.Deserialize<T>(cachedJson);
        if (item != null) context.Attach(item);
        return item;
    }

    public async Task CacheEntityAsync(int id, T entity, CancellationToken ct = default)
    {
        var key = StringUtil.GenerateCacheKey<T>(id);
        var json = JsonSerializer.Serialize(entity);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = timeService.Now.AddMinutes(DefaultConstants.CACHE_MINUTE)
        };
        await cache.SetStringAsync(key, json, cacheOptions, ct);
    }

    public IQueryable<T> GetAll()
    {
        return context.Set<T>().AsNoTrackingWithIdentityResolution();
    }

    // public virtual async Task<Page<TModel>> GetPageAsync<TModel>(int pageIndex, int pageSize) where TModel : class
    // {
    //     var source = _context.Set<T>();
    //     var count = await source.CountAsync();
    //     var items = await _context.Set<T>().Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
    //     return new Page<TModel>
    //     {
    //         TotalCount = count,
    //         Items = items.Adapt<List<TModel>>(),
    //         PageIndex = pageIndex,
    //         PageSize = pageSize
    //     };
    // }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await context.AddAsync(entity, ct);
    }

    public async Task AddAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        await context.AddRangeAsync(entities, ct);
    }

    public void Update(T entity)
    {
        context.Update(entity);
    }

    public void Delete(T entity)
    {
        context.Remove(entity);
    }
}