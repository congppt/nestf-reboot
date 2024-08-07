﻿using NestF.Application.DTOs.Account;
using NestF.Application.DTOs.Generic;
using NestF.Domain.Entities;

namespace NestF.Application.Interfaces.Services;

public interface IAccountService : IGenericService<Account>
{
    Task<Page<CustomerBasicInfo>> GetCustomerPageAsync(int pageIndex, int pageSize);
    Task<Page<StaffBasicInfo>> GetStaffPageAsync(int pageIndex, int pageSize);
    Task<StaffBasicInfo> RegisterStaffAsync(StaffRegister model);
    Task<Account?> GetCustomerByPhoneAsync(string phone);
    Task<CustomerBasicInfo> GetCustomerDetailAsync(int id);
    Task<StaffBasicInfo> GetStaffDetailAsync(int id);
    Task<CustomerBasicInfo> RegisterCustomerAsync(CustomerRegister model);
    Task<AuthToken> AuthorizeStaffAsync(StaffAuthorize model);
}