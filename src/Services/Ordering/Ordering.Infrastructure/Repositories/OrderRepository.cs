using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository(OrderContext dbContext, IUnitOfWork<OrderContext> unitOfWork)
    : RepositoryBaseAsync<Order, long, OrderContext>(dbContext, unitOfWork), IOrderRepository
{
    public async Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string userName) =>
        await FindByCondition(x => x.UserName.Equals(userName)).ToListAsync();

    public async Task<Order?> GetOrderByIdAsync(long id) => 
        await FindByCondition(x=>x.Id == id).FirstOrDefaultAsync();
    
    public async Task<Order> CreateOrderAsync(Order order)
    {
        await CreateAsync(order);
        return order;
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        await UpdateAsync(order);
        return order;
    }
}