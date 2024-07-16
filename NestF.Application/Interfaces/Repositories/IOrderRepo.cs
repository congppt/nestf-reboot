using NestF.Domain.Entities;
using NestF.Domain.Enums;

namespace NestF.Application.Interfaces.Repositories;

public interface IOrderRepo : IGenericRepo<Order>
{
    IQueryable<Order> GetOrders(int? accountId, OrderStatus? status);
    Task<Order?> GetCartAsync(int accountId, int productId);
}