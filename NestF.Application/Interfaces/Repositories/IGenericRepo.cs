using NestF.Application.DTOs.Generic;

namespace NestF.Application.Interfaces.Repositories;

public interface IGenericRepo<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task CacheEntityAsync(int id, T entity, CancellationToken ct = default);
    IQueryable<T> GetAll();
    // Task<Page<TModel>> GetPageAsync<TModel>(int pageIndex, int pageSize) where TModel : class;
    Task AddAsync(T entity, CancellationToken ct = default);
    Task AddAsync(IEnumerable<T> entities, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
}