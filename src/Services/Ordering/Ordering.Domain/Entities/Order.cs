using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Common.Events;
using Ordering.Domain.OrderAggregate.Events;
using Shared.Enums.Order;

namespace Ordering.Domain.Entities;

public class Order : AuditableEventEntity<long>
{
    [Column(TypeName = "NVARCHAR(150)")]
    public required string UserName { get; set; }
    
    [Column(TypeName = "NVARCHAR(150)")]
    public required string DocumentNo { get; set; } = Guid.NewGuid().ToString();
    
    [Column(TypeName = "decimal(10,2)")]
    public required decimal TotalPrice { get; set; }
    
    [Column(TypeName = "NVARCHAR(250)")]
    public required string FirstName { get; set; }
    
    [Column(TypeName = "NVARCHAR(250)")]
    public required string LastName { get; set; }
    
    [Column(TypeName = "NVARCHAR(250)")]
    public required string EmailAddress { get; set; }
    
    [Column(TypeName = "NVARCHAR(250)")]
    public required string ShippingAddress { get; set; }
    
    [Column(TypeName = "NVARCHAR(250)")]
    public string? InvoiceAddress { get; set; }
    
    public EOrderStatus Status { get; set; } = EOrderStatus.New;

    public Order AddedOrder()
    {
        AddDomainEvent(new OrderCreatedEvent(Id, UserName, DocumentNo, EmailAddress, TotalPrice, ShippingAddress, InvoiceAddress));
        return this;
    }

    public Order DeletedOrder()
    {
        AddDomainEvent(new OrderDeletedEvent(Id));
        return this;
    }
}