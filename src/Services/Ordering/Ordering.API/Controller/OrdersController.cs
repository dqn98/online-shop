﻿using System.ComponentModel.DataAnnotations;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;

namespace Ordering.API.Controller;

public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        
        _mediator = mediator;
    }
    
     private static class RouteNames
     {
         public const string GetOrders = nameof(GetOrders);
     }

     [HttpGet("{username}", Name = RouteNames.GetOrders)]
     [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
     public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required]string username)
     {
         var query = new GetOrdersQuery(username);
         var result = await _mediator.Send(query);
         return Ok(result);
     }
}