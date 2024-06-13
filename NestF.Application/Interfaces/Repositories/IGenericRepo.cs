namespace NestF.Application.Interfaces.Repositories;

public interface IGenericRepo<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    IQueryable<T> GetAll();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}