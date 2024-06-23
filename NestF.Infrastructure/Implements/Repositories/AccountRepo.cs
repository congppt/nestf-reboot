using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;
using NestF.Domain.Enums;

namespace NestF.Infrastructure.Implements.Repositories;

public class AccountRepo : GenericRepo<Account>, IAccountRepo
{
    public AccountRepo(AppDbContext context, IDistributedCache cache, ITimeService timeService) : base(context, cache, timeService)
    {
    }

    public IQueryable<Account> GetCustomers()
    {
        return context.Accounts.AsNoTrackingWithIdentityResolution().Where(a => a.Role == Role.Customer)
            .OrderByDescending(a => a.Id);
    }

    public IQueryable<Account> GetStaffs()
    {
        return context.Accounts.AsNoTrackingWithIdentityResolution().Where(a => a.Role == Role.Staff)
            .OrderByDescending(a => a.Id);
    }

    public async Task<Account?> GetCustomerByIdAsync(int id, CancellationToken ct = default)
    {
        var account = await GetByIdAsync(id, ct);
        return account is not { Role: Role.Customer } ? null : account;
    }

    public async Task<Account?> GetStaffByIdAsync(int id, CancellationToken ct = default)
    {
        var account = await GetByIdAsync(id, ct);
        return account is not { Role: Role.Staff } ? null : account;
    }

    public async Task<Account?> GetCustomerByPhoneAsync(string phone, CancellationToken ct = default)
    {
        return await context.Accounts.FirstOrDefaultAsync(a => a.Phone == phone, ct);
    }
}