using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;
using NestF.Domain.Enums;

namespace NestF.Infrastructure.Implements.Repositories;

public class OrderRepo : GenericRepo<Order>, IOrderRepo
{
    public OrderRepo(AppDbContext context, IDistributedCache cache, ITimeService timeService) : base(context, cache, timeService)
    {
    }

    public IQueryable<Order> GetOrders(int? accountId, OrderStatus? status)
    {
        var source = context.Orders
            .AsNoTrackingWithIdentityResolution()
            .Include(o => o.Account)
            .OrderByDescending(o => o.Id)
            .ThenBy(o => o.Status)
            .Where(o => o.Status != OrderStatus.Shopping);
        if (accountId != null) source = source.Where(o => o.AccountId == accountId);
        if (status != null) source = source.Where(o => o.Status == status);
        return source;
    }
}