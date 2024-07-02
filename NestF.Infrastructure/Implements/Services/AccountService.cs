using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NestF.Application.DTOs.Account;
using NestF.Application.DTOs.Generic;
using NestF.Application.Interfaces.Repositories;
using NestF.Application.Interfaces.Services;
using NestF.Domain.Entities;
using NestF.Domain.Enums;
using NestF.Infrastructure.Constants;
using NestF.Infrastructure.Utils;

namespace NestF.Infrastructure.Implements.Services;

public class AccountService : GenericService<Account>, IAccountService
{
    private readonly IBackgroundService _bgService;
    private readonly IConfiguration _config;
    public AccountService(IUnitOfWork uow, IClaimService claimService, ITimeService timeService,
        IBackgroundService bgService, IConfiguration config) : base(uow,
        claimService, timeService)
    {
        _bgService = bgService;
        _config = config;
    }

    public async Task<Page<CustomerBasicInfo>> GetCustomerPageAsync(int pageIndex, int pageSize)
    {
        var source = uow.AccountRepo.GetCustomers();
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
        var source = uow.AccountRepo.GetStaffs();
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
        account.CreatedAt = timeService.Now;
        account.Role = Role.Staff;
        account.IsActive = true;
        await uow.AccountRepo.AddAsync(account);
        if (!await uow.SaveChangesAsync()) throw new DbUpdateException();
        await _bgService.EnqueueSendPasswordMailJobAsync(account.Id);
        return account.Adapt<StaffBasicInfo>();
    }

    public async Task<Account?> GetCustomerByPhoneAsync(string phone)
    {
        return await uow.AccountRepo.GetCustomerByPhoneAsync(phone);
    }

    public async Task<CustomerBasicInfo> GetCustomerDetailAsync(int id)
    {
        var customer = await uow.AccountRepo.GetCustomerByIdAsync(id) ?? throw new KeyNotFoundException();
        return customer.Adapt<CustomerBasicInfo>();
    }

    public async Task<StaffBasicInfo> GetStaffDetailAsync(int id)
    {
        var staff = await uow.AccountRepo.GetStaffByIdAsync(id) ?? throw new KeyNotFoundException();
        return staff.Adapt<StaffBasicInfo>();
    }

    public async Task<CustomerBasicInfo> RegisterCustomerAsync(CustomerRegister model)
    {
        var customer = model.Adapt<Account>();
        customer.Phone = claimService.GetClaim(ClaimConstants.PHONE, string.Empty);
        customer.IsActive = true;
        customer.Role = Role.Customer;
        customer.CreatedAt = timeService.Now;
        await uow.AccountRepo.AddAsync(customer);
        if (!await uow.SaveChangesAsync()) throw new DbUpdateException();
        return customer.Adapt<CustomerBasicInfo>();
    }

    private AuthToken GenerateAuthToken(Dictionary<string, object> claims, int? accountId = null)
    {
        var now = timeService.Now;
        var refreshToken = JwtUtil.GenerateJwt(claims,
            now,
            DefaultConstants.REFRESH_TOKEN_MINUTE,
            _config,
            true,
            _config["Jwt:RefreshKey"]);
        var accessToken = JwtUtil.GenerateJwt(claims,
            now,
            DefaultConstants.ACCESS_TOKEN_MINUTE,
            _config);
        return new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
    public async Task<AuthToken> AuthorizeStaffAsync(StaffAuthorize model)
    {
        Dictionary<string, object>? claims = null;
        if (model.Email.Equals(_config["Admin:Email"], StringComparison.InvariantCultureIgnoreCase) &&
            model.Password == _config["Admin:Password"])
        {
            claims = new() { [ClaimConstants.ROLE] = Role.Admin.ToString() };
            return GenerateAuthToken(claims);
        }
        var account = await uow.AccountRepo.GetStaffByEmailAsync(model.Email) ?? throw new KeyNotFoundException();
        if (!account.PasswordHash!.VerifyHashString(model.Password)) throw new ArgumentException();
        claims = new() { [ClaimConstants.ROLE] = Role.Staff.ToString(), [ClaimConstants.ID] = account.Id };
        return GenerateAuthToken(claims);
    }
}