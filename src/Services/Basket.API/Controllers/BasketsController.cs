using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.IntegrationEvents.Events.Basket;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketsController : Controller
{
    private readonly IBasketRepository _basketRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    
    public BasketsController(
        IBasketRepository basketRepository,
        ILogger logger,
        IMapper mapper, 
        IPublishEndpoint publishEndpoint)
    {
        ArgumentNullException.ThrowIfNull(basketRepository);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(publishEndpoint);

        _basketRepository = basketRepository;
        _logger = logger;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    #region Implement Controller


    [HttpGet("{username}", Name = "GetBasket")]
    [Route("api/basket/{username}")]
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

    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        ArgumentNullException.ThrowIfNull(basketCheckout);
        var basket = await _basketRepository.GetBasketByUserName(basketCheckout.UserName);
        if (basket == null) 
            return NotFound();
        
        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        // Only get total price from basket 
        eventMessage.TotalPrice = basket.TotalPrice;
        
        _publishEndpoint.Publish(eventMessage);
        
        await _basketRepository.DeleteBasketFromUsername(basketCheckout.UserName);
        return Accepted();
    }              
    #endregion
}