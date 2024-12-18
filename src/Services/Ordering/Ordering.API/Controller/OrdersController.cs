using System.ComponentModel.DataAnnotations;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;
using ILogger = Serilog.ILogger;

namespace Ordering.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
private readonly ILogger _logger;
    public OrdersController(
        IMediator mediator, 
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(logger);
        
        _mediator = mediator;
        _logger = logger;
    }
    
     private static class RouteNames
     {
         public const string GetOrders = nameof(GetOrders);
     }

     [HttpGet("{username}", Name = RouteNames.GetOrders)]
     [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
     public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required]string userName)
     {
         var query = new GetOrdersQuery(userName);
         _logger.Information($"OrderQuery: {query}");
         var result = await _mediator.Send(query);
         return Ok(result);
     }
}