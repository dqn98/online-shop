using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.V1.Orders;
using Shared.DTOs.Order;
using ILogger = Serilog.ILogger;
using OrderDto = Ordering.Application.Common.Models.OrderDto;

namespace Ordering.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    public OrdersController(
        IMediator mediator, 
        ILogger logger,
        IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(mapper);
        
        _mediator = mediator;
        _logger = logger;
        _mapper = mapper;
    }
    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
        // public const string GetOrder = nameof(GetOrder);
        public const string CreateOrder = nameof(CreateOrder);
        public const string UpdateOrder = nameof(UpdateOrder);
        public const string DeleteOrder = nameof(DeleteOrder);
        // public const string DeleteOrderByDocumentNo = nameof(DeleteOrderByDocumentNo);
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
     
     [HttpDelete("{id:long}", Name = RouteNames.DeleteOrder)]
     [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
     public async Task<ActionResult> DeleteOrder([Required] long id)
     {
         var command = new DeleteOrderCommand(id);
         await _mediator.Send(command);
         return NoContent();
     }
     
    [HttpPost(Name = RouteNames.CreateOrder)] 
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        var command = _mapper.Map<CreateOrderCommand>(createOrderDto);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPut(Name = RouteNames.UpdateOrder)]
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
    public IActionResult UpdateOrder([FromBody] UpdateOrderCommand command)
    {
        var result = _mediator.Send(command);
        return Ok(result);
    }
}