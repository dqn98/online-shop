namespace Shared.Order;

public class CreateOrderDto
{
    public decimal TotalPrice { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public string? ShippingAddress { get; set; }
    public string? InvoiceAddress { get; set; }
}