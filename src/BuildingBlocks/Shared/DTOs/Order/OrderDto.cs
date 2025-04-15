using Shared.Enums.Order;

namespace Shared.DTOs.Order;

public class OrderDto
{
    public long Id { get; set; }
    public required string UserName { get; set; }
    public required string DocumentNo { get; set; } = Guid.NewGuid().ToString();

    public required decimal TotalPrice { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string EmailAddress { get; set; }

    public required string ShippingAddress { get; set; }
    
    public string? InvoiceAddress { get; set; }
    
    public EOrderStatus Status { get; set; } = EOrderStatus.New;
}