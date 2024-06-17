using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;

namespace NestF.Infrastructure.Implements.Services;

public class OrderService : GenericService<Order>, IOrderService
{
    public OrderService(IUnitOfWork uow, IClaimService claimService, ITimeService timeService) : base(uow, claimService, timeService)
    {
    }
}