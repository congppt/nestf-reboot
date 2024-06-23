using NestF.Domain.Entities;

namespace NestF.Application.Interfaces.Repositories;

public interface IProductRepo : IGenericRepo<Product>
{
    IQueryable<Product> GetProducts(bool includeUnavailable);
}