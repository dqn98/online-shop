using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
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
    private readonly StockItemGrpcService _stockItemGrpcService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    
    public BasketsController(
        IBasketRepository basketRepository,
        StockItemGrpcService stockItemGrpcService,
        ILogger logger,
        IMapper mapper, 
        IPublishEndpoint publishEndpoint)
    {
        ArgumentNullException.ThrowIfNull(basketRepository);
        ArgumentNullException.ThrowIfNull(stockItemGrpcService);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(publishEndpoint);

        _basketRepository = basketRepository;
        _stockItemGrpcService = stockItemGrpcService;
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
        // Comunicate with Inventory.Grpc and check available quantity of products
        foreach (var item in cart.Items)
        {
            var stock = await _stockItemGrpcService.GetStock(item.ItemNo);

            if (stock.Quantity < item.Quantity)
            {
                _logger.Error("Stock not found or not enough for item no: {ItemNo}", item.ItemNo);
            }
            item.SetAvailableQuantity(stock.Quantity);
        }
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(1))
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        
        _logger.Information("Updating basket");
        
        var result = await _basketRepository.UpdateBasket(cart, options);
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