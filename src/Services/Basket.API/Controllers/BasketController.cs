using System.ComponentModel.DataAnnotations;
using System.Net;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BasketController : Controller
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger _logger;
    
    public BasketController(
        IBasketRepository basketRepository,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(basketRepository);
        ArgumentNullException.ThrowIfNull(logger);

        _basketRepository = basketRepository;
        _logger = logger;
    }

    #region Implement Controller


    [HttpGet("{username}", Name = "GetBasket")]
    public async Task<ActionResult<Cart>> GetBasketByUsername([Required] string username)
    {
        var result = await _basketRepository.GetBasketByUserName(username);
        return Ok(result ?? new Cart());
    }

    [HttpPost(Name = "UpdateBasket")]
    public async Task<ActionResult<Cart>> UpdateBasket([FromBody] Cart cart)
    {
        var option = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(1))
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        
        _logger.Information("Updating basket");
        
        var result = await _basketRepository.UpdateBasket(cart, option);
        return Ok(result);
    }

    [HttpDelete("{username}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteBasket([Required] string username)
    {
        var result = await _basketRepository.DeleteBasketFromUsername(username);
        _logger.Information("Deleting basket");
        return Ok(result);
    }
    
    #endregion
}