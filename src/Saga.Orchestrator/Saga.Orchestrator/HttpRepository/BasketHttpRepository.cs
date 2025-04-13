using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.HttpRepository;

public class BasketHttpRepository : IBasketHttpRepository
{
    public Task<CartDto> GetBasket(string username)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteBasket(string username)
    {
        throw new NotImplementedException();
    }
}