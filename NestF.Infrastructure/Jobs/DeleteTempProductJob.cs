using NestF.Application.Interfaces.Repositories;
using NestF.Domain.Entities;
using NestF.Domain.Enums;
using NestF.Infrastructure.Constants;
using Quartz;

namespace NestF.Infrastructure.Jobs;

public class DeleteTempProductJob : IJob
{
    private readonly IUnitOfWork _uow;

    public DeleteTempProductJob(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var productId = context.MergedJobDataMap.GetInt(BackgroundConstants.PRODUCT_ID_KEY);
        var product = await _uow.GetRepo<Product>().GetByIdAsync(productId);
        if (product == null || product.Status != ProductStatus.Temp) return;
        _uow.GetRepo<Product>().Delete(product);
        await _uow.SaveChangesAsync();
    }
}