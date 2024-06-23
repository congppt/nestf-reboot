using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NestF.Application.DTOs.Account;
using NestF.Application.DTOs.Generic;
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
        return _context.Accounts.Where(a => a.Role == Role.Customer).OrderByDescending(a => a.Id);
    }

    public IQueryable<Account> GetStaffs()
    {
        return _context.Accounts.Where(a => a.Role == Role.Staff).OrderByDescending(a => a.Id);
    }
}