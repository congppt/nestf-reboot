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
        _context = context;
        _cache = cache;
        _timeService = timeService;
    }

    protected readonly AppDbContext _context;
    protected readonly IDistributedCache _cache;
    protected readonly ITimeService _timeService;
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var key = StringUtil.GenerateCacheKey<T>(id);
        var cachedJson = await _cache.GetStringAsync(key, ct);
        T? item = null;
        if (string.IsNullOrEmpty((cachedJson)))
        {
            item = await _context.FindAsync<T>(id);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = _timeService.Now.AddMinutes(DefaultConstants.CACHE_MINUTE)
            };
            if (item != null) await _cache.SetStringAsync(key, JsonSerializer.Serialize(item), cacheOptions, ct);
            return item;
        }
        item = JsonSerializer.Deserialize<T>(cachedJson);
        if (item != null) _context.Attach(item);
        return item;
    }

    public async Task CacheEntityAsync(int id, T entity, CancellationToken ct = default)
    {
        var key = StringUtil.GenerateCacheKey<T>(id);
        var json = JsonSerializer.Serialize(entity);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = _timeService.Now.AddMinutes(DefaultConstants.CACHE_MINUTE)
        };
        await _cache.SetStringAsync(key, json, cacheOptions, ct);
    }

    public IQueryable<T> GetAll()
    {
        return _context.Set<T>().AsNoTrackingWithIdentityResolution();
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
        await _context.AddAsync(entity, ct);
    }

    public async Task AddAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        await _context.AddRangeAsync(entities, ct);
    }

    public void Update(T entity)
    {
        _context.Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Remove(entity);
    }
}