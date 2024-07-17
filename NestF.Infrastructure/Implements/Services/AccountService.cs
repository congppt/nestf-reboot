using System.Collections.ObjectModel;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
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
    private readonly FirebaseAuth _fbAuth;
    public AccountService(IUnitOfWork uow, IClaimService claimService, ITimeService timeService,
        IBackgroundService bgService, IConfiguration config, FirebaseApp firebase) : base(uow,
        claimService, timeService)
    {
        _bgService = bgService;
        _config = config;
        _fbAuth = FirebaseAuth.GetAuth(firebase);
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
        var role = claimService.GetClaim(ClaimConstants.ROLE, Role.Guest);
        if (role != Role.Guest) throw new ArgumentException();
        var phone = claimService.GetClaim(ClaimConstants.PHONE, string.Empty);
        if (await uow.AccountRepo.GetCustomerByPhoneAsync(phone) != null) throw new ArgumentException();
        var customer = model.Adapt<Account>();
        customer.Phone = phone;
        customer.IsActive = true;
        customer.Role = Role.Customer;
        customer.CreatedAt = timeService.Now;
        await uow.AccountRepo.AddAsync(customer);
        if (!await uow.SaveChangesAsync()) throw new DbUpdateException();
        Dictionary<string, object> claims = new()
        {
            [ClaimConstants.ROLE] = Role.Customer.ToString(),
            [ClaimConstants.ID] = customer.Id
        };
        var uid = claimService.GetClaim(ClaimConstants.FIRE_UID, string.Empty);
        await _fbAuth.SetCustomUserClaimsAsync(uid, new ReadOnlyDictionary<string, object>(claims));
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