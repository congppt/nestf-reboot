using Microsoft.Extensions.DependencyInjection;
using NestF.Application.Interfaces.Repositories;

namespace NestF.Infrastructure.Implements.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider _services;
    private readonly AppDbContext _context;
    private readonly IAccountRepo _accountRepo;
    private readonly IProductRepo _productRepo;
    private readonly IOrderRepo _orderRepo;
    private readonly ICategoryRepo _categoryRepo;
    private readonly IOrderDetailRepo _orderDetailRepo;

    public UnitOfWork(IServiceProvider services, AppDbContext context, ICategoryRepo categoryRepo, IOrderRepo orderRepo,
        IProductRepo productRepo, IAccountRepo accountRepo, IOrderDetailRepo orderDetailRepo)
    {
        _services = services;
        _context = context;
        _categoryRepo = categoryRepo;
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _accountRepo = accountRepo;
        _orderDetailRepo = orderDetailRepo;
    }

    public IAccountRepo AccountRepo => _accountRepo;
    public IProductRepo ProductRepo => _productRepo;
    public IOrderRepo OrderRepo => _orderRepo;
    public ICategoryRepo CategoryRepo => _categoryRepo;
    public IOrderDetailRepo OrderDetailRepo => _orderDetailRepo;

    public IGenericRepo<T> GetRepo<T>() where T : class
    {
        return (IGenericRepo<T>) _services.GetRequiredService(typeof(IGenericRepo<T>));
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}