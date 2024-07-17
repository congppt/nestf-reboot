using NestF.Application.DTOs.Generic;
using NestF.Application.DTOs.Order;
using NestF.Domain.Entities;
using NestF.Domain.Enums;

namespace NestF.Application.Interfaces.Services;

public interface IOrderService : IGenericService<Order>
{
    Task<Page<OrderBasicInfo>> GetOrderPageAsync(int pageIndex, int pageSize, OrderStatus? status);
    Task<Page<OrderDetailBasicInfo>> GetCartPageAsync(int pageIndex, int pageSize);
    Task AddToCartAsync(CartAdd model);
}