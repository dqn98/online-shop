using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Services;

public class CustomerService : ICustomerServices
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    
    public async Task<IResult> GetCustomerByUserNameAsync(string userName)
    {
        var customer = await _customerRepository.GetCustomerByUsernameAsync(userName);
        return Results.Ok(customer);
    }

    public async Task<IResult> GetCustomers()
    {
        var result = await _customerRepository.FindAll().ToListAsync();
        return Results.Ok(result);
    }
}