using Mapster;
using Microsoft.EntityFrameworkCore;
using NestF.Application.DTOs.Account;
using NestF.Application.DTOs.Generic;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;
using NestF.Domain.Enums;
using NestF.Infrastructure.Constants;

namespace NestF.Infrastructure.Implements.Services;

public class AccountService : GenericService<Account>, IAccountService
{
    private readonly IBackgroundService _bgService;
    public AccountService(IUnitOfWork uow, IClaimService claimService, ITimeService timeService, IBackgroundService bgService) : base(uow,
        claimService, timeService)
    {
        _bgService = bgService;
    }

    public async Task<Page<CustomerBasicInfo>> GetCustomerPageAsync(int pageIndex, int pageSize)
    {
        var source = _uow.AccountRepo.GetCustomers();
        var count = await source.CountAsync();
        var items = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
        return new()
        {
            TotalCount = count,
            Items = items.Adapt<List<CustomerBasicInfo>>(),
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<Page<StaffBasicInfo>> GetStaffPageAsync(int pageIndex, int pageSize)
    {
        var source = _uow.AccountRepo.GetStaffs();
        var count = await source.CountAsync();
        var items = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
        return new ()
        {
            TotalCount = count,
            Items = items.Adapt<List<StaffBasicInfo>>(),
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<StaffBasicInfo> RegisterStaffAsync(StaffRegister model)
    {
        var account = model.Adapt<Account>();
        account.CreatedAt = _timeService.Now;
        account.Role = Role.Staff;
        account.IsActive = true;
        await _uow.AccountRepo.AddAsync(account);
        if (!await _uow.SaveChangesAsync()) throw new DbUpdateException();
        await _bgService.EnqueueSendPasswordMailJobAsync(account.Id);
        return account.Adapt<StaffBasicInfo>();
    }

    public async Task<Account?> GetCustomerByPhoneAsync(string phone)
    {
        return await _uow.AccountRepo.GetCustomerByPhoneAsync(phone);
    }

    public async Task<CustomerBasicInfo> GetCustomerDetailAsync(int id)
    {
        var customer = await _uow.AccountRepo.GetCustomerByIdAsync(id) ?? throw new KeyNotFoundException();
        return customer.Adapt<CustomerBasicInfo>();
    }

    public async Task<StaffBasicInfo> GetStaffDetailAsync(int id)
    {
        var staff = await _uow.AccountRepo.GetStaffByIdAsync(id) ?? throw new KeyNotFoundException();
        return staff.Adapt<StaffBasicInfo>();
    }

    public async Task<CustomerBasicInfo> RegisterCustomerAsync(CustomerRegister model)
    {
        var customer = model.Adapt<Account>();
        customer.Phone = _claimService.GetClaim(ClaimConstants.PHONE, string.Empty);
        customer.IsActive = true;
        customer.Role = Role.Customer;
        customer.CreatedAt = _timeService.Now;
        await _uow.AccountRepo.AddAsync(customer);
        if (!await _uow.SaveChangesAsync()) throw new DbUpdateException();
        return customer.Adapt<CustomerBasicInfo>();
    }
}