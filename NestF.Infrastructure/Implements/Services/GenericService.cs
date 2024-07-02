using Mapster;
using NestF.Application.DTOs.Generic;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;

namespace NestF.Infrastructure.Implements.Services;

public class GenericService<T> : IGenericService<T> where T : class
{
    public GenericService(IUnitOfWork uow, IClaimService claimService, ITimeService timeService)
    {
        this.uow = uow;
        this.claimService = claimService;
        this.timeService = timeService;
    }

    protected readonly IUnitOfWork uow;
    protected readonly IClaimService claimService;
    protected readonly ITimeService timeService;

    // public async Task<T?> GetByIdAsync(int id)
    // {
    //     return await _uow.GetRepo<T>().GetByIdAsync(id);
    // }
    //
    // public Task<Page<T>> GetPageAsync(int pageIndex, int pageSize)
    // {
    //     
    //     throw new NotImplementedException();
    // }
    // public virtual async Task<TModel?> GetByIdAsync<TModel>(int id) where TModel : class
    // {
    //     var item = await _uow.GetRepo<T>().GetByIdAsync(id);
    //     return item.Adapt<TModel?>();
    // }
}