using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NestF.Application.Interfaces.Repositories;
using NestF.Infrastructure.Utils;

namespace NestF.Infrastructure.Implements.Repositories;

public class GenericRepo<T> : IGenericRepo<T> where T : class
{
    public GenericRepo(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var key = StringUtil.GenerateCacheKey<T>(id);
        var cachedJson = await _cache.GetStringAsync(key, ct);
        T? item = null;
        if (string.IsNullOrEmpty((cachedJson)))
        {
            item = await _context.FindAsync<T>(id);
            if (item != null) await _cache.SetStringAsync(key, JsonSerializer.Serialize(item), ct);
            return item;
        }
        item = JsonSerializer.Deserialize<T>(cachedJson);
        if (item != null) _context.Attach(item);
        return item;
    }

    public IQueryable<T> GetAll()
    {
        return _context.Set<T>().AsNoTrackingWithIdentityResolution();
    }

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