using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.Controllers;

public class CheckoutController : Controller
{
    private readonly ICheckoutSagaService _checkoutSagaService;
    
    public CheckoutController(ICheckoutSagaService checkoutSagaService)
    {
        ArgumentNullException.ThrowIfNull(checkoutSagaService);
        
        _checkoutSagaService = checkoutSagaService;
    }

    [HttpPost]
    [Route("{username}")]
    public async Task<IActionResult> Checkout([Required] string username, [FromBody] BasketCheckoutDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkoutSagaService.CheckoutOrder(username, model);
        if (result)
        {
            return Accepted();
        }
        
        return BadRequest("Error during checkout");
    }
}