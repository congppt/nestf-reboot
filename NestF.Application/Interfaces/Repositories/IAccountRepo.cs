using NestF.Application.DTOs.Account;
using NestF.Application.DTOs.Generic;
using NestF.Domain.Entities;

namespace NestF.Application.Interfaces.Repositories;

public interface IAccountRepo : IGenericRepo<Account>
{
    IQueryable<Account> GetCustomers();
    IQueryable<Account> GetStaffs();
    Task<Account?> GetCustomerByIdAsync(int id, CancellationToken ct = default);
    Task<Account?> GetStaffByIdAsync(int id, CancellationToken ct = default);
    Task<Account?> GetCustomerByPhoneAsync(string phone, CancellationToken ct = default);
}