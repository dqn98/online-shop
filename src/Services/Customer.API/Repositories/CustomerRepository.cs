using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories;

public class CustomerRepository(CustomerContext dbContext, IUnitOfWork<CustomerContext> unitOfWork)
    : RepositoryBaseAsync<Entities.Customer, int, CustomerContext>(dbContext, unitOfWork), ICustomerRepository
{
    public Task<Entities.Customer?> GetCustomerByUsernameAsync(string username)
    => FindByCondition(x=>x.Username == username).SingleOrDefaultAsync();
}