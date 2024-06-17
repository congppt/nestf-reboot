using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;

namespace NestF.Infrastructure.Implements.Services;

public class ProductService : GenericService<Product>, IProductService
{
    public ProductService(IUnitOfWork uow,
        IClaimService claimService,
        ITimeService timeService) : base(uow,
        claimService,
        timeService)
    {
    }
}