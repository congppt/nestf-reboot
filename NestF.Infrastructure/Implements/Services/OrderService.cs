﻿using Mapster;
using Microsoft.EntityFrameworkCore;
using NestF.Application.DTOs.Generic;
using NestF.Application.DTOs.Order;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;
using NestF.Domain.Enums;
using NestF.Infrastructure.Constants;

namespace NestF.Infrastructure.Implements.Services;

public class OrderService : GenericService<Order>, IOrderService
{
    public OrderService(IUnitOfWork uow, IClaimService claimService, ITimeService timeService) : base(uow, claimService,
        timeService)
    {
    }

    public async Task<Page<OrderBasicInfo>> GetOrderPageAsync(int pageIndex, int pageSize, OrderStatus? status)
    {
        var role = claimService.GetClaim(ClaimConstants.ROLE, Role.Customer);
        IQueryable<Order> source;
        if (role == Role.Customer)
        {
            var accountId = claimService.GetClaim(ClaimConstants.ID, -1);
            source = uow.OrderRepo.GetOrders(accountId, status);
        }
        else source = uow.OrderRepo.GetOrders(null, status);

        var count = await source.CountAsync();
        var items = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
        return new()
        {
            TotalCount = count,
            Items = items.Adapt<List<OrderBasicInfo>>(),
            PageIndex = pageIndex,
            PageSize = pageSize
        };

    }

    public async Task<Page<OrderDetailBasicInfo>> GetCartPageAsync(int pageIndex, int pageSize)
    {
        var accountId = claimService.GetClaim(ClaimConstants.ID, -1);
        var source = uow.OrderDetailRepo.GetCart(accountId);
        var count = await source.CountAsync();
        var items = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
        return new()
        {
            TotalCount = count,
            Items = items.Adapt<List<OrderDetailBasicInfo>>(),
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }
}