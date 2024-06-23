using Microsoft.Extensions.Caching.Distributed;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;

namespace NestF.Infrastructure.Implements.Repositories;

public class OrderRepo : GenericRepo<Order>, IOrderRepo
{
    public OrderRepo(AppDbContext context, IDistributedCache cache, ITimeService timeService) : base(context, cache, timeService)
    {
    }
}