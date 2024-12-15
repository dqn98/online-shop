using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _redisCache;
    private readonly ISerializeService _serializeService;
    private readonly ILogger _logger;
    public BasketRepository(
        IDistributedCache redisCache,
        ISerializeService serializeService, 
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(redisCache);
        ArgumentNullException.ThrowIfNull(serializeService);
        ArgumentNullException.ThrowIfNull(logger);
        
        _redisCache = redisCache;
        _serializeService = serializeService;
        _logger = logger;
    }
    
    public async Task<Cart?> GetBasketByUserName(string username)
    {
        var basket = await _redisCache.GetStringAsync(username);
        return string.IsNullOrEmpty(basket) ? null : 
            _serializeService.Deserialize<Cart>(basket);
    }

    public async Task<Cart?> UpdateBasket(Cart cart, DistributedCacheEntryOptions? options = null)
    {
        if (options != null)
        {
            await _redisCache.SetStringAsync(
                cart.UserName, 
                _serializeService.Serialize(cart), 
                options);
        }
        else
        {
            await _redisCache.SetStringAsync(
                cart.UserName, 
                _serializeService.Serialize(cart));
        }

        return await GetBasketByUserName(cart.UserName);
    }

    public async Task<bool> DeleteBasketFromUsername(string username)
    {
        try
        {
            await _redisCache.RemoveAsync(username);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"DeleteBasketFromUsername: {ex.Message}");
            throw;
        }
    }
}