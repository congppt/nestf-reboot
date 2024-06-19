using NestF.Application.DTOs.Generic;

namespace NestF.Application.Interfaces.Services;

public interface IGenericService<T> where T : class
{
    // Task<T?> GetByIdAsync(int id);
    // Task<Page<T>> GetPageAsync(int pageIndex, int pageSize);
    Task<TModel?> GetByIdAsync<TModel>(int id) where TModel : class;
}