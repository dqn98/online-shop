using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.HttpRepository;

public class BasketHttpRepository : IBasketHttpRepository
{
    private readonly HttpClient _httpCLient;
    
    public BasketHttpRepository(HttpClient httpCLient)
    {
        ArgumentNullException.ThrowIfNull(httpCLient, nameof(httpCLient));
        
        _httpCLient = httpCLient;
    }
    public async Task<CartDto?> GetBasket(string username)
    {
        var cart = await _httpCLient.GetFromJsonAsync<CartDto?>($"baskets/{username}");
        return cart;
    }

    public async Task<bool> DeleteBasket(string username)
    {
        var response = await _httpCLient.DeleteAsync($"baskets/{username}");
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            throw new Exception($"Delete basket for Username: {username} not success");

        var result = response.IsSuccessStatusCode;
        return result;
    }
}