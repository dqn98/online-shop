using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Ordering.Domain.Enums;

namespace Ordering.Domain.Entities;

public class Order : EntityAuditBase<long>
{
    [Column(TypeName = "NVARCHAR(150)")]
    public required string UserName { get; set; }
    
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
    
    public EOrderStatus Status { get; set; }
}