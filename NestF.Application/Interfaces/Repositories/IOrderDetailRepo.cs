﻿using NestF.Domain.Entities;

namespace NestF.Application.Interfaces.Repositories;

public interface IOrderDetailRepo : IGenericRepo<OrderDetail>
{
    IQueryable<OrderDetail> GetCart(int accountId);
}