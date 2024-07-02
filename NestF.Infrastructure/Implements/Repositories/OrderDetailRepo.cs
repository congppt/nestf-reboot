using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;
using NestF.Domain.Enums;

namespace NestF.Infrastructure.Implements.Repositories;

public class OrderDetailRepo : GenericRepo<OrderDetail>, IOrderDetailRepo
{
    public OrderDetailRepo(AppDbContext context, IDistributedCache cache, ITimeService timeService) : base(context,
        cache, timeService)
    {
    }

    public IQueryable<OrderDetail> GetCart(int accountId)
    {
        return context.OrderDetails.AsNoTrackingWithIdentityResolution()
            .Include(d => d.Product)
            .OrderByDescending(d => d.Id)
            .Where(d => d.Order.AccountId == accountId && d.Order.Status == OrderStatus.Shopping);
    }
}