using NestF.Application.DTOs.Account;
using NestF.Application.DTOs.Generic;
using NestF.Domain.Entities;

namespace NestF.Application.Interfaces.Repositories;

public interface IAccountRepo : IGenericRepo<Account>
{
    IQueryable<Account> GetCustomers();
    IQueryable<Account> GetStaffs();
}