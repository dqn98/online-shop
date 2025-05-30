﻿using Contracts.Common.Interfaces;
using Ordering.Domain.Entities;
using Shared.DTOs.Order;

namespace Ordering.Application.Common.Interfaces;

public interface IOrderRepository : IRepositoryBaseAsync<Order, long>
{
    Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string userName);
    Task<Order?> GetOrderByIdAsync(long id);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderAsync(Order order);
}