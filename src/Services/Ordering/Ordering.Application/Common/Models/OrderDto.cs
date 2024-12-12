﻿using Ordering.Application.Common.Mapping;
using Ordering.Domain.Entities;
using Ordering.Domain.Enums;

namespace Ordering.Application.Common.Models;

public class OrderDto : IMapFrom<Order>
{
    public long Id { get; set; }
    public required string UserName { get; set; }
    public decimal TotalPrice { get; set; }
    
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string EmailAddress { get; set; }
    
    // Adress
    
    public required string ShippingAddress { get; set; }
    public required string InvoiceAddress { get; set; }
    
    public EOrderStatus Status { get; set; }
}