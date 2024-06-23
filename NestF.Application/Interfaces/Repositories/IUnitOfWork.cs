namespace NestF.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IAccountRepo AccountRepo { get; }
    IProductRepo ProductRepo { get; }
    IOrderRepo OrderRepo { get; }
    ICategoryRepo CategoryRepo { get; }
    IGenericRepo<T> GetRepo<T>() where T : class;
    Task<bool> SaveChangesAsync();
}