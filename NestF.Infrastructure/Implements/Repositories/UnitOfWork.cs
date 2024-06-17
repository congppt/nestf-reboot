using Microsoft.Extensions.DependencyInjection;
using NestF.Application.Interfaces.Repositories;

namespace NestF.Infrastructure.Implements.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider _services;
    private readonly AppDbContext _context;

    public UnitOfWork(IServiceProvider services, AppDbContext context)
    {
        _services = services;
        _context = context;
    }

    public IGenericRepo<T> GetRepo<T>() where T : class
    {
        return (IGenericRepo<T>) _services.GetRequiredService(typeof(IGenericRepo<T>));
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}